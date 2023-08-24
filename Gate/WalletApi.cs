using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Cryptomarkets.Apis.GateIO
{
    public class WalletApi
    {
        private readonly string _secret;
        private const string ApiUrl = "https://api.gateio.ws/api/v4";
        private readonly HttpClient _httpClient;

        public WalletApi(string apiKey, string apiSecret)
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

        public string ChainsList(string currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("currency cannot be empty. ", nameof(currency));

            string param = string.Format("currency={0}", currency);

            return Call(HttpMethod.Get, Endpoints.Wallet.ChainsList, "", param, false);
        }

        public string DepositAddress(string currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("currency cannot be empty. ", nameof(currency));

            string param = string.Format("currency={0}", currency);

            return Call(HttpMethod.Get, Endpoints.Wallet.DepositAddress, "", param, true);
        }

        public string WithdrawalRecords(string currency = "", string from = "", string to = "", string limit = "", string offset = "")
        {
            string parameters =
                (!string.IsNullOrWhiteSpace(currency) ? string.Format("currency={0}", currency) : "") +
                (!string.IsNullOrWhiteSpace(from) ? string.Format("&from={0}", from) : "") +
                (!string.IsNullOrWhiteSpace(to) ? string.Format("&to={0}", to) : "") +
                (!string.IsNullOrWhiteSpace(limit) ? string.Format("&limit={0}", limit) : "") +
                (!string.IsNullOrWhiteSpace(offset) ? string.Format("&offset={0}", offset) : "");

            return Call(HttpMethod.Get, Endpoints.Wallet.WithdrawalRecords, "", parameters, true);
        }

        public string DepositRecords(string currency = "", string from = "", string to = "", string limit = "", string offset = "")
        {
            string parameters =
                (!string.IsNullOrWhiteSpace(currency) ? string.Format("currency={0}", currency) : "") +
                (!string.IsNullOrWhiteSpace(from) ? string.Format("&from={0}", from) : "") +
                (!string.IsNullOrWhiteSpace(to) ? string.Format("&to={0}", to) : "") +
                (!string.IsNullOrWhiteSpace(limit) ? string.Format("&limit={0}", limit) : "") +
                (!string.IsNullOrWhiteSpace(offset) ? string.Format("&offset={0}", offset) : "");

            return Call(HttpMethod.Get, Endpoints.Wallet.DepositRecords, "", parameters, true);
        }

        public string TransferBetweenAccs(
            string currency,
            string from,
            string to,
            string amount,
            string curryncyPair = "",
            string settle = "")
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("currencyPair cannot be empty. ", nameof(currency));

            if (string.IsNullOrWhiteSpace(from))
                throw new ArgumentException("from cannot be empty. ", nameof(from));

            if (string.IsNullOrWhiteSpace(to))
                throw new ArgumentException("to cannot be empty. ", nameof(to));

            if (string.IsNullOrWhiteSpace(amount))
                throw new ArgumentException("amount cannot be empty. ", nameof(amount));

            string parameters = string.Format("currency={0}&from={1}&to={2}&amount={3}", currency, from, to, amount) +
                                (!string.IsNullOrWhiteSpace(curryncyPair) ? string.Format("&currency_pair={0}", curryncyPair) : "") +
                                (!string.IsNullOrWhiteSpace(settle) ? string.Format("&settle={0}", settle) : "");

            return Call(HttpMethod.Post, Endpoints.Wallet.TransferBetweenAccs, "", parameters, true);
        }

        public string WithdrawalStatus(string currency = "")
        {
            string param = !string.IsNullOrWhiteSpace(currency) ? string.Format("currency={0}", currency) : "";

            return Call(HttpMethod.Get, Endpoints.Wallet.WithdrawalStatus, "", param, true);
        }

        public string SavedAddress(string currency, string chain = "", string limit = "")
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("currency cannot be empty. ", nameof(currency));

            string parameters = string.Format("currency={0}", currency) +
                                (!string.IsNullOrWhiteSpace(chain) ? string.Format("&chain={0}", chain) : "") +
                                (!string.IsNullOrWhiteSpace(limit) ? string.Format("&limit={0}", limit) : "");

            return Call(HttpMethod.Get, Endpoints.Wallet.SavedAddress, "", parameters, true);
        }

        public string TradingFee(string currencyPair = "")
        {
            string param = !string.IsNullOrWhiteSpace(currencyPair) ? string.Format("currency_pair={0}", currencyPair) : "";

            return Call(HttpMethod.Get, Endpoints.Wallet.TradingFee, "", param, true);
        }

        public string TotalBalance(string currency = "")
        {
            string param = !string.IsNullOrWhiteSpace(currency) ? string.Format("currency_pair={0}", currency) : "";

            return Call(HttpMethod.Get, Endpoints.Wallet.TotalBalance, "", param, true);
        }

        public string SubAccBalances(string subUid = "")
        {
            string param = !string.IsNullOrWhiteSpace(subUid) ? string.Format("sub_uid={0}", subUid) : "";

            return Call(HttpMethod.Get, Endpoints.Wallet.SubAccBalances, "", param, true);
        }

        #endregion
    }
}
