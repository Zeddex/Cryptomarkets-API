using System;

namespace Cryptomarkets.Apis.Bybit
{
    internal class Endpoints
    {
        internal static class Public
        {
            public const string CoinInfo = "/api/...";
        }

        internal static class Wallet
        {
            public const string DepositAddress = "/api/...";
            public const string AccountBalance = "/api/...";

        }

        internal static class Withdrawal
        {
            public const string Withdraw = "/api/...";
        }

        internal static class Spot
        {
            public const string Buy = "/api/...";
            public const string Sell = "/api/...";
        }
    }
}
