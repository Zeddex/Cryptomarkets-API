using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Cryptomarkets.Apis.Binance
{
    [Obsolete("Please use BinanceWalletApi instead")]
    public class BinanceWithdrawalApi
    {
        private readonly string _secret;
        private const string ApiUrl = "https://api.binance.com";
        private readonly HttpClient _httpClient;

        internal BinanceWithdrawalApi(string apiKey, string apiSecret)
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

        /// <summary>Submit a withdraw request.</summary>
        /// <param name="asset">Asset</param>
        /// <param name="address">Address</param>
        /// <param name="amount">Amount</param>
        /// <param name="addressTag">Secondary address identifier for coins like XRP,XMR etc.</param>
        /// <param name="addressName">Description of the address</param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string Withdraw(
          string asset,
          string address,
          string amount,
          string addressTag = null,
          string addressName = null,
          long recvWindow = 5000)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>
            {
                [asset] = asset,
                [address] = address,
                [amount] = amount
            };

            if (addressTag != null)
                dictionary[addressTag] = addressTag;

            if (addressTag != null)
                dictionary["name"] = addressName;

            dictionary[nameof(recvWindow)] = string.Format("{0}", recvWindow);

            return Call(HttpMethod.Post, Endpoints.Withdrawal.Withdraw, true, Extensions.ConvertDictionaryToQueryString(dictionary));
        }

        /// <summary>Fetch deposit history.</summary>
        /// <param name="asset">Asset</param>
        /// <param name="status">0(0:pending,1:success)</param>
        /// <param name="startTime">Start time</param>
        /// <param name="endTime">End time</param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string GetDepositHistory(
          string asset = null,
          string status = null,
          string startTime = null,
          string endTime = null,
          long recvWindow = 5000)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            if (asset != null)
                dictionary[asset] = asset;

            if (status != null)
                dictionary[nameof(status)] = status;

            if (startTime != null)
                dictionary[startTime] = startTime;

            if (endTime != null)
                dictionary[endTime] = endTime;

            dictionary[nameof(recvWindow)] = string.Format("{0}", recvWindow);
            return Call(HttpMethod.Get, Endpoints.Withdrawal.DepositHistory, true, Extensions.ConvertDictionaryToQueryString(dictionary));
        }

        /// <summary>Fetch withdraw history.</summary>
        /// <param name="asset">Asset</param>
        /// <param name="status">0(0:Email Sent,1:Cancelled 2:Awaiting Approval 3:Rejected 4:Processing 5:Failure 6:Completed)</param>
        /// <param name="startTime">Start time</param>
        /// <param name="endTime">End time</param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string GetWithdrawHistory(
          string asset = null,
          string status = null,
          string startTime = null,
          string endTime = null,
          long recvWindow = 5000)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            if (asset != null)
                dictionary[asset] = asset;

            if (status != null)
                dictionary[nameof(status)] = status;

            if (startTime != null)
                dictionary[startTime] = startTime;

            if (endTime != null)
                dictionary[endTime] = endTime;

            dictionary[nameof(recvWindow)] = string.Format("{0}", recvWindow);

            return Call(HttpMethod.Get, Endpoints.Withdrawal.WithdrawHistory, true, Extensions.ConvertDictionaryToQueryString(dictionary));
        }

        /// <summary>Fetch deposit address.</summary>
        /// <param name="asset">Asset</param>
        /// <param name="status">Status</param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string GetDepositAddress(string asset, string status = null, long recvWindow = 5000)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            if (asset != null)
                dictionary[asset] = asset;

            if (status != null)
                dictionary[nameof(status)] = status;

            dictionary[nameof(recvWindow)] = string.Format("{0}", recvWindow);

            return Call(HttpMethod.Get, Endpoints.Withdrawal.DepositAddress, true, Extensions.ConvertDictionaryToQueryString(dictionary));
        }

        /// <summary>Fetch account status detail.</summary>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string GetAccountStatus(long recvWindow = 5000) => Call(HttpMethod.Get, Endpoints.Withdrawal.AccountStatus, true,
            Extensions.ConvertDictionaryToQueryString(new Dictionary<string, string>()
            {
                [nameof(recvWindow)] = string.Format("{0}", recvWindow)
            }));

        /// <summary>Fetch system status.</summary>
        /// <returns></returns>
        public string GetSystemStatus() => Call(HttpMethod.Get, Endpoints.Withdrawal.SystemStatus);

        /// <summary>Fetch small amounts of assets exchanged BNB records.</summary>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string GetDustLog(long recvWindow = 5000)
        {
            var dictionary = new Dictionary<string, string>
            {
                [nameof(recvWindow)] = string.Format("{0}", recvWindow)
            };

            string parameters = Extensions.ConvertDictionaryToQueryString(dictionary);

            return Call(HttpMethod.Get, Endpoints.Withdrawal.DustLog, true, parameters);
        }
            

        /// <summary>Fetch trade fee.</summary>
        /// <param name="symbol">Symbol</param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string GetTradeFee(string symbol = null, long recvWindow = 5000)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            if (symbol != null)
                dictionary[symbol] = symbol;

            dictionary[nameof(recvWindow)] = string.Format("{0}", recvWindow);

            return Call(HttpMethod.Get, Endpoints.Withdrawal.TradeFee, true, Extensions.ConvertDictionaryToQueryString(dictionary));
        }

        /// <summary>Fetch asset detail.</summary>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string GetAssetDetail(long recvWindow = 5000) => Call(HttpMethod.Get, Endpoints.Withdrawal.AssetDetail, true,
            Extensions.ConvertDictionaryToQueryString(new Dictionary<string, string>()
            {
                [nameof(recvWindow)] = string.Format("{0}", recvWindow)
            }));

        /// <summary>Fetch sub account list. (For Master Account)</summary>
        /// <param name="email">Sub-account email</param>
        /// <param name="status">Sub-account status: enabled or disabled</param>
        /// <param name="page">Default value: 1</param>
        /// <param name="limit">Default value: 500</param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string QuerySubAccountList(
          string email = null,
          string status = null,
          string page = "1",
          string limit = "500",
          long recvWindow = 5000)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            if (email != null)
                dictionary[email] = email;

            if (status != null)
                dictionary[nameof(status)] = status;

            dictionary[page] = page;
            dictionary[limit] = limit;
            dictionary[nameof(recvWindow)] = string.Format("{0}", recvWindow);

            return Call(HttpMethod.Get, Endpoints.Withdrawal.QuerySubAccountList, true, Extensions.ConvertDictionaryToQueryString(dictionary));
        }

        /// <summary>Fetch transfer history list (For Master Account)</summary>
        /// <param name="email">Sub-account email</param>
        /// <param name="startTime">Default return the history with in 100 days</param>
        /// <param name="endTime">Default return the history with in 100 days</param>
        /// <param name="page">Default value: 1</param>
        /// <param name="limit">Default value: 500</param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string QuerySubAccountTransferHistory(
          string email = null,
          string startTime = null,
          string endTime = null,
          string page = "1",
          string limit = "500",
          long recvWindow = 5000)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            if (email != null)
                dictionary[email] = email;

            if (startTime != null)
                dictionary[startTime] = startTime;

            if (endTime != null)
                dictionary[endTime] = endTime;

            dictionary[page] = page;
            dictionary[limit] = limit;
            dictionary[nameof(recvWindow)] = string.Format("{0}", recvWindow);

            return Call(HttpMethod.Get, Endpoints.Withdrawal.QuerySubAccountTransferHistory, true, Extensions.ConvertDictionaryToQueryString(dictionary));
        }

        /// <summary>Execute sub-account transfer (For Master Account)</summary>
        /// <param name="fromEmail">Sender email</param>
        /// <param name="toEmail">Recipient email</param>
        /// <param name="asset">Asset</param>
        /// <param name="amount">Amount</param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string SubAccountTransfer(
          string fromEmail,
          string toEmail,
          string asset,
          string amount,
          long recvWindow = 5000)
        {
            return Call(HttpMethod.Post, Endpoints.Withdrawal.SubAccountTransfer, true, Extensions.ConvertDictionaryToQueryString(new Dictionary<string, string>()
            {
                [nameof(fromEmail)] = fromEmail,
                [nameof(toEmail)] = toEmail,
                [asset] = asset,
                [amount] = amount,
                [nameof(recvWindow)] = string.Format("{0}", recvWindow)
            }));
        }
    }
}
