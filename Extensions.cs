using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using System.Security.Policy;
using System.Security;
using System.Collections;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
//using Cryptomarkets.Apis.Binance;
//using Cryptomarkets.Apis.GateIO;
//using Cryptomarkets.Apis.Poloniex;
//using Cryptomarkets.Apis.Latoken;
//using Cryptomarkets.Apis.Lbank;
//using Cryptomarkets.Apis.Mexc;
using Cryptomarkets.Apis.Bybit;

namespace Cryptomarkets
{
    public static class Extensions
    {
        #region Encoding

        public static string GenerateSignatureHex(string secret, string message)
        {
            StringBuilder sb = new StringBuilder();

            byte[] sha256Bytes;

            using (var hmacSHA256 = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
                sha256Bytes = hmacSHA256.ComputeHash(Encoding.UTF8.GetBytes(message));

            foreach (var b in sha256Bytes)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }

        public static string GenerateSignatureHMACSHA256(string apiSecret, string message)
        {
            using (var hmacSHA256 = new HMACSHA256(Encoding.UTF8.GetBytes(apiSecret)))
                return BitConverter.ToString(hmacSHA256.ComputeHash(Encoding.UTF8.GetBytes(message))).Replace("-", "").ToLower();
        }

        public static string GenerateSignaturePoloniex(string apiSecret, string message)
        {
            byte[] sha256Bytes;

            using (var hmacSHA256 = new HMACSHA256(Encoding.UTF8.GetBytes(apiSecret)))
                sha256Bytes = hmacSHA256.ComputeHash(Encoding.UTF8.GetBytes(message));

            return Convert.ToBase64String(sha256Bytes);
        }

        public static string RsaSignTest(string privateKey, string payload)
        {
            string privKey = "-----BEGIN PRIVATE KEY-----\n" + privateKey + "\n-----END PRIVATE KEY-----";
            var bytesToEncrypt = Encoding.UTF8.GetBytes(payload);
            byte[] encoded;

            RSAParameters rsaParams;

            using (var tr = new StringReader(privKey))
            {
                var pemReader = new PemReader(tr);
                var privateRsaParams = pemReader.ReadObject() as RsaPrivateCrtKeyParameters;
                rsaParams = DotNetUtilities.ToRSAParameters(privateRsaParams);
            }

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(rsaParams);
                encoded = rsa.SignData(bytesToEncrypt, SHA256.Create());
            }

            return Convert.ToBase64String(encoded);
        }

        public static string RsaSignTest2(string privateKey, string payload)
        {
            string privKey = "-----BEGIN PRIVATE KEY-----\n" + privateKey + "\n-----END PRIVATE KEY-----";
            var privateByte = Encoding.UTF8.GetBytes(privateKey);
            var bytesToEncrypt = Encoding.UTF8.GetBytes(payload);
            byte[] encoded;

            

            return "";
        }

        public static string RsaEncryptWithPrivate(string privateKey, string clearText) // TODO
        {
            string privKey = "-----BEGIN PRIVATE KEY-----\n" + privateKey + "\n-----END PRIVATE KEY-----";
            var privateByte = Encoding.UTF8.GetBytes(privateKey);
            var bytesToEncrypt = Encoding.UTF8.GetBytes(clearText);

            


            //------------------------


            using (var txtreader = new StringReader(privKey))
            {
                //var keyPair = (AsymmetricCipherKeyPair)new PemReader(txtreader).ReadObject();
                //encryptEngine.Init(true, keyPair.Private);

                var pemObject = new PemReader(txtreader).ReadObject();
                var rsaPrivateCrtKeyParameters = (RsaPrivateCrtKeyParameters)pemObject;
                var rsaKeyParameters = new RsaKeyParameters(false, rsaPrivateCrtKeyParameters.Modulus, rsaPrivateCrtKeyParameters.PublicExponent);

            }


            //-------------------------

            //var keyParam = PrivateKeyFactory.CreateKey(privateByte);
            var keyParam = PrivateKeyFactory.CreateKey(privateByte);
            var key = (ECPrivateKeyParameters)keyParam;

            var encryptEngine = new Pkcs1Encoding(new RsaEngine());

            encryptEngine.Init(true, key);

            var encrypted = Convert.ToBase64String(encryptEngine.ProcessBlock(bytesToEncrypt, 0, bytesToEncrypt.Length));


            return encrypted;
        }

