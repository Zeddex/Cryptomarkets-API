using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;

namespace Cryptomarkets.Apis.Mexc
{
    public class WalletApi
    {
        private readonly string _secret;
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://api.mexc.com";

        public WalletApi(string apiKey, string apiSecret)
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
            configureHttpClient.DefaultRequestHeaders.Add("X-MEXC-APIKEY", apiKey);
            configureHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return configureHttpClient;
        }

        /// <summary>
        /// Empty or 1 query string parameter call
        /// </summary>
        /// <param name="method"></param>
        /// <param name="endpoint"></param>
        /// <param name="queryStringParam"></param>
        /// <returns></returns>
        private string Call(HttpMethod method, string endpoint, string queryStringParam = "")
        {
            string timestamp = Extensions.GetMexcServerTime();
            queryStringParam = queryStringParam + (!string.IsNullOrWhiteSpace(queryStringParam) ? "&timestamp=" : "timestamp=") + timestamp;
            string signature = Extensions.GenerateSignatureHMACSHA256(_secret, queryStringParam);
            string requestUri = $"{endpoint}?{queryStringParam}&signature={signature}";

            return _httpClient.SendAsync(new HttpRequestMessage(method, requestUri)).Result.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// Request with multi parameters
        /// </summary>
        /// <param name="method"></param>
        /// <param name="endpoint"></param>
        /// <param name="requestParams"></param>
        /// <param name="isBodyParams">as request body</param>
        /// <returns></returns>
        private string Call(HttpMethod method, string endpoint, Dictionary<string, string> requestParams)
        {
            string requestUri;
            string timestamp = Extensions.GetMexcServerTime();
            requestParams.Add("timestamp", timestamp);
            string queryParams = Extensions.GenerateParamsString(requestParams);
            string signature = Extensions.GenerateSignatureHMACSHA256(_secret, queryParams);
            requestParams.Add("signature", signature);
            queryParams = Extensions.GenerateParamsString(requestParams);

            if (method.Method == "POST" || method.Method == "DELETE")
            {
                requestUri = endpoint;
                var content = new StringContent(queryParams, Encoding.UTF8, "application/json");

                return _httpClient.PostAsync(requestUri, content).Result.Content.ReadAsStringAsync().Result;
            }

            requestUri = $"{endpoint}?{queryParams}";

            return _httpClient.SendAsync(new HttpRequestMessage(method, requestUri)).Result.Content.ReadAsStringAsync().Result;
        }

        // TODO
        /// <summary>
        /// Mixed query string and request body call
        /// </summary>
        /// <param name="method">Only Post or Delete method</param>
        /// <param name="endpoint"></param>
        /// <param name="queryStringParams">Query string parameters</param>
        /// <param name="requestBodyParams">Request body parameters</param>
        /// <returns></returns>
        //private string Call(HttpMethod method, string endpoint, Dictionary<string, string> queryStringParams, Dictionary<string, string> requestBodyParams)
        //{
        //    return _httpClient.SendAsync(new HttpRequestMessage(method, "requestUri")).Result.Content.ReadAsStringAsync().Result;
        //}

        #region Queries

        public string CurrencyInfo()
        {
            return Call(HttpMethod.Get, Endpoints.Wallet.CurrencyInfo);
        }

        public string DepositAddress(string coin, string network = "")
        {
            if (string.IsNullOrWhiteSpace(coin))
                throw new ArgumentException("Empty parameter", nameof(coin));

            var parameters = new Dictionary<string, string>
            {
                { $"{nameof(coin)}", coin }
            };

            if (!string.IsNullOrEmpty(network))
                parameters.Add(nameof(network), network);

            return Call(HttpMethod.Get, Endpoints.Wallet.DepositAddress, parameters);
        }

        public string GenerateDepositAddress(string coin, string network)
        {
            if (string.IsNullOrWhiteSpace(coin))
                throw new ArgumentException("Empty parameter", nameof(coin));
            if (string.IsNullOrWhiteSpace(network))
                throw new ArgumentException("Empty parameter", nameof(network));

            var parameters = new Dictionary<string, string>
            {
                {  $"{nameof(coin)}", coin },
                {  $"{nameof(network)}", network }
            };

            return Call(HttpMethod.Post, Endpoints.Wallet.GenerateDepositAddress, parameters);
        }

        public string Withdraw(
            string coin, 
            string address, 
            string amount, 
            string withdrawOrderId = "", 
            string network = "", 
            string memo = "",
            string remark = "")
        {
            if (string.IsNullOrWhiteSpace(coin))
                throw new ArgumentException("Empty parameter", nameof(coin));
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Empty parameter", nameof(address));
            if (string.IsNullOrWhiteSpace(amount))
                throw new ArgumentException("Empty parameter", nameof(amount));


            var parameters = new Dictionary<string, string>
            {
                {  $"{nameof(coin)}", coin },
                {  $"{nameof(address)}", address },
                {  $"{nameof(amount)}", amount }
            };

            if (!string.IsNullOrEmpty(withdrawOrderId))
                parameters.Add(nameof(withdrawOrderId), withdrawOrderId);
            if (!string.IsNullOrEmpty(network))
                parameters.Add(nameof(network), network);
            if (!string.IsNullOrEmpty(memo))
                parameters.Add(nameof(memo), memo);
            if (!string.IsNullOrEmpty(remark))
                parameters.Add(nameof(remark), remark);

            return Call(HttpMethod.Post, Endpoints.Wallet.Withdraw, parameters);
        }

        #endregion
    }
}
