using System;

namespace Cryptomarkets.Apis.Bybit
{
    public class BybitApi
    {
        private static MarketApi _market;
        private static WalletApi _wallet;
        private static SpotApi _spot;
        private static AccountApi _account;
        private static TradeApi _trade;
        private static bool _isInit;

        public static MarketApi Public => _market;

        public static WalletApi Wallet
        {
            get
            {
                CheckIsInit();
                return _wallet;
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

        public static AccountApi Account
        {
            get
            {
                CheckIsInit();
                return _account;
            }
        }

        public static TradeApi Trade
        {
            get
            {
                CheckIsInit();
                return _trade;
            }
        }

        public static void Init(string apiKey, string apiSecret)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentNullException("Api key is empty");

            if (string.IsNullOrWhiteSpace(apiSecret))
                throw new ArgumentNullException("Api secret is empty");

            _market = new MarketApi();
            _wallet = new WalletApi(apiKey, apiSecret);
            _spot = new SpotApi(apiKey, apiSecret);
            _account = new AccountApi(apiKey, apiSecret);
            _trade = new TradeApi(apiKey, apiSecret);
            _isInit = true;
        }

        private static void CheckIsInit()
        {
            if (!_isInit)
                throw new Exception("No api key");
        }
    }
}
