using Newtonsoft.Json;
using RaveApi.Responses;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RaveApi
{
    public class Payment
    {
        /// <summary>
        /// Gets an encryption key from rave secret key.
        /// </summary>
        /// <param name="secretKey">The secret key generated from your rave dashboard</param>
        /// <returns>a string value encrypted</returns>
        public string GetEncryptionKey(string secretKey)
        {

            //MD5 is the hash algorithm expected by rave to generate encryption key
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            //MD5CryptoServiceProvider works with bytes so a conversion of plain secretKey to it bytes equivalent is required.
            //UTF8Encoding.UTF8.GetBytes(secretKey) can also be used.
            byte[] secretKeyBytes = ASCIIEncoding.UTF8.GetBytes(secretKey);


            byte[] hashedSecret = md5.ComputeHash(secretKeyBytes, 0, secretKeyBytes.Length);
            byte[] hashedSecretLast12Bytes = new byte[12];
            Array.Copy(hashedSecret, hashedSecret.Length - 12, hashedSecretLast12Bytes, 0, 12);
            String hashedSecretLast12HexString = BitConverter.ToString(hashedSecretLast12Bytes);
            hashedSecretLast12HexString = hashedSecretLast12HexString.ToLower().Replace("-", "");
            String secretKeyFirst12 = secretKey.Replace("FLWSECK-", "").Substring(0, 12);
            byte[] hashedSecretLast12HexBytes = ASCIIEncoding.UTF8.GetBytes(hashedSecretLast12HexString);
            byte[] secretFirst12Bytes = ASCIIEncoding.UTF8.GetBytes(secretKeyFirst12);
            byte[] combineKey = new byte[24];
            Array.Copy(secretFirst12Bytes, 0, combineKey, 0, secretFirst12Bytes.Length);
            Array.Copy(hashedSecretLast12HexBytes, hashedSecretLast12HexBytes.Length - 12, combineKey, 12, 12);
            return ASCIIEncoding.UTF8.GetString(combineKey);
        }

        // This is the encryption function that encrypts your payload by passing the stringified format and your encryption Key.
        public string EncryptData(string encryptionKey, string data)
        {
            TripleDES des = new TripleDESCryptoServiceProvider();
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;
            des.Key = ASCIIEncoding.UTF8.GetBytes(encryptionKey);
            ICryptoTransform cryptoTransform = des.CreateEncryptor();
            byte[] dataBytes = ASCIIEncoding.UTF8.GetBytes(data);
            byte[] encryptedDataBytes = cryptoTransform.TransformFinalBlock(dataBytes, 0, dataBytes.Length);
            des.Clear();
            return Convert.ToBase64String(encryptedDataBytes);
        }

        public string DecryptData(string encryptedData, string encryptionKey)
        {
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.Key = ASCIIEncoding.UTF8.GetBytes(encryptionKey);
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;
            ICryptoTransform cryptoTransform = des.CreateDecryptor();
            byte[] EncryptDataBytes = Convert.FromBase64String(encryptedData);
            byte[] plainDataBytes = cryptoTransform.TransformFinalBlock(EncryptDataBytes, 0, EncryptDataBytes.Length);
            des.Clear();
            return ASCIIEncoding.UTF8.GetString(plainDataBytes);

        }

        public async Task<ChargeResponse> SuggestCardType(string apiUrl, dynamic jsondata)
        {
            ChargeResponse resp = await PostAsync<ChargeResponse>(apiUrl, jsondata);
            return resp;
        }

        public async Task<CardResponse> InitializePayment(string apiUrl, dynamic jsondata)
        {
            CardResponse resp = await PostAsync<CardResponse>(apiUrl, jsondata);
            return resp;
        }

        public async Task<ValidateResponse> ValidatePayment(string valUrl, dynamic jsondata)
        {
            ValidateResponse resp = await PostAsync<ValidateResponse>(valUrl, jsondata);
            return resp;
        }

        private static async Task<T> PostAsync<T>(string endpoint, string content)
        {
            try
            {
                string baseUrl = "https://ravesandboxapi.flutterwave.com/flwv3-pug/getpaidx/api/";
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent stringContent = new StringContent(content.ToString(), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(endpoint, stringContent);
                    string respMessage = string.Empty;
                    if (response.IsSuccessStatusCode)
                    {
                        respMessage = await response.Content.ReadAsStringAsync();
                        var resp = JsonConvert.DeserializeObject<T>(respMessage);
                        return resp;
                    }
                    else
                    {
                        respMessage = await response.Content.ReadAsStringAsync();
                        var resp = JsonConvert.DeserializeObject<T>(respMessage);
                        return resp;
                    }
                }
            }catch(Exception ex)
            {
                return default(T);
            }
        }
    }
}