        public static string GenerateSignatureLbank(string apiSecret, string message)
        {
            var messageByte = Encoding.UTF8.GetBytes(message);
            var secretByte = Encoding.UTF8.GetBytes(apiSecret);

            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();

            RSAParameters RSAKeyInfo = RSA.ExportParameters(false);
            RSAKeyInfo.Modulus = Encoding.UTF8.GetBytes(apiSecret);
            //RSA.ImportParameters(RSAKeyInfo);

            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(RSAKeyInfo);

                var signedBytes = rsa.SignData(messageByte, HashAlgorithmName.SHA256);

                return Convert.ToBase64String(signedBytes);
            }



            //---------------------------------

            //string xmlKey = "<RSAKeyValue><Modulus>MIICdQIBADANBgkqhkiG9w0BAQEFAASCAl8wggJbAgEAAoGBAIsbZVM5JFd2pY61icPnieIjXyCR4tya+0hmZ35QmzCrXrhc1UcAjqZTLBLbzqB3ejdOSFDA7V9XjFznRw16QlRO+U2u1G1HMsnOetDnr66DUCmbpAwLg0rxKbChyvKVStYPIscoVjD/Hb1nX96EDrhwK9YpJb89O0USqhkH6H4pAgMBAAECgYBYw4t9dnn9IaV1Edwt2OJAHagG5XBoqrBru4SQsqjEfqW7aOljHDTqZyo5gm8wL+0Zu2cjuGf/raLQaXgKXphRrjpUUW4ro5yJe82Ma5z2gxUON4ISv/jvz/pssHjVeS0USLbUnzImtB7ic4TOUU27IgrJknunpvUOiuNAka2Q3QJBAP6RbWtqqWHwVRU+bKYz1+I/TDPOipTIgf1aDJNIO2O8t+cYe5qPs8CJVycOMpCdI85VF68SUfql2zsIsHBEjh8CQQCL47T5A50DKaHYDUvwkmuXpbjyqhiCItkCicuShrgvalJmSk6NoTvSL8sQ03BTYaMhK0SCEC53hnL7cwjVEFq3AkBNucV46KYy+xhfViICVQ3zTHRN1SBG8TmPS3FPftxzRWm5K6aBuKKfhM+RYypZMUF/fEew8p0JNJ7NVYfZn3TtAkBXthrO19knFn+H/C5VVTlpCFwCq2xajIcM9GFUKmxqLnwj7wt5+lKL47OrhSe04E9siLiX5JV+FCscRnCPR4XZAkBUh6kdJ3uIkSrcqhnYFQHaYnMEuCk/0XeRue4rHEfw8zjhjdx6iPTCuUXJzYPyWbgsQ7hJxpc5fWzyi1+JjpBR</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>\";";

            //RSACryptoServiceProvider key = new RSACryptoServiceProvider();
            //key.FromXmlString(xmlKey);

            //var hashData = SHA256.Create().ComputeHash(secretByte);

            //var sig = key.SignData(messageByte, CryptoConfig.MapNameToOID("SHA256"));
            //string base64Encrypted = Convert.ToBase64String(sig);

            //-----------------------------------------

            //using (RSA rsa = RSA.Create(1024))
            //{
            //    var rsaParameters = rsa.ExportParameters(true);


            //    byte[] signature = rsa.SignData(messageByte, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            //}
            //---------------------------------------


            //RSACryptoServiceProvider rsaCSP = new RSACryptoServiceProvider();
            //var res = rsaCSP.Encrypt(Encoding.UTF8.GetBytes(message), RSAEncryptionPadding.OaepSHA256);

            //--------------------------------------


            //--------------------------------------
            //RSA rsa = RSA.Create(1024);

            //RSAParameters rsap = new RSAParameters();
            //rsap.Modulus = Encoding.ASCII.GetBytes(apiSecret);

            //rsa.ImportParameters(rsap);
            //byte[] encryptedData = rsa.Encrypt(Encoding.UTF8.GetBytes(message), RSAEncryptionPadding.OaepSHA256);
            //string base64Encrypted = Convert.ToBase64String(encryptedData);
        }

