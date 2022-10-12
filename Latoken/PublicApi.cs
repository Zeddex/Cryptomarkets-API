using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Cryptomarkets.Apis.Latoken
{
    public class PublicApi
    {
        private const string ApiUrl = "https://api.latoken.com";
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

        public string GetActiveCurrencies() => Call(HttpMethod.Get, Endpoints.Public.ActiveCurrencies);

        public string GetAvailableCurrencies() => Call(HttpMethod.Get, Endpoints.Public.AvailableCurrencies);

        public string GetActivePairs() => Call(HttpMethod.Get, Endpoints.Public.ActivePairs);

        public string GetAvailablePairs() => Call(HttpMethod.Get, Endpoints.Public.AvailablePairs);

        public string GetAllTickers() => Call(HttpMethod.Get, Endpoints.Public.AllTickers);

        public string GetTickerForPair(string baseCurrency, string quoteCurrency)
        {
            if (string.IsNullOrWhiteSpace(baseCurrency))
                throw new ArgumentException("baseCurrency cannot be empty. ", nameof(baseCurrency));

            if (string.IsNullOrWhiteSpace(quoteCurrency))
                throw new ArgumentException("quoteCurrency cannot be empty. ", nameof(quoteCurrency));

            string pathParam = $"{baseCurrency}/{quoteCurrency}";

            return Call(HttpMethod.Get, Endpoints.Public.TickerForPair, pathParam);
        }

        public string GetCurrencyByTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                throw new ArgumentException("id cannot be empty. ", nameof(tag));

            string pathParam = tag;

            return Call(HttpMethod.Get, Endpoints.Public.CurrencyByTag, pathParam);
        }

        public string GetOrderBookByPair(string baseCurrency, string quoteCurrency)
        {
            if (string.IsNullOrWhiteSpace(baseCurrency))
                throw new ArgumentException("baseCurrency cannot be empty. ", nameof(baseCurrency));

            if (string.IsNullOrWhiteSpace(quoteCurrency))
                throw new ArgumentException("quoteCurrency cannot be empty. ", nameof(quoteCurrency));

            string pathParam = $"{baseCurrency}/{quoteCurrency}";

            return Call(HttpMethod.Get, Endpoints.Public.OrderBookByPair, pathParam);
        }

        public string GetAnyTradesByPair(string currency, string quote)
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("currency cannot be empty. ", nameof(currency));

            if (string.IsNullOrWhiteSpace(quote))
                throw new ArgumentException("quote cannot be empty. ", nameof(quote));

            string pathParam = $"{currency}/{quote}";

            return Call(HttpMethod.Get, Endpoints.Public.AnyTradesByPair, pathParam);
        }

        #endregion
    }
}
