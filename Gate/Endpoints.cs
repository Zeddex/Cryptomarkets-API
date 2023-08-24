using System;

namespace Cryptomarkets.Apis.GateIO
{
    internal class Endpoints
    {
        internal static class Public
        {
            public const string TraidingPairs = "/api2/1/pairs";
            public const string MarketInfo = "/api2/1/marketinfo";
            public const string CoinInfo = "/api2/1/coininfo";
            public const string MarketDetails = "/api2/1/marketlist";
            public const string Tickers = "/api2/1/tickers";
            public const string Ticker = "/api2/1/ticker/";
            public const string OrderBooks = "/api2/1/orderBooks";
            public const string OrderBook = "/api2/1/orderBook/";
            public const string TradeHistory = "/api2/1/tradeHistory/";
            public const string P2PDepth = "/api2/1/orderBook_c2c";
            public const string CurrentP2PDepth = "/api2/1/orderBook_c2c/";
        }

        [Obsolete]
        internal static class Private
        {
            public const string AccountBalance = "/api2/1/private/balances";
            public const string MarginBalances = "/api2/1/private/marginbalances";
            public const string FundingBalances = "/api2/1/private/fundingbalances";
            public const string DepositAddress = "/api2/1/private/depositAddress";
            public const string RepaidHistory = "/api2/1/private/depositsWithdrawals";
            public const string Buy = "/api2/1/private/buy";
            public const string Sell = "/api2/1/private/sell";
            public const string BatchOrders = "/api2/1/private/batch_orders";
            public const string CancelOrder = "/api2/1/private/cancelOrder";
            public const string CancelAllOrders = "/api2/1/private/cancelOrders";
            public const string OrderStatus = "/api2/1/private/getOrder";
            public const string OpenOrders = "/api2/1/private/openOrders";
            public const string TradeHistory = "/api2/1/private/tradeHistory";
            public const string Withdraw = "/api2/1/private/withdraw";
            public const string WithdrawCancel = "/api2/1/private/withdraw_cancel";
            public const string Fees = "/api2/1/private/feelist";
        }

        internal static class Wallet
        {
            public const string ChainsList = "/api/v4/wallet/currency_chains";
            public const string DepositAddress = "/api/v4/wallet/deposit_address";
            public const string WithdrawalRecords = "/api/v4/wallet/withdrawals";
            public const string DepositRecords = "/api/v4/wallet/deposits";
            public const string TransferBetweenAccs = "/api/v4/wallet/transfers";
            public const string WithdrawalStatus = "/api/v4/wallet/withdraw_status";
            public const string SavedAddress = "/api/v4/wallet/saved_address";
            public const string TradingFee = "/api/v4/wallet/fee";
            public const string TotalBalance = "/api/v4/wallet/total_balance";
            public const string SubAccBalances = "/api/v4/wallet/sub_account_balances";
        }

        internal static class Withdrawal
        {
            public const string Withdraw = "/api/v4/withdrawals";
            public const string CancelWithdraw = "/api/v4/withdrawals/";
        }

        internal static class Spot
        {
            public const string GetServerTime = "/api/v4/spot/time";
            public const string AllCurrencies = "/api/v4/spot/currencies";
            public const string CurrencyDetails = "/api/v4/spot/currencies/";
            public const string CurrencyPairs = "/api/v4/spot/currency_pairs";
            public const string CurrencyPair = "/api/v4/spot/currency_pairs/";
            public const string Tickers = "/api/v4/spot/tickers";
            public const string OrderBook = "/api/v4/spot/order_book";
            public const string MarketTrades = "/api/v4/spot/trades";
            public const string Candlesticks = "/api/v4/spot/candlesticks";
            public const string FeeRates = "/api/v4/spot/fee";
            public const string SpotAccounts = "/api/v4/spot/accounts";
            public const string OpenOrders = "/api/v4/spot/open_orders";
            public const string CreateOrder = "/api/v4/spot/orders";
            public const string OrdersList = "/api/v4/spot/orders";
            public const string CancelAllOrders = "/api/v4/spot/orders";
            public const string GetSingleOrder = "/api/v4/spot/orders/";
            public const string CancelOrder = "/api/v4/spot/orders/";
            public const string TradingHistory = "/api/v4/spot/my_trades";
            public const string CountdownCancelOrders = "/api/v4/spot/countdown_cancel_all";
        }

        internal static class Margin
        {
            public const string CurrencyPairs = "/api/v4/margin/currency_pairs";
            public const string QueryPair = "/api/v4/margin/currency_pairs/";
            public const string FundingBook = "/api/v4/margin/funding_book";
            public const string AccountsList = "/api/v4/margin/accounts";
            public const string AccountBook = "/api/v4/margin/account_book";
            public const string FundingAccounts = "/api/v4/margin/funding_accounts";
            public const string LendBorrow = "/api/v4/margin/loans";
            public const string LoansList = "/api/v4/margin/loans";
            public const string MergeLoans = "/api/v4/margin/merged_loans";
            public const string LoanDetails = "/api/v4/margin/loans/";
            public const string CancelLendingLoan = "/api/v4/margin/loans/";
            public const string AllRepaymentRecords = "/api/v4/margin/loans/";
            public const string LoanRepaymentRecords = "/api/v4/margin/loan_records";
            public const string GetLoanRecord = "/api/v4/margin/loan_records/";
            public const string GetMaxTransferAmount = "/api/v4/margin/transferable";
            public const string GetMaxBorrowAmount = "/api/v4/margin/borrowable";
            public const string CrossMarginSupported = "/api/v4/margin/cross/currencies";
            public const string CrossMarginSupportedDetails = "/api/v4/margin/cross/currencies/";
            public const string CrossMarginAccount = "/api/v4/margin/cross/accounts";
            public const string CrossMarginAccHistory = "/api/v4/margin/cross/account_book";
            public const string CreateCMBorrowLoan = "/api/v4/margin/cross/loans";
            public const string CrossMarginBorrowHistory = "/api/v4/margin/cross/loans";
            public const string BorrowLoanDetail = "/api/v4/margin/cross/loans/";
            public const string Repayments = "/api/v4/margin/cross/repayments";
            public const string CrossMarginRepayments = "/api/v4/margin/cross/repayments";
            public const string MaxTransferAmount = "/api/v4/margin/cross/transferable";
            public const string MaxBorrowableAmount = "/api/v4/margin/cross/borrowable";
        }
    }
}