        public static string GenerateSignatureHMACSHA512(string apiSecret, string message)
        {
            using (var hmacSHA512 = new HMACSHA512(Encoding.UTF8.GetBytes(apiSecret)))
                return BitConverter.ToString(hmacSHA512.ComputeHash(Encoding.UTF8.GetBytes(message))).Replace("-", "").ToLower();
        }

        public static string MD5Sign(string apiSecret, string message)
        {
            using (var hmac = new HMACMD5(Encoding.UTF8.GetBytes(apiSecret)))
                return BitConverter.ToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(message))).Replace("-", "");
        }

        public static string MD5Sign(string message)
        {
            var md5 = new MD5CryptoServiceProvider();

            var messageByte = Encoding.UTF8.GetBytes(message);
            var hashmessage = md5.ComputeHash(messageByte);

            return BitConverter.ToString(hashmessage).Replace("-", "");
        }

        public static string EncodeSHA512(string data)
        {
            using (var hmacSHA512 = new SHA512Managed())
                return BitConverter.ToString(hmacSHA512.ComputeHash(Encoding.UTF8.GetBytes(data))).Replace("-", "").ToLower();
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static byte[] GenerateSalt(int length)
        {
            byte[] salt = new byte[length];

            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        #endregion

        #region Time

        public static string GenerateTimeStamp()
        {
            DateTime startEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            DateTime currentTime = DateTime.UtcNow;

            double currentStamp = (currentTime - startEpoch).TotalMilliseconds;
            return Math.Round(currentStamp).ToString();
        }

        public static string GenerateTimeStamp(DateTime baseDateTime) => ToUnixTimeMilliseconds(new DateTimeOffset(baseDateTime)).ToString();

        public static long ToUnixTimeMilliseconds(DateTimeOffset dtOffset)
        {
            long num1 = 864000000000L * 719162L;
            long num2 = num1 / 10000000L;
            return DateTime.UtcNow.Ticks / 10000L - num1 / 10000L;
        }

        public static string GenerateTimeStampInSeconds()
        {
            return DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
        }
        public static async Task<string> GetBybitServerTime() => JsonParse(await BybitApi.Market.GetServerTime(), "time", "");

        //public static string GetBinanceServerTime() => JsonParse(BinanceApi.Public.GetServerTime(), "serverTime");

        //public static string GetPoloniexServerTime() => JsonParse(PoloniexApi.Public.GetServerTime(), "serverTime");

        //public static string GetMexcServerTime() => JsonParse(MexcApi.Public.GetServerTime(), "serverTime");

        //public static string GetGateIOServerTime()
        //{
        //    string serverTime = JsonParse(GateApi.Spot.GetServerTime(), "server_time");

        //    //var timeInSeconds = serverTime.Remove(serverTime.Length-3);
        //    //return timeInSeconds;
        //    return serverTime;
        //}

        #endregion

        #region Converters

        public static DateTime TimeStampToDateTime(double timeStamp)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(timeStamp);
        }

        public static string ConvertDictionaryToQueryString(Dictionary<string, string> dictionary)
        {
            return string.Join("&", dictionary.Select(p => p.Key + "=" + WebUtility.UrlEncode(p.Value)));
        }

        public static string ConvertDictionaryToQueryString(Dictionary<string, object> dictionary)
        {
            return string.Join("&", dictionary.Select(p => p.Key + "=" + p.Value));
        }

        public static string QueryStringToJson(string query)
        {
            var dictionary = query.Replace("?", "").Split('&')
                .ToDictionary(x => x.Split('=')[0], x => x.Split('=')[1]);

            var json = JsonConvert.SerializeObject(dictionary);

            return json;
        }

        public static string JsonParse(string data, string property, string highLevelProp = "")
        {
            dynamic json = JObject.Parse(data);
            string value = string.Empty;

            if (string.IsNullOrWhiteSpace(highLevelProp))
            {
                value = json[property];
            }

            else
            {
                value = json[highLevelProp][property];
            }

            return value;
        }

        public static string JsonToQueryString(string jsonString)
        {
            var jsonObject = JObject.Parse(jsonString);

            var queryStringParameters = new Dictionary<string, string>();

            FlattenJson(jsonObject, queryStringParameters, null);

            var queryString = string.Join("&", queryStringParameters.Select(kvp =>
                $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}"));

            return queryString;
        }

        private static void FlattenJson(JObject jsonObject, Dictionary<string, string> queryStringParameters, string parentKey)
        {
            foreach (var property in jsonObject.Properties())
            {
                var key = string.IsNullOrEmpty(parentKey) ? property.Name : $"{parentKey}.{property.Name}";
                var value = property.Value;

                if (value is JObject nestedObject)
                {
                    FlattenJson(nestedObject, queryStringParameters, key);
                }
                else if (value is JArray array)
                {
                    for (int i = 0; i < array.Count; i++)
                    {
                        var arrayItem = array[i];
                        if (arrayItem is JObject)
                        {
                            FlattenJson((JObject)arrayItem, queryStringParameters, $"{key}[{i}]");
                        }
                        else
                        {
                            queryStringParameters[$"{key}[{i}]"] = arrayItem.ToString();
                        }
                    }
                }
                else
                {
                    queryStringParameters[key] = value.ToString();
                }
            }
        }

        #endregion

        #region Data

        public static string GenerateOrderedPayloadPoloniex(string method, string path, string timestamp, Dictionary<string, string> parameters)
        {
            var sortParams = new SortedDictionary<string, string>(parameters)
            {
                { "signTimestamp", timestamp }
            };

            var payloadBuffer = new StringBuilder();
            payloadBuffer.Append(method).Append("\n").Append(path).Append("\n");

            foreach (var entry in sortParams)
            {
                payloadBuffer.Append(entry.Key).Append("=").Append(entry.Value).Append("&");
            }

            string payload = payloadBuffer.ToString().Substring(0, payloadBuffer.Length - 1);

            return payload;
        }

        public static string GeneratePostPayloadPoloniex(string method, string path, string timestamp, Dictionary<string, string> parameters)
        {
            string jsonStr = string.Empty;

            if (parameters.Count > 0)
            {
                string queryStr = ConvertDictionaryToQueryString(parameters);
                jsonStr = QueryStringToJson(queryStr);
            }
            
            var payloadBuffer = new StringBuilder();
            payloadBuffer.Append(method).Append("\n").Append(path).Append("\n").Append("requestBody=").Append(jsonStr).Append($"&signTimestamp={timestamp}");

            return payloadBuffer.ToString();
        }

        public static string GeneratePostPayloadPoloniex(string method, string path, string timestamp, Dictionary<string, object> parameters)
        {
            string jsonStr = string.Empty;

            if (parameters.Count > 0)
            {
                string queryStr = ConvertDictionaryToQueryString(parameters);
                jsonStr = QueryStringToJson(queryStr);
            }

            var payloadBuffer = new StringBuilder();
            payloadBuffer.Append(method).Append("\n").Append(path).Append("\n").Append("requestBody=").Append(jsonStr).Append($"&signTimestamp={timestamp}");

            return payloadBuffer.ToString();
        }

        public static string GenerateEmptyPayloadPoloniex(string method, string path, string timestamp)
        {
            string payload = method + "\n" + path + "\n" + $"signTimestamp={timestamp}";

            return payload;
        }

        public static string GenerateParamsString(Dictionary<string, object> parameters, bool isSorted = false)
        {
            IDictionary<string, object> paramList;

            if (isSorted)
            {
                paramList = new SortedDictionary<string, object>(parameters);
            }

            else
            {
                paramList = new Dictionary<string, object>(parameters);
            }

            var stringBuffer = new StringBuilder();

            foreach (var entry in paramList)
            {
                stringBuffer.Append(entry.Key).Append("=").Append(entry.Value).Append("&");
            }

            string paramsString = stringBuffer.ToString().Substring(0, stringBuffer.Length - 1);

            return paramsString;
        }

        public static string GenerateParamsString(string path, Dictionary<string, string> parameters)
        {
            var sortParams = new SortedDictionary<string, string>(parameters);

            var stringBuffer = new StringBuilder();
            stringBuffer.Append(path).Append("?");

            foreach (var entry in sortParams)
            {
                stringBuffer.Append(entry.Key).Append("=").Append(entry.Value).Append("&");
            }

            string paramsString = stringBuffer.ToString().Substring(0, stringBuffer.Length - 1);

            return paramsString;
        }

        #endregion
    }
}
