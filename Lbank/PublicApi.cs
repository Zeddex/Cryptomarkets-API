using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Cryptomarkets.Apis.Lbank
{
    public class PublicApi
    {
        private const string ApiUrl = "https://api...";
        private readonly HttpClient _httpClient;

        public PublicApi() => _httpClient = CreateAndConfigureHttpClient();

        private static HttpClient CreateAndConfigureHttpClient()
        {
            HttpClientHandler handler = new HttpClientHandler();

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

        private string Call(HttpMethod method, string endpoint, string param = null)
        {
            string requestUri = endpoint + param;

            return _httpClient.SendAsync(new HttpRequestMessage(method, requestUri)).Result.Content.ReadAsStringAsync().Result;
        }

        private string Call(HttpMethod method, string endpoint, Dictionary<string, string> parameters)
        {
            string requestUri = Extensions.GenerateParamsString(endpoint, parameters);

            return _httpClient.SendAsync(new HttpRequestMessage(method, requestUri)).Result.Content.ReadAsStringAsync().Result;
        }

        #region Queries



        #endregion
    }
}
