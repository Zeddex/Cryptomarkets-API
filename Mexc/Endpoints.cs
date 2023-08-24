using System;

namespace Cryptomarkets.Apis.Mexc
{
    // https://mxcdevelop.github.io/apidocs/spot_v3_en/

    internal class Endpoints
    {
        internal static class Public
        {
            public const string GetServerTime = "/api/v3/time";
        }

        internal static class Wallet
        {
            public const string DepositAddress = "/api/v3/capital/deposit/address";
            public const string GenerateDepositAddress = "/api/v3/capital/deposit/address";
            public const string CurrencyInfo = "/api/v3/capital/config/getall";
            public const string Withdraw = "/api/v3/capital/withdraw/apply";
        }

        internal static class Spot
        {
            public const string NewOrder = "/api/v3/order";
            public const string AccountInfo = "/api/v3/account";
        }
    }
}
