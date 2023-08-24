using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Cryptomarkets.Apis.Mexc
{
    public class SpotApi
    {
        private readonly string _secret;
        private const string ApiUrl = "https://api.gateio.ws";
        private readonly HttpClient _httpClient;

        public SpotApi(string apiKey, string apiSecret)
        {
            _secret = apiSecret;
            _httpClient = CreateAndConfigureHttpClient(apiKey);
        }

        private static HttpClient CreateAndConfigureHttpClient(string apiKey)
        {
            var handler = new HttpClientHandler();

            handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            HttpClient configureHttpClient = new HttpClient(handler);

            configureHttpClient.BaseAddress = new Uri(ApiUrl);
            configureHttpClient.DefaultRequestHeaders.Add("KEY", apiKey);
            configureHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return configureHttpClient;
        }

        private string Call(HttpMethod method, string endpoint, string pathParam = null, string parameters = null, bool isSigned = false)
        {
            string data = null;
            string requestUri = endpoint + pathParam + (string.IsNullOrWhiteSpace(parameters) ? "" : string.Format("?{0}", parameters));

            if (isSigned)
            {
                string payloadEncoded;

                if (method.Method == "POST")
                {
                    requestUri = endpoint;
                    data = Extensions.QueryStringToJson(parameters);
                    parameters = "";
                    payloadEncoded = Extensions.EncodeSHA512(data);
                }
                else
                {
                    payloadEncoded = Extensions.EncodeSHA512("");
                }

                string url = endpoint + pathParam;
                string timestamp = Extensions.GetGateIOServerTime();
                string stringToSign = method + "\n" + url + "\n" + parameters + "\n" + payloadEncoded + "\n" + timestamp;
                string signature = Extensions.GenerateSignatureHMACSHA512(_secret, stringToSign);

                _httpClient.DefaultRequestHeaders.Add("Timestamp", timestamp);
                _httpClient.DefaultRequestHeaders.Add("SIGN", signature);
            }

            if (method.Method == "POST")
            {
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                return _httpClient.PostAsync(requestUri, content).Result.Content.ReadAsStringAsync().Result;
            }

            return _httpClient.SendAsync(new HttpRequestMessage(method, requestUri)).Result.Content.ReadAsStringAsync().Result;
        }

        #region Queries

        public string Buy(string currencyPair, string interval = "", string limit = "", string withId = "false")
        {
            if (string.IsNullOrWhiteSpace(currencyPair))
                throw new ArgumentException("currencyPair cannot be empty. ", nameof(currencyPair));

            string parameters = string.Format("currency_pair={0}", currencyPair) +
                (!string.IsNullOrWhiteSpace(interval) ? string.Format("&interval={0}", interval) : "") +
                (!string.IsNullOrWhiteSpace(limit) ? string.Format("&limit={0}", limit) : "") +
                (!string.IsNullOrWhiteSpace(withId) ? string.Format("&withId={0}", withId) : "");

            return Call(HttpMethod.Get, Endpoints.Spot.Buy, "", parameters);
        }

        public string Sell(
            string currencyPair, 
            string limit = "", 
            string lastId = "", 
            string reverse = "",
            string from = "",
            string to = "",
            string page = "")
        {
            if (string.IsNullOrWhiteSpace(currencyPair))
                throw new ArgumentException("currencyPair cannot be empty. ", nameof(currencyPair));

            string parameters = string.Format("currency_pair={0}", currencyPair) +
                (!string.IsNullOrWhiteSpace(limit) ? string.Format("&limit={0}", limit) : "") +
                (!string.IsNullOrWhiteSpace(lastId) ? string.Format("&last_id={0}", lastId) : "") +
                (!string.IsNullOrWhiteSpace(reverse) ? string.Format("&reverse={0}", reverse) : "") +
                (!string.IsNullOrWhiteSpace(from) ? string.Format("&from={0}", from) : "") +
                (!string.IsNullOrWhiteSpace(to) ? string.Format("&to={0}", to) : "") +
                (!string.IsNullOrWhiteSpace(page) ? string.Format("&page={0}", page) : "");

            return Call(HttpMethod.Get, Endpoints.Spot.Sell, "", parameters);
        }

        #endregion
    }
}
