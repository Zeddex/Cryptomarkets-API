using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Cryptomarkets.Apis.GateIO
{
    public class GateIOPublicApi
    {
        private const string ApiUrl = "https://data.gateapi.io";
        private readonly HttpClient _httpClient;

        internal GateIOPublicApi() => _httpClient = CreateAndConfigureHttpClient();

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

        private string Call(HttpMethod method, string endpoint, string param = null)
        {
            string requestUri = endpoint + param;

            return _httpClient.SendAsync(new HttpRequestMessage(method, requestUri)).Result.Content.ReadAsStringAsync().Result;
        }

        #region Queries

        public string TraidingPairs() => Call(HttpMethod.Get, Endpoints.Public.TraidingPairs);

        public string MarketInfo() => Call(HttpMethod.Get, Endpoints.Public.MarketInfo);

        public string CoinInfo() => Call(HttpMethod.Get, Endpoints.Public.CoinInfo);

        public string MarketDetails() => Call(HttpMethod.Get, Endpoints.Public.MarketDetails);

        public string Tickers() => Call(HttpMethod.Get, Endpoints.Public.Tickers);

        public string Ticker(string pair) => Call(HttpMethod.Get, Endpoints.Public.Ticker, pair);

        public string OrderBooks() => Call(HttpMethod.Get, Endpoints.Public.OrderBooks);

        public string OrderBook(string pair) => Call(HttpMethod.Get, Endpoints.Public.OrderBook, pair);

        public string TradeHistory(string pair) => Call(HttpMethod.Get, Endpoints.Public.TradeHistory, pair);

        public string P2PDepth() => Call(HttpMethod.Get, Endpoints.Public.P2PDepth);

        public string CurrentP2PDepth(string pair) => Call(HttpMethod.Get, Endpoints.Public.CurrentP2PDepth, pair);

        #endregion
    }
}