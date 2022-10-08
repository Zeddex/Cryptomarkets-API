using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Policy;
using System.Text;

namespace Cryptomarkets.Apis.GateIO
{
    public class GateIOMarginApi
    {
        private readonly string _key;
        private readonly string _secret;
        private const string ApiUrl = "https://api.gateio.ws";
        private readonly HttpClient _httpClient;

        public GateIOMarginApi(string apiKey, string apiSecret)
        {
            _key = apiKey;
            _secret = apiSecret;
            _httpClient = CreateAndConfigureHttpClient(_key);
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

                if (method.Method == "POST" || method.Method == "PATCH")
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

            else
            {
                return _httpClient.SendAsync(new HttpRequestMessage(method, requestUri)).Result.Content.ReadAsStringAsync().Result;
            }

        }

        #region Queries

        public string CurrencyPairs() => Call(HttpMethod.Get, Endpoints.Margin.CurrencyPairs);

        public string QueryPair(string pair) => Call(HttpMethod.Get, Endpoints.Margin.QueryPair, pair);

        public string FundingBook(string currency) => Call(HttpMethod.Get, Endpoints.Margin.FundingBook, "", currency);

        public string AccountsList(string pair) => Call(HttpMethod.Get, Endpoints.Margin.AccountsList, "", pair, true);

        public string AccountBook(
            string currency = "",
            string currencyPair = "",
            string from = "",
            string to = "",
            string page = "",
            string limit = "")
        {
            string parameters = (!string.IsNullOrWhiteSpace(currency) ? string.Format("&currency={0}", currency) : "") +
                                (!string.IsNullOrWhiteSpace(currencyPair) ? string.Format("&currency_pair={0}", currencyPair) : "") +
                                (!string.IsNullOrWhiteSpace(from) ? string.Format("&from={0}", from) : "") +
                                (!string.IsNullOrWhiteSpace(to) ? string.Format("&to={0}", to) : "") +
                                (!string.IsNullOrWhiteSpace(page) ? string.Format("&page={0}", page) : "") +
                                (!string.IsNullOrWhiteSpace(limit) ? string.Format("&limit={0}", limit) : "");

            return Call(HttpMethod.Get, Endpoints.Margin.AccountBook, "", parameters, true);
        }

        public string FundingAccounts(string currency = "")
        {
            string param = !string.IsNullOrWhiteSpace(currency) ? string.Format("currency={0}", currency) : "";

            return Call(HttpMethod.Get, Endpoints.Margin.FundingAccounts, "", param, true);
        }

        public string LendBorrow(
            string side,
            string currency,
            string amount,
            string rate = "",
            string days = "",
            string autoRenew = "",
            string currencyPair = "",
            string feeRate = "",
            string origId = "",
            string text = "")
        {
            if (string.IsNullOrWhiteSpace(side))
                throw new ArgumentException("side cannot be empty. ", nameof(side));

            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("currency cannot be empty. ", nameof(currency));

            if (string.IsNullOrWhiteSpace(amount))
                throw new ArgumentException("amount cannot be empty. ", nameof(amount));

            string parameters = string.Format("side={0}&currency={1}&amount={2}", side, currency, amount) +
                                (!string.IsNullOrWhiteSpace(rate) ? string.Format("&rate={0}", rate) : "") +
                                (!string.IsNullOrWhiteSpace(days) ? string.Format("&days={0}", days) : "") +
                                (!string.IsNullOrWhiteSpace(autoRenew) ? string.Format("&auto_renew={0}", autoRenew) : "") +
                                (!string.IsNullOrWhiteSpace(currencyPair) ? string.Format("&currency_pair={0}", currencyPair) : "") +
                                (!string.IsNullOrWhiteSpace(feeRate) ? string.Format("&fee_rate={0}", feeRate) : "") +
                                (!string.IsNullOrWhiteSpace(origId) ? string.Format("&orig_id={0}", origId) : "") +
                                (!string.IsNullOrWhiteSpace(text) ? string.Format("&text={0}", text) : "");

            return Call(HttpMethod.Post, Endpoints.Margin.LendBorrow, "", parameters, true);
        }

