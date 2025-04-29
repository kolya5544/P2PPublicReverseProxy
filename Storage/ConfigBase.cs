using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P2PPublicReverseProxy
{
    public abstract class ConfigBase<T> where T : ConfigBase<T>, new()
    {
        [JsonIgnore]
        public object DbLock = new object();

        private string filename;

        public static T Load(string filepath)
        {
            Directory.CreateDirectory("conf");
            if (File.Exists($"conf/{filepath}"))
            {
                var tmp = JsonConvert.DeserializeObject<T>(File.ReadAllText($"conf/{filepath}"));
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
            lock (DbLock) File.WriteAllText($"conf/{filename}", JsonConvert.SerializeObject(this, Formatting.Indented));
        }
    }
}
