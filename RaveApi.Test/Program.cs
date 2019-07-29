using Newtonsoft.Json;
using RaveApi.Models;
using RaveApi.Responses;
using System;
using System.Net;

namespace RaveApi.Test
{
    class Program
    {
        static string Secretkey = "FLWSECK_TEST-9ee9d0e4622c04dbd2bc066266135e06-X";
        static string PublicKey = "FLWPUBK_TEST-534e215f89614de928d42d61126a696c-X";
        static string chargeEndpoint = "charge";
        static string validationEndpoint = "validatecharge";
        static async System.Threading.Tasks.Task Main(string[] args)
        {

            Payment pay = new Payment();

            var key = pay.GetEncryptionKey(Secretkey);
            var json = JsonConvert.SerializeObject(GetData());
            var encryptedData = pay.EncryptData(key, json);

            PayLoad postData = GetPayLoad(encryptedData);
            var jsondata = JsonConvert.SerializeObject(postData);
            ChargeResponse rep = await pay.SuggestCardType(chargeEndpoint, jsondata);
            if (rep != null && rep.status == "success" && rep.data.suggested_auth == "PIN")
            {
                var json2 = JsonConvert.SerializeObject(GetData(rep.data.suggested_auth, "3310"));
                var encryptedData2 = pay.EncryptData(key, json2);
                PayLoad postData2 = GetPayLoad(encryptedData2);
                var jsondata2 = JsonConvert.SerializeObject(postData2);
                CardResponse rep2 = await pay.InitializePayment(chargeEndpoint, jsondata2);
                if (rep2 != null && rep2.data.chargeResponseCode == "02" && rep2.data.authModelUsed == "PIN")
                {
                    var dataRf = new
                    {
                        PBFPubKey = PublicKey,
                        transaction_reference = rep2.data.txRef,
                        otp = "12345"
                    };
                    var jsondf = JsonConvert.SerializeObject(dataRf);
                    ValidateResponse valReps = await pay.ValidatePayment(validationEndpoint, jsondf);
                }
            }

        }


        private static PayLoad GetPayLoad(string encryptedData)
        {
            return new PayLoad
            {
                PBFPubKey = PublicKey,
                client = encryptedData,
                alg = "3DES-24"
            };
        }

        private static ChargeModel GetData(string suggested_auth = "", string pin = "")
        {
            var rand = new Random().Next(1000, 9999);
            return new ChargeModel
            {
                PBFPubKey = PublicKey,
                cardno = "5438898014560229",
                currency = "NGN",
                country = "NG",
                cvv = "789",
                amount = "300",
                expiryyear = "19",
                expirymonth = "09",
                suggested_auth = suggested_auth,
                pin = pin,
                email = "ekeneduru@yahoo.com",
                phonenumber = "08067827856",
                firstname = "Ekene",
                lastname = "Duru",
                IP = GetIPAddress(), // ":1",//HttpContext.Request.UserHostAddress,
                txRef = $"MXX-ASC-{rand}",
            };
        }

        public static string GetIPAddress()
        {
            String strHostName = string.Empty;
            // Getting Ip address of local machine...
            // First get the host name of local machine.
            strHostName = Dns.GetHostName();
            Console.WriteLine("Local Machine's Host Name: " + strHostName);
            // Then using host name, get the IP address list..
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            IPAddress[] addr = ipEntry.AddressList;
            string ipAddress = string.Empty;
            for (int i = 0; i < addr.Length; i++)
            {
                Console.WriteLine("IP Address {0}: {1} ", i, addr[i].ToString());

              ipAddress=  string.Format("{0}", addr[i].ToString());
            }

            return ipAddress;
        }


    }
}
