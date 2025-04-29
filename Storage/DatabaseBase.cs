using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P2PPublicReverseProxy
{
    public abstract class DatabaseBase<T> where T : DatabaseBase<T>, new()
    {
        [JsonIgnore]
        public object DbLock = new object();

        private string filename;

        public static T Load(string filepath)
        {
            Directory.CreateDirectory("db");
            if (File.Exists($"db/{filepath}"))
            {
                var tmp = JsonConvert.DeserializeObject<T>(File.ReadAllText($"db/{filepath}"));
                tmp.filename = filepath;
                return tmp;
            }
            else
            {
                T db = new T();
                db.filename = filepath;
                db.Save();
                return db;
            }
        }

        public void Save()
        {
            lock (DbLock) File.WriteAllText($"db/{filename}", JsonConvert.SerializeObject(this, Formatting.Indented));
        }
    }
}
