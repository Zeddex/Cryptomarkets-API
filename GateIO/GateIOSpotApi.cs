using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Cryptomarkets.Apis.GateIO
{
    public class GateIOSpotApi
    {
        private readonly string _secret;
        private const string ApiUrl = "https://api.gateio.ws";
        private readonly HttpClient _httpClient;

        public GateIOSpotApi(string apiKey, string apiSecret)
        {
            _secret = apiSecret;
            _httpClient = CreateAndConfigureHttpClient(apiKey);
        }

        private static HttpClient CreateAndConfigureHttpClient(string apiKey)
        {
            var handler = new HttpClientHandler();

            if (DebugMode.On)
                handler.Proxy = new WebProxy(new Uri(DebugMode.Proxy));

            handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            HttpClient configureHttpClient = new HttpClient(handler);

            configureHttpClient.BaseAddress = new Uri(ApiUrl);
            configureHttpClient.DefaultRequestHeaders.Add("KEY", apiKey);
            configureHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return configureHttpClient;
        }

        private string Call(HttpMethod method, string endpoint, string pathParam = null, string parameters = null, bool isSigned = false)
        {
            string data = null;
            string requestUri = endpoint + pathParam + (string.IsNullOrWhiteSpace(parameters) ? "" : string.Format("?{0}", parameters));

            if (isSigned)
            {
                string payloadEncoded;

                if (method.Method == "POST")
                {
                    requestUri = endpoint;
                    data = Extensions.QueryStringToJson(parameters);
                    parameters = "";
                    payloadEncoded = Extensions.EncodeSHA512(data);
                }
                else
                {
                    payloadEncoded = Extensions.EncodeSHA512("");
                }

                string url = endpoint + pathParam;
                string timestamp = Extensions.GetGateIOServerTime();
                string stringToSign = method + "\n" + url + "\n" + parameters + "\n" + payloadEncoded + "\n" + timestamp;
                string signature = Extensions.GenerateSignatureHMACSHA512(_secret, stringToSign);

                _httpClient.DefaultRequestHeaders.Add("Timestamp", timestamp);
                _httpClient.DefaultRequestHeaders.Add("SIGN", signature);
            }

            if (method.Method == "POST")
            {
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                return _httpClient.PostAsync(requestUri, content).Result.Content.ReadAsStringAsync().Result;
            }

            return _httpClient.SendAsync(new HttpRequestMessage(method, requestUri)).Result.Content.ReadAsStringAsync().Result;
        }

        #region Queries

        public string GetServerTime() => Call(HttpMethod.Get, Endpoints.Spot.GetServerTime);

        public string AllCurrencies() => Call(HttpMethod.Get, Endpoints.Spot.AllCurrencies);

        public string CurrencyDetails(string pair) => Call(HttpMethod.Get, Endpoints.Spot.CurrencyDetails, pair);

        public string CurrencyPairs() => Call(HttpMethod.Get, Endpoints.Spot.CurrencyPairs);

        public string CurrencyPair(string pair) => Call(HttpMethod.Get, Endpoints.Spot.CurrencyPair, pair);

        public string Tickers(string timezone = "all", string currencyPair = null)
        {
            string parameters = string.Format("timezone={0}", timezone) +
                (!string.IsNullOrWhiteSpace(currencyPair) ? string.Format("&currency_pair={0}", currencyPair) : "");

            return Call(HttpMethod.Get, Endpoints.Spot.Tickers, "", parameters);
        }

        public string OrderBook(string currencyPair, string interval = "", string limit = "", string withId = "false")
        {
            if (string.IsNullOrWhiteSpace(currencyPair))
                throw new ArgumentException("currencyPair cannot be empty. ", nameof(currencyPair));

            string parameters = string.Format("currency_pair={0}", currencyPair) +
                (!string.IsNullOrWhiteSpace(interval) ? string.Format("&interval={0}", interval) : "") +
                (!string.IsNullOrWhiteSpace(limit) ? string.Format("&limit={0}", limit) : "") +
                (!string.IsNullOrWhiteSpace(withId) ? string.Format("&withId={0}", withId) : "");

            return Call(HttpMethod.Get, Endpoints.Spot.OrderBook, "", parameters);
        }

        public string MarketTrades(
            string currencyPair, 
            string limit = "", 
            string lastId = "", 
            string reverse = "",
            string from = "",
            string to = "",
            string page = "")
        {
            if (string.IsNullOrWhiteSpace(currencyPair))
                throw new ArgumentException("currencyPair cannot be empty. ", nameof(currencyPair));

            string parameters = string.Format("currency_pair={0}", currencyPair) +
                (!string.IsNullOrWhiteSpace(limit) ? string.Format("&limit={0}", limit) : "") +
                (!string.IsNullOrWhiteSpace(lastId) ? string.Format("&last_id={0}", lastId) : "") +
                (!string.IsNullOrWhiteSpace(reverse) ? string.Format("&reverse={0}", reverse) : "") +
                (!string.IsNullOrWhiteSpace(from) ? string.Format("&from={0}", from) : "") +
                (!string.IsNullOrWhiteSpace(to) ? string.Format("&to={0}", to) : "") +
                (!string.IsNullOrWhiteSpace(page) ? string.Format("&page={0}", page) : "");

            return Call(HttpMethod.Get, Endpoints.Spot.MarketTrades, "", parameters);
        }

        public string Candlesticks(
            string currencyPair,
            string limit = "",
            string from = "",
            string to = "",
            string interval = "")
        {
            if (string.IsNullOrWhiteSpace(currencyPair))
                throw new ArgumentException("currencyPair cannot be empty. ", nameof(currencyPair));

            string parameters = string.Format("currency_pair={0}", currencyPair) +
                (!string.IsNullOrWhiteSpace(limit) ? string.Format("&limit={0}", limit) : "") +
                (!string.IsNullOrWhiteSpace(from) ? string.Format("&from={0}", from) : "") +
                (!string.IsNullOrWhiteSpace(to) ? string.Format("&to={0}", to) : "") +
                (!string.IsNullOrWhiteSpace(interval) ? string.Format("&interval={0}", interval) : "");

            return Call(HttpMethod.Get, Endpoints.Spot.Candlesticks, "", parameters);
        }

        public string FeeRates(string currencyPair = "")
        {
            string param = !string.IsNullOrWhiteSpace(currencyPair) ? string.Format("currency_pair={0}", currencyPair) : "";

            return Call(HttpMethod.Get, Endpoints.Spot.FeeRates, "", param, true);
        }

        public string SpotAccounts(string currency = "")
        {
            string param = !string.IsNullOrWhiteSpace(currency) ? string.Format("currency={0}", currency) : "";

            return Call(HttpMethod.Get, Endpoints.Spot.SpotAccounts, "", param, true);
        }

        public string OpenOrders(string page = "", string limit = "", string account = "")
        {
            string parameters =
                (!string.IsNullOrWhiteSpace(page) ? string.Format("page={0}", page) : "") +
                (!string.IsNullOrWhiteSpace(limit) ? string.Format("&limit={0}", limit) : "") +
                (!string.IsNullOrWhiteSpace(account) ? string.Format("&account={0}", account) : "");

            return Call(HttpMethod.Get, Endpoints.Spot.OpenOrders, "", parameters, true);
        }

        public string CreateOrder(
            string currencyPair,
            string side,
            string amount,
            string price,
            string text = "",
            string type = "",
            string account = "",
            string timeInForce = "",
            string iceberg = "",
            string autoBorrow = "",
            string autoRepay = "")
        {
            if (string.IsNullOrWhiteSpace(currencyPair))
                throw new ArgumentException("currencyPair cannot be empty. ", nameof(currencyPair));

            if (string.IsNullOrWhiteSpace(side))
                throw new ArgumentException("side cannot be empty. ", nameof(side));

            if (string.IsNullOrWhiteSpace(amount))
                throw new ArgumentException("amount cannot be empty. ", nameof(amount));

            if (string.IsNullOrWhiteSpace(price))
                throw new ArgumentException("price cannot be empty. ", nameof(price));

            string parameters = string.Format("currency_pair={0}&side={1}&amount={2}&price={3}", currencyPair, side, amount, price) +
                                (!string.IsNullOrWhiteSpace(text) ? string.Format("&text={0}", text) : "") +
                                (!string.IsNullOrWhiteSpace(type) ? string.Format("&type={0}", type) : "") +
                                (!string.IsNullOrWhiteSpace(account) ? string.Format("&account={0}", account) : "") +
                                (!string.IsNullOrWhiteSpace(timeInForce) ? string.Format("&time_in_force={0}", timeInForce) : "") +
                                (!string.IsNullOrWhiteSpace(iceberg) ? string.Format("&iceberg={0}", iceberg) : "") +
                                (!string.IsNullOrWhiteSpace(autoBorrow) ? string.Format("&auto_borrow={0}", autoBorrow) : "") +
                                (!string.IsNullOrWhiteSpace(autoRepay) ? string.Format("&auto_repay={0}", autoRepay) : "");

            return Call(HttpMethod.Post, Endpoints.Spot.CreateOrder, "", parameters, true);
        }

        public string OrdersList(
            string currencyPair,
            string status,
            string page = "",
            string limit = "",
            string account = "",
            string from = "",
            string to = "",
            string side = "")
        {
            if (string.IsNullOrWhiteSpace(currencyPair))
                throw new ArgumentException("currencyPair cannot be empty. ", nameof(currencyPair));

            if (string.IsNullOrWhiteSpace(status))
                throw new ArgumentException("status cannot be empty. ", nameof(status));

            string parameters = string.Format("currency_pair={0}&status={1}", currencyPair, status) +
                                (!string.IsNullOrWhiteSpace(page) ? string.Format("&page={0}", page) : "") +
                                (!string.IsNullOrWhiteSpace(limit) ? string.Format("&limit={0}", limit) : "") +
                                (!string.IsNullOrWhiteSpace(account) ? string.Format("&account={0}", account) : "") +
                                (!string.IsNullOrWhiteSpace(from) ? string.Format("&from={0}", from) : "") +
                                (!string.IsNullOrWhiteSpace(to) ? string.Format("&to={0}", to) : "") +
                                (!string.IsNullOrWhiteSpace(side) ? string.Format("&side={0}", side) : "");

            return Call(HttpMethod.Get, Endpoints.Spot.OrdersList, "", parameters, true);
        }

        public string CancelAllOrders(string currencyPair, string side = "", string account = "")
        {
            if (string.IsNullOrWhiteSpace(currencyPair))
                throw new ArgumentException("currencyPair cannot be empty. ", nameof(currencyPair));

            string parameters = string.Format("currency_pair={0}", currencyPair) +
                                (!string.IsNullOrWhiteSpace(side) ? string.Format("&side={0}", side) : "") +
                                (!string.IsNullOrWhiteSpace(account) ? string.Format("&account={0}", account) : "");

            return Call(HttpMethod.Delete, Endpoints.Spot.CancelAllOrders, "", parameters, true);
        }

        public string GetSingleOrder(string orderId, string currencyPair, string account = "")
        {
            if (string.IsNullOrWhiteSpace(orderId))
                throw new ArgumentException("orderId cannot be empty. ", nameof(orderId));

            if (string.IsNullOrWhiteSpace(currencyPair))
                throw new ArgumentException("currencyPair cannot be empty. ", nameof(currencyPair));


            string parameters = string.Format("currency_pair={0}", currencyPair) +
                                (!string.IsNullOrWhiteSpace(account) ? string.Format("&account={0}", account) : "");

            return Call(HttpMethod.Get, Endpoints.Spot.GetSingleOrder, orderId, parameters, true);
        }

        public string CancelOrder(string orderId, string currencyPair, string account = "")
        {
            if (string.IsNullOrWhiteSpace(orderId))
                throw new ArgumentException("orderId cannot be empty. ", nameof(orderId));

            if (string.IsNullOrWhiteSpace(currencyPair))
                throw new ArgumentException("currencyPair cannot be empty. ", nameof(currencyPair));


            string parameters = string.Format("order_id={0}&currency_pair={1}", orderId, currencyPair) +
                                (!string.IsNullOrWhiteSpace(account) ? string.Format("&account={0}", account) : "");

            return Call(HttpMethod.Delete, Endpoints.Spot.CancelOrder, orderId, parameters, true);
        }

        public string TradingHistory(
            string currencyPair,
            string limit = "",
            string page = "",
            string orderId = "",
            string account = "",
            string from = "",
            string to = "")
        {
            if (string.IsNullOrWhiteSpace(currencyPair))
                throw new ArgumentException("currencyPair cannot be empty. ", nameof(currencyPair));

            string parameters = string.Format("currency_pair={0}", currencyPair) +
                                (!string.IsNullOrWhiteSpace(limit) ? string.Format("&limit={0}", limit) : "") +
                                (!string.IsNullOrWhiteSpace(page) ? string.Format("&page={0}", page) : "") +
                                (!string.IsNullOrWhiteSpace(orderId) ? string.Format("&order_id={0}", orderId) : "") +
                                (!string.IsNullOrWhiteSpace(account) ? string.Format("&account={0}", account) : "") +
                                (!string.IsNullOrWhiteSpace(from) ? string.Format("&from={0}", from) : "") +
                                (!string.IsNullOrWhiteSpace(to) ? string.Format("&to={0}", to) : "");

            return Call(HttpMethod.Get, Endpoints.Spot.TradingHistory, "", parameters, true);
        }

        public string CountdownCancelOrders(string timeout, string currencyPair = "")
        {
            if (string.IsNullOrWhiteSpace(timeout))
                throw new ArgumentException("timeout cannot be empty. ", nameof(timeout));

            string parameters = string.Format("currency_pair={0}", currencyPair) +
                                (!string.IsNullOrWhiteSpace(currencyPair) ? string.Format("&currency_pair={0}", currencyPair) : "");

            return Call(HttpMethod.Post, Endpoints.Spot.CountdownCancelOrders, "", parameters, true);
        }

        #endregion
    }
}
