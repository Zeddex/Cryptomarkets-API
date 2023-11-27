using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Cryptomarkets.Apis.Mexc
{
    public class PublicApi
    {
        private const string ApiUrl = "https://api.mexc.com";
        private readonly HttpClient _httpClient;

        internal PublicApi() => _httpClient = CreateAndConfigureHttpClient();

        private static HttpClient CreateAndConfigureHttpClient()
        {
            var handler = new HttpClientHandler();

            handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            return new HttpClient(handler)
            {
                BaseAddress = new Uri(ApiUrl),
                DefaultRequestHeaders =
                {
                    Accept =
                    {
                        new MediaTypeWithQualityHeaderValue("application/json")
                    }
                }
            };
        }

        private string Call(HttpMethod method, string endpoint)
        {
            string requestUri = endpoint;

            return _httpClient.SendAsync(new HttpRequestMessage(method, requestUri)).Result.Content.ReadAsStringAsync().Result;
        }

        private string Call(HttpMethod method, string endpoint, Dictionary<string, string> requestParams)
        {
            string requestUri = Extensions.GenerateParamsString(endpoint, requestParams);

            return _httpClient.SendAsync(new HttpRequestMessage(method, requestUri)).Result.Content.ReadAsStringAsync().Result;
        }

        #region Queries

        public string GetServerTime() => Call(HttpMethod.Get, Endpoints.Public.GetServerTime);

        #endregion
    }
}