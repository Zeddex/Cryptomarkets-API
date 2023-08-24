using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Cryptomarkets.Apis.Binance
{
    public class PublicApi
    {
        private const string ApiUrl = "https://api.binance.com";
        private readonly HttpClient _httpClient;

        internal PublicApi() => _httpClient = CreateAndConfigureHttpClient();

        private static HttpClient CreateAndConfigureHttpClient()
        {
            HttpClientHandler handler = new HttpClientHandler();

            if (DebugMode.On)
                handler.Proxy = new WebProxy(new Uri(DebugMode.Proxy));

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

        private string Call(HttpMethod method, string endpoint, string parameters = null)
        {
            string requestUri = endpoint + (string.IsNullOrWhiteSpace(parameters) ? "" : string.Format("?{0}", parameters));

            return _httpClient.SendAsync(new HttpRequestMessage(method, requestUri)).Result.Content.ReadAsStringAsync().Result;
        }

        /// <summary>/api/v3/ping</summary>
        /// <returns></returns>
        public string TestConnectivity() => Call(HttpMethod.Get, Endpoints.Public.TestConnectivity);

        /// <summary>/api/v3/time</summary>
        /// <returns></returns>
        public string GetServerTime() => Call(HttpMethod.Get, Endpoints.Public.GetServerTime);

        /// <summary>
        /// /api/v3/exchangeInfo
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public string GetExchangeInfo(string symbol = null)
        {
            string parameter = !string.IsNullOrWhiteSpace(symbol) ? string.Format("&symbol={0}", symbol.ToUpper()) : "";

            return Call(HttpMethod.Get, Endpoints.Public.GetExchangeInfo, parameter);
        }

        /// <summary>/api/v3/ticker/price</summary>
        /// <returns></returns>
        public string GetAllPrices() => Call(HttpMethod.Get, Endpoints.Public.GetAllPrices);

        /// <summary>
        /// /api/v3/ticker/price
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public string GetSymbolPrice(string symbol = null)
        {
            string parameter = string.IsNullOrWhiteSpace(symbol) ? "" : string.Format("symbol={0}", symbol.ToUpper());

            return Call(HttpMethod.Get, Endpoints.Public.GetSymbolPrice, parameter);
        }

        /// <summary>
        /// /api/v3/ticker/bookTicker
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public string GetOrderBookTicker(string symbol = null)
        {
            string parameter = string.IsNullOrWhiteSpace(symbol) ? "" : string.Format("symbol={0}", symbol.ToUpper());

            return Call(HttpMethod.Get, Endpoints.Public.GetOrderBookTicker, parameter);
        }
            
        /// <summary>/api/v3/depth</summary>
        /// <param name="symbol"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public string GetOrderBook(string symbol, string limit = "100")
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));

            return Call(HttpMethod.Get, Endpoints.Public.GetOrderBook, string.Format("symbol={0}&limit={1}", symbol.ToUpper(), limit));
        }

        /// <summary>/api/v3/aggTrades</summary>
        /// <param name="symbol"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public string GetAggregateTrades(string symbol, string limit = "500")
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));

            return Call(HttpMethod.Get, Endpoints.Public.GetAggregateTrades, string.Format("symbol={0}&limit={1}", symbol.ToUpper(), limit));
        }

        /// <summary>/api/v3/ticker/24hr</summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public string GetPriceChange24H(string symbol = null)
        {
            string parameter = string.IsNullOrWhiteSpace(symbol) ? "" : string.Format("symbol={0}", symbol.ToUpper());

            return Call(HttpMethod.Get, Endpoints.Public.GetPriceChange24H, parameter);
        }

        /// <summary>
        /// /api/v3/trades
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public string GetRecentTradesList(string symbol, string limit = "500")
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));

            return Call(HttpMethod.Get, Endpoints.Public.GetRecentTradesList, string.Format("symbol={0}&limit={1}", symbol.ToUpper(), limit));
        }

        /// <summary>
        /// /api/v3/avgPrice
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public string GetCurrentAvgPrice(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));

            return Call(HttpMethod.Get, Endpoints.Public.GetCurrentAvgPrice, string.Format("symbol={0}", symbol.ToUpper()));
        }

        /// <summary>
        /// /api/v3/klines
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public string KlineCandlestickData(string symbol, string interval, long startTime = 0, long endTime = 0, int limit = 500)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));

            if (string.IsNullOrWhiteSpace(interval))
                throw new ArgumentException("interval cannot be empty. ", nameof(interval));

            string parameters = string.Format("symbol={0}&interval={1}&limit={2}", symbol.ToUpper(), interval, limit) +
                (startTime != 0 ? string.Format("&startTime={0}", startTime) : "") +
                (endTime != 0 ? string.Format("&endTime={0}", endTime) : "");

            return Call(HttpMethod.Get, Endpoints.Public.GetKlineCandlestickData, parameters);
        }
    }
}