        public string LoansList(
            string status,
            string side,
            string currency = "",
            string currencyPair = "",
            string sortBy = "",
            string reverseSort = "",
            string page = "",
            string limit = "")
        {
            if (string.IsNullOrWhiteSpace(status))
                throw new ArgumentException("status cannot be empty. ", nameof(status));

            if (string.IsNullOrWhiteSpace(side))
                throw new ArgumentException("side cannot be empty. ", nameof(side));

            string parameters = string.Format("status={0}&side={1}", status, side) +
                                (!string.IsNullOrWhiteSpace(currency) ? string.Format("&currency={0}", currency) : "") +
                                (!string.IsNullOrWhiteSpace(currencyPair) ? string.Format("&currency_pair={0}", currencyPair) : "") +
                                (!string.IsNullOrWhiteSpace(sortBy) ? string.Format("&sort_by={0}", sortBy) : "") +
                                (!string.IsNullOrWhiteSpace(reverseSort) ? string.Format("&reverse_sort={0}", reverseSort) : "") +
                                (!string.IsNullOrWhiteSpace(page) ? string.Format("&page={0}", page) : "") +
                                (!string.IsNullOrWhiteSpace(limit) ? string.Format("&limit={0}", limit) : "");

            return Call(HttpMethod.Get, Endpoints.Margin.LoansList, "", parameters, true);
        }

        public string MergeLoans(string currency, string ids)
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("currency cannot be empty. ", nameof(currency));

            if (string.IsNullOrWhiteSpace(ids))
                throw new ArgumentException("ids cannot be empty. ", nameof(ids));

            string parameters = string.Format("currency={0}&ids={1}", currency, ids);

            return Call(HttpMethod.Post, Endpoints.Margin.MergeLoans, "", parameters, true);
        }

        public string LoanDetails(string side, string loanId)
        {
            if (string.IsNullOrWhiteSpace(side))
                throw new ArgumentException("side cannot be empty. ", nameof(side));

            if (string.IsNullOrWhiteSpace(loanId))
                throw new ArgumentException("loanId cannot be empty. ", nameof(loanId));

            string param = string.Format("side={0}", side);

            return Call(HttpMethod.Get, Endpoints.Margin.LoanDetails, loanId, param, true);
        }

        public string CancelLendingLoan(string currency, string loanId)
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("currency cannot be empty. ", nameof(currency));

            if (string.IsNullOrWhiteSpace(loanId))
                throw new ArgumentException("loanId cannot be empty. ", nameof(loanId));

            string param = string.Format("currency={0}", currency);

            return Call(HttpMethod.Delete, Endpoints.Margin.CancelLendingLoan, loanId, param, true);
        }

        public string AllRepaymentRecords(string loanId)
        {
            if (string.IsNullOrWhiteSpace(loanId))
                throw new ArgumentException("loanId cannot be empty. ", nameof(loanId));

            string pathParam = loanId + "/repayment";

            return Call(HttpMethod.Get, Endpoints.Margin.AllRepaymentRecords, pathParam, "", true);
        }

        public string LoanRepaymentRecords(string loanId, string status = "", string page = "", string limit = "")
        {
            if (string.IsNullOrWhiteSpace(loanId))
                throw new ArgumentException("loanId cannot be empty. ", nameof(loanId));


            string parameters = string.Format("loan_id={0}", loanId) +
                                (!string.IsNullOrWhiteSpace(status) ? string.Format("&status={0}", status) : "") +
                                (!string.IsNullOrWhiteSpace(page) ? string.Format("&page={0}", page) : "") +
                                (!string.IsNullOrWhiteSpace(limit) ? string.Format("&limit={0}", limit) : "");

            return Call(HttpMethod.Get, Endpoints.Margin.LoanRepaymentRecords, "", parameters, true);
        }

        public string GetLoanRecord(string loanId, string loanRecordId)
        {
            if (string.IsNullOrWhiteSpace(loanId))
                throw new ArgumentException("loanId cannot be empty. ", nameof(loanId));

            if (string.IsNullOrWhiteSpace(loanRecordId))
                throw new ArgumentException("loanRecordId cannot be empty. ", nameof(loanRecordId));

            string param = string.Format("loan_id={0}", loanId);

            return Call(HttpMethod.Get, Endpoints.Margin.GetLoanRecord, loanRecordId, param, true);
        }

        public string GetMaxTransferAmount(string currency, string currencyPair = "")
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("currency cannot be empty. ", nameof(currency));

            string parameters = string.Format("currency={0}", currency) +
                                (!string.IsNullOrWhiteSpace(currencyPair) ? string.Format("&currency_pair={0}", currencyPair) : "");

            return Call(HttpMethod.Get, Endpoints.Margin.GetMaxTransferAmount, "", parameters, true);
        }

        public string GetMaxBorrowAmount(string currency, string currencyPair = "")
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("currency cannot be empty. ", nameof(currency));

            string parameters = string.Format("currency={0}", currency) +
                                (!string.IsNullOrWhiteSpace(currencyPair) ? string.Format("&currency_pair={0}", currencyPair) : "");

            return Call(HttpMethod.Get, Endpoints.Margin.GetMaxBorrowAmount, "", parameters, true);
        }

        public string CrossMarginSupported() => Call(HttpMethod.Get, Endpoints.Margin.CrossMarginSupported);

        public string CrossMarginSupportedDetails(string currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("currency cannot be empty. ", nameof(currency));

            return Call(HttpMethod.Get, Endpoints.Margin.CrossMarginSupportedDetails, currency);
        }

        public string CrossMarginAccount() => Call(HttpMethod.Get, Endpoints.Margin.CrossMarginAccount, "", "", true);

        public string CrossMarginAccHistory(
            string currency = "",
            string from = "",
            string to = "",
            string page = "",
            string limit = "",
            string type = "")
        {
            string parameters = (!string.IsNullOrWhiteSpace(currency) ? string.Format("&currency={0}", currency) : "") +
                                (!string.IsNullOrWhiteSpace(from) ? string.Format("&from={0}", from) : "") +
                                (!string.IsNullOrWhiteSpace(to) ? string.Format("&to={0}", to) : "") +
                                (!string.IsNullOrWhiteSpace(page) ? string.Format("&page={0}", page) : "") +
                                (!string.IsNullOrWhiteSpace(limit) ? string.Format("&limit={0}", limit) : "") +
                                (!string.IsNullOrWhiteSpace(type) ? string.Format("&type={0}", type) : "");

            return Call(HttpMethod.Get, Endpoints.Margin.CrossMarginAccHistory, "", parameters, true);
        }

        public string CreateCMBorrowLoan(string currency, string amount, string text = "")
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("currency cannot be empty. ", nameof(currency));

            if (string.IsNullOrWhiteSpace(amount))
                throw new ArgumentException("amount cannot be empty. ", nameof(amount));

            string parameters = string.Format("currency={0}&amount={1}", currency, amount) +
                                (!string.IsNullOrWhiteSpace(text) ? string.Format("&text={0}", text) : "");

            return Call(HttpMethod.Post, Endpoints.Margin.CreateCMBorrowLoan, "", parameters, true);
        }

        public string CrossMarginBorrowHistory(string status, string currency = "", string limit = "", string offset = "", string reverse = "")
        {
            if (string.IsNullOrWhiteSpace(status))
                throw new ArgumentException("status cannot be empty. ", nameof(status));

            string parameters = string.Format("status={0}", status) +
                                (!string.IsNullOrWhiteSpace(currency) ? string.Format("&currency={0}", currency) : "") +
                                (!string.IsNullOrWhiteSpace(limit) ? string.Format("&limit={0}", limit) : "") +
                                (!string.IsNullOrWhiteSpace(offset) ? string.Format("&offset={0}", offset) : "") +
                                (!string.IsNullOrWhiteSpace(reverse) ? string.Format("&reverse={0}", reverse) : "");

            return Call(HttpMethod.Get, Endpoints.Margin.CrossMarginBorrowHistory, "", parameters, true);
        }

        public string BorrowLoanDetail(string loanId)
        {
            if (string.IsNullOrWhiteSpace(loanId))
                throw new ArgumentException("loanId cannot be empty. ", nameof(loanId));

            return Call(HttpMethod.Get, Endpoints.Margin.BorrowLoanDetail, loanId, "", true);
        }

        public string Repayments(string currency, string amount)
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("currency cannot be empty. ", nameof(currency));

            if (string.IsNullOrWhiteSpace(amount))
                throw new ArgumentException("amount cannot be empty. ", nameof(amount));

            string parameters = string.Format("currency={0}&amount={1}", currency, amount);

            return Call(HttpMethod.Post, Endpoints.Margin.Repayments, "", parameters, true);
        }

        public string CrossMarginRepayments(string currency = "", string loanId = "", string limit = "", string offset = "", string reverse = "")
        {
            string parameters = (!string.IsNullOrWhiteSpace(currency) ? string.Format("&currency={0}", currency) : "") +
                                (!string.IsNullOrWhiteSpace(loanId) ? string.Format("&loan_id={0}", loanId) : "") +
                                (!string.IsNullOrWhiteSpace(limit) ? string.Format("&limit={0}", limit) : "") +
                                (!string.IsNullOrWhiteSpace(offset) ? string.Format("&offset={0}", offset) : "") +
                                (!string.IsNullOrWhiteSpace(reverse) ? string.Format("&reverse={0}", reverse) : "");

            return Call(HttpMethod.Get, Endpoints.Margin.CrossMarginRepayments, "", parameters, true);
        }

        public string MaxTransferAmount(string currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("currency cannot be empty. ", nameof(currency));

            string param = string.Format("currency={0}", currency);

            return Call(HttpMethod.Get, Endpoints.Margin.MaxTransferAmount, "", param, true);
        }

        public string MaxBorrowableAmount(string currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("currency cannot be empty. ", nameof(currency));

            string param = string.Format("currency={0}", currency);

            return Call(HttpMethod.Get, Endpoints.Margin.MaxBorrowableAmount, "", param, true);
        }

        #endregion
    }
}
