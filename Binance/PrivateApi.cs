using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Cryptomarkets.Apis.Binance
{
    [Obsolete("Please use BinanceTradeApi instead")]
    public class PrivateApi
    {
        private readonly string _secret;
        private const string ApiUrl = "https://api.binance.com";
        private readonly HttpClient _httpClient;

        internal PrivateApi(string apiKey, string apiSecret)
        {
            _secret = apiSecret;
            _httpClient = CreateAndConfigureHttpClient(apiKey);
        }

        private static HttpClient CreateAndConfigureHttpClient(string apiKey)
        {
            HttpClientHandler handler = new HttpClientHandler();

            if (DebugMode.On)
                handler.Proxy = new WebProxy(new Uri(DebugMode.Proxy));

            handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            HttpClient configureHttpClient = new HttpClient(handler);

            configureHttpClient.BaseAddress = new Uri(ApiUrl);
            configureHttpClient.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);
            configureHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return configureHttpClient;
        }

        private string Call(HttpMethod method, string endpoint, bool isSigned = false, string parameters = null)
        {
            string requestUri = endpoint + (string.IsNullOrWhiteSpace(parameters) ? "" : string.Format("?{0}", parameters));

            if (isSigned)
            {
                parameters = parameters + (!string.IsNullOrWhiteSpace(parameters) ? "&timestamp=" : "timestamp=") + Extensions.GenerateTimeStamp();
                string signature = Extensions.GenerateSignatureHMACSHA256(_secret, parameters);
                requestUri = string.Format("{0}?{1}&signature={2}", endpoint, parameters, signature);
            }
            return _httpClient.SendAsync(new HttpRequestMessage(method, requestUri)).Result.Content.ReadAsStringAsync().Result;
        }

        /// <summary>/api/v3/account</summary>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string GetAccountInfo(long recvWindow = 5000) => Call(HttpMethod.Get, Endpoints.Private.GetAccountInfo, true, string.Format("recvWindow={0}", recvWindow));

        /// <summary>/api/v3/order/test</summary>
        /// <param name="symbol">Ticker symbol.</param>
        /// <param name="quantity">Quantity to transaction.</param>
        /// <param name="price">Price of the transaction.</param>
        /// <param name="orderType">Order type (LIMIT-MARKET).</param>
        /// <param name="orderSide">Order side (BUY-SELL).</param>
        /// <param name="timeInForce">Indicates how long an order will remain active before it is executed or expires.</param>
        /// <param name="recvWindow">Specific number of milliseconds the request is valid for.</param>
        /// <returns></returns>
        public string PostNewOrderTest(
          string symbol,
          string quantity,
          string price,
          string orderSide,
          string orderType = "LIMIT",
          string timeInForce = "GTC",
          Decimal icebergQty = 0M,
          long recvWindow = 5000)
        {
            return Call(HttpMethod.Post, Endpoints.Private.PostNewOrderTest, true, string.Format("symbol={0}&side={1}&type={2}&quantity={3}", symbol.ToUpper(), orderSide, orderType, quantity) + 
                (orderType == "LIMIT" ? string.Format("&timeInForce={0}", timeInForce) : "") + 
                (orderType == "LIMIT" ? string.Format("&price={0}", price) : "") + 
                (icebergQty > 0M ? string.Format("&icebergQty={0}", icebergQty) : "") + 
                string.Format("&recvWindow={0}", recvWindow));
        }

        /// <summary>/api/v3/order</summary>
        /// <param name="symbol">Ticker symbol.</param>
        /// <param name="quantity">Quantity to transaction.</param>
        /// <param name="price">Price of the transaction.</param>
        /// <param name="orderType">Order type (LIMIT-MARKET).</param>
        /// <param name="orderSide">Order side (BUY-SELL).</param>
        /// <param name="timeInForce">Indicates how long an order will remain active before it is executed or expires.</param>
        /// <param name="recvWindow">Specific number of milliseconds the request is valid for.</param>
        /// <returns></returns>
        public string PostNewOrder(
          string symbol,
          string quantity,
          string price,
          string orderSide,
          string orderType = "LIMIT",
          string timeInForce = "GTC",
          Decimal icebergQty = 0M,
          long recvWindow = 5000)
        {
            return Call(HttpMethod.Post, Endpoints.Private.PostNewOrder, true, string.Format("symbol={0}&side={1}&type={2}&quantity={3}", symbol.ToUpper(), orderSide, orderType, quantity) + 
                (orderType == "LIMIT" ? string.Format("&timeInForce={0}", timeInForce) : "") + 
                (orderType == "LIMIT" ? string.Format("&price={0}", price) : "") + 
                (icebergQty > 0M ? string.Format("&icebergQty={0}", icebergQty) : "") + 
                string.Format("&recvWindow={0}", recvWindow));
        }

        /// <summary>/api/v3/myTrades</summary>
        /// <param name="symbol"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string GetTradeList(string symbol, long recvWindow = 5000)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));

            return Call(HttpMethod.Get, Endpoints.Private.GetTradeList, true, string.Format("symbol={0}&recvWindow={1}", symbol.ToUpper(), recvWindow));
        }

        /// <summary>/api/v3/allOrders</summary>
        /// <param name="symbol"></param>
        /// <param name="orderId"></param>
        /// <param name="limit"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string GetAllOrders(string symbol, string orderId = "", string limit = "500", long recvWindow = 5000)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));

            return Call(HttpMethod.Get, Endpoints.Private.GetAllOrders, true, string.Format("symbol={0}&limit={1}&recvWindow={2}", symbol.ToUpper(), limit, recvWindow) + 
                (!string.IsNullOrWhiteSpace(orderId) ? string.Format("&orderId={0}", orderId) : ""));
        }

        /// <summary>/api/v3/openOrders</summary>
        /// <param name="symbol"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string GetCurrentOpenOrders(string symbol, long recvWindow = 5000)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));

            return Call(HttpMethod.Get, Endpoints.Private.GetCurrentOpenOrders, true, string.Format("symbol={0}&recvWindow={1}", symbol.ToUpper(), recvWindow));
        }

        /// <summary>/api/v3/order</summary>
        /// <param name="symbol"></param>
        /// <param name="orderId"></param>
        /// <param name="origClientOrderId"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string GetOrder(
          string symbol,
          string orderId = "",
          string origClientOrderId = null,
          long recvWindow = 5000)
        {
            string str = string.Format("symbol={0}&recvWindow={1}", symbol.ToUpper(), recvWindow);

            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));

            string parameters;

            if (!string.IsNullOrWhiteSpace(orderId))
            {
                parameters = str + string.Format("&orderId={0}", orderId);
            }

            else
            {
                if (string.IsNullOrWhiteSpace(origClientOrderId))
                    throw new ArgumentException("Either orderId or origClientOrderId must be sent.");

                parameters = str + string.Format("&origClientOrderId={0}", origClientOrderId);
            }

            return Call(HttpMethod.Get, Endpoints.Private.GetOrder, true, parameters);
        }

        /// <summary>/api/v3/order</summary>
        /// <param name="symbol"></param>
        /// <param name="orderId"></param>
        /// <param name="origClientOrderId"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string CancelOrder(
          string symbol,
          string orderId = "",
          string origClientOrderId = null,
          long recvWindow = 5000)
        {
            string str = !string.IsNullOrWhiteSpace(symbol) 
                ? string.Format("symbol={0}&recvWindow={1}", symbol.ToUpper(), recvWindow) 
                : throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));

            string parameters;

            if (!string.IsNullOrWhiteSpace(orderId))
            {
                parameters = str + string.Format("&orderId={0}", orderId);
            }

            else
            {
                if (!string.IsNullOrWhiteSpace(origClientOrderId))
                    throw new ArgumentException("Either orderId or origClientOrderId must be sent.");

                parameters = str + string.Format("&origClientOrderId={0}", origClientOrderId);
            }

            return Call(HttpMethod.Delete, Endpoints.Private.CancelOrder, true, parameters);
        }
    }
}
