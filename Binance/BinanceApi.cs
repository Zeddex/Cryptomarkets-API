using System;

namespace Cryptomarkets.Apis.Binance
{
    public class BinanceApi
    {
        private static PublicApi _public;
        private static SpotTradeApi _spotTrade;
        private static MarginTradeApi _marginTrade;
        private static WalletApi _wallet;
        private static FuturesApi _futures;
        private static PrivateApi _private;
        private static WithdrawalApi _withdrawal;
        private static P2pApi _p2p;
        private static bool _isInit;

        /// <summary>
        /// Публичный Api
        /// </summary>
        public static PublicApi Public
        {
            get
            {
                return _public;
            }
        }

        /// <summary>
        /// Приватный Api (заменён на Trade)
        /// </summary>
        public static PrivateApi Private
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
        public static WithdrawalApi Withdrawal
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
        public static SpotTradeApi SpotTrade
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
        public static MarginTradeApi MarginTrade
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
        public static WalletApi Wallet
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
        public static FuturesApi Futures
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
        public static P2pApi P2P
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

            _public = new PublicApi();
            _spotTrade = new SpotTradeApi(apiKey, apiSecret);
            _marginTrade = new MarginTradeApi(apiKey, apiSecret);
            _wallet = new WalletApi(apiKey, apiSecret);
            _futures = new FuturesApi(apiKey, apiSecret);
            _private = new PrivateApi(apiKey, apiSecret);
            _withdrawal = new WithdrawalApi(apiKey, apiSecret);
            _p2p = new P2pApi(apiKey, apiSecret);
            _isInit = true;
        }

        private static void CheckIsInit()
        {
            if (!_isInit)
                throw new Exception("Не установлены ключи BinanceApi");
        }
    }
}
