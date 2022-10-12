using System;

namespace Cryptomarkets.Apis.Latoken
{
    public class LatokenApi
    {
        private static PublicApi _public;
        private static AccountApi _account;
        private static TransactionApi _transaction;
        private static OrderApi _order;
        private static TradeApi _trade;
        private static bool _isInit;

        public static PublicApi Public => _public;

        public static AccountApi Account
        {
            get
            {
                CheckIsInit();
                return _account;
            }
        }

        public static TransactionApi Transaction
        {
            get
            {
                CheckIsInit();
                return _transaction;
            }
        }

        public static OrderApi Order
        {
            get
            {
                CheckIsInit();
                return _order;
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
                throw new ArgumentNullException("ApiKey не может быть пустым");

            if (string.IsNullOrWhiteSpace(apiSecret))
                throw new ArgumentNullException("ApiSecret не может быть пустым");

            _public = new PublicApi();
            _account = new AccountApi(apiKey, apiSecret);
            _transaction = new TransactionApi(apiKey, apiSecret);
            _order = new OrderApi(apiKey, apiSecret);
            _trade = new TradeApi(apiKey, apiSecret);
            _isInit = true;
        }

        private static void CheckIsInit()
        {
            if (!_isInit)
                throw new Exception("Не установлены ключи Api");
        }
    }
}
