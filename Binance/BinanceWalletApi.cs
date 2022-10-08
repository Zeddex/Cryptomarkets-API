using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Cryptomarkets.Apis.Binance
{
    public class BinanceWalletApi
    {
        private readonly string _secret;
        private const string ApiUrl = "https://api.binance.com";
        private readonly HttpClient _httpClient;

        internal BinanceWalletApi(string apiKey, string apiSecret)
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
        /// /sapi/v1/system/status
        /// </summary>
        /// <returns></returns>
        public string SystemStatus()
        {
            return Call(HttpMethod.Get, Endpoints.Wallet.SystemStatus, true);
        }

        /// <summary>
        /// /sapi/v1/capital/config/getall
        /// </summary>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string AllCoinsInfo(long recvWindow = 5000)
        {
            return Call(HttpMethod.Get, Endpoints.Wallet.AllCoinsInfo, true, string.Format("recvWindow={0}", recvWindow));
        }

        /// <summary>
        /// /sapi/v1/accountSnapshot
        /// </summary>
        /// <param name="type">"SPOT", "MARGIN", "FUTURES"</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="limit">min 7, max 30, default 7</param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string DailyAccSnapshot(string type, long startTime = 0, long endTime = 0, int limit = 7, long recvWindow = 5000)
        {
            string parameters = string.Format("type={0}&limit={1}&recvWindow={2}", type.ToUpper(), limit, recvWindow) +
                (startTime != 0 ? string.Format("&startTime={0}", startTime) : "") +
                (endTime != 0 ? string.Format("&endTime={0}", endTime) : "");

            return Call(HttpMethod.Get, Endpoints.Wallet.DailyAccSnapshot, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/account/disableFastWithdrawSwitch
        /// </summary>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string DisableFastWithdraw(long recvWindow = 5000)
        {
            return Call(HttpMethod.Post, Endpoints.Wallet.DisableFastWithdraw, true, string.Format("recvWindow={0}", recvWindow));
        }

        /// <summary>
        /// /sapi/v1/account/enableFastWithdrawSwitch
        /// </summary>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string EnableFastWithdraw(long recvWindow = 5000)
        {
            return Call(HttpMethod.Post, Endpoints.Wallet.EnableFastWithdraw, true, string.Format("recvWindow={0}", recvWindow));
        }

        /// <summary>
        /// /sapi/v1/capital/withdraw/apply
        /// </summary>
        /// <param name="coin"></param>
        /// <param name="address"></param>
        /// <param name="amount"></param>
        /// <param name="withdrawOrderId">client id for withdraw</param>
        /// <param name="network"></param>
        /// <param name="addressTag"></param>
        /// <param name="transactionFeeFlag"></param>
        /// <param name="name"></param>
        /// <param name="walletType">0-spot wallet ，1-funding wallet</param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string Withdraw(
            string coin,
            string address,
            string amount,
            string withdrawOrderId = null,
            string network = null,
            string addressTag = null,
            bool transactionFeeFlag = false,
            string name = null,
            int walletType = 0,
            long recvWindow = 5000)
        {
            string parameters = string.Format("coin={0}&address={1}&amount={2}&walletType={3}&transactionFeeFlag={4}&recvWindow={5}",
                coin.ToUpper(), address, amount, walletType, transactionFeeFlag, recvWindow) +
                (!string.IsNullOrWhiteSpace(withdrawOrderId) ? string.Format("&withdrawOrderId={0}", withdrawOrderId) : "") +
                (!string.IsNullOrWhiteSpace(network) ? string.Format("&network={0}", network) : "") +
                (!string.IsNullOrWhiteSpace(addressTag) ? string.Format("&addressTag={0}", addressTag) : "") +
                (!string.IsNullOrWhiteSpace(name) ? string.Format("&name={0}", name) : "");

            return Call(HttpMethod.Post, Endpoints.Wallet.Withdraw, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/capital/deposit/hisrec
        /// </summary>
        /// <param name="coin"></param>
        /// <param name="status">0:pending,6: credited but cannot withdraw, 1:success</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="offset"></param>
        /// <param name="limit">Max:1000</param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string DepositHistory(
            string coin = null,
            int status = 0,
            long startTime = 0,
            long endTime = 0,
            int offset = 0,
            int limit = 1000,
            long recvWindow = 5000)
        {
            string parameters = string.Format("limit={0}&recvWindow={1}", limit, recvWindow) +
                (!string.IsNullOrWhiteSpace(coin) ? string.Format("&coin={0}", coin.ToUpper()) : "") +
                (status != 0 ? string.Format("&status={0}", status) : "") +
                (startTime != 0 ? string.Format("&startTime={0}", startTime) : "") +
                (endTime != 0 ? string.Format("&endTime={0}", endTime) : "") +
                (offset != 0 ? string.Format("&offset={0}", offset) : "");

            return Call(HttpMethod.Get, Endpoints.Wallet.DepositHistory, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/capital/withdraw/history
        /// </summary>
        /// <param name="coin"></param>
        /// <param name="withdrawOrderId"></param>
        /// <param name="status">0:Email Sent,1:Cancelled 2:Awaiting Approval 3:Rejected 4:Processing 5:Failure 6:Completed</param>
        /// <param name="offset"></param>
        /// <param name="limit">Max: 1000</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string WithdrawHistory(
            string coin = null,
            string withdrawOrderId = null,
            int status = 0,
            int offset = 0,
            int limit = 1000,
            long startTime = 0,
            long endTime = 0,
            long recvWindow = 5000)
        {
            string parameters = string.Format("limit={0}&recvWindow={1}", limit, recvWindow) +
                (!string.IsNullOrWhiteSpace(coin) ? string.Format("&coin={0}", coin.ToUpper()) : "") +
                (!string.IsNullOrWhiteSpace(withdrawOrderId) ? string.Format("&withdrawOrderId={0}", withdrawOrderId) : "") +
                (status != 0 ? string.Format("&status={0}", status) : "") +
                (offset != 0 ? string.Format("&offset={0}", offset) : "") +
                (startTime != 0 ? string.Format("&startTime={0}", startTime) : "") +
                (endTime != 0 ? string.Format("&endTime={0}", endTime) : "");

            return Call(HttpMethod.Get, Endpoints.Wallet.WithdrawHistory, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/capital/deposit/address
        /// </summary>
        /// <param name="coin"></param>
        /// <param name="network"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string DepositAddress(string coin, string network = null, long recvWindow = 5000)
        {
            string parameters = string.Format("coin={0}&recvWindow={1}", coin.ToUpper(), recvWindow) +
                (!string.IsNullOrWhiteSpace(network) ? string.Format("&network={0}", network) : "");

            return Call(HttpMethod.Get, Endpoints.Wallet.DepositAddress, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/account/status
        /// </summary>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string AccountStatus(long recvWindow = 5000)
        {
            return Call(HttpMethod.Get, Endpoints.Wallet.AccountStatus, true, string.Format("recvWindow={0}", recvWindow));
        }

        /// <summary>
        /// /sapi/v1/account/apiTradingStatus
        /// </summary>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string AccountTradingStatus(long recvWindow = 5000)
        {
            return Call(HttpMethod.Get, Endpoints.Wallet.AccountTradingStatus, true, string.Format("recvWindow={0}", recvWindow));
        }

        /// <summary>
        /// /sapi/v1/asset/dribblet
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string DustLog(long startTime = 0, long endTime = 0, long recvWindow = 5000)
        {
            string parameters = string.Format("recvWindow={0}", recvWindow) +
                (startTime != 0 ? string.Format("&startTime={0}", startTime) : "") +
                (endTime != 0 ? string.Format("&endTime={0}", endTime) : "");

            return Call(HttpMethod.Get, Endpoints.Wallet.DustLog, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/asset/dust
        /// </summary>
        /// <param name="asset">The asset being converted</param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string DustTransfer(string asset, long recvWindow = 5000)
        {
            string parameters = string.Format("asset={0}&recvWindow={1}", asset, recvWindow);

            return Call(HttpMethod.Post, Endpoints.Wallet.DustTransfer, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/asset/assetDividend
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="limit">max 500</param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string AssetDividendRecord(
            string asset = null,
            long startTime = 0,
            long endTime = 0,
            int limit = 20,
            long recvWindow = 5000)
        {
            string parameters = string.Format("limit={0}&recvWindow={1}", limit, recvWindow) +
                (!string.IsNullOrWhiteSpace(asset) ? string.Format("&asset={0}", asset) : "") +
                (startTime != 0 ? string.Format("&startTime={0}", startTime) : "") +
                (endTime != 0 ? string.Format("&endTime={0}", endTime) : "");

            return Call(HttpMethod.Get, Endpoints.Wallet.AssetDividendRecord, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/asset/assetDetail
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string AssetDetail(string asset = null, long recvWindow = 5000)
        {
            string parameters = string.Format("recvWindow={0}", recvWindow) +
                (!string.IsNullOrWhiteSpace(asset) ? string.Format("&asset={0}", asset) : "");

            return Call(HttpMethod.Get, Endpoints.Wallet.AssetDetail, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/asset/tradeFee
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string TradeFee(string symbol = null, long recvWindow = 5000)
        {
            string parameters = string.Format("recvWindow={0}", recvWindow) +
                (!string.IsNullOrWhiteSpace(symbol) ? string.Format("&symbol={0}", symbol) : "");

            return Call(HttpMethod.Get, Endpoints.Wallet.TradeFee, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/asset/transfer
        /// </summary>
        /// <param name="type"></param>
        /// <param name="asset"></param>
        /// <param name="amount"></param>
        /// <param name="fromSymbol"></param>
        /// <param name="toSymbol"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string UserUniversalTransfer(
            string type,
            string asset,
            string amount,
            string fromSymbol = null,
            string toSymbol = null,
            long recvWindow = 5000)
        {
            string parameters = string.Format("type={0}&asset={1}&amount={2}&recvWindow={3}", type, asset, amount, recvWindow) +
                (!string.IsNullOrWhiteSpace(fromSymbol) ? string.Format("&fromSymbol={0}", fromSymbol) : "") +
                (!string.IsNullOrWhiteSpace(toSymbol) ? string.Format("&toSymbol={0}", toSymbol) : "");

            return Call(HttpMethod.Post, Endpoints.Wallet.UserUniversalTransfer, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/asset/transfer
        /// </summary>
        /// <param name="type"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="curret"></param>
        /// <param name="size">Max 100</param>
        /// <param name="fromSymbol"></param>
        /// <param name="toSymbol"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string QueryUserTransferHistory(
            string type,
            long startTime = 0,
            long endTime = 0,
            int curret = 1,
            int size = 10,
            string fromSymbol = null,
            string toSymbol = null,
            long recvWindow = 5000)
        {
            string parameters = string.Format("type={0}&curret={1}&size={2}&recvWindow={3}", type, curret, size, recvWindow) +
                (startTime != 0 ? string.Format("&startTime={0}", startTime) : "") +
                (endTime != 0 ? string.Format("&endTime={0}", endTime) : "") +
                (!string.IsNullOrWhiteSpace(fromSymbol) ? string.Format("&fromSymbol={0}", fromSymbol) : "") +
                (!string.IsNullOrWhiteSpace(toSymbol) ? string.Format("&toSymbol={0}", toSymbol) : "");

            return Call(HttpMethod.Get, Endpoints.Wallet.QueryUserTransferHistory, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/asset/get-funding-asset
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="needBtcValuation">true or false</param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string FundingWallet(string asset = null, string needBtcValuation = null, long recvWindow = 5000)
        {
            string parameters = string.Format("recvWindow={0}", recvWindow) +
                (!string.IsNullOrWhiteSpace(asset) ? string.Format("&asset={0}", asset) : "") +
                (!string.IsNullOrWhiteSpace(needBtcValuation) ? string.Format("&needBtcValuation={0}", needBtcValuation) : "");

            return Call(HttpMethod.Post, Endpoints.Wallet.FundingWallet, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/account/apiRestrictions
        /// </summary>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string ApiKeyPermission(long recvWindow = 5000)
        {
            return Call(HttpMethod.Get, Endpoints.Wallet.ApiKeyPermission, true, string.Format("recvWindow={0}", recvWindow));
        }

        #endregion
    }
}
