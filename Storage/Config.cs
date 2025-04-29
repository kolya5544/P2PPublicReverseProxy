using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P2PPublicReverseProxy.Storage
{
    public class Config : ConfigBase<Config>
    {
        public string API_KEY = "x";
        public string API_SECRET = "y";
        public string SERVE_URI = "http://127.0.0.1:8080/";
    }
}
