using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomarkets.Apis.Bybit
{
    public class SpotApi
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://api.bybit.com";

        public static string ApiKey { get; private set; }
        public static string ApiSecret { get; private set; }
        public static string RecvWindow { get; set; } = "5000";

        public SpotApi(string apiKey, string apiSecret)
        {
            ApiKey = apiKey;
            ApiSecret = apiSecret;
            _httpClient = CreateAndConfigureHttpClient();
        }

        private static HttpClient CreateAndConfigureHttpClient()
        {
            var handler = new HttpClientHandler();

            handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            var configureHttpClient = new HttpClient(handler);

            configureHttpClient.BaseAddress = new Uri(ApiUrl);
            configureHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            configureHttpClient.DefaultRequestHeaders.Add("X-BAPI-API-KEY", ApiKey);
            configureHttpClient.DefaultRequestHeaders.Add("X-BAPI-RECV-WINDOW", RecvWindow);

            return configureHttpClient;
        }

        private void RewriteHeaders(string timestamp, string signature)
        {
            if (_httpClient.DefaultRequestHeaders.Contains("X-BAPI-TIMESTAMP"))
            {
                _httpClient.DefaultRequestHeaders.Remove("X-BAPI-TIMESTAMP");
            }

            if (_httpClient.DefaultRequestHeaders.Contains("X-BAPI-SIGN"))
            {
                _httpClient.DefaultRequestHeaders.Remove("X-BAPI-SIGN");
            }

            _httpClient.DefaultRequestHeaders.Add("X-BAPI-TIMESTAMP", timestamp);
            _httpClient.DefaultRequestHeaders.Add("X-BAPI-SIGN", signature);
        }

        /// <summary>
        /// Request without parameters
        /// </summary>
        /// <param name="method"></param>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        private async Task<string> Call(HttpMethod method, string endpoint)
        {
            string timestamp = await Extensions.GetBybitServerTime();
            string paramStr = timestamp + ApiKey + RecvWindow;
            string signature = Extensions.GenerateSignatureHMACSHA256(ApiSecret, paramStr);
            string requestUri = endpoint;

            RewriteHeaders(timestamp, signature);

            var response = await _httpClient.SendAsync(new HttpRequestMessage(method, requestUri));
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Request with multi parameters
        /// </summary>
        /// <param name="method"></param>
        /// <param name="endpoint"></param>
        /// <param name="requestParams"></param>
        /// <returns></returns>
        private async Task<string> Call(HttpMethod method, string endpoint, Dictionary<string, object> requestParams)
        {
            string timestamp = await Extensions.GetBybitServerTime();
            string queryParams = Extensions.GenerateParamsString(requestParams);
            string toSign;
            string requestUri;

            HttpRequestMessage request;

            if (method.Method == "POST" || method.Method == "DELETE")
            {
                string jsonBody = Extensions.QueryStringToJson(queryParams);
                toSign = timestamp + ApiKey + RecvWindow + jsonBody;

                requestUri = endpoint;
                request = new HttpRequestMessage(method, requestUri)
                {
                    Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
                };
            }

            // if GET request
            else
            {
                toSign = timestamp + ApiKey + RecvWindow + queryParams;

                requestUri = endpoint + "?" + queryParams;
                request = new HttpRequestMessage(method, requestUri);
            }

            string signature = Extensions.GenerateSignatureHMACSHA256(ApiSecret, toSign);

            RewriteHeaders(timestamp, signature);

            var response = await _httpClient.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }

        #region Queries

        public async Task<string> Borrow(string coin, string qty)
        {
            if (string.IsNullOrWhiteSpace(coin))
                throw new ArgumentException("Empty parameter", nameof(coin));
            if (string.IsNullOrWhiteSpace(qty))
                throw new ArgumentException("Empty parameter", nameof(qty));

            var parameters = new Dictionary<string, object>
            {
                { nameof(coin), coin },
                { nameof(qty), qty }
            };

            return await Call(HttpMethod.Post, Endpoints.Spot.Borrow, parameters);
        }

        #endregion
    }
}
