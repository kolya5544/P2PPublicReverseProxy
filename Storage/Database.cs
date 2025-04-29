using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace P2PPublicReverseProxy.Storage
{
    public class Database : DatabaseBase<Database>
    {
        public List<ApiKey> apiKeys = new List<ApiKey>();

        public ApiKey? ReturnByUID(long uid)
        {
            ApiKey? dbEntry = null;
            lock (DbLock)
            {
                dbEntry = apiKeys.FirstOrDefault(x => x.uid == uid);
            }
            return dbEntry;
        }

        public ApiKey? ReturnByKey(string key)
        {
            ApiKey? dbEntry = null;
            lock (DbLock)
            {
                dbEntry = apiKeys.FirstOrDefault(x => x.key == key);
            }
            return dbEntry;
        }
    }
}
