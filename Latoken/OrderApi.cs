using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Cryptomarkets.Apis.Latoken
{
    public class OrderApi
    {
        private readonly string _key;
        private readonly string _secret;
        private const string ApiUrl = "https://api.latoken.com";
        private readonly HttpClient _httpClient;

        public OrderApi(string apiKey, string apiSecret)
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

        public string GetAllOrders(string from = "", string limit = "")
        {
            var parameters = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(from))
                parameters.Add("from", from);
            if (!string.IsNullOrEmpty(limit))
                parameters.Add("limit", limit);

            return Call(HttpMethod.Get, Endpoints.Order.AllOrders, parameters);
        }

        public string CancelOrder(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("id cannot be empty. ", nameof(id));

            var parameters = new Dictionary<string, string>
            {
                { "id", id }
            };

            return Call(HttpMethod.Post, Endpoints.Order.CancelOrder, parameters);
        }

        public string CancelAllOrders() => Call(HttpMethod.Post, Endpoints.Order.CancelAllOrders);

        public string GetOrder(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("id cannot be empty. ", nameof(id));

            return Call(HttpMethod.Get, Endpoints.Order.GetOrder, id);
        }

        public string ActiveOrders() => Call(HttpMethod.Get, Endpoints.Order.ActiveOrders);

        public string NewOrder(
            string baseCurrency,
            string quoteCurrency,
            string side,
            string condition,
            string type,
            string quantity,
            string clientOrderId = "",
            string price = "")
        {
            if (string.IsNullOrWhiteSpace(baseCurrency))
                throw new ArgumentException("baseCurrency cannot be empty. ", nameof(baseCurrency));
            if (string.IsNullOrWhiteSpace(quoteCurrency))
                throw new ArgumentException("quoteCurrency cannot be empty. ", nameof(quoteCurrency));
            if (string.IsNullOrWhiteSpace(side))
                throw new ArgumentException("side cannot be empty. ", nameof(side));
            if (string.IsNullOrWhiteSpace(condition))
                throw new ArgumentException("condition cannot be empty. ", nameof(condition));
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("type cannot be empty. ", nameof(type));
            if (string.IsNullOrWhiteSpace(quantity))
                throw new ArgumentException("quantity cannot be empty. ", nameof(quantity));

            var parameters = new Dictionary<string, string>
            {
                { "baseCurrency", baseCurrency },
                { "quoteCurrency", quoteCurrency },
                { "side", side },
                { "condition", condition },
                { "type", type },
                { "quantity", quantity }
            };

            if (!string.IsNullOrEmpty(clientOrderId))
                parameters.Add("clientOrderId", clientOrderId);
            if (!string.IsNullOrEmpty(price))
                parameters.Add("price", price);

            return Call(HttpMethod.Post, Endpoints.Order.NewOrder, parameters);
        }

        #endregion
    }
}
