using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Cryptomarkets.Apis.Lbank
{
    public class PrivateApi
    {
        private readonly string _key;
        private readonly string _secret;
        private const string ApiUrl = "https://api.lbkex.com";
        private readonly HttpClient _httpClient;

        public PrivateApi(string apiKey, string apiSecret)
        {
            _key = apiKey;
            _secret = apiSecret;
            _httpClient = CreateAndConfigureHttpClient(_key);
        }

        private static HttpClient CreateAndConfigureHttpClient(string apiKey)
        {
            var handler = new HttpClientHandler();

            handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            HttpClient configureHttpClient = new HttpClient(handler);

            configureHttpClient.BaseAddress = new Uri(ApiUrl);
            configureHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

            return configureHttpClient;
        }

        private string Call(HttpMethod method, string endpoint, Dictionary<string, string> parameters = null)
        {
            if (parameters == null)
            {
                parameters = new Dictionary<string, string>();
            }

            parameters.Add("api_key", _key);

            string queryString = Extensions.ConvertDictionaryToQueryString(parameters);

            string md5Hash = Extensions.MD5Sign(queryString);
            //string signature = Extensions.GenerateSignatureLbank(_secret, md5Hash);
            //string signature = Extensions.RsaEncryptWithPrivate(_secret, md5Hash);
            string signature = Extensions.RsaSignTest(_secret, md5Hash);
            //string signature = "";

            string requestUri = Extensions.GenerateParamsString(endpoint, parameters) + $"&sign={signature}";

            if (method.Method == "POST" || method.Method == "DELETE")
            {
                string requestBody = queryString != "" ? Extensions.QueryStringToJson(queryString + $"&sign={signature}") : "";

                //requestUri = endpoint;
                var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                return _httpClient.PostAsync(requestUri, content).Result.Content.ReadAsStringAsync().Result;
            }

            return _httpClient.SendAsync(new HttpRequestMessage(method, requestUri)).Result.Content.ReadAsStringAsync().Result;
        }

        #region Queries

        public string UserInfo() => Call(HttpMethod.Post, Endpoints.Private.UserInfo);

        public string PlaceOrder(string symbol, string type, string price, string amount)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("type cannot be empty. ", nameof(type));
            if (string.IsNullOrWhiteSpace(price))
                throw new ArgumentException("price cannot be empty. ", nameof(price));
            if (string.IsNullOrWhiteSpace(amount))
                throw new ArgumentException("amount cannot be empty. ", nameof(amount));

            var parameters = new Dictionary<string, string>
            {
                { "symbol", symbol },
                { "type", type },
                { "price", price },
                { "amount", amount }
            };

            return Call(HttpMethod.Post, Endpoints.Private.PlaceOrder, parameters);
        }

        public string CancelOrder(string symbol, string orderId)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));
            if (string.IsNullOrWhiteSpace(orderId))
                throw new ArgumentException("orderId cannot be empty. ", nameof(orderId));

            var parameters = new Dictionary<string, string>
            {
                { "symbol", symbol },
                { "order_id", orderId }
            };

            return Call(HttpMethod.Post, Endpoints.Private.CancelOrder, parameters);
        }

        public string QueryOrder(string symbol, string orderId)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));
            if (string.IsNullOrWhiteSpace(orderId))
                throw new ArgumentException("orderId cannot be empty. ", nameof(orderId));

            var parameters = new Dictionary<string, string>
            {
                { "symbol", symbol },
                { "order_id", orderId }
            };

            return Call(HttpMethod.Post, Endpoints.Private.QueryOrder, parameters);
        }

        public string OrdersHistory(string symbol, string currentPage, string pageLength, string status = "")
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));
            if (string.IsNullOrWhiteSpace(currentPage))
                throw new ArgumentException("currentPage cannot be empty. ", nameof(currentPage));
            if (string.IsNullOrWhiteSpace(pageLength))
                throw new ArgumentException("pageLength cannot be empty. ", nameof(pageLength));

            var parameters = new Dictionary<string, string>
            {
                { "symbol", symbol },
                { "current_page", currentPage },
                { "page_length", pageLength }
            };

            if (!string.IsNullOrEmpty(status))
                parameters.Add("status", status);

            return Call(HttpMethod.Post, Endpoints.Private.OrdersHistory, parameters);
        }

        public string TransactionDetails(string symbol, string orderId)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));
            if (string.IsNullOrWhiteSpace(orderId))
                throw new ArgumentException("orderId cannot be empty. ", nameof(orderId));

            var parameters = new Dictionary<string, string>
            {
                { "symbol", symbol },
                { "order_id", orderId }
            };

            return Call(HttpMethod.Post, Endpoints.Private.TransactionDetails, parameters);
        }

        public string TransactionHistory(string symbol, string type = "", string startDate = "", string endDate = "", string from = "", string direct = "", string size = "")
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));

            var parameters = new Dictionary<string, string>
            {
                { "symbol", symbol }
            };

            if (!string.IsNullOrEmpty(type))
                parameters.Add("type", type);
            if (!string.IsNullOrEmpty(startDate))
                parameters.Add("start_date", startDate);
            if (!string.IsNullOrEmpty(endDate))
                parameters.Add("end_date", endDate);
            if (!string.IsNullOrEmpty(from))
                parameters.Add("from", from);
            if (!string.IsNullOrEmpty(direct))
                parameters.Add("direct", direct);
            if (!string.IsNullOrEmpty(size))
                parameters.Add("size", size);

            return Call(HttpMethod.Post, Endpoints.Private.TransactionHistory, parameters);
        }

        public string OpenOrdersInfo(string symbol, string currentPage, string pageLength)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("symbol cannot be empty. ", nameof(symbol));
            if (string.IsNullOrWhiteSpace(currentPage))
                throw new ArgumentException("currentPage cannot be empty. ", nameof(currentPage));
            if (string.IsNullOrWhiteSpace(pageLength))
                throw new ArgumentException("pageLength cannot be empty. ", nameof(pageLength));

            var parameters = new Dictionary<string, string>
            {
                { "symbol", symbol },
                { "current_page", currentPage },
                { "page_length", pageLength }
            };

            return Call(HttpMethod.Post, Endpoints.Private.OpenOrdersInfo, parameters);
        }

        public string Withdraw(string account, string assetCode, string amount, string memo = "", string mark = "", string type = "")
        {
            if (string.IsNullOrWhiteSpace(account))
                throw new ArgumentException("account cannot be empty. ", nameof(account));
            if (string.IsNullOrWhiteSpace(assetCode))
                throw new ArgumentException("assetCode cannot be empty. ", nameof(assetCode));
            if (string.IsNullOrWhiteSpace(amount))
                throw new ArgumentException("amount cannot be empty. ", nameof(amount));

            var parameters = new Dictionary<string, string>
            {
                { "account", account },
                { "assetCode", assetCode },
                { "amount", amount }
            };

            if (!string.IsNullOrEmpty(memo))
                parameters.Add("memo", memo);
            if (!string.IsNullOrEmpty(mark))
                parameters.Add("mark", mark);
            if (!string.IsNullOrEmpty(type))
                parameters.Add("type", type);

            return Call(HttpMethod.Post, Endpoints.Private.Withdraw, parameters);
        }

        public string CancelWithdraw(string withdrawId)
        {
            if (string.IsNullOrWhiteSpace(withdrawId))
                throw new ArgumentException("withdrawId cannot be empty. ", nameof(withdrawId));

            var parameters = new Dictionary<string, string>
            {
                { "withdrawId", withdrawId }
            };

            return Call(HttpMethod.Post, Endpoints.Private.CancelWithdraw, parameters);
        }

        public string WithdrawalRecord(string assetCode, string status, string pageNo, string pageSize)
        {
            if (string.IsNullOrWhiteSpace(assetCode))
                throw new ArgumentException("assetCode cannot be empty. ", nameof(assetCode));
            if (string.IsNullOrWhiteSpace(status))
                throw new ArgumentException("status cannot be empty. ", nameof(status));
            if (string.IsNullOrWhiteSpace(pageNo))
                throw new ArgumentException("pageNo cannot be empty. ", nameof(pageNo));
            if (string.IsNullOrWhiteSpace(pageSize))
                throw new ArgumentException("pageSize cannot be empty. ", nameof(pageSize));

            var parameters = new Dictionary<string, string>
            {
                { "assetCode", assetCode },
                { "status", status },
                { "pageNo", pageNo },
                { "pageSize", pageSize }
            };

            return Call(HttpMethod.Post, Endpoints.Private.WithdrawalRecord, parameters);
        }

        #endregion
    }
}
