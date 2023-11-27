using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomarkets.Apis.Bybit
{
    /// <summary>
    /// All requests without signature
    /// </summary>
    public class MarketApi
    {
        private const string ApiUrl = "https://api.bybit.com";
        private readonly HttpClient _httpClient;

        internal MarketApi() => _httpClient = CreateAndConfigureHttpClient();

        private static HttpClient CreateAndConfigureHttpClient()
        {
            var handler = new HttpClientHandler();

            handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            return new HttpClient(handler)
            {
                BaseAddress = new Uri(ApiUrl),
                DefaultRequestHeaders =
                {
                    Accept =
                    {
                        new MediaTypeWithQualityHeaderValue("application/json")
                    }
                }
            };
        }

        /// <summary>
        /// Request without parameters
        /// </summary>
        /// <param name="method"></param>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        private async Task<string> Call(string endpoint)
        {
            var response = await _httpClient.GetAsync(endpoint);
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Request with multi parameters
        /// </summary>
        /// <param name="method"></param>
        /// <param name="endpoint"></param>
        /// <param name="requestParams"></param>
        /// <returns></returns>
        private async Task<string> Call(string endpoint, Dictionary<string, object> requestParams)
        {
            string queryParams = Extensions.GenerateParamsString(requestParams);
            string requestUri = endpoint + "?" + queryParams;

            var response = await _httpClient.GetAsync(requestUri);
            return await response.Content.ReadAsStringAsync();
        }

        #region Queries

        public async Task<string> GetServerTime() => await Call(Endpoints.Market.GetServerTime);

        public async Task<string> GetKline(string category, string symbol, string interval, int start = -1, int end = -1, int limit = -1)
        {
            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentException("Empty parameter", nameof(category));
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("Empty parameter", nameof(symbol));
            if (string.IsNullOrWhiteSpace(interval))
                throw new ArgumentException("Empty parameter", nameof(interval));

            var parameters = new Dictionary<string, object>
            {
                { nameof(category), category },
                { nameof(symbol), symbol },
                { nameof(interval), interval }
            };

            if (start >= 0)
                parameters.Add(nameof(start), start);
            if (end >= 0)
                parameters.Add(nameof(end), end);
            if (limit >= 0)
                parameters.Add(nameof(limit), limit);

            return await Call(Endpoints.Market.GetKline, parameters);
        }

        public async Task<string> GetOrderBook(string category, string symbol, int limit = -1)
        {
            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentException("Empty parameter", nameof(category));
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("Empty parameter", nameof(symbol));

            var parameters = new Dictionary<string, object>
            {
                { nameof(category), category },
                { nameof(symbol), symbol }
            };

            if (limit >= 0)
                parameters.Add(nameof(limit), limit);

            return await Call(Endpoints.Market.GetOrderBook, parameters);
        }

        public async Task<string> GetTickers(string category, string symbol = null, string baseCoin = null, string expDate = null)
        {
            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentException("Empty parameter", nameof(category));

            var parameters = new Dictionary<string, object>
            {
                { nameof(category), category }
            };

            if (string.IsNullOrWhiteSpace(symbol))
                parameters.Add(nameof(symbol), symbol);
            if (string.IsNullOrWhiteSpace(baseCoin))
                parameters.Add(nameof(baseCoin), baseCoin);
            if (string.IsNullOrWhiteSpace(expDate))
                parameters.Add(nameof(expDate), expDate);

            return await Call(Endpoints.Market.GetTickers, parameters);
        }

        #endregion
    }
}