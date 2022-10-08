using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;

namespace Cryptomarkets.Apis.Binance
{
    public class BinanceMarginTradeApi
    {
        private readonly string _secret;
        private const string ApiUrl = "https://api.binance.com";
        private readonly HttpClient _httpClient;

        internal BinanceMarginTradeApi(string apiKey, string apiSecret)
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
                string signature = Extensions.GenerateSignature(_secret, parameters);
                requestUri = string.Format("{0}?{1}&signature={2}", endpoint, parameters, signature);
            }
            return _httpClient.SendAsync(new HttpRequestMessage(method, requestUri)).Result.Content.ReadAsStringAsync().Result;
        }

        #region Queries

        /// <summary>
        /// /sapi/v1/margin/transfer
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="amount"></param>
        /// <param name="type"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string Transfer(string asset, string amount, int type, long recvWindow = 5000)
        {
            if (string.IsNullOrWhiteSpace(asset))
                throw new ArgumentException("asset cannot be empty. ", nameof(asset));

            if (string.IsNullOrWhiteSpace(amount))
                throw new ArgumentException("amount cannot be empty. ", nameof(amount));

            if (type < 1 || type > 2)
                throw new ArgumentException("wrong type parameter", nameof(type));

            string parameters = string.Format("asset={0}&amount={1}&type={2}&recvWindow={3}", asset, amount, type, recvWindow);

            return Call(HttpMethod.Post, Endpoints.MarginTrade.Transfer, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/margin/loan
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="isIsolated"></param>
        /// <param name="symbol"></param>
        /// <param name="amount"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string Borrow( string asset, string amount, string isIsolated = null, string symbol = null, long recvWindow = 5000)
        {
            if (string.IsNullOrWhiteSpace(asset))
                throw new ArgumentException("asset cannot be empty. ", nameof(asset));

            if (string.IsNullOrWhiteSpace(amount))
                throw new ArgumentException("amount cannot be empty. ", nameof(amount));

            string parameters = string.Format("asset={0}&amount={1}&recvWindow={2}",
                asset, amount, recvWindow) +
                (!string.IsNullOrWhiteSpace(isIsolated) ? string.Format("&isIsolated={0}", isIsolated) : "") +
                (!string.IsNullOrWhiteSpace(symbol) ? string.Format("&symbol={0}", symbol.ToUpper()) : "");


            return Call(HttpMethod.Post, Endpoints.MarginTrade.Borrow, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/margin/repay
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="isIsolated"></param>
        /// <param name="symbol"></param>
        /// <param name="amount"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public string Repay(string asset, string amount, string isIsolated = null, string symbol = null, long recvWindow = 5000)
        {
            if (string.IsNullOrWhiteSpace(asset))
                throw new ArgumentException("asset cannot be empty. ", nameof(asset));

            if (string.IsNullOrWhiteSpace(amount))
                throw new ArgumentException("amount cannot be empty. ", nameof(amount));

            string parameters = string.Format("asset={0}&amount={1}&recvWindow={2}",
                asset, amount, recvWindow) +
                (!string.IsNullOrWhiteSpace(isIsolated) ? string.Format("&isIsolated={0}", isIsolated) : "") +
                (!string.IsNullOrWhiteSpace(symbol) ? string.Format("&symbol={0}", symbol.ToUpper()) : "");

            return Call(HttpMethod.Post, Endpoints.MarginTrade.Repay, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/margin/asset
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public string QueryAsset(string asset, long recvWindow = 5000)
        {
            if (string.IsNullOrWhiteSpace(asset))
                throw new ArgumentException("asset cannot be empty. ", nameof(asset));

            string parameters = string.Format("asset={0}&recvWindow={1}", asset, recvWindow);

            return Call(HttpMethod.Get, Endpoints.MarginTrade.QueryAsset, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/margin/pair
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public string QueryPair(string symbol, long recvWindow = 5000)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));

            string parameters = string.Format("symbol={0}&recvWindow={1}", symbol.ToUpper(), recvWindow);

            return Call(HttpMethod.Get, Endpoints.MarginTrade.QueryPair, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/margin/allAssets
        /// </summary>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string GetAllAssets(long recvWindow = 5000)
        {
            string parameters = string.Format("recvWindow={0}", recvWindow);

            return Call(HttpMethod.Get, Endpoints.MarginTrade.GetAllAssets, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/margin/allPairs
        /// </summary>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string GetAllPairs(long recvWindow = 5000)
        {
            string parameters = string.Format("recvWindow={0}", recvWindow);

            return Call(HttpMethod.Get, Endpoints.MarginTrade.GetAllPairs, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/margin/priceIndex
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public string QueryPriceIndex(string symbol, long recvWindow = 5000)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));

            string parameters = string.Format("symbol={0}&recvWindow={1}", symbol.ToUpper(), recvWindow);

            return Call(HttpMethod.Get, Endpoints.MarginTrade.QueryPriceIndex, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/margin/order
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="isIsolated"></param>
        /// <param name="side"></param>
        /// <param name="type"></param>
        /// <param name="quantity"></param>
        /// <param name="quoteOrderQty"></param>
        /// <param name="price"></param>
        /// <param name="stopPrice"></param>
        /// <param name="newClientOrderId"></param>
        /// <param name="newOrderRespType"></param>
        /// <param name="sideEffectType"></param>
        /// <param name="icebergQty"></param>
        /// <param name="timeInForce"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string NewOrder(
            string symbol, 
            string side,
            string type, 
            string isIsolated = null, 
            string quantity = null, 
            string quoteOrderQty = null,
            string price = null,
            string stopPrice = null,
            string newClientOrderId = null,
            string newOrderRespType = null,
            string sideEffectType = null,
            decimal icebergQty = 0M,
            string timeInForce = "GTC",
            long recvWindow = 5000)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));

            if (string.IsNullOrWhiteSpace(side))
                throw new ArgumentException("side cannot be empty. ", nameof(side));

            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("type cannot be empty. ", nameof(type));

            string parameters = string.Format("symbol={0}&side={1}&type={2}&recvWindow={3}", symbol.ToUpper(), side, type, recvWindow) +
                (type == "LIMIT" ? string.Format("&price={0}", price) : "") +
                (icebergQty > 0M ? string.Format("&icebergQty={0}", icebergQty) : "") +
                (!string.IsNullOrWhiteSpace(isIsolated) ? string.Format("&isIsolated={0}", isIsolated) : "") +
                (!string.IsNullOrWhiteSpace(quantity) ? string.Format("&quantity={0}", quantity) : "") +
                (!string.IsNullOrWhiteSpace(quoteOrderQty) ? string.Format("&quoteOrderQty={0}", quoteOrderQty) : "") +
                (!string.IsNullOrWhiteSpace(stopPrice) ? string.Format("&stopPrice={0}", stopPrice) : "") +
                (!string.IsNullOrWhiteSpace(newClientOrderId) ? string.Format("&newClientOrderId={0}", newClientOrderId) : "") +
                (!string.IsNullOrWhiteSpace(newOrderRespType) ? string.Format("&newOrderRespType={0}", newOrderRespType) : "") +
                (!string.IsNullOrWhiteSpace(sideEffectType) ? string.Format("&sideEffectType={0}", sideEffectType) : "");

            return Call(HttpMethod.Post, Endpoints.MarginTrade.NewOrder, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/margin/order
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="isIsolated"></param>
        /// <param name="orderId"></param>
        /// <param name="origClientOrderId"></param>
        /// <param name="newClientOrderId"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public string CancelOrder(
            string symbol,
            string isIsolated = null,
            string orderId = null,
            string origClientOrderId = null,
            string newClientOrderId = null,
            long recvWindow = 5000)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));

            string parameters = string.Format("symbol={0}&recvWindow={1}", symbol.ToUpper(), recvWindow) +
                (!string.IsNullOrWhiteSpace(isIsolated) ? string.Format("&isIsolated={0}", isIsolated) : "") +
                (!string.IsNullOrWhiteSpace(orderId) ? string.Format("&orderId={0}", orderId) : "") +
                (!string.IsNullOrWhiteSpace(origClientOrderId) ? string.Format("&origClientOrderId={0}", origClientOrderId) : "") +
                (!string.IsNullOrWhiteSpace(newClientOrderId) ? string.Format("&newClientOrderId={0}", newClientOrderId) : "");

            return Call(HttpMethod.Delete, Endpoints.MarginTrade.CancelOrder, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/margin/openOrders
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="isIsolated"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public string CancelOpenOrders(
            string symbol,
            string isIsolated = null,
            long recvWindow = 5000)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));

            string parameters = string.Format("symbol={0}&recvWindow={1}", symbol.ToUpper(), recvWindow) +
                (!string.IsNullOrWhiteSpace(isIsolated) ? string.Format("&isIsolated={0}", isIsolated) : "");

            return Call(HttpMethod.Delete, Endpoints.MarginTrade.CancelOpenOrders, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/margin/transfer
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="type"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="current"></param>
        /// <param name="size"></param>
        /// <param name="archived"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string TransferHistory(
            string asset = null,
            string type = null,
            string startTime = null, 
            string endTime = null,
            string current = null, 
            string size = null,
            string archived = null,
            long recvWindow = 5000)
        {
            string parameters = string.Format("recvWindow={0}", recvWindow) +
                (!string.IsNullOrWhiteSpace(asset) ? string.Format("&asset={0}", asset) : "") +
                (!string.IsNullOrWhiteSpace(type) ? string.Format("&type={0}", type) : "") +
                (!string.IsNullOrWhiteSpace(startTime) ? string.Format("&startTime={0}", startTime) : "") +
                (!string.IsNullOrWhiteSpace(endTime) ? string.Format("&endTime={0}", endTime) : "") +
                (!string.IsNullOrWhiteSpace(current) ? string.Format("&current={0}", current) : "") +
                (!string.IsNullOrWhiteSpace(size) ? string.Format("&size={0}", size) : "") +
                (!string.IsNullOrWhiteSpace(archived) ? string.Format("&archived={0}", archived) : "");

            return Call(HttpMethod.Get, Endpoints.MarginTrade.TransferHistory, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/margin/loan
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="isolatedSymbol"></param>
        /// <param name="txId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="current"></param>
        /// <param name="size"></param>
        /// <param name="archived"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string QueryLoanRecord(
            string asset,
            string isolatedSymbol = null,
            string txId = null,
            string startTime = null,
            string endTime = null,
            string current = null,
            string size = null,
            string archived = null,
            long recvWindow = 5000)
        {
            if (string.IsNullOrWhiteSpace(asset))
                throw new ArgumentException("asset cannot be empty. ", nameof(asset));

            string parameters = string.Format("asset={0}&recvWindow={1}", asset, recvWindow) +
                (!string.IsNullOrWhiteSpace(isolatedSymbol) ? string.Format("&isolatedSymbol={0}", isolatedSymbol) : "") +
                (!string.IsNullOrWhiteSpace(txId) ? string.Format("&txId={0}", txId) : "") +
                (!string.IsNullOrWhiteSpace(startTime) ? string.Format("&startTime={0}", startTime) : "") +
                (!string.IsNullOrWhiteSpace(endTime) ? string.Format("&endTime={0}", endTime) : "") +
                (!string.IsNullOrWhiteSpace(current) ? string.Format("&current={0}", current) : "") +
                (!string.IsNullOrWhiteSpace(size) ? string.Format("&size={0}", size) : "") +
                (!string.IsNullOrWhiteSpace(archived) ? string.Format("&archived={0}", archived) : "");

            return Call(HttpMethod.Get, Endpoints.MarginTrade.QueryLoanRecord, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/margin/repay
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="isolatedSymbol"></param>
        /// <param name="txId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="current"></param>
        /// <param name="size"></param>
        /// <param name="archived"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string QueryRepayRecord(
            string asset,
            string isolatedSymbol = null,
            string txId = null,
            string startTime = null,
            string endTime = null,
            string current = null,
            string size = null,
            string archived = null,
            long recvWindow = 5000)
        {
            if (string.IsNullOrWhiteSpace(asset))
                throw new ArgumentException("asset cannot be empty. ", nameof(asset));

            string parameters = string.Format("asset={0}&recvWindow={1}", asset, recvWindow) +
                (!string.IsNullOrWhiteSpace(isolatedSymbol) ? string.Format("&isolatedSymbol={0}", isolatedSymbol) : "") +
                (!string.IsNullOrWhiteSpace(txId) ? string.Format("&txId={0}", txId) : "") +
                (!string.IsNullOrWhiteSpace(startTime) ? string.Format("&startTime={0}", startTime) : "") +
                (!string.IsNullOrWhiteSpace(endTime) ? string.Format("&endTime={0}", endTime) : "") +
                (!string.IsNullOrWhiteSpace(current) ? string.Format("&current={0}", current) : "") +
                (!string.IsNullOrWhiteSpace(size) ? string.Format("&size={0}", size) : "") +
                (!string.IsNullOrWhiteSpace(archived) ? string.Format("&archived={0}", archived) : "");

            return Call(HttpMethod.Get, Endpoints.MarginTrade.QueryRepayRecord, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/margin/interestHistory
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="isolatedSymbol"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="current"></param>
        /// <param name="size"></param>
        /// <param name="archived"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string GetInterestHistory(
            string asset = null,
            string isolatedSymbol = null,
            string startTime = null,
            string endTime = null,
            string current = null,
            string size = null,
            string archived = null,
            long recvWindow = 5000)
        {
            string parameters = string.Format("recvWindow={0}", recvWindow) +
                (!string.IsNullOrWhiteSpace(asset) ? string.Format("&asset={0}", asset) : "") +
                (!string.IsNullOrWhiteSpace(isolatedSymbol) ? string.Format("&isolatedSymbol={0}", isolatedSymbol) : "") +
                (!string.IsNullOrWhiteSpace(startTime) ? string.Format("&startTime={0}", startTime) : "") +
                (!string.IsNullOrWhiteSpace(endTime) ? string.Format("&endTime={0}", endTime) : "") +
                (!string.IsNullOrWhiteSpace(current) ? string.Format("&current={0}", current) : "") +
                (!string.IsNullOrWhiteSpace(size) ? string.Format("&size={0}", size) : "") +
                (!string.IsNullOrWhiteSpace(archived) ? string.Format("&archived={0}", archived) : "");

            return Call(HttpMethod.Get, Endpoints.MarginTrade.GetInterestHistory, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/margin/forceLiquidationRec
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="isolatedSymbol"></param>
        /// <param name="current"></param>
        /// <param name="size"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string GetForceLiquidationRecord(
            string startTime = null,
            string endTime = null,
            string isolatedSymbol = null,
            string current = null,
            string size = null,
            long recvWindow = 5000)
        {
            string parameters = string.Format("recvWindow={0}", recvWindow) +
                (!string.IsNullOrWhiteSpace(startTime) ? string.Format("&startTime={0}", startTime) : "") +
                (!string.IsNullOrWhiteSpace(endTime) ? string.Format("&endTime={0}", endTime) : "") +
                (!string.IsNullOrWhiteSpace(isolatedSymbol) ? string.Format("&isolatedSymbol={0}", isolatedSymbol) : "") +
                (!string.IsNullOrWhiteSpace(current) ? string.Format("&current={0}", current) : "") +
                (!string.IsNullOrWhiteSpace(size) ? string.Format("&size={0}", size) : "");

            return Call(HttpMethod.Get, Endpoints.MarginTrade.GetForceLiquidationRecord, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/margin/account
        /// </summary>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string QueryAccountDetails(long recvWindow = 5000)
        {
            string parameters = string.Format("recvWindow={0}", recvWindow);

            return Call(HttpMethod.Get, Endpoints.MarginTrade.QueryAccountDetails, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/margin/order
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="isolatedSymbol"></param>
        /// <param name="orderId"></param>
        /// <param name="origClientOrderId"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string QueryAccountOrder(
            string symbol,
            string isolatedSymbol = null,
            string orderId = null,
            string origClientOrderId = null,
            long recvWindow = 5000)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));

            string parameters = string.Format("symbol={0}&recvWindow={1}", symbol.ToUpper(), recvWindow) +
                (!string.IsNullOrWhiteSpace(isolatedSymbol) ? string.Format("&isolatedSymbol={0}", isolatedSymbol) : "") +
                (!string.IsNullOrWhiteSpace(orderId) ? string.Format("&orderId={0}", orderId) : "") +
                (!string.IsNullOrWhiteSpace(origClientOrderId) ? string.Format("&origClientOrderId={0}", origClientOrderId) : "");

            return Call(HttpMethod.Get, Endpoints.MarginTrade.QueryAccountOrder, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/margin/openOrders
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="isIsolated"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string QueryAccountOpenOrders(string symbol = null, string isIsolated = null, long recvWindow = 5000)
        {
            string parameters = string.Format("recvWindow={0}", recvWindow) +
                (!string.IsNullOrWhiteSpace(symbol) ? string.Format("&symbol={0}", symbol) : "") +
                (!string.IsNullOrWhiteSpace(isIsolated) ? string.Format("&isIsolated={0}", isIsolated) : "");

            return Call(HttpMethod.Get, Endpoints.MarginTrade.QueryAccountOpenOrders, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/margin/allOrders
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="isIsolated"></param>
        /// <param name="orderId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="limit"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public string QueryAccountAllOrders(
            string symbol,
            string isIsolated = null,
            string orderId = null,
            string startTime = null,
            string endTime = null,
            string limit = null,
            long recvWindow = 5000)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));

            string parameters = string.Format("symbol={0}&recvWindow={1}", symbol.ToUpper(), recvWindow) +
                (!string.IsNullOrWhiteSpace(isIsolated) ? string.Format("&isIsolated={0}", isIsolated) : "") +
                (!string.IsNullOrWhiteSpace(orderId) ? string.Format("&orderId={0}", orderId) : "") +
                (!string.IsNullOrWhiteSpace(startTime) ? string.Format("&startTime={0}", startTime) : "") +
                (!string.IsNullOrWhiteSpace(endTime) ? string.Format("&endTime={0}", endTime) : "") +
                (!string.IsNullOrWhiteSpace(limit) ? string.Format("&current={0}", limit) : "");

            return Call(HttpMethod.Get, Endpoints.MarginTrade.QueryAccountAllOrders, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/margin/order/oco
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="side"></param>
        /// <param name="quantity"></param>
        /// <param name="price"></param>
        /// <param name="stopPrice"></param>
        /// <param name="isIsolated"></param>
        /// <param name="listClientOrderId"></param>
        /// <param name="limitClientOrderId"></param>
        /// <param name="limitIcebergQty"></param>
        /// <param name="stopClientOrderId"></param>
        /// <param name="stopLimitPrice"></param>
        /// <param name="stopIcebergQty"></param>
        /// <param name="stopLimitTimeInForce"></param>
        /// <param name="newOrderRespType"></param>
        /// <param name="sideEffectType"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public string NewOCO(
            string symbol,
            string side,
            string quantity,
            string price,
            string stopPrice,
            string isIsolated = null,
            string listClientOrderId = null,
            string limitClientOrderId = null,
            string limitIcebergQty = null,
            string stopClientOrderId = null,
            string stopLimitPrice = null,
            string stopIcebergQty = null,
            string stopLimitTimeInForce = null,
            string newOrderRespType = null,
            string sideEffectType = null,
            long recvWindow = 5000)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));
            if (string.IsNullOrWhiteSpace(side))
                throw new ArgumentException("side cannot be empty. ", nameof(side));
            if (string.IsNullOrWhiteSpace(quantity))
                throw new ArgumentException("quantity cannot be empty. ", nameof(quantity));
            if (string.IsNullOrWhiteSpace(price))
                throw new ArgumentException("price cannot be empty. ", nameof(price));
            if (string.IsNullOrWhiteSpace(stopPrice))
                throw new ArgumentException("stopPrice cannot be empty. ", nameof(stopPrice));

            string parameters = string.Format("symbol={0}&side={1}&quantity={2}&price={3}&stopPrice={4}&recvWindow={5}",
                symbol.ToUpper(), side, quantity, price, stopPrice, recvWindow) +
                (!string.IsNullOrWhiteSpace(isIsolated) ? string.Format("&isIsolated={0}", isIsolated) : "") +
                (!string.IsNullOrWhiteSpace(listClientOrderId) ? string.Format("&listClientOrderId={0}", listClientOrderId) : "") +
                (!string.IsNullOrWhiteSpace(limitClientOrderId) ? string.Format("&limitClientOrderId={0}", limitClientOrderId) : "") +
                (!string.IsNullOrWhiteSpace(limitIcebergQty) ? string.Format("&limitIcebergQty={0}", limitIcebergQty) : "") +
                (!string.IsNullOrWhiteSpace(stopClientOrderId) ? string.Format("&stopClientOrderId={0}", stopClientOrderId) : "") +
                (!string.IsNullOrWhiteSpace(stopLimitPrice) ? string.Format("&stopLimitPrice={0}", stopLimitPrice) : "") +
                (!string.IsNullOrWhiteSpace(stopIcebergQty) ? string.Format("&stopIcebergQty={0}", stopIcebergQty) : "") +
                (!string.IsNullOrWhiteSpace(stopLimitTimeInForce) ? string.Format("&stopLimitTimeInForce={0}", stopLimitTimeInForce) : "") +
                (!string.IsNullOrWhiteSpace(sideEffectType) ? string.Format("&sideEffectType={0}", sideEffectType) : "") +
                (!string.IsNullOrWhiteSpace(newOrderRespType) ? string.Format("&newOrderRespType={0}", newOrderRespType) : "");

            return Call(HttpMethod.Post, Endpoints.MarginTrade.NewOCO, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/margin/orderList
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="isIsolated"></param>
        /// <param name="orderListId"></param>
        /// <param name="listClientOrderId"></param>
        /// <param name="newClientOrderId"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public string CancelOCO(
            string symbol,
            string isIsolated = null,
            string orderListId = null,
            string listClientOrderId = null,
            string newClientOrderId = null,
            long recvWindow = 5000)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));

            string parameters = string.Format("symbol={0}&recvWindow={1}", symbol.ToUpper(), recvWindow) +
                (!string.IsNullOrWhiteSpace(isIsolated) ? string.Format("&isIsolated={0}", isIsolated) : "") +
                (!string.IsNullOrWhiteSpace(orderListId) ? string.Format("&orderListId={0}", orderListId) : "") +
                (!string.IsNullOrWhiteSpace(listClientOrderId) ? string.Format("&listClientOrderId={0}", listClientOrderId) : "") +
                (!string.IsNullOrWhiteSpace(newClientOrderId) ? string.Format("&newClientOrderId={0}", newClientOrderId) : "");

            return Call(HttpMethod.Delete, Endpoints.MarginTrade.CancelOCO, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/margin/orderList
        /// </summary>
        /// <param name="isIsolated"></param>
        /// <param name="symbol"></param>
        /// <param name="orderListId"></param>
        /// <param name="origClientOrderId"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string QueryOCO(
            string isIsolated = null,
            string symbol = null,
            string orderListId = null,
            string origClientOrderId = null,
            long recvWindow = 5000)
        {
            string parameters = string.Format("recvWindow={0}", recvWindow) +
                (!string.IsNullOrWhiteSpace(isIsolated) ? string.Format("&isIsolated={0}", isIsolated) : "") +
                (!string.IsNullOrWhiteSpace(symbol) ? string.Format("&symbol={0}", symbol) : "") +
                (!string.IsNullOrWhiteSpace(orderListId) ? string.Format("&orderListId={0}", orderListId) : "") +
                (!string.IsNullOrWhiteSpace(origClientOrderId) ? string.Format("&origClientOrderId={0}", origClientOrderId) : "");

            return Call(HttpMethod.Get, Endpoints.MarginTrade.QueryOCO, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/margin/allOrderList
        /// </summary>
        /// <param name="isIsolated"></param>
        /// <param name="symbol"></param>
        /// <param name="fromId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="limit"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string QueryAllOCO(
            string isIsolated = null,
            string symbol = null,
            string fromId = null,
            string startTime = null,
            string endTime = null,
            string limit = null,
            long recvWindow = 5000)
        {
            string parameters = string.Format("recvWindow={0}", recvWindow) +
                (!string.IsNullOrWhiteSpace(isIsolated) ? string.Format("&isIsolated={0}", isIsolated) : "") +
                (!string.IsNullOrWhiteSpace(symbol) ? string.Format("&symbol={0}", symbol) : "") +
                (!string.IsNullOrWhiteSpace(fromId) ? string.Format("&fromId={0}", fromId) : "") +
                (!string.IsNullOrWhiteSpace(startTime) ? string.Format("&startTime={0}", startTime) : "") +
                (!string.IsNullOrWhiteSpace(endTime) ? string.Format("&endTime={0}", endTime) : "") +
                (!string.IsNullOrWhiteSpace(limit) ? string.Format("&current={0}", limit) : "");

            return Call(HttpMethod.Get, Endpoints.MarginTrade.QueryAllOCO, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/margin/openOrderList
        /// </summary>
        /// <param name="isIsolated"></param>
        /// <param name="symbol"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string QueryOpenOCO(string isIsolated = null, string symbol = null, long recvWindow = 5000)
        {
            string parameters = string.Format("recvWindow={0}", recvWindow) +
                (!string.IsNullOrWhiteSpace(isIsolated) ? string.Format("&isIsolated={0}", isIsolated) : "") +
                (!string.IsNullOrWhiteSpace(symbol) ? string.Format("&symbol={0}", symbol) : "");

            return Call(HttpMethod.Get, Endpoints.MarginTrade.QueryOpenOCO, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/margin/myTrades
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="isIsolated"></param>
        /// <param name="orderId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="fromId"></param>
        /// <param name="limit"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public string QueryTradeList(
            string symbol,
            string isIsolated = null,
            string orderId = null,
            string startTime = null,
            string endTime = null,
            string fromId = null,
            string limit = null,
            long recvWindow = 5000)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));

            string parameters = string.Format("recvWindow={0}", recvWindow) +
                (!string.IsNullOrWhiteSpace(isIsolated) ? string.Format("&isIsolated={0}", isIsolated) : "") +
                (!string.IsNullOrWhiteSpace(orderId) ? string.Format("&orderId={0}", orderId) : "") +
                (!string.IsNullOrWhiteSpace(startTime) ? string.Format("&startTime={0}", startTime) : "") +
                (!string.IsNullOrWhiteSpace(endTime) ? string.Format("&endTime={0}", endTime) : "") +
                (!string.IsNullOrWhiteSpace(fromId) ? string.Format("&fromId={0}", fromId) : "") +
                (!string.IsNullOrWhiteSpace(limit) ? string.Format("&current={0}", limit) : "");

            return Call(HttpMethod.Get, Endpoints.MarginTrade.QueryTradeList, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/margin/maxBorrowable
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="isolatedSymbol"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public string QueryMaxBorrow(string asset, string isolatedSymbol = null, long recvWindow = 5000)
        {
            if (string.IsNullOrWhiteSpace(asset))
                throw new ArgumentException("asset cannot be empty. ", nameof(asset));

            string parameters = string.Format("asset={0}&recvWindow={1}", asset, recvWindow) +
                (!string.IsNullOrWhiteSpace(isolatedSymbol) ? string.Format("&isolatedSymbol={0}", isolatedSymbol) : "");

            return Call(HttpMethod.Get, Endpoints.MarginTrade.QueryMaxBorrow, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/margin/maxTransferable
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="isolatedSymbol"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public string QueryMaxTransferOutAmount(string asset, string isolatedSymbol = null, long recvWindow = 5000)
        {
            if (string.IsNullOrWhiteSpace(asset))
                throw new ArgumentException("asset cannot be empty. ", nameof(asset));

            string parameters = string.Format("asset={0}&recvWindow={1}", asset, recvWindow) +
                (!string.IsNullOrWhiteSpace(isolatedSymbol) ? string.Format("&isolatedSymbol={0}", isolatedSymbol) : "");

            return Call(HttpMethod.Get, Endpoints.MarginTrade.QueryMaxTransferOutAmount, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/margin/isolated/transfer
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="symbol"></param>
        /// <param name="transFrom"></param>
        /// <param name="transTo"></param>
        /// <param name="amount"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public string IsolatedTransfer(
            string asset,
            string symbol,
            string transFrom,
            string transTo,
            string amount,
            long recvWindow = 5000)
        {
            if (string.IsNullOrWhiteSpace(asset))
                throw new ArgumentException("asset cannot be empty. ", nameof(asset));
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));
            if (string.IsNullOrWhiteSpace(transFrom))
                throw new ArgumentException("transFrom cannot be empty. ", nameof(transFrom));
            if (string.IsNullOrWhiteSpace(transTo))
                throw new ArgumentException("transTo cannot be empty. ", nameof(transTo));
            if (string.IsNullOrWhiteSpace(amount))
                throw new ArgumentException("amount cannot be empty. ", nameof(amount));

            string parameters = string.Format("asset={0}&symbol={1}&transFrom={2}&transTo={3}&amount={4}&recvWindow={5}",
                asset, symbol.ToUpper(), transFrom, transTo, amount, recvWindow);

            return Call(HttpMethod.Post, Endpoints.MarginTrade.IsolatedTransfer, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/margin/isolated/transfer
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="asset"></param>
        /// <param name="transFrom"></param>
        /// <param name="transTo"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="current"></param>
        /// <param name="size"></param>
        /// <param name="archived"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public string GetIsolatedTransferHistory(
            string symbol,
            string asset = null,
            string transFrom = null,
            string transTo = null,
            string startTime = null,
            string endTime = null,
            string current = null,
            string size = null,
            string archived = null,
            long recvWindow = 5000)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));

            string parameters = string.Format("symbol={0}&recvWindow={1}", symbol.ToUpper(), recvWindow) +
                (!string.IsNullOrWhiteSpace(asset) ? string.Format("&asset={0}", asset) : "") +
                (!string.IsNullOrWhiteSpace(transFrom) ? string.Format("&transFrom={0}", transFrom) : "") +
                (!string.IsNullOrWhiteSpace(transTo) ? string.Format("&transTo={0}", transTo) : "") +
                (!string.IsNullOrWhiteSpace(startTime) ? string.Format("&startTime={0}", startTime) : "") +
                (!string.IsNullOrWhiteSpace(endTime) ? string.Format("&endTime={0}", endTime) : "") +
                (!string.IsNullOrWhiteSpace(current) ? string.Format("&current={0}", current) : "") +
                (!string.IsNullOrWhiteSpace(size) ? string.Format("&size={0}", size) : "") +
                (!string.IsNullOrWhiteSpace(archived) ? string.Format("&archived={0}", archived) : "");

            return Call(HttpMethod.Get, Endpoints.MarginTrade.GetIsolatedTransferHistory, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/margin/isolated/account
        /// </summary>
        /// <param name="symbols"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string QueryIsolatedInfo(string symbols = null, long recvWindow = 5000)
        {
            string parameters = string.Format("recvWindow={0}", recvWindow) +
                (!string.IsNullOrWhiteSpace(symbols) ? string.Format("&symbols={0}", symbols) : "");

            return Call(HttpMethod.Get, Endpoints.MarginTrade.QueryIsolatedInfo, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/margin/isolated/account
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public string DisableIsolatedAccount(string symbol, long recvWindow = 5000)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));

            string parameters = string.Format("symbol={0}&recvWindow={1}", symbol.ToUpper(), recvWindow);

            return Call(HttpMethod.Delete, Endpoints.MarginTrade.DisableIsolatedAccount, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/margin/isolated/account
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public string EnableIsolatedAccount(string symbol, long recvWindow = 5000)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));

            string parameters = string.Format("symbol={0}&recvWindow={1}", symbol.ToUpper(), recvWindow);

            return Call(HttpMethod.Post, Endpoints.MarginTrade.EnableIsolatedAccount, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/margin/isolated/accountLimit
        /// </summary>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string QueryEnabledIsolatedAccountLimit(long recvWindow = 5000)
        {
            string parameters = string.Format("recvWindow={0}", recvWindow);

            return Call(HttpMethod.Get, Endpoints.MarginTrade.QueryEnabledIsolatedAccountLimit, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/margin/isolated/pair
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public string QueryIsolatedSymbol(string symbol, long recvWindow = 5000)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));

            string parameters = string.Format("symbol={0}&recvWindow={1}", symbol.ToUpper(), recvWindow);

            return Call(HttpMethod.Get, Endpoints.MarginTrade.QueryIsolatedSymbol, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/margin/isolated/allPairs
        /// </summary>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string GetAllIsolatedSymbol(long recvWindow = 5000)
        {
            string parameters = string.Format("recvWindow={0}", recvWindow);

            return Call(HttpMethod.Get, Endpoints.MarginTrade.GetAllIsolatedSymbol, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/bnbBurn
        /// </summary>
        /// <param name="spotBNBBurn"></param>
        /// <param name="interestBNBBurn"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string ToggleBNBBurnOnSpotTrade(string spotBNBBurn = null, string interestBNBBurn = null, long recvWindow = 5000)
        {
            string parameters = string.Format("recvWindow={0}", recvWindow) +
                (!string.IsNullOrWhiteSpace(spotBNBBurn) ? string.Format("&spotBNBBurn={0}", spotBNBBurn) : "") +
                (!string.IsNullOrWhiteSpace(interestBNBBurn) ? string.Format("&interestBNBBurn={0}", interestBNBBurn) : "");

            return Call(HttpMethod.Post, Endpoints.MarginTrade.ToggleBNBBurnOnSpotTrade, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/bnbBurn
        /// </summary>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string GetBNBBurnStatus(long recvWindow = 5000)
        {
            string parameters = string.Format("recvWindow={0}", recvWindow);

            return Call(HttpMethod.Get, Endpoints.MarginTrade.GetBNBBurnStatus, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/margin/interestRateHistory
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="vipLevel"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public string QueryInterestRateHistory(
            string asset,
            string vipLevel = null,
            string startTime = null,
            string endTime = null,
            long recvWindow = 5000)
        {
            if (string.IsNullOrWhiteSpace(asset))
                throw new ArgumentException("asset cannot be empty. ", nameof(asset));

            string parameters = string.Format("asset={0}&recvWindow={1}", asset, recvWindow) +
                (!string.IsNullOrWhiteSpace(vipLevel) ? string.Format("&vipLevel={0}", vipLevel) : "") +
                (!string.IsNullOrWhiteSpace(startTime) ? string.Format("&startTime={0}", startTime) : "") +
                (!string.IsNullOrWhiteSpace(endTime) ? string.Format("&endTime={0}", endTime) : "");

            return Call(HttpMethod.Get, Endpoints.MarginTrade.QueryInterestRateHistory, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/margin/crossMarginData
        /// </summary>
        /// <param name="vipLevel"></param>
        /// <param name="coin"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string QueryCrossMarginFeeData(string vipLevel = null, string coin = null, long recvWindow = 5000)
        {
            string parameters = string.Format("recvWindow={0}", recvWindow) +
                (!string.IsNullOrWhiteSpace(vipLevel) ? string.Format("&vipLevel={0}", vipLevel) : "") +
                (!string.IsNullOrWhiteSpace(coin) ? string.Format("&coin={0}", coin) : "");

            return Call(HttpMethod.Get, Endpoints.MarginTrade.QueryCrossMarginFeeData, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/margin/isolatedMarginData
        /// </summary>
        /// <param name="vipLevel"></param>
        /// <param name="symbol"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string QueryIsolatedMarginFeeData(string vipLevel = null, string symbol = null, long recvWindow = 5000)
        {
            string parameters = string.Format("recvWindow={0}", recvWindow) +
                (!string.IsNullOrWhiteSpace(vipLevel) ? string.Format("&vipLevel={0}", vipLevel) : "") +
                (!string.IsNullOrWhiteSpace(symbol) ? string.Format("&symbol={0}", symbol) : "");

            return Call(HttpMethod.Get, Endpoints.MarginTrade.QueryIsolatedMarginFeeData, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/margin/isolatedMarginTier
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="tier"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public string QueryIsolatedMarginTierData(string symbol, string tier = null, long recvWindow = 5000)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));

            string parameters = string.Format("symbol={0}&recvWindow={1}", symbol.ToUpper(), recvWindow) +
                (!string.IsNullOrWhiteSpace(tier) ? string.Format("&tier={0}", tier) : "");

            return Call(HttpMethod.Get, Endpoints.MarginTrade.QueryIsolatedMarginTierData, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/margin/rateLimit/order
        /// </summary>
        /// <param name="isIsolated"></param>
        /// <param name="symbol"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string QueryCurrentOrderCountUsage(string isIsolated = null, string symbol = null, long recvWindow = 5000)
        {
            string parameters = string.Format("recvWindow={0}", recvWindow) +
                (!string.IsNullOrWhiteSpace(isIsolated) ? string.Format("&isIsolated={0}", isIsolated) : "") +
                (!string.IsNullOrWhiteSpace(symbol) ? string.Format("&symbol={0}", symbol) : "");

            return Call(HttpMethod.Get, Endpoints.MarginTrade.QueryCurrentOrderCountUsage, true, parameters);
        }

        public string Dustlog(string startTime = null, string endTime = null, long recvWindow = 5000)
        {
            string parameters = string.Format("recvWindow={0}", recvWindow) +
                (!string.IsNullOrWhiteSpace(startTime) ? string.Format("&startTime={0}", startTime) : "") +
                (!string.IsNullOrWhiteSpace(endTime) ? string.Format("&endTime={0}", endTime) : "");

            return Call(HttpMethod.Get, Endpoints.MarginTrade.Dustlog, true, parameters);
        }

        #endregion
    }
}
