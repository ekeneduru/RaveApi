using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RaveApi.Responses
{
    public class ChargeResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public ChargeData data { get; set; }
    }

    public class ChargeData
    {
       public string suggested_auth { get; set; }
    }
}