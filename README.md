# RaveApi
Check RaveApi.Test project for full Implementation.


         // use your secret key from Rave Dashboard
        static string Secretkey = "Your Rave Secret key";

        // use your Public key from Rave Dashboard
        static string PublicKey = "Your Rave Public Key";
        static string chargeEndpoint = "charge";
        static string validationEndpoint = "validatecharge";
        
        
        
        
        
            Payment payment = new Payment();

            var key = payment.GetEncryptionKey(Secretkey);
            var json = JsonConvert.SerializeObject(GetData());
            var encryptedData = payment.EncryptData(key, json);

            PayLoad postData = GetPayLoad(encryptedData);
            var jsondata = JsonConvert.SerializeObject(postData);
            ChargeResponse chargeReponse = await payment.SuggestCardType(chargeEndpoint, jsondata);
            
            
            
            
       #Payload     
            
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

