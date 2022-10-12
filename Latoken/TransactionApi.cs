using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Cryptomarkets.Apis.Latoken
{
    public class TransactionApi
    {
        private readonly string _key;
        private readonly string _secret;
        private const string ApiUrl = "https://api.latoken.com";
        private readonly HttpClient _httpClient;

        public TransactionApi(string apiKey, string apiSecret)
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

        private string Call(HttpMethod method, string endpoint, string param = "")
        {
            string path = endpoint + param;
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

        public string GetTransactionById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("id cannot be empty. ", nameof(id));

            string pathParam = id;

            return Call(HttpMethod.Get, Endpoints.Transaction.TransactionById, pathParam);
        }

        public string ConfirmWithdrawal(string id, string confirmationCode)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("id cannot be empty. ", nameof(id));

            if (string.IsNullOrWhiteSpace(confirmationCode))
                throw new ArgumentException("confirmationCode cannot be empty. ", nameof(confirmationCode));

            var parameters = new Dictionary<string, string>
            {
                { "id", id },
                { "confirmationCode", confirmationCode }
            };

            return Call(HttpMethod.Post, Endpoints.Transaction.ConfirmWithdrawal, parameters);
        }

        public string RequestWithdrawal(string currencyBinding, string amount, string recipientAddress, string twoFaCode = "", string memo = "")
        {
            if (string.IsNullOrWhiteSpace(currencyBinding))
                throw new ArgumentException("currencyBinding cannot be empty. ", nameof(currencyBinding));

            if (string.IsNullOrWhiteSpace(amount))
                throw new ArgumentException("amount cannot be empty. ", nameof(amount));

            if (string.IsNullOrWhiteSpace(recipientAddress))
                throw new ArgumentException("recipientAddress cannot be empty. ", nameof(currencyBinding));

            var parameters = new Dictionary<string, string>
            {
                { "currencyBinding", currencyBinding },
                { "amount", amount },
                { "recipientAddress", recipientAddress }
            };

            if (!string.IsNullOrEmpty(twoFaCode))
                parameters.Add("twoFaCode", twoFaCode);
            if (!string.IsNullOrEmpty(memo))
                parameters.Add("memo", memo);

            return Call(HttpMethod.Post, Endpoints.Transaction.RequestWithdrawal, parameters);
        }

        #endregion
    }
}
