using System;

namespace Cryptomarkets.Apis.Bybit
{
    // https://bybit-exchange.github.io/docs/v5/intro

    internal class Endpoints
    {
        internal static class Market
        {
            public const string GetServerTime = "/v5/market/time";
            public const string GetKline = "/v5/market/kline";
            public const string GetOrderBook = "/v5/market/orderbook";
            public const string GetTickers = "/v5/market/tickers";
        }

        internal static class Wallet
        {
            public const string Withdraw = "/v5/asset/withdraw/create";
        }

        internal static class Account
        {
            public const string Info = "/v5/account/info";
            public const string Balance = "/v5/account/wallet-balance";
        }

        internal static class Spot
        {
            public const string Borrow = "/v5/spot-cross-margin-trade/loan";
        }

        internal static class Trade
        {
            public const string PlaceOrder = "/v5/order/create";
            public const string CancelOrder = "/v5/order/cancel";
            public const string GetOpenOrders = "/v5/order/realtime";
        }
    }
}