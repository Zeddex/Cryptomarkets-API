using System;

namespace Cryptomarkets.Apis.Lbank
{
    public class Endpoints
    {
        public static class Public
        {
            public const string MarketData = "/v1/ticker.do";
            public const string TradingPairs = "/v1/currencyPairs.do";
            public const string MarketDepth = "/v1/depth.do";
            public const string HistoricalTransactions = "/v1/trades.do";
            public const string KBarData = "/v1/kline.do";
            public const string WithdrawConfig = "/v1/withdrawConfigs.do";
        }

        public static class Private
        {
            public const string UserInfo = "/v1/user_info.do";
            public const string PlaceOrder = "/v1/create_order.do";
            public const string CancelOrder = "/v1/cancel_order.do";
            public const string QueryOrder = "/v1/orders_info.do";
            public const string OrdersHistory = "/v1/orders_info_history.do";
            public const string TransactionDetails = "/v1/order_transaction_detail.do";
            public const string TransactionHistory = "/v1/transaction_history.do";
            public const string OpenOrdersInfo = "/v1/orders_info_no_deal.do";
            public const string Withdraw = "/v1/withdraw.do";
            public const string CancelWithdraw = "/v1/withdrawCancel.do";
            public const string WithdrawalRecord = "/v1/withdraws.do";
        }
    }
}
