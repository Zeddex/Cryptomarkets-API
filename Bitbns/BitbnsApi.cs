using System;

namespace Cryptomarkets.Apis.Bitbns
{
    public class BitbnsApi
    {
        private static PublicApi _public;
        private static WalletApi _wallet;
        private static WithdrawalApi _withdrawal;
        private static SpotApi _spot;
        private static MarginApi _margin;
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

        public static MarginApi Margin
        {
            get
            {
                CheckIsInit();
                return _margin;
            }
        }


        public static void Init(string apiKey, string apiSecret)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentNullException("ApiKey не может быть пустым");

            if (string.IsNullOrWhiteSpace(apiSecret))
                throw new ArgumentNullException("ApiSecret не может быть пустым");

            _public = new PublicApi();
            _wallet = new WalletApi(apiKey, apiSecret);
            _withdrawal = new WithdrawalApi(apiKey, apiSecret);
            _spot = new SpotApi(apiKey, apiSecret);
            _margin = new MarginApi(apiKey, apiSecret);
            _isInit = true;
        }

        private static void CheckIsInit()
        {
            if (!_isInit)
                throw new Exception("Не установлены ключи Api");
        }
    }
}
