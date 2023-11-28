using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomarkets.Apis.Bybit
{
    public class TradeApi
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://api.bybit.com";

        public static string ApiKey { get; private set; }
        public static string ApiSecret { get; private set; }
        public static string RecvWindow { get; set; } = "5000";

        public TradeApi(string apiKey, string apiSecret)
        {
            ApiKey = apiKey;
            ApiSecret = apiSecret;
            _httpClient = CreateAndConfigureHttpClient();
        }

        private static HttpClient CreateAndConfigureHttpClient()
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            var configureHttpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(ApiUrl)
            };
            configureHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            configureHttpClient.DefaultRequestHeaders.Add("X-BAPI-API-KEY", ApiKey);
            configureHttpClient.DefaultRequestHeaders.Add("X-BAPI-RECV-WINDOW", RecvWindow);

            return configureHttpClient;
        }

        private void RewriteHeaders(string timestamp, string signature)
        {
            if (_httpClient.DefaultRequestHeaders.Contains("X-BAPI-TIMESTAMP"))
            {
                _httpClient.DefaultRequestHeaders.Remove("X-BAPI-TIMESTAMP");
            }

            if (_httpClient.DefaultRequestHeaders.Contains("X-BAPI-SIGN"))
            {
                _httpClient.DefaultRequestHeaders.Remove("X-BAPI-SIGN");
            }

            _httpClient.DefaultRequestHeaders.Add("X-BAPI-TIMESTAMP", timestamp);
            _httpClient.DefaultRequestHeaders.Add("X-BAPI-SIGN", signature);
        }

        /// <summary>
        /// Request without parameters
        /// </summary>
        /// <param name="method"></param>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        private async Task<string> Call(HttpMethod method, string endpoint)
        {
            string timestamp = await Extensions.GetBybitServerTime();
            string paramStr = timestamp + ApiKey + RecvWindow;
            string signature = Extensions.GenerateSignatureHMACSHA256(ApiSecret, paramStr);
            string requestUri = endpoint;

            RewriteHeaders(timestamp, signature);

            var response = await _httpClient.SendAsync(new HttpRequestMessage(method, requestUri));
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Request with multi parameters
        /// </summary>
        /// <param name="method"></param>
        /// <param name="endpoint"></param>
        /// <param name="requestParams"></param>
        /// <returns></returns>
        private async Task<string> Call(HttpMethod method, string endpoint, Dictionary<string, object> requestParams)
        {
            string timestamp = await Extensions.GetBybitServerTime();
            string queryParams = Extensions.GenerateParamsString(requestParams);
            string toSign;
            string requestUri;

            HttpRequestMessage request;

            if (method.Method == "POST" || method.Method == "DELETE")
            {
                string jsonBody = Extensions.QueryStringToJson(queryParams);
                toSign = timestamp + ApiKey + RecvWindow + jsonBody;

                requestUri = endpoint;
                request = new HttpRequestMessage(method, requestUri)
                {
                    Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
                };
            }

            // if GET request
            else
            {
                toSign = timestamp + ApiKey + RecvWindow + queryParams;

                requestUri = endpoint + "?" + queryParams;
                request = new HttpRequestMessage(method, requestUri);
            }

            string signature = Extensions.GenerateSignatureHMACSHA256(ApiSecret, toSign);

            RewriteHeaders(timestamp, signature);

            var response = await _httpClient.SendAsync(request);

            return await response.Content.ReadAsStringAsync();
        }

        #region Queries

        /// <summary>
        /// https://bybit-exchange.github.io/docs/v5/order/create-order
        /// </summary>
        /// <param name="category"></param>
        /// <param name="symbol"></param>
        /// <param name="side"></param>
        /// <param name="orderType"></param>
        /// <param name="qty"></param>
        /// <param name="isLeverage"></param>
        /// <param name="price"></param>
        /// <param name="triggerDirection"></param>
        /// <param name="orderFilter"></param>
        /// <param name="triggerBy"></param>
        /// <param name="orderIv"></param>
        /// <param name="timeInForce"></param>
        /// <param name="positionIdx"></param>
        /// <param name="orderLinkId"></param>
        /// <param name="takeProfit"></param>
        /// <param name="stopLoss"></param>
        /// <param name="tpTriggerBy"></param>
        /// <param name="slTriggerBy"></param>
        /// <param name="reduceOnly"></param>
        /// <param name="closeOnTrigger"></param>
        /// <param name="smpType"></param>
        /// <param name="mmp"></param>
        /// <param name="tpslMode"></param>
        /// <param name="tpLimitPrice"></param>
        /// <param name="slLimitPrice"></param>
        /// <param name="tpOrderType"></param>
        /// <param name="slOrderType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<string> PlaceOrder(
            string category,
            string symbol,
            string side,
            string orderType,
            string qty,
            int isLeverage = -1,
            string price = null,
            int triggerDirection = -1,
            string orderFilter = null,
            string triggerBy = null,
            string orderIv = null,
            string timeInForce = null,
            int positionIdx = -1,
            string orderLinkId = null,
            string takeProfit = null,
            string stopLoss = null,
            string tpTriggerBy = null,
            string slTriggerBy = null,
            string reduceOnly = null,   // bool
            string closeOnTrigger = null,   // bool
            string smpType = null,
            string mmp = null,  // bool
            string tpslMode = null,
            string tpLimitPrice = null,
            string slLimitPrice = null,
            string tpOrderType = null,
            string slOrderType = null)
        {
            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentException("Empty parameter", nameof(category));
            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentException("Empty parameter", nameof(symbol));
            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentException("Empty parameter", nameof(side));
            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentException("Empty parameter", nameof(orderType));
            if (string.IsNullOrWhiteSpace(qty))
                throw new ArgumentException("Empty parameter", nameof(qty));

            var parameters = new Dictionary<string, object>
            {
                { nameof(category), category },
                { nameof(symbol), symbol },
                { nameof(side), side },
                { nameof(orderType), orderType },
                { nameof(qty), qty }
            };

            if (isLeverage >= 0)
                parameters.Add(nameof(isLeverage), isLeverage);
            if (!string.IsNullOrWhiteSpace(price))
                parameters.Add(nameof(price), price);
            if (triggerDirection >= 0)
                parameters.Add(nameof(triggerDirection), triggerDirection);
            if (!string.IsNullOrWhiteSpace(orderFilter))
                parameters.Add(nameof(orderFilter), orderFilter);
            if (!string.IsNullOrWhiteSpace(triggerBy))
                parameters.Add(nameof(triggerBy), triggerBy);
            if (!string.IsNullOrWhiteSpace(orderIv))
                parameters.Add(nameof(orderIv), orderIv);
            if (!string.IsNullOrWhiteSpace(timeInForce))
                parameters.Add(nameof(timeInForce), timeInForce);
            if (positionIdx >= 0)
                parameters.Add(nameof(positionIdx), positionIdx);
            if (!string.IsNullOrWhiteSpace(orderLinkId))
                parameters.Add(nameof(orderLinkId), orderLinkId);
            if (!string.IsNullOrWhiteSpace(takeProfit))
                parameters.Add(nameof(takeProfit), takeProfit);
            if (!string.IsNullOrWhiteSpace(stopLoss))
                parameters.Add(nameof(stopLoss), stopLoss);
            if (!string.IsNullOrWhiteSpace(tpTriggerBy))
                parameters.Add(nameof(tpTriggerBy), tpTriggerBy);
            if (!string.IsNullOrWhiteSpace(slTriggerBy))
                parameters.Add(nameof(slTriggerBy), slTriggerBy);
            if (!string.IsNullOrWhiteSpace(reduceOnly))
                parameters.Add(nameof(reduceOnly), reduceOnly);
            if (!string.IsNullOrWhiteSpace(closeOnTrigger))
                parameters.Add(nameof(closeOnTrigger), closeOnTrigger);
            if (!string.IsNullOrWhiteSpace(smpType))
                parameters.Add(nameof(smpType), smpType);
            if (!string.IsNullOrWhiteSpace(mmp))
                parameters.Add(nameof(mmp), mmp);
            if (!string.IsNullOrWhiteSpace(tpslMode))
                parameters.Add(nameof(tpslMode), tpslMode);
            if (!string.IsNullOrWhiteSpace(tpLimitPrice))
                parameters.Add(nameof(tpLimitPrice), tpLimitPrice);
            if (!string.IsNullOrWhiteSpace(slLimitPrice))
                parameters.Add(nameof(slLimitPrice), slLimitPrice);
            if (!string.IsNullOrWhiteSpace(tpOrderType))
                parameters.Add(nameof(tpOrderType), tpOrderType);
            if (!string.IsNullOrWhiteSpace(slOrderType))
                parameters.Add(nameof(slOrderType), slOrderType);

            return await Call(HttpMethod.Post, Endpoints.Trade.PlaceOrder, parameters);
        }

        public async Task<string> CancelOrder(
            string category, 
            string symbol, 
            string orderId = null, 
            string orderLinkId = null, 
            string orderFilter = null)
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

            if (!string.IsNullOrEmpty(orderId))
                parameters.Add(nameof(orderId), orderId);
            if (!string.IsNullOrEmpty(orderLinkId))
                parameters.Add(nameof(orderLinkId), orderLinkId);
            if (!string.IsNullOrEmpty(orderFilter))
                parameters.Add(nameof(orderFilter), orderFilter);

            return await Call(HttpMethod.Post, Endpoints.Trade.CancelOrder, parameters);
        }

        public async Task<string> GetOpenOrders(
            string category,
            string symbol = null,
            string baseCoin = null,
            string settleCoin = null,
            string orderId = null,
            string orderLinkId = null,
            int openOnly = -1,
            string orderFilter = null,
            int limit = -1,
            string cursor = null)
        {
            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentException("Empty parameter", nameof(category));

            var parameters = new Dictionary<string, object>
            {
                { nameof(category), category }
            };

            if (!string.IsNullOrEmpty(symbol))
                parameters.Add(nameof(symbol), symbol);
            if (!string.IsNullOrEmpty(baseCoin))
                parameters.Add(nameof(baseCoin), baseCoin);
            if (!string.IsNullOrEmpty(settleCoin))
                parameters.Add(nameof(settleCoin), settleCoin);
            if (!string.IsNullOrEmpty(orderId))
                parameters.Add(nameof(orderId), orderId);
            if (!string.IsNullOrEmpty(orderLinkId))
                parameters.Add(nameof(orderLinkId), orderLinkId);
            if (openOnly >= 0)
                parameters.Add(nameof(openOnly), openOnly);
            if (!string.IsNullOrEmpty(orderFilter))
                parameters.Add(nameof(orderFilter), orderFilter);
            if (limit >= 0)
                parameters.Add(nameof(limit), limit);
            if (!string.IsNullOrEmpty(cursor))
                parameters.Add(nameof(cursor), cursor);

            return await Call(HttpMethod.Get, Endpoints.Trade.GetOpenOrders, parameters);
        }

        #endregion
    }
}
