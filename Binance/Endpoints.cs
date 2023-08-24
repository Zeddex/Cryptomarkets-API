namespace Cryptomarkets.Apis.Binance
{
    internal static class Endpoints
    {
        internal static class Public
        {
            public const string TestConnectivity = "/api/v3/ping";
            public const string GetServerTime = "/api/v3/time";
            public const string GetExchangeInfo = "/api/v3/exchangeInfo";
            public const string GetAllPrices = "/api/v3/ticker/price";
            public const string GetSymbolPrice = "/api/v3/ticker/price";
            public const string GetOrderBookTicker = "/api/v3/ticker/bookTicker";
            public const string GetOrderBook = "/api/v3/depth";
            public const string GetAggregateTrades = "/api/v3/aggTrades";
            public const string GetPriceChange24H = "/api/v3/ticker/24hr";
            public const string GetRecentTradesList = "/api/v3/trades";
            public const string GetCurrentAvgPrice = "/api/v3/avgPrice";
            public const string GetKlineCandlestickData = "/api/v3/klines";
        }

        internal static class Private
        {
            public const string GetAccountInfo = "/api/v3/account";
            public const string PostNewOrderTest = "/api/v3/order/test";
            public const string PostNewOrder = "/api/v3/order";
            public const string GetTradeList = "/api/v3/myTrades";
            public const string GetAllOrders = "/api/v3/allOrders";
            public const string GetCurrentOpenOrders = "/api/v3/openOrders";
            public const string GetOrder = "/api/v3/order";
            public const string CancelOrder = "/api/v3/order";
        }

        internal static class Withdrawal        // old
        {
            public const string Withdraw = "/wapi/v3/withdraw.html";
            public const string DepositHistory = "/wapi/v3/depositHistory.html";
            public const string WithdrawHistory = "/wapi/v3/withdrawHistory.html";
            public const string DepositAddress = "/wapi/v3/depositAddress.html";
            public const string AccountStatus = "/wapi/v3/accountStatus.html";
            public const string SystemStatus = "/wapi/v3/systemStatus.html";
            public const string DustLog = "/wapi/v3/userAssetDribbletLog.html";
            public const string TradeFee = "/wapi/v3/tradeFee.html";
            public const string AssetDetail = "/wapi/v3/assetDetail.html";
            public const string QuerySubAccountList = "/wapi/v3/sub-account/list.html";
            public const string QuerySubAccountTransferHistory = "/wapi/v3/sub-account/transfer/history.html";
            public const string SubAccountTransfer = "/wapi/v3/sub-account/transfer.html";
        }

        internal static class SpotTrade
        {
            public const string TestNewOrder = "/api/v3/order/test";
            public const string NewOrder = "/api/v3/order";
            public const string CancelOrder = "/api/v3/order";
            public const string QueryOrder = "/api/v3/order";
            public const string CurrentOpenOrders = "/api/v3/openOrders";
            public const string AllOrders = "/api/v3/allOrders";
            public const string AccountInfo = "/api/v3/account";
            public const string AccountTradeList = "/api/v3/myTrades";
            public const string CancelAllOrders = "/api/v3/openOrders";
            public const string NewOCO = "/api/v3/order/oco";
            public const string CancelOCO = "/api/v3/orderList";
            public const string QueryOCO = "/api/v3/orderList";
            public const string QueryAllOCO = "/api/v3/allOrderList";
            public const string QueryOpenOCO = "/api/v3/openOrderList";
            public const string QueryCurrentOrderUsage = "/api/v3/rateLimit/order";
        }

        internal static class MarginTrade
        {
            public const string Transfer = "/sapi/v1/margin/transfer";
            public const string Borrow = "/sapi/v1/margin/loan";
            public const string Repay = "/sapi/v1/margin/repay";
            public const string QueryAsset = "/sapi/v1/margin/asset";
            public const string QueryPair = "/sapi/v1/margin/pair";
            public const string GetAllAssets = "/sapi/v1/margin/allAssets";
            public const string GetAllPairs = "/sapi/v1/margin/allPairs";
            public const string QueryPriceIndex = "/sapi/v1/margin/priceIndex";
            public const string NewOrder = "/sapi/v1/margin/order";
            public const string CancelOrder = "/sapi/v1/margin/order";
            public const string CancelOpenOrders = "/sapi/v1/margin/openOrders";
            public const string TransferHistory = "/sapi/v1/margin/transfer";
            public const string QueryLoanRecord = "/sapi/v1/margin/loan";
            public const string QueryRepayRecord = "/sapi/v1/margin/repay";
            public const string GetInterestHistory = "/sapi/v1/margin/interestHistory";
            public const string GetForceLiquidationRecord = "/sapi/v1/margin/forceLiquidationRec";
            public const string QueryAccountDetails = "/sapi/v1/margin/account";
            public const string QueryAccountOrder = "/sapi/v1/margin/order";
            public const string QueryAccountOpenOrders = "/sapi/v1/margin/openOrders";
            public const string QueryAccountAllOrders = "/sapi/v1/margin/allOrders";
            public const string NewOCO = "/sapi/v1/margin/order/oco";
            public const string CancelOCO = "/sapi/v1/margin/orderList";
            public const string QueryOCO = "/sapi/v1/margin/orderList";
            public const string QueryAllOCO = "/sapi/v1/margin/allOrderList";
            public const string QueryOpenOCO = "/sapi/v1/margin/openOrderList";
            public const string QueryTradeList = "/sapi/v1/margin/myTrades";
            public const string QueryMaxBorrow = "/sapi/v1/margin/maxBorrowable";
            public const string QueryMaxTransferOutAmount = "/sapi/v1/margin/maxTransferable";
            public const string IsolatedTransfer = "/sapi/v1/margin/isolated/transfer";
            public const string GetIsolatedTransferHistory = "/sapi/v1/margin/isolated/transfer";
            public const string QueryIsolatedInfo = "/sapi/v1/margin/isolated/account";
            public const string DisableIsolatedAccount = "/sapi/v1/margin/isolated/account";
            public const string EnableIsolatedAccount = "/sapi/v1/margin/isolated/account";
            public const string QueryEnabledIsolatedAccountLimit = "/sapi/v1/margin/isolated/accountLimit";
            public const string QueryIsolatedSymbol = "/sapi/v1/margin/isolated/pair";
            public const string GetAllIsolatedSymbol = "/sapi/v1/margin/isolated/allPairs";
            public const string ToggleBNBBurnOnSpotTrade= "/sapi/v1/bnbBurn";
            public const string GetBNBBurnStatus = "/sapi/v1/bnbBurn";
            public const string QueryInterestRateHistory = "/sapi/v1/margin/interestRateHistory";
            public const string QueryCrossMarginFeeData = "/sapi/v1/margin/crossMarginData";
            public const string QueryIsolatedMarginFeeData = "/sapi/v1/margin/isolatedMarginData";
            public const string QueryIsolatedMarginTierData = "/sapi/v1/margin/isolatedMarginTier";
            public const string QueryCurrentOrderCountUsage = "/sapi/v1/margin/rateLimit/order";
            public const string Dustlog = "/sapi/v1/margin/dribblet";
        }

        internal static class Wallet
        {
            public const string SystemStatus = "/sapi/v1/system/status";
            public const string AllCoinsInfo = "/sapi/v1/capital/config/getall";
            public const string DailyAccSnapshot = "/sapi/v1/accountSnapshot";
            public const string DisableFastWithdraw = "/sapi/v1/account/disableFastWithdrawSwitch";
            public const string EnableFastWithdraw = "/sapi/v1/account/enableFastWithdrawSwitch";
            public const string Withdraw = "/sapi/v1/capital/withdraw/apply";
            public const string DepositHistory = "/sapi/v1/capital/deposit/hisrec";
            public const string WithdrawHistory = "/sapi/v1/capital/withdraw/history";
            public const string DepositAddress = "/sapi/v1/capital/deposit/address";
            public const string AccountStatus = "/sapi/v1/account/status";
            public const string AccountTradingStatus = "/sapi/v1/account/apiTradingStatus";
            public const string DustLog = "/sapi/v1/asset/dribblet";
            public const string DustTransfer = "/sapi/v1/asset/dust";
            public const string AssetDividendRecord = "/sapi/v1/asset/assetDividend";
            public const string AssetDetail = "/sapi/v1/asset/assetDetail";
            public const string TradeFee = "/sapi/v1/asset/tradeFee";
            public const string UserUniversalTransfer = "/sapi/v1/asset/transfer";
            public const string QueryUserTransferHistory = "/sapi/v1/asset/transfer";
            public const string FundingWallet = "/sapi/v1/asset/get-funding-asset";
            public const string ApiKeyPermission = "/sapi/v1/account/apiRestrictions";
        }

        internal static class Futures
        {
            public const string Transfer = "/sapi/v1/futures/transfer";
            public const string TransactionHistory = "/sapi/v1/futures/transfer";
            public const string Borrow = "/sapi/v1/futures/loan/borrow";
            public const string BorrowHistory = "/sapi/v1/futures/loan/borrow/history";
            public const string Repay = "/sapi/v1/futures/loan/repay";
            public const string RepayHistory = "/sapi/v1/futures/loan/repay/history";
            public const string Wallet = "/sapi/v1/futures/loan/wallet";
            public const string WalletV2 = "/sapi/v2/futures/loan/wallet";
            public const string Info = "/sapi/v1/futures/loan/configs";
            public const string InfoV2 = "/sapi/v2/futures/loan/configs";
            public const string CalcAdjustLevel = "/sapi/v1/futures/loan/calcAdjustLevel";
            public const string CalcAdjustLevelV2 = "/sapi/v2/futures/loan/calcAdjustLevel";
            public const string CalcMaxAdjustAmount = "/sapi/v1/futures/loan/calcMaxAdjustAmount";
            public const string CalcMaxAdjustAmountV2 = "/sapi/v2/futures/loan/calcMaxAdjustAmount";
            public const string AdjustCollateral = "/sapi/v1/futures/loan/adjustCollateral";
            public const string AdjustCollateralV2 = "/sapi/v2/futures/loan/adjustCollateral";
            public const string AdjustCollateralHistory = "/sapi/v1/futures/loan/adjustCollateral/history";
            public const string LiquidationHistory = "/sapi/v1/futures/loan/liquidationHistory";
            public const string CollateralRepayLimit = "/sapi/v1/futures/loan/collateralRepayLimit";
            public const string CollateralRepayQuote = "/sapi/v1/futures/loan/collateralRepay";
            public const string RepayCollateral = "/sapi/v1/futures/loan/collateralRepay";
            public const string CollateralRepayResult = "/sapi/v1/futures/loan/collateralRepayResult";
            public const string InterestHistory = "/sapi/v1/futures/loan/interestHistory";
        }

        internal static class P2P
        {
            //public const string MakeOrder = "/bapi/c2c/v2/private/c2c/order-match/makeOrder";
            public const string Aaa = "";
        }
    }
}
