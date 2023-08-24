using System;

namespace Cryptomarkets.Apis.Huobi
{
    public class HuobiApi
    {
        private static PublicApi _public;
        private static WalletApi _wallet;
        private static WithdrawalApi _withdrawal;
        private static SpotApi _spot;
        private static bool _isInit;

        public static PublicApi Public => _public;

        public static WalletApi Wallet
        {
            get
            {
                CheckIsInit();
                return _wallet;
            }
        }

        public static WithdrawalApi Withdrawal
        {
            get
            {
                CheckIsInit();
                return _withdrawal;
            }
        }

        public static SpotApi Spot
        {
            get
            {
                CheckIsInit();
                return _spot;
            }
        }

        public static void Init(string apiKey, string apiSecret)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentNullException("Api key is empty");

            if (string.IsNullOrWhiteSpace(apiSecret))
                throw new ArgumentNullException("Api secret is empty");

            _public = new PublicApi();
            _wallet = new WalletApi(apiKey, apiSecret);
            _withdrawal = new WithdrawalApi(apiKey, apiSecret);
            _spot = new SpotApi(apiKey, apiSecret);
            _isInit = true;
        }

        private static void CheckIsInit()
        {
            if (!_isInit)
                throw new Exception("No api key");
        }
    }
}
