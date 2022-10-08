using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Cryptomarkets.Apis.Binance
{
    public class BinanceFuturesApi
    {
        private readonly string _secret;
        private const string ApiUrl = "https://api.binance.com";
        private readonly HttpClient _httpClient;

        internal BinanceFuturesApi(string apiKey, string apiSecret)
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
        /// /sapi/v1/futures/transfer
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="amount"></param>
        /// <param name="type"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string Transfer(string asset, string amount, int type, long recvWindow = 5000)
        {
            string parameters = string.Format("asset={0}&amount={1}&type={2}&recvWindow={3}", asset, amount, type, recvWindow);

            return Call(HttpMethod.Post, Endpoints.Futures.Transfer, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/futures/transfer
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="current">Currently querying page. Start from 1</param>
        /// <param name="size">Max:100</param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string TransactionHistory(
            string asset,
            long startTime = 0,
            long endTime = 0,
            long current = 1,
            long size = 10,
            long recvWindow = 5000)
        {
            string parameters = string.Format("asset={0}&current={1}&size={2}&recvWindow={3}", asset, current, size, recvWindow) +
                (startTime != 0 ? string.Format("&startTime={0}", startTime) : "") +
                (endTime != 0 ? string.Format("&endTime={0}", endTime) : "");

            return Call(HttpMethod.Get, Endpoints.Futures.TransactionHistory, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/futures/loan/borrow
        /// </summary>
        /// <param name="coin"></param>
        /// <param name="amount">is mandatory when collateralAmount is empty</param>
        /// <param name="collateralCoin"></param>
        /// <param name="collateralAmount">is mandatory when amount is empty</param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string Borrow(
            string coin,
            string amount,
            string collateralCoin,
            string collateralAmount,
            long recvWindow = 5000)
        {
            string parameters = string.Format("coin={0}&amount={1}&collateralCoin={2}&collateralAmount={3}&recvWindow={4}",
                coin, amount, collateralCoin, collateralAmount, recvWindow);

            return Call(HttpMethod.Post, Endpoints.Futures.Borrow, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/futures/loan/borrow/history
        /// </summary>
        /// <param name="coin"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="limit">max 1000</param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string BorrowHistory(
            string coin = null,
            long startTime = 0,
            long endTime = 0,
            long limit = 500,
            long recvWindow = 5000)
        {
            string parameters = string.Format("limit={0}&recvWindow={1}", limit, recvWindow) +
                (!string.IsNullOrWhiteSpace(coin) ? string.Format("&coin={0}", coin) : "") +
                (startTime != 0 ? string.Format("&startTime={0}", startTime) : "") +
                (endTime != 0 ? string.Format("&endTime={0}", endTime) : "");

            return Call(HttpMethod.Get, Endpoints.Futures.BorrowHistory, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/futures/loan/repay
        /// </summary>
        /// <param name="coin"></param>
        /// <param name="collateralCoin"></param>
        /// <param name="amount"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string Repay(
            string coin,
            string collateralCoin,
            string amount,
            long recvWindow = 5000)
        {
            string parameters = string.Format("coin={0}&collateralCoin={1}&amount={2}&recvWindow={3}", coin, collateralCoin, amount, recvWindow);

            return Call(HttpMethod.Post, Endpoints.Futures.Repay, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/futures/loan/repay/history
        /// </summary>
        /// <param name="coin"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="limit">max 1000</param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string RepayHistory(
            string coin = null,
            long startTime = 0,
            long endTime = 0,
            long limit = 500,
            long recvWindow = 5000)
        {
            string parameters = string.Format("limit={0}&recvWindow={1}", limit, recvWindow) +
                (!string.IsNullOrWhiteSpace(coin) ? string.Format("&coin={0}", coin) : "") +
                (startTime != 0 ? string.Format("&startTime={0}", startTime) : "") +
                (endTime != 0 ? string.Format("&endTime={0}", endTime) : "");

            return Call(HttpMethod.Get, Endpoints.Futures.RepayHistory, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/futures/loan/wallet
        /// </summary>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string Wallet(long recvWindow = 5000)
        {
            string parameters = string.Format("recvWindow={0}", recvWindow);

            return Call(HttpMethod.Get, Endpoints.Futures.Wallet, true, parameters);
        }

        /// <summary>
        /// /sapi/v2/futures/loan/wallet
        /// </summary>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string WalletV2(long recvWindow = 5000)
        {
            string parameters = string.Format("recvWindow={0}", recvWindow);

            return Call(HttpMethod.Get, Endpoints.Futures.WalletV2, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/futures/loan/configs
        /// </summary>
        /// <param name="collateralCoin"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string Info(string collateralCoin = null, long recvWindow = 5000)
        {
            string parameters = string.Format("recvWindow={0}", recvWindow) +
                (!string.IsNullOrWhiteSpace(collateralCoin) ? string.Format("&collateralCoin={0}", collateralCoin) : "");

            return Call(HttpMethod.Get, Endpoints.Futures.Info, true, parameters);
        }

        /// <summary>
        /// /sapi/v2/futures/loan/configs
        /// </summary>
        /// <param name="loanCoin"></param>
        /// <param name="collateralCoin"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string InfoV2(string loanCoin = null, string collateralCoin = null, long recvWindow = 5000)
        {
            string parameters = string.Format("recvWindow={0}", recvWindow) +
                (!string.IsNullOrWhiteSpace(loanCoin) ? string.Format("&loanCoin={0}", loanCoin) : "") +
                (!string.IsNullOrWhiteSpace(collateralCoin) ? string.Format("&collateralCoin={0}", collateralCoin) : "");

            return Call(HttpMethod.Get, Endpoints.Futures.InfoV2, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/futures/loan/calcAdjustLevel
        /// </summary>
        /// <param name="collateralCoin"></param>
        /// <param name="amoun"></param>
        /// <param name="direction">"ADDITIONAL", "REDUCED"</param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string CalcAdjustLevel(
            string collateralCoin,
            string amount,
            string direction,
            long recvWindow = 5000)
        {
            string parameters = string.Format("collateralCoin={0}&amount={1}&direction={2}&recvWindow={3}", 
                collateralCoin, amount, direction, recvWindow);

            return Call(HttpMethod.Get, Endpoints.Futures.CalcAdjustLevel, true, parameters);
        }

        /// <summary>
        /// /sapi/v2/futures/loan/calcAdjustLevel
        /// </summary>
        /// <param name="loanCoin"></param>
        /// <param name="collateralCoin"></param>
        /// <param name="amount"></param>
        /// <param name="direction">"ADDITIONAL", "REDUCED"</param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string CalcAdjustLevelV2(
            string loanCoin,
            string collateralCoin,
            string amount,
            string direction,
            long recvWindow = 5000)
        {
            string parameters = string.Format("loanCoin={0}&collateralCoin={1}&amount={2}&direction={3}&recvWindow={4}",
                loanCoin, collateralCoin, amount, direction, recvWindow);

            return Call(HttpMethod.Get, Endpoints.Futures.CalcAdjustLevelV2, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/futures/loan/calcMaxAdjustAmount
        /// </summary>
        /// <param name="collateralCoin"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string CalcMaxAdjustAmount(string collateralCoin, long recvWindow = 5000)
        {
            string parameters = string.Format("collateralCoin={0}&recvWindow={1}", collateralCoin, recvWindow);

            return Call(HttpMethod.Get, Endpoints.Futures.CalcMaxAdjustAmount, true, parameters);
        }

        /// <summary>
        /// /sapi/v2/futures/loan/calcMaxAdjustAmount
        /// </summary>
        /// <param name="loanCoin"></param>
        /// <param name="collateralCoin"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string CalcMaxAdjustAmountV2(string loanCoin, string collateralCoin, long recvWindow = 5000)
        {
            string parameters = string.Format("loanCoin={0}&collateralCoin={1}&recvWindow={2}", loanCoin, collateralCoin, recvWindow);

            return Call(HttpMethod.Get, Endpoints.Futures.CalcMaxAdjustAmountV2, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/futures/loan/adjustCollateral
        /// </summary>
        /// <param name="collateralCoin"></param>
        /// <param name="amount"></param>
        /// <param name="direction">"ADDITIONAL", "REDUCED"</param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string AdjustCollateral(
            string collateralCoin,
            string amount,
            string direction,
            long recvWindow = 5000)
        {
            string parameters = string.Format("collateralCoin={0}&amount={1}&direction={2}&recvWindow={3}",
                collateralCoin, amount, direction, recvWindow);

            return Call(HttpMethod.Post, Endpoints.Futures.AdjustCollateral, true, parameters);
        }

        /// <summary>
        /// /sapi/v2/futures/loan/adjustCollateral
        /// </summary>
        /// <param name="loanCoin"></param>
        /// <param name="collateralCoin"></param>
        /// <param name="amount"></param>
        /// <param name="direction">"ADDITIONAL", "REDUCED"</param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string AdjustCollateralV2(
            string loanCoin,
            string collateralCoin,
            string amount,
            string direction,
            long recvWindow = 5000)
        {
            string parameters = string.Format("loanCoin={0}&collateralCoin={1}&amount={2}&direction={3}&recvWindow={4}",
                loanCoin, collateralCoin, amount, direction, recvWindow);

            return Call(HttpMethod.Post, Endpoints.Futures.AdjustCollateralV2, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/futures/loan/adjustCollateral/history
        /// </summary>
        /// <param name="loanCoin"></param>
        /// <param name="collateralCoin"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="limit">max 1000</param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string AdjustCollateralHistory(
            string loanCoin = null,
            string collateralCoin = null,
            long startTime = 0,
            long endTime = 0,
            long limit = 500,
            long recvWindow = 5000)
        {
            string parameters = string.Format("limit={0}&recvWindow={1}", limit, recvWindow) +
                (!string.IsNullOrWhiteSpace(loanCoin) ? string.Format("&loanCoin={0}", loanCoin) : "") +
                (!string.IsNullOrWhiteSpace(collateralCoin) ? string.Format("&collateralCoin={0}", collateralCoin) : "") +
                (startTime != 0 ? string.Format("&startTime={0}", startTime) : "") +
                (endTime != 0 ? string.Format("&endTime={0}", endTime) : "");

            return Call(HttpMethod.Get, Endpoints.Futures.AdjustCollateralHistory, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/futures/loan/liquidationHistory
        /// </summary>
        /// <param name="loanCoin"></param>
        /// <param name="collateralCoin"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="limit">max 1000</param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string LiquidationHistory(
            string loanCoin = null,
            string collateralCoin = null,
            long startTime = 0,
            long endTime = 0,
            long limit = 500,
            long recvWindow = 5000)
        {
            string parameters = string.Format("limit={0}&recvWindow={1}", limit, recvWindow) +
                (!string.IsNullOrWhiteSpace(loanCoin) ? string.Format("&loanCoin={0}", loanCoin) : "") +
                (!string.IsNullOrWhiteSpace(collateralCoin) ? string.Format("&collateralCoin={0}", collateralCoin) : "") +
                (startTime != 0 ? string.Format("&startTime={0}", startTime) : "") +
                (endTime != 0 ? string.Format("&endTime={0}", endTime) : "");

            return Call(HttpMethod.Get, Endpoints.Futures.LiquidationHistory, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/futures/loan/collateralRepayLimit
        /// </summary>
        /// <param name="coin"></param>
        /// <param name="collateralCoin"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string CollateralRepayLimit(string coin, string collateralCoin, long recvWindow = 5000)
        {
            string parameters = string.Format("coin={0}&collateralCoin={1}&recvWindow={2}", coin, collateralCoin, recvWindow);

            return Call(HttpMethod.Get, Endpoints.Futures.CollateralRepayLimit, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/futures/loan/collateralRepay
        /// </summary>
        /// <param name="coin"></param>
        /// <param name="collateralCoin"></param>
        /// <param name="amount">repay amount</param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string CollateralRepayQuote(string coin, string collateralCoin, string amount, long recvWindow = 5000)
        {
            string parameters = string.Format("coin={0}&collateralCoin={1}&amount={2}&recvWindow={3}", coin, collateralCoin, amount, recvWindow);

            return Call(HttpMethod.Get, Endpoints.Futures.CollateralRepayQuote, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/futures/loan/collateralRepay
        /// </summary>
        /// <param name="quoteId"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string RepayCollateral(string quoteId, long recvWindow = 5000)
        {
            string parameters = string.Format("quoteId={0}&recvWindow={1}", quoteId, recvWindow);

            return Call(HttpMethod.Post, Endpoints.Futures.RepayCollateral, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/futures/loan/collateralRepayResult
        /// </summary>
        /// <param name="quoteId"></param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string CollateralRepayResult(string quoteId, long recvWindow = 5000)
        {
            string parameters = string.Format("quoteId={0}&recvWindow={1}", quoteId, recvWindow);

            return Call(HttpMethod.Get, Endpoints.Futures.CollateralRepayResult, true, parameters);
        }

        /// <summary>
        /// /sapi/v1/futures/loan/interestHistory
        /// </summary>
        /// <param name="collateralCoin"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="current">Currently querying page</param>
        /// <param name="limit">Max:1000</param>
        /// <param name="recvWindow"></param>
        /// <returns></returns>
        public string InterestHistory(
            string collateralCoin = null,
            long startTime = 0,
            long endTime = 0,
            long current = 1,
            long limit = 500,
            long recvWindow = 5000)
        {
            string parameters = string.Format("current={0}&limit={1}&recvWindow={2}", current, limit, recvWindow) +
                (!string.IsNullOrWhiteSpace(collateralCoin) ? string.Format("&collateralCoin={0}", collateralCoin) : "") +
                (startTime != 0 ? string.Format("&startTime={0}", startTime) : "") +
                (endTime != 0 ? string.Format("&endTime={0}", endTime) : "");

            return Call(HttpMethod.Get, Endpoints.Futures.InterestHistory, true, parameters);
        }

        #endregion
    }
}
