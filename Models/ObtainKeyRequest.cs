using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P2PPublicReverseProxy.Models
{
    public class ObtainKeyRequest
    {
        public string apiKey { get; set; }
        public string apiSecret { get; set; }
    }
}
