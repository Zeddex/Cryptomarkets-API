using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Cryptomarkets.Apis.Bitbns
{
    public class SpotApi
    {
        private readonly string _key;
        private readonly string _secret;
        private const string ApiUrl = "https://api...";
        private readonly HttpClient _httpClient;

        public SpotApi(string apiKey, string apiSecret)
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
            configureHttpClient.DefaultRequestHeaders.Add("X-LA-APIKEY", apiKey);
            configureHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return configureHttpClient;
        }


        #region Queries



        #endregion
    }
}
