# RaveApi
Check RaveApi.Test for Implementation.

            Payment payment = new Payment();

            var key = payment.GetEncryptionKey(Secretkey);
            var json = JsonConvert.SerializeObject(GetData());
            var encryptedData = payment.EncryptData(key, json);

            PayLoad postData = GetPayLoad(encryptedData);
            var jsondata = JsonConvert.SerializeObject(postData);
            ChargeResponse chargeReponse = await payment.SuggestCardType(chargeEndpoint, jsondata);

