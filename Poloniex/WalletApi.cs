using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Cryptomarkets.Apis.Poloniex
{
    public class WalletApi
    {
        private readonly string _key;
        private readonly string _secret;
        private const string ApiUrl = "https://api.poloniex.com";
        private readonly HttpClient _httpClient;

        public WalletApi(string apiKey, string apiSecret)
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
            string signature = signature = Extensions.GenerateSignaturePoloniex(_secret, payload);
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
                return _httpClient.PostAsync(requestUri, content).Result.Content.ReadAsStringAsync().Result;
            }

            return _httpClient.SendAsync(new HttpRequestMessage(method, requestUri)).Result.Content.ReadAsStringAsync().Result;
        }

        #region Queries

        public string DepositAddresses() => Call(HttpMethod.Get, Endpoints.Wallet.DepositAddresses);

        public string NewCurrencyAddress(string currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("currency cannot be empty. ", nameof(currency));

            var parameters = new Dictionary<string, string>
            {
                { "currency", currency }
            };

            return Call(HttpMethod.Post, Endpoints.Wallet.NewCurrencyAddress, parameters, true);
        }

        public string Withdraw(string currency, string amount, string address, string paymentId = "")
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("currency cannot be empty. ", nameof(currency));

            if (string.IsNullOrWhiteSpace(amount))
                throw new ArgumentException("amount cannot be empty. ", nameof(amount));

            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("address cannot be empty. ", nameof(address));

            var parameters = new Dictionary<string, string>
            {
                { "currency", currency },
                { "amount", amount },
                { "address", address }
            };

            if (!string.IsNullOrEmpty(paymentId))
                parameters.Add("paymentId", paymentId);

            return Call(HttpMethod.Post, Endpoints.Wallet.Withdraw, parameters, true);
        }

        #endregion
    }
}
