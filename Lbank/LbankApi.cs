using System;

namespace Cryptomarkets.Apis.Lbank
{
    public class LbankApi
    {
        private static PublicApi _public;
        private static PrivateApi _private;
        private static bool _isInit;

        public static PublicApi Public => _public;

        public static PrivateApi Private
        {
            get
            {
                CheckIsInit();
                return _private;
            }
        }


        public static void Init(string apiKey, string apiSecret)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentNullException("Api Key is empty");

            if (string.IsNullOrWhiteSpace(apiSecret))
                throw new ArgumentNullException("Api Secret is empty");

            _public = new PublicApi();
            _private = new PrivateApi(apiKey, apiSecret);
            _isInit = true;
        }

        private static void CheckIsInit()
        {
            if (!_isInit)
                throw new Exception("Api data not set");
        }
    }
}
