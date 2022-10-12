using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Cryptomarkets.Apis.Poloniex
{
    public class PublicApi
    {
        private const string ApiUrl = "https://api.poloniex.com";
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
                        new MediaTypeWithQualityHeaderValue("application/json")
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

        public string GetServerTime() => Call(HttpMethod.Get, Endpoints.Public.ServerTime);

        public string SymbolInfo(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));

            return Call(HttpMethod.Get, Endpoints.Public.SymbolInfo, symbol);
        }

        public string AllSymbolsInfo() => Call(HttpMethod.Get, Endpoints.Public.AllSymbolsInfo);

        public string SymbolPrice(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));

            string pathParam = symbol + "/price";

            return Call(HttpMethod.Get, Endpoints.Public.SymbolPrice, pathParam);
        }

        public string AllSymbolsPrices() => Call(HttpMethod.Get, Endpoints.Public.AllSymbolsPrices);

        public string CurrencyInfo(string currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("currency cannot be empty. ", nameof(currency));

            return Call(HttpMethod.Get, Endpoints.Public.CurrencyInfo, currency);
        }

        public string Ticker(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));

            string pathParam = symbol + "/ticker24h";

            return Call(HttpMethod.Get, Endpoints.Public.Ticker, pathParam);
        }

        public string Tickers() => Call(HttpMethod.Get, Endpoints.Public.Tickers);

        public string OrderBook(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));

            string pathParam = symbol + "/orderBook";

            return Call(HttpMethod.Get, Endpoints.Public.OrderBook, pathParam);
        }

        public string Candles(string symbol, string interval, string limit = "", string startTime = "", string endTime = "")
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));

            if (string.IsNullOrWhiteSpace(interval))
                throw new ArgumentException("interval cannot be empty. ", nameof(interval));

            string pathParam = symbol + "/candles";

            var parameters = new Dictionary<string, string>
            {
                { "symbol", symbol },
                { "interval", interval }
            };

            if (!string.IsNullOrEmpty(limit))
                parameters.Add("limit", limit);
            if (!string.IsNullOrEmpty(startTime))
                parameters.Add("startTime", startTime);
            if (!string.IsNullOrEmpty(endTime))
                parameters.Add("endTime", endTime);

            return Call(HttpMethod.Get, Endpoints.Public.Candles, parameters);
        }

        public string Trades(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));

            string pathParam = symbol + "/trades";

            return Call(HttpMethod.Get, Endpoints.Public.Trades, pathParam);
        }

        #endregion
    }
}
