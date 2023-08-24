using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Cryptomarkets.Apis.Poloniex
{
    public class OrderApi
    {
        private readonly string _key;
        private readonly string _secret;
        private const string ApiUrl = "https://api.poloniex.com";
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

            var configureHttpClient = new HttpClient(handler);

            configureHttpClient.BaseAddress = new Uri(ApiUrl);
            configureHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            configureHttpClient.DefaultRequestHeaders.Add("key", apiKey);
            configureHttpClient.DefaultRequestHeaders.Add("signMethod", "HmacSHA256");
            configureHttpClient.DefaultRequestHeaders.Add("signVersion", "2");

            return configureHttpClient;
        }

        private string Call(HttpMethod method, string endpoint, string paramsPath = "")
        {
            string timestamp = Extensions.GetPoloniexServerTime();
            string extEndpoint = endpoint + paramsPath;
            string payload = Extensions.GenerateEmptyPayloadPoloniex(method.ToString(), extEndpoint, timestamp);
            string signature = Extensions.GenerateSignaturePoloniex(_secret, payload);
            string requestUri = extEndpoint;

            _httpClient.DefaultRequestHeaders.Add("signTimestamp", timestamp);
            _httpClient.DefaultRequestHeaders.Add("signature", signature);

            return _httpClient.SendAsync(new HttpRequestMessage(method, requestUri)).Result.Content.ReadAsStringAsync().Result;
        }

        private string Call(HttpMethod method, string endpoint, Dictionary<string, string> parameters, bool isBodyParams = false)
        {
            string requestBody = "", requestUri, payload, signature;
            string timestamp = Extensions.GetPoloniexServerTime();

            if (isBodyParams)
            {
                string queryStr = Extensions.ConvertDictionaryToQueryString(parameters);
                payload = Extensions.GeneratePostPayloadPoloniex(method.ToString(), endpoint, timestamp, parameters);
                signature = Extensions.GenerateSignaturePoloniex(_secret, payload);
                requestUri = endpoint;

                if (queryStr != "")
                {
                    requestBody = Extensions.QueryStringToJson(queryStr);
                }
            }

            else
            {
                payload = Extensions.GenerateOrderedPayloadPoloniex(method.ToString(), endpoint, timestamp, parameters);
                signature = Extensions.GenerateSignaturePoloniex(_secret, payload);
                requestUri = Extensions.GenerateParamsString(endpoint, parameters);
            }

            _httpClient.DefaultRequestHeaders.Add("signTimestamp", timestamp);
            _httpClient.DefaultRequestHeaders.Add("signature", signature);

            if (method.Method == "POST" || method.Method == "DELETE")
            {
                var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                //return _httpClient.PostAsync(requestUri, content).Result.Content.ReadAsStringAsync().Result;
                return _httpClient.PostAsync(requestUri, content).Result.StatusCode.ToString();
            }

            return _httpClient.SendAsync(new HttpRequestMessage(method, requestUri)).Result.Content.ReadAsStringAsync().Result;
        }

        #region Queries

        public string CreateOrder(
            string symbol, 
            string side, 
            string timeInForce = "", 
            string type = "", 
            string accountType = "", 
            string price = "", 
            string quantity = "", 
            string amount = "", 
            string clientOrderId = "")
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));

            if (string.IsNullOrWhiteSpace(side))
                throw new ArgumentException("side cannot be empty. ", nameof(side));

            var parameters = new Dictionary<string, string>
            {
                { "symbol", symbol },
                { "side", side }
            };
            if (!string.IsNullOrEmpty(timeInForce)) 
                parameters.Add("timeInForce", timeInForce);
            if (!string.IsNullOrEmpty(type))
                parameters.Add("type", type);
            if (!string.IsNullOrEmpty(accountType))
                parameters.Add("accountType", accountType);
            if (!string.IsNullOrEmpty(price))
                parameters.Add("price", price);
            if (!string.IsNullOrEmpty(quantity))
                parameters.Add("quantity", quantity);
            if (!string.IsNullOrEmpty(amount))
                parameters.Add("amount", amount);
            if (!string.IsNullOrEmpty(clientOrderId))
                parameters.Add("clientOrderId", clientOrderId);

            return Call(HttpMethod.Post, Endpoints.Order.CreateOrder, parameters);
        }

        public string OpenOrders(
            string symbol = "",
            string side = "",
            string from = "",
            string direction = "",
            string limit = "")
        {
            var parameters = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(symbol))
                parameters.Add("symbol", symbol);
            if (!string.IsNullOrEmpty(side))
                parameters.Add("side", side);
            if (!string.IsNullOrEmpty(from))
                parameters.Add("from", from);
            if (!string.IsNullOrEmpty(direction))
                parameters.Add("direction", direction);
            if (!string.IsNullOrEmpty(limit))
                parameters.Add("limit", limit);

            return Call(HttpMethod.Get, Endpoints.Order.OpenOrders, parameters);
        }

        public string OrderDetails(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("id cannot be empty. ", nameof(id));

            string paramsPath = id;

            return Call(HttpMethod.Get, Endpoints.Order.OrderDetails, paramsPath);
        }

        public string CancelOrder(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("id cannot be empty. ", nameof(id));

            string paramsPath = id;

            return Call(HttpMethod.Delete, Endpoints.Order.CancelOrder, paramsPath);
        }

        public string CancelAllOrders(string symbols = "", string accountTypes = "")
        {
            var parameters = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(symbols))
                parameters.Add("symbols", symbols);
            if (!string.IsNullOrEmpty(accountTypes))
                parameters.Add("accountTypes", accountTypes);

            return Call(HttpMethod.Delete, Endpoints.Order.CancelAllOrders, parameters, true);
        }

        public string OrdersHistory(
            string accountType = "",
            string type = "",
            string side = "",
            string symbol = "",
            string from = "",
            string direction = "",
            string states = "",
            string limit = "",
            string hideCancel = "",
            string startTime = "",
            string endTime = "")
        {
            var parameters = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(accountType))
                parameters.Add("accountType", accountType);
            if (!string.IsNullOrEmpty(type))
                parameters.Add("type", type);
            if (!string.IsNullOrEmpty(side))
                parameters.Add("side", side);
            if (!string.IsNullOrEmpty(symbol))
                parameters.Add("symbol", symbol);
            if (!string.IsNullOrEmpty(from))
                parameters.Add("from", from);
            if (!string.IsNullOrEmpty(direction))
                parameters.Add("direction", direction);
            if (!string.IsNullOrEmpty(states))
                parameters.Add("states", states);
            if (!string.IsNullOrEmpty(limit))
                parameters.Add("limit", limit);
            if (!string.IsNullOrEmpty(hideCancel))
                parameters.Add("hideCancel", hideCancel);
            if (!string.IsNullOrEmpty(startTime))
                parameters.Add("startTime", startTime);
            if (!string.IsNullOrEmpty(endTime))
                parameters.Add("endTime", endTime);

            return Call(HttpMethod.Get, Endpoints.Order.OrdersHistory, parameters);
        }

        #endregion
    }
}
