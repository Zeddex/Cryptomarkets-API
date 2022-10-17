using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Cryptomarkets.Apis.Lbank
{
    public class PublicApi
    {
        private const string ApiUrl = "https://api.lbkex.com";
        private readonly HttpClient _httpClient;

        public PublicApi() => _httpClient = CreateAndConfigureHttpClient();

        private static HttpClient CreateAndConfigureHttpClient()
        {
            HttpClientHandler handler = new HttpClientHandler();

            handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            return new HttpClient(handler)
            {
                BaseAddress = new Uri(ApiUrl),
                DefaultRequestHeaders =
                {
                    Accept =
                    {
                        new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded")
                    }
                }
            };
        }

        private string Call(HttpMethod method, string endpoint, string param = null)
        {
            string requestUri = endpoint + param;

            return _httpClient.SendAsync(new HttpRequestMessage(method, requestUri)).Result.Content.ReadAsStringAsync().Result;
        }

        private string Call(HttpMethod method, string endpoint, Dictionary<string, string> parameters)
        {
            string requestUri = Extensions.GenerateParamsString(endpoint, parameters);

            return _httpClient.SendAsync(new HttpRequestMessage(method, requestUri)).Result.Content.ReadAsStringAsync().Result;
        }

        #region Queries

        public string MarketData(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));

            var parameters = new Dictionary<string, string>
            {
                { "symbol", symbol }
            };

            return Call(HttpMethod.Get, Endpoints.Public.MarketData, parameters);
        }

        public string TradingPairs() => Call(HttpMethod.Get, Endpoints.Public.TradingPairs);

        public string MarketDepth(string symbol, string size, string merge = "")
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));
            if (string.IsNullOrWhiteSpace(size))
                throw new ArgumentException("size cannot be empty. ", nameof(size));

            var parameters = new Dictionary<string, string>
            {
                { "symbol", symbol },
                { "size", size }
            };

            if (!string.IsNullOrEmpty(merge))
                parameters.Add("merge", merge);

            return Call(HttpMethod.Get, Endpoints.Public.MarketDepth, parameters);
        }

        public string HistoricalTransactions(string symbol, string size, string time = "")
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));
            if (string.IsNullOrWhiteSpace(size))
                throw new ArgumentException("size cannot be empty. ", nameof(size));


            var parameters = new Dictionary<string, string>
            {
                { "symbol", symbol },
                { "size", size }
            };

            if (!string.IsNullOrEmpty(time))
                parameters.Add("time", time);

            return Call(HttpMethod.Get, Endpoints.Public.HistoricalTransactions, parameters);
        }

        public string KBarData(string symbol, string size, string type, string time)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));
            if (string.IsNullOrWhiteSpace(size))
                throw new ArgumentException("size cannot be empty. ", nameof(size));
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("type cannot be empty. ", nameof(type));
            if (string.IsNullOrWhiteSpace(time))
                throw new ArgumentException("time cannot be empty. ", nameof(time));

            var parameters = new Dictionary<string, string>
            {
                { "symbol", symbol },
                { "size", size },
                { "type", type },
                { "time", time }
            };

            return Call(HttpMethod.Get, Endpoints.Public.KBarData, parameters);
        }

        public string WithdrawConfig(string assetCode = "")
        {
            var parameters = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(assetCode))
                parameters.Add("assetCode", assetCode);

            return Call(HttpMethod.Get, Endpoints.Public.WithdrawConfig, parameters);
        }

        #endregion
    }
}
