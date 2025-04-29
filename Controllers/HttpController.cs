using Bybit.Net.Clients;
using Bybit.P2P;
using CryptoExchange.Net.Authentication;
using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using Newtonsoft.Json;
using P2PPublicReverseProxy.Models;
using P2PPublicReverseProxy.Storage;
using P2PPublicReverseProxy.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static P2PPublicReverseProxy.Program;

namespace P2PPublicReverseProxy.Controllers
{
    public class HttpController : WebApiController
    {
        public static APIClient sharedAPI = null;
        public static PerUserRateLimiter limiter = new(maxRequests: 2, window: TimeSpan.FromSeconds(1));

        public static async Task AsJSON(IHttpContext context, object? data)
        {
            if (data is null) return;

            context.Response.ContentType = "application/json";
            using var text = context.OpenResponseText();
            await text.WriteLineAsync((string)data).ConfigureAwait(false);
        }

        [Route(HttpVerbs.Post, "/acquire-key")]
        public async Task<string> AcquirePublicKey([JsonData] ObtainKeyRequest req)
        {
            if (req.apiKey.Length != 18
                || req.apiSecret.Length != 36) throw new HttpException(400);

            // create a JKorf's Bybit API client using the provided API key and secret
            var bybit = new BybitRestClient((x) =>
            {
                x.ApiCredentials = new ApiCredentials(req.apiKey, req.apiSecret);
            });

            // check if the API key and secret are valid
            var keyInfo = await bybit.V5Api.Account.GetApiKeyInfoAsync();

            if (!keyInfo.Data.IsMaster) throw new HttpException(403);

            // obtain UID
            var uid = keyInfo.Data.UserId;

            // find a database entry
            var dbEntry = db.ReturnByUID(uid);
            if (dbEntry is null)
            {
                dbEntry = new ApiKey(uid);
                db.apiKeys.Add(dbEntry);
                db.Save();
            }

            return JsonConvert.SerializeObject(dbEntry);
        }

        [Route(HttpVerbs.Post, "/v5/p2p/item/online")]
        public async Task<string> Proxy_GetAds([JsonData] GetAdsRequest req)
        {
            // extract the key from headers
            var key = HttpContext.Request.Headers.Get("X-BAPI-API-KEY");
            if (key is null) throw new HttpException(403);
            var dbEntry = db.ReturnByKey(key);
            if (dbEntry is null) throw new HttpException(403);

            var att = limiter.TryRequest(dbEntry.uid);
            if (!att) throw new HttpException(429);

            // complete the proxification
            try
            {
                var resp = await sharedAPI.GetOnlineAds(new
                {
                    tokenId = req.tokenId,
                    currencyId = req.currencyId,
                    side = req.side,
                    page = req.page,
                    size = req.size
                });

                return JsonConvert.SerializeObject(resp);
            } catch (Exception e)
            {
                throw new HttpException(500, e.Message);
            }
        }
    }
}
