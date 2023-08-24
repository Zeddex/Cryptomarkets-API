using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Cryptomarkets.Apis.Kucoin
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


        public string DepositAddress(string currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("currency cannot be empty. ", nameof(currency));

            string param = string.Format("currency={0}", currency);

            return Call(HttpMethod.Get, Endpoints.Wallet.DepositAddress, "", param, true);
        }

        public string AccountBallance(string currency = "", string from = "", string to = "", string limit = "", string offset = "")
        {
            string parameters =
                (!string.IsNullOrWhiteSpace(currency) ? string.Format("currency={0}", currency) : "") +
                (!string.IsNullOrWhiteSpace(from) ? string.Format("&from={0}", from) : "") +
                (!string.IsNullOrWhiteSpace(to) ? string.Format("&to={0}", to) : "") +
                (!string.IsNullOrWhiteSpace(limit) ? string.Format("&limit={0}", limit) : "") +
                (!string.IsNullOrWhiteSpace(offset) ? string.Format("&offset={0}", offset) : "");

            return Call(HttpMethod.Get, Endpoints.Wallet.AccountBalance, "", parameters, true);
        }

        #endregion
    }
}
