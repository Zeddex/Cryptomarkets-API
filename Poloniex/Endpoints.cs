using System;

namespace Cryptomarkets.Apis.Poloniex
{
    public class Endpoints
    {
        public static class Public
        {
            public const string SymbolInfo = "/markets/";
            public const string AllSymbolsInfo = "/markets";
            public const string CurrencyInfo = "/currencies/";
            public const string ServerTime = "/timestamp";
            public const string AllSymbolsPrices = "/markets/price";
            public const string SymbolPrice = "/markets/"; // "/markets/{symbol}/price"
            public const string OrderBook = "/markets/";    // "/markets/{symbol}/orderBook"
            public const string Trades = "/markets/";    // "/markets/{symbol}/trades"
            public const string Candles = "/markets/";    // "/markets/{symbol}/candles"
            public const string Ticker = "/markets/";    // "/markets/{symbol}/ticker24h"
            public const string Tickers = "/markets/ticker24h";
        }

        public static class Account
        {
            public const string AccountInfo = "/accounts";
            public const string AccountBalance = "/accounts/";   // "/accounts/{id}/balances"
            public const string AllAccountsBalances = "/accounts/balances";
            public const string FeeInfo = "/feeinfo";
        }

        public static class Order
        {
            public const string CreateOrder = "/orders";
            public const string OpenOrders = "/orders";
            public const string OrderDetails = "/orders/";
            public const string CancelOrder = "/orders/";
            public const string CancelAllOrders = "/orders";
            public const string OrdersHistory = "/orders/history";
        }

        public static class Trade
        {
            public const string TradeHistory = "/trades";
            public const string TradesByOrderId = "/orders/";   // "/orders/{id}/trades"
        }

        public static class Wallet
        {
            public const string DepositAddresses = "/wallets/addresses";
            public const string NewCurrencyAddress = "/wallets/address";
            public const string Withdraw = "/wallets/withdraw";
        }
    }
}
