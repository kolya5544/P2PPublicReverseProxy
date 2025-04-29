using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P2PPublicReverseProxy.Models
{
    public class GetAdsRequest
    {
        public string tokenId { get; set; }
        public string currencyId { get; set; }
        public string side { get; set; }
        public string page { get; set; }
        public string size { get; set; }
    }
}
