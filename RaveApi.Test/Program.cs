using Newtonsoft.Json;
using RaveApi.Models;
using RaveApi.Responses;
using System;
using System.Net;

namespace RaveApi.Test
{
    class Program
    {
        // use your secret key from Rave Dashboard
        static string Secretkey = "Your Rave Secret key";

        // use your Public key from Rave Dashboard
        static string PublicKey = "Your Rave Public Key";
        static string chargeEndpoint = "charge";
        static string validationEndpoint = "validatecharge";
        static async System.Threading.Tasks.Task Main(string[] args)
        {

            Payment payment = new Payment();

            var key = payment.GetEncryptionKey(Secretkey);
            var json = JsonConvert.SerializeObject(GetData());
            var encryptedData = payment.EncryptData(key, json);

            PayLoad postData = GetPayLoad(encryptedData);
            var jsondata = JsonConvert.SerializeObject(postData);
            ChargeResponse chargeReponse = await payment.SuggestCardType(chargeEndpoint, jsondata);
            if (chargeReponse != null && chargeReponse.status == "success" && chargeReponse.data.suggested_auth == "PIN")
            {
                var rawJsonDataInit = JsonConvert.SerializeObject(GetData(chargeReponse.data.suggested_auth, "3310"));
                var encryptedDataInit = payment.EncryptData(key, rawJsonDataInit);
                PayLoad payloadInit = GetPayLoad(encryptedDataInit);
                var encrptedjsondataInit = JsonConvert.SerializeObject(payloadInit);
                CardResponse cardReponse = await payment.InitializePayment(chargeEndpoint, encrptedjsondataInit);
                if (cardReponse != null && cardReponse.data.chargeResponseCode == "02" && cardReponse.data.authModelUsed == "PIN")
                {
                    var dataRf = new
                    {
                        PBFPubKey = PublicKey,
                        transaction_reference = cardReponse.data.txRef,
                        otp = "12345"
                    };
                    var jsondf = JsonConvert.SerializeObject(dataRf);
                    ValidateResponse valReps = await payment.ValidatePayment(validationEndpoint, jsondf);
                }
            }
            else if(chargeReponse.status=="error")
            {

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
            return new ChargeModel
            {
                PBFPubKey = PublicKey,
                cardno = "5438898014560229",
                currency = "NGN",
                country = "NG",
                cvv = "789",
                amount = "3002",
                expiryyear = "19",
                expirymonth = "09",
                suggested_auth = suggested_auth,
                pin = pin,
                email = "info@storefella.com",
                phonenumber = "08067827856",
                firstname = "Ekene",
                lastname = "Duru",
                IP = GetIPAddress(), 
                txRef = GenerateTransactionCode(),
                redirect_url= "https://yourreturnurl/receivepayment",
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

        public static string GenerateTransactionCode()
        {
            Random rd = new Random();
            string code= "T" + rd.Next(Convert.ToInt32(Math.Pow(10, 9))).ToString("D" + "10");
            return code;
        }


    }
}
