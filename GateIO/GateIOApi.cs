using System;

namespace Cryptomarkets.Apis.GateIO
{
    public class GateIOApi
    {
        private static GateIOPublicApi _public;
        private static GateIOWalletApi _wallet;
        private static GateIOWithdrawalApi _withdrawal;
        private static GateIOSpotApi _spot;
        private static GateIOMarginApi _margin;
        private static bool _isInit;

        public static GateIOPublicApi Public => _public;

        public static GateIOWalletApi Wallet
        {
            get
            {
                CheckIsInit();
                return _wallet;
            }
        }

        public static GateIOWithdrawalApi Withdrawal
        {
            get
            {
                CheckIsInit();
                return _withdrawal;
            }
        }

        public static GateIOSpotApi Spot
        {
            get
            {
                CheckIsInit();
                return _spot;
            }
        }

        public static GateIOMarginApi Margin
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

            _public = new GateIOPublicApi();
            _wallet = new GateIOWalletApi(apiKey, apiSecret);
            _withdrawal = new GateIOWithdrawalApi(apiKey, apiSecret);
            _spot = new GateIOSpotApi(apiKey, apiSecret);
            _margin = new GateIOMarginApi(apiKey, apiSecret);
            _isInit = true;
        }

        private static void CheckIsInit()
        {
            if (!_isInit)
                throw new Exception("Не установлены ключи Api");
        }
    }
}
