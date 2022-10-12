using System;

namespace Cryptomarkets.Apis.Latoken
{
    public class Endpoints
    {
        public static class Public
        {
            public const string ActivePairs = "/v2/pair";
            public const string AvailablePairs = "/v2/pair/available";
            public const string ActiveCurrencies = "/v2/currency";
            public const string AvailableCurrencies = "/v2/currency/available";
            public const string AllTickers = "/v2/ticker";
            public const string CurrencyByTag = "/v2/currency/";
            public const string TickerForPair = "/v2/ticker/";    // /v2/ticker/{base}/{quote}
            public const string OrderBookByPair = "/v2/book/";    // /v2/book/{currency}/{quote}
            public const string AnyTradesByPair = "/v2/trade/history/";   // /v2/trade/history/{currency}/{quote}
            public const string ServerTime = "/v2/time";
        }

        public static class Account
        {
            public const string Balances = "/v2/auth/account";
            public const string BalancesByCurrency = "/v2/auth/account/currency/"; // /v2/auth/account/currency/{currency}/{type}
        }

        public static class Transaction
        {
            public const string TransactionById = "/v2/auth/transaction/"; // /v2/auth/transaction/{id}
            public const string RequestWithdrawal = "/v2/auth/transaction/withdraw";
            public const string ConfirmWithdrawal = "/v2/auth/transaction/withdraw/confirm";
        }

        public static class Order
        {
            public const string AllOrders = "/v2/auth/order";
            public const string CancelOrder = "/v2/auth/order/cancel";
            public const string CancelAllOrders = "/v2/auth/order/cancelAll";
            public const string GetOrder = "/v2/auth/order/getOrder/";  // /v2/auth/order/getOrder/{id}
            public const string ActiveOrders = "/v2/auth/order/active";
            public const string NewOrder = "/v2/auth/order/place";
        }

        public static class Trade
        {
            public const string GetTrades = "/v2/auth/trade";
            public const string TradesByPair = "/v2/auth/trade/pair/";    // /v2/auth/trade/pair/{currency}/{quote}
            public const string FeeForPair = "/v2/trade/fee/";    // /v2/trade/fee/{currency}/{quote}
        }
    }
}
