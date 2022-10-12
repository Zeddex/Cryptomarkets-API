using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Cryptomarkets.Apis.Binance
{
    public class BinanceSpotTradeApi
    {
        private readonly string _secret;
        private const string ApiUrl = "https://api.binance.com";
        private readonly HttpClient _httpClient;

        internal BinanceSpotTradeApi(string apiKey, string apiSecret)
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

        #region Queries

        /// <summary>/api/v3/account</summary>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string AccountInfo(long recvWindow = 5000)
        {
            return Call(HttpMethod.Get, Endpoints.SpotTrade.AccountInfo, true, string.Format("recvWindow={0}", recvWindow));
        }

        /// <summary>
        /// /api/v3/myTrades
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="orderId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="fromId"></param>
        /// <param name="limit"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public string AccountTradeList(
            string symbol = null,
            string orderId = null,
            string startTime = null,
            string endTime = null,
            string fromId = null,
            string limit = "500",
            long recvWindow = 5000)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));

            string parameters = string.Format("symbol={0}&limit={1}&recvWindow={2}", symbol.ToUpper(), limit, recvWindow) +
                (!string.IsNullOrWhiteSpace(orderId) ? string.Format("&orderId={0}", orderId) : "") +
                (!string.IsNullOrWhiteSpace(startTime) ? string.Format("&startTime={0}", startTime) : "") +
                (!string.IsNullOrWhiteSpace(endTime) ? string.Format("&endTime={0}", endTime) : "") +
                (!string.IsNullOrWhiteSpace(fromId) ? string.Format("&fromId={0}", fromId) : "");

            return Call(HttpMethod.Get, Endpoints.SpotTrade.AccountTradeList, true, parameters);
        }

        /// <summary>
        /// /api/v3/rateLimit/order
        /// </summary>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string QueryCurrentOrderUsage(long recvWindow = 5000)
        {
            return Call(HttpMethod.Get, Endpoints.SpotTrade.QueryCurrentOrderUsage, true, string.Format("recvWindow={0}", recvWindow));
        }

        /// <summary>
        /// /api/v3/allOrders
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="orderId"></param>
        /// <param name="limit"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public string AllOrders(
            string symbol,
            string orderId = null,
            string limit = "500",
            string startTime = null,
            string endTime = null,
            long recvWindow = 5000)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));

            string parameters = string.Format("symbol={0}&limit={1}&recvWindow={2}", symbol.ToUpper(), limit, recvWindow) +
                (!string.IsNullOrWhiteSpace(orderId) ? string.Format("&orderId={0}", orderId) : "") +
                (!string.IsNullOrWhiteSpace(startTime) ? string.Format("&startTime={0}", startTime) : "") +
                (!string.IsNullOrWhiteSpace(endTime) ? string.Format("&endTime={0}", endTime) : "");

            return Call(HttpMethod.Get, Endpoints.SpotTrade.AllOrders, true, parameters);
        }

        /// <summary>/api/v3/order/test</summary>
        /// <param name="symbol">Ticker symbol.</param>
        /// <param name="orderSide">Order side (BUY-SELL).</param>
        /// <param name="orderType">Order type (LIMIT-MARKET).</param>
        /// <param name="quantity">Quantity to transaction.</param>
        /// <param name="price">Price of the transaction.</param>
        /// <param name="quoteOrderQty"></param>
        /// <param name="newClientOrderId"></param>
        /// <param name="stopPrice"></param>
        /// <param name="trailingDelta"></param>
        /// <param name="newOrderRespType"></param>
        /// <param name="timeInForce">Indicates how long an order will remain active before it is executed or expires.</param>
        /// <param name="icebergQty"></param>
        /// <param name="recvWindow">Specific number of milliseconds the request is valid for.</param>
        /// <returns></returns>
        public string TestNewOrder(
          string symbol,
          string orderSide,
          string orderType,
          string quantity = null,
          string price = null,
          string quoteOrderQty = null,
          string newClientOrderId = null,
          string stopPrice = null,
          string trailingDelta = null,
          string newOrderRespType = null,
          string timeInForce = "GTC",
          decimal icebergQty = 0M,
          long recvWindow = 5000)
        {
            string parameters = string.Format("symbol={0}&side={1}&type={2}&quantity={3}", symbol.ToUpper(), orderSide, orderType, quantity) +
                (orderType == "LIMIT" ? string.Format("&timeInForce={0}", timeInForce) : "") +
                (orderType == "LIMIT" ? string.Format("&price={0}", price) : "") +
                (icebergQty > 0M ? string.Format("&icebergQty={0}", icebergQty) : "") +
                string.Format("&recvWindow={0}", recvWindow) +
                (!string.IsNullOrWhiteSpace(quoteOrderQty) ? string.Format("&quoteOrderQty={0}", quoteOrderQty) : "") +
                (!string.IsNullOrWhiteSpace(newClientOrderId) ? string.Format("&newClientOrderId={0}", newClientOrderId) : "") +
                (!string.IsNullOrWhiteSpace(stopPrice) ? string.Format("&stopPrice={0}", stopPrice) : "") +
                (!string.IsNullOrWhiteSpace(trailingDelta) ? string.Format("&trailingDelta={0}", trailingDelta) : "") +
                (!string.IsNullOrWhiteSpace(newOrderRespType) ? string.Format("&newOrderRespType={0}", newOrderRespType) : "");

            //string parameters2 = string.Format("symbol={0}&side={1}&type={2}&quantity={3}&price={4}&quoteOrderQty={5}&newClientOrderId={6}" +
            //    "&stopPrice={7}&trailingDelta={8}&newOrderRespType={9}&timeInForce={10}&icebergQty={11}&recvWindow={12}",
            //    symbol.ToUpper(), orderSide, orderType, quantity, price, quoteOrderQty, newClientOrderId, stopPrice,
            //    trailingDelta, newOrderRespType, timeInForce, icebergQty, recvWindow);

            return Call(HttpMethod.Post, Endpoints.SpotTrade.TestNewOrder, true, parameters);
        }

        /// <summary>/api/v3/order</summary>
        /// <param name="symbol">Ticker symbol.</param>
        /// <param name="orderSide">Order side (BUY-SELL).</param>
        /// <param name="orderType">Order type (LIMIT-MARKET).</param>
        /// <param name="quantity">Quantity to transaction.</param>
        /// <param name="price">Price of the transaction.</param>
        /// <param name="quoteOrderQty"></param>
        /// <param name="newClientOrderId"></param>
        /// <param name="stopPrice"></param>
        /// <param name="trailingDelta"></param>
        /// <param name="newOrderRespType"></param>
        /// <param name="timeInForce">Indicates how long an order will remain active before it is executed or expires.</param>
        /// <param name="icebergQty"></param>
        /// <param name="recvWindow">Specific number of milliseconds the request is valid for.</param>
        /// <returns></returns>
        public string NewOrder(
          string symbol,
          string orderSide,
          string orderType,
          string quantity = null,
          string price = null,
          string quoteOrderQty = null,
          string newClientOrderId = null,
          string stopPrice = null,
          string trailingDelta = null,
          string newOrderRespType = null,
          string timeInForce = "GTC",
          decimal icebergQty = 0M,
          long recvWindow = 5000)
        {
            string parameters = string.Format("symbol={0}&side={1}&type={2}&quantity={3}", symbol.ToUpper(), orderSide, orderType, quantity) +
                (orderType == "LIMIT" ? string.Format("&timeInForce={0}", timeInForce) : "") +
                (orderType == "LIMIT" ? string.Format("&price={0}", price) : "") +
                (icebergQty > 0M ? string.Format("&icebergQty={0}", icebergQty) : "") +
                string.Format("&recvWindow={0}", recvWindow) +
                (!string.IsNullOrWhiteSpace(quoteOrderQty) ? string.Format("&quoteOrderQty={0}", quoteOrderQty) : "") +
                (!string.IsNullOrWhiteSpace(newClientOrderId) ? string.Format("&newClientOrderId={0}", newClientOrderId) : "") +
                (!string.IsNullOrWhiteSpace(stopPrice) ? string.Format("&stopPrice={0}", stopPrice) : "") +
                (!string.IsNullOrWhiteSpace(trailingDelta) ? string.Format("&trailingDelta={0}", trailingDelta) : "") +
                (!string.IsNullOrWhiteSpace(newOrderRespType) ? string.Format("&newOrderRespType={0}", newOrderRespType) : "");

            //string parameters2 = string.Format("symbol={0}&side={1}&type={2}&quantity={3}&price={4}&quoteOrderQty={5}&newClientOrderId={6}" +
            //    "&stopPrice={7}&trailingDelta={8}&newOrderRespType={9}&timeInForce={10}&icebergQty={11}&recvWindow={12}",
            //    symbol.ToUpper(), orderSide, orderType, quantity, price, quoteOrderQty, newClientOrderId, stopPrice,
            //    trailingDelta, newOrderRespType, timeInForce, icebergQty, recvWindow);

            return Call(HttpMethod.Post, Endpoints.SpotTrade.NewOrder, true, parameters);
        }

        /// <summary>/api/v3/order</summary>
        /// <param name="symbol"></param>
        /// <param name="orderId"></param>
        /// <param name="origClientOrderId"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string QueryOrder(
            string symbol, 
            string orderId = null, 
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

            return Call(HttpMethod.Get, Endpoints.SpotTrade.QueryOrder, true, parameters);
        }

        /// <summary>/api/v3/openOrders</summary>
        /// <param name="symbol"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string CurrentOpenOrders(string symbol = null, long recvWindow = 5000)
        {
            string parameters = string.Format("symbol={0}&recvWindow={1}", symbol.ToUpper(), recvWindow);

            return Call(HttpMethod.Get, Endpoints.SpotTrade.CurrentOpenOrders, true, parameters);
        }

        /// <summary>
        /// /api/v3/order
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="orderId"></param>
        /// <param name="origClientOrderId"></param>
        /// <param name="newClientOrderId"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public string CancelOrder(
            string symbol, 
            string orderId = null, 
            string origClientOrderId = null,
            string newClientOrderId = null,
            long recvWindow = 5000)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));

            string parameters = string.Format("symbol={0}&recvWindow={1}", symbol.ToUpper(), recvWindow) +
                (!string.IsNullOrWhiteSpace(orderId) ? string.Format("&orderId={0}", orderId) : "") +
                (!string.IsNullOrWhiteSpace(origClientOrderId) ? string.Format("&origClientOrderId={0}", origClientOrderId) : "") +
                (!string.IsNullOrWhiteSpace(newClientOrderId) ? string.Format("&newClientOrderId={0}", newClientOrderId) : "");

            return Call(HttpMethod.Delete, Endpoints.SpotTrade.CancelOrder, true, parameters);
        }

        /// <summary>
        /// /api/v3/openOrders
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public string CancelAllOrders(string symbol, long recvWindow = 5000)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));
            
            return Call(HttpMethod.Delete, Endpoints.SpotTrade.CancelAllOrders, true, string.Format("symbol={0}&recvWindow={1}", symbol.ToUpper(), recvWindow));
        }

        /// <summary>
        /// /api/v3/order/oco
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="side"></param>
        /// <param name="quantity"></param>
        /// <param name="price"></param>
        /// <param name="stopPrice"></param>
        /// <param name="listClientOrderId"></param>
        /// <param name="limitClientOrderId"></param>
        /// <param name="limitIcebergQty"></param>
        /// <param name="trailingDelta"></param>
        /// <param name="stopClientOrderId"></param>
        /// <param name="stopLimitPrice"></param>
        /// <param name="stopIcebergQty"></param>
        /// <param name="stopLimitTimeInForce"></param>
        /// <param name="newOrderRespType"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string NewOCO(
            string symbol, 
            string side, 
            string quantity, 
            string price, 
            string stopPrice,
            string listClientOrderId = null,
            string limitClientOrderId = null,
            string limitIcebergQty = null,
            string trailingDelta = null,
            string stopClientOrderId = null,
            string stopLimitPrice = null,
            string stopIcebergQty = null,
            string stopLimitTimeInForce = null,
            string newOrderRespType = null,
            long recvWindow = 5000)
        {
            string parameters = string.Format("symbol={0}&side={1}&quantity={2}&price={3}&stopPrice={4}&recvWindow={5}",
                symbol.ToUpper(), side, quantity, price, stopPrice, recvWindow) +
                (!string.IsNullOrWhiteSpace(listClientOrderId) ? string.Format("&listClientOrderId={0}", listClientOrderId) : "") +
                (!string.IsNullOrWhiteSpace(limitClientOrderId) ? string.Format("&limitClientOrderId={0}", limitClientOrderId) : "") +
                (!string.IsNullOrWhiteSpace(limitIcebergQty) ? string.Format("&limitIcebergQty={0}", limitIcebergQty) : "") +
                (!string.IsNullOrWhiteSpace(trailingDelta) ? string.Format("&trailingDelta={0}", trailingDelta) : "") +
                (!string.IsNullOrWhiteSpace(stopClientOrderId) ? string.Format("&stopClientOrderId={0}", stopClientOrderId) : "") +
                (!string.IsNullOrWhiteSpace(stopLimitPrice) ? string.Format("&stopLimitPrice={0}", stopLimitPrice) : "") +
                (!string.IsNullOrWhiteSpace(stopIcebergQty) ? string.Format("&stopIcebergQty={0}", stopIcebergQty) : "") +
                (!string.IsNullOrWhiteSpace(stopLimitTimeInForce) ? string.Format("&stopLimitTimeInForce={0}", stopLimitTimeInForce) : "") +
                (!string.IsNullOrWhiteSpace(newOrderRespType) ? string.Format("&newOrderRespType={0}", newOrderRespType) : "");

            return Call(HttpMethod.Post, Endpoints.SpotTrade.NewOCO, true, parameters);
        }

        /// <summary>
        /// /api/v3/orderList
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="orderListId"></param>
        /// <param name="listClientOrderId"></param>
        /// <param name="newClientOrderId"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public string CancelOCO(
            string symbol,
            string orderListId = null,
            string listClientOrderId = null,
            string newClientOrderId = null,
            long recvWindow = 5000)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));

            string parameters = string.Format("symbol={0}&recvWindow={1}", symbol.ToUpper(), recvWindow) +
                (!string.IsNullOrWhiteSpace(orderListId) ? string.Format("&orderListId={0}", orderListId) : "") +
                (!string.IsNullOrWhiteSpace(listClientOrderId) ? string.Format("&listClientOrderId={0}", listClientOrderId) : "") +
                (!string.IsNullOrWhiteSpace(newClientOrderId) ? string.Format("&newClientOrderId={0}", newClientOrderId) : "");

            return Call(HttpMethod.Delete, Endpoints.SpotTrade.CancelOCO, true, parameters);
        }

        /// <summary>
        /// /api/v3/orderList
        /// </summary>
        /// <param name="orderListId"></param>
        /// <param name="origClientOrderId"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string QueryOCO(string orderListId = null, string origClientOrderId = null, long recvWindow = 5000)
        {
            string parameters = string.Format("recvWindow={0}", recvWindow) +
                (!string.IsNullOrWhiteSpace(orderListId) ? string.Format("&orderListId={0}", orderListId) : "") +
                (!string.IsNullOrWhiteSpace(origClientOrderId) ? string.Format("&origClientOrderId={0}", origClientOrderId) : "");

            return Call(HttpMethod.Get, Endpoints.SpotTrade.QueryOCO, true, parameters);
        }

        /// <summary>
        /// /api/v3/allOrderList
        /// </summary>
        /// <param name="fromId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="limit"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string QueryAllOCO(
            string fromId = null,
            string startTime = null,
            string endTime = null,
            string limit = "500",
            long recvWindow = 5000)
        {
            string parameters = string.Format("limit={0}&recvWindow={1}", limit, recvWindow) +
                (!string.IsNullOrWhiteSpace(fromId) ? string.Format("&fromId={0}", fromId) : "") +
                (!string.IsNullOrWhiteSpace(startTime) ? string.Format("&startTime={0}", startTime) : "") +
                (!string.IsNullOrWhiteSpace(endTime) ? string.Format("&endTime={0}", endTime) : "");

            return Call(HttpMethod.Get, Endpoints.SpotTrade.QueryAllOCO, true, parameters);
        }

        /// <summary>
        /// /api/v3/openOrderList
        /// </summary>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string QueryOpenOCO(long recvWindow = 5000)
        {
            return Call(HttpMethod.Get, Endpoints.SpotTrade.QueryOpenOCO, true, string.Format("recvWindow={0}", recvWindow));
        }

        #endregion
    }
}
