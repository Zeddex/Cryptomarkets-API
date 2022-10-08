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
            public const string CurrencyById = "/v2/currency/";   // /v2/currency/{currency}
            public const string AllTickers = "/v2/ticker";
            public const string TickerForPair = "/v2/ticker/";    // /v2/ticker/{base}/{quote}
            public const string CurrnetTime = "/v2/time";
        }

        public static class Account
        {
            public const string Balances = "/v2/auth/account";
            public const string BalancesByCurrency = "/v2/auth/account/currency/"; // /v2/auth/account/currency/{currency}/{type}
        }

        public static class Transaction
        {
            public const string RequestWithdrawal = "/v2/auth/transaction/withdraw";
            public const string ConfirmWithdrawal = "/v2/auth/transaction/withdraw/confirm";
            public const string TransactionById = "/v2/auth/transaction/"; // /v2/auth/transaction/{id}
        }

        public static class Order
        {
            public const string Aaa = "";
            public const string Aaa1 = "";
            public const string Aaa2 = "";
            public const string Aaa3 = "";
            public const string Aaa4 = "";
            public const string Aaa5 = "";
            public const string Aaa6 = "";
            public const string Aaa7 = "";
        }
    }
}
