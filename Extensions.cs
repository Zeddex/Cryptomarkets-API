using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Cryptomarkets.Apis.Binance;
using Cryptomarkets.Apis.GateIO;
using Cryptomarkets.Apis.Poloniex;
using Cryptomarkets.Apis.Bitbns;
using Cryptomarkets.Apis.Latoken;
using Cryptomarkets.Apis.Lbank;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Security.Policy;

namespace Cryptomarkets
{
    public static class Extensions
    {
        #region Encoding

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

        public static string GenerateSignatureHMACSHA512(string apiSecret, string message)
        {
            using (var hmacSHA512 = new HMACSHA512(Encoding.UTF8.GetBytes(apiSecret)))
                return BitConverter.ToString(hmacSHA512.ComputeHash(Encoding.UTF8.GetBytes(message))).Replace("-", "").ToLower();
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

        public static string GetBinanceServerTime() => Extensions.JsonParse(BinanceApi.Public.GetServerTime(), "serverTime");

        public static string GetPoloniexServerTime() => Extensions.JsonParse(PoloniexApi.Public.GetServerTime(), "serverTime");

        public static string GetGateIOServerTime()
        {
            string serverTime = Extensions.JsonParse(GateIOApi.Spot.GetServerTime(), "server_time");

            var timeInSeconds = serverTime.Remove(serverTime.Length-3);

            return timeInSeconds;
        }

        #endregion

        #region Converters

        public static string ConvertDictionaryToQueryString(Dictionary<string, string> dictionary)
        {
            return string.Join("&", dictionary.Select(p => p.Key + "=" + WebUtility.UrlEncode(p.Value)));
        }

        public static DateTime TimeStampToDateTime(double timeStamp)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(timeStamp);
        }

        public static string QueryStringToJson(string query)
        {
            var dictionary = query.Replace("?", "").Split('&')
                .ToDictionary(x => x.Split('=')[0], x => x.Split('=')[1]);

            var json = JsonConvert.SerializeObject(dictionary);

            return json;
        }

        public static string JsonParse(string data, string property)
        {
            dynamic json = JObject.Parse(data);
            string value = json[property];

            return value;
        }

        #endregion

        #region Data

        public static string GenerateOrderedPayloadPoloniex(string method, string path, string timestamp, Dictionary<string, string> parameters)
        {
            var sortParams = new SortedDictionary<string, string>(parameters);
            sortParams.Add("signTimestamp", timestamp);

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

        public static string GenerateEmptyPayloadPoloniex(string method, string path, string timestamp)
        {
            string payload = method + "\n" + path + "\n" + $"signTimestamp={timestamp}";

            return payload;
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
