using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomarkets.Apis.Latoken
{
    public class TradeApi
    {
        private readonly string _key;
        private readonly string _secret;
        private const string ApiUrl = "https://api.latoken.com";
        private readonly HttpClient _httpClient;

        public TradeApi(string apiKey, string apiSecret)
        {
            _key = apiKey;
            _secret = apiSecret;
            _httpClient = CreateAndConfigureHttpClient(_key);
        }

        private static HttpClient CreateAndConfigureHttpClient(string apiKey)
        {
            var handler = new HttpClientHandler();

            handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            HttpClient configureHttpClient = new HttpClient(handler);

            configureHttpClient.BaseAddress = new Uri(ApiUrl);
            configureHttpClient.DefaultRequestHeaders.Add("X-LA-APIKEY", apiKey);
            configureHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return configureHttpClient;
        }

        private string Call(HttpMethod method, string endpoint, string queryParam = "")
        {
            string path = endpoint + queryParam;
            string toSign = method.Method + path;
            string signature = Extensions.GenerateSignatureHMACSHA256(_secret, toSign);
            string requestUri = path;

            _httpClient.DefaultRequestHeaders.Add("X-LA-SIGNATURE", signature);

            return _httpClient.SendAsync(new HttpRequestMessage(method, requestUri)).Result.Content.ReadAsStringAsync().Result;
        }

        private string Call(HttpMethod method, string endpoint, Dictionary<string, string> parameters)
        {
            string requestUri = Extensions.GenerateParamsString(endpoint, parameters);
            string queryString = Extensions.ConvertDictionaryToQueryString(parameters);

            string toSign = method.Method + endpoint + queryString;
            string signature = Extensions.GenerateSignatureHMACSHA256(_secret, toSign);

            _httpClient.DefaultRequestHeaders.Add("X-LA-SIGNATURE", signature);

            if (method.Method == "POST" || method.Method == "DELETE")
            {
                string requestBody = queryString != "" ? Extensions.QueryStringToJson(queryString) : "";

                requestUri = endpoint;
                var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                return _httpClient.PostAsync(requestUri, content).Result.Content.ReadAsStringAsync().Result;
            }

            return _httpClient.SendAsync(new HttpRequestMessage(method, requestUri)).Result.Content.ReadAsStringAsync().Result;
        }

        #region Queries

        public string GetTrades(string from = "", string limit = "")
        {
            var parameters = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(from))
                parameters.Add("from", from);
            if (!string.IsNullOrEmpty(limit))
                parameters.Add("limit", limit);

            return Call(HttpMethod.Get, Endpoints.Trade.GetTrades, parameters);
        }

        public string FeeForPair(string currency, string quote)
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("currency cannot be empty. ", nameof(currency));

            if (string.IsNullOrWhiteSpace(quote))
                throw new ArgumentException("quote cannot be empty. ", nameof(quote));

            var parameters = new Dictionary<string, string>
            {
                { "currency", currency },
                { "quote", quote }
            };

            return Call(HttpMethod.Get, Endpoints.Trade.FeeForPair, parameters);
        }

        public string TradesByPair(string currency, string quote)
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("currency cannot be empty. ", nameof(currency));

            if (string.IsNullOrWhiteSpace(quote))
                throw new ArgumentException("quote cannot be empty. ", nameof(quote));

            var parameters = new Dictionary<string, string>
            {
                { "currency", currency },
                { "quote", quote }
            };

            return Call(HttpMethod.Get, Endpoints.Trade.TradesByPair, parameters);
        }

        #endregion
    }
}
