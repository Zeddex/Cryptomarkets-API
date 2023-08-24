using System;

namespace Cryptomarkets.Apis.Binance
{
    public class BinanceApi
    {
        private static BinancePublicApi _public;
        private static BinanceSpotTradeApi _spotTrade;
        private static BinanceMarginTradeApi _marginTrade;
        private static BinanceWalletApi _wallet;
        private static BinanceFuturesApi _futures;
        private static BinancePrivateApi _private;
        private static BinanceWithdrawalApi _withdrawal;
        private static BinanceP2P _p2p;
        private static bool _isInit;

        /// <summary>
        /// Публичный Api
        /// </summary>
        public static BinancePublicApi Public
        {
            get
            {
                return _public;
            }
        }

        /// <summary>
        /// Приватный Api (заменён на Trade)
        /// </summary>
        public static BinancePrivateApi Private
        {
            get
            {
                CheckIsInit();
                return _private;
            }
        }

        /// <summary>
        /// Api для снятия (заменён на Wallet)
        /// </summary>
        public static BinanceWithdrawalApi Withdrawal
        {
            get
            {
                CheckIsInit();
                return _withdrawal;
            }
        }

        /// <summary>
        /// Spot Trade API
        /// </summary>
        public static BinanceSpotTradeApi SpotTrade
        {
            get
            {
                CheckIsInit();
                return _spotTrade;
            }
        }

        /// <summary>
        /// Margin Trade API
        /// </summary>
        public static BinanceMarginTradeApi MarginTrade
        {
            get
            {
                CheckIsInit();
                return _marginTrade;
            }
        }


        /// <summary>
        /// Wallet API
        /// </summary>
        public static BinanceWalletApi Wallet
        {
            get
            {
                CheckIsInit();
                return _wallet;
            }
        }

        /// <summary>
        /// Futures API
        /// </summary>
        public static BinanceFuturesApi Futures
        {
            get
            {
                CheckIsInit();
                return _futures;
            }
        }

        /// <summary>
        /// P2P
        /// </summary>
        public static BinanceP2P P2P
        {
            get
            {
                CheckIsInit();
                return _p2p;
            }
        }

        public static void Init(string apiKey, string apiSecret)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentNullException("ApiKey не может быть пустым");

            if (string.IsNullOrWhiteSpace(apiSecret))
                throw new ArgumentNullException("ApiSecret не может быть пустым");

            _public = new BinancePublicApi();
            _spotTrade = new BinanceSpotTradeApi(apiKey, apiSecret);
            _marginTrade = new BinanceMarginTradeApi(apiKey, apiSecret);
            _wallet = new BinanceWalletApi(apiKey, apiSecret);
            _futures = new BinanceFuturesApi(apiKey, apiSecret);
            _private = new BinancePrivateApi(apiKey, apiSecret);
            _withdrawal = new BinanceWithdrawalApi(apiKey, apiSecret);
            _p2p = new BinanceP2P(apiKey, apiSecret);
            _isInit = true;
        }

        private static void CheckIsInit()
        {
            if (!_isInit)
                throw new Exception("Не установлены ключи BinanceApi");
        }
    }
}
