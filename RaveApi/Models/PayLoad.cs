using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RaveApi.Models
{
    public class PayLoad
    {
        public string PBFPubKey { get; set; }
        public string client { get; set; }
        public string alg { get; set; }
    }
}