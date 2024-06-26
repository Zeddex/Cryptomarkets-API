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

            //GET_MARK_PRICE_KLINE = "/v5/market/mark-price-kline"
            //GET_INDEX_PRICE_KLINE = "/v5/market/index-price-kline"
            //GET_PREMIUM_INDEX_PRICE_KLINE = "/v5/market/premium-index-price-kline"
            //GET_INSTRUMENTS_INFO = "/v5/market/instruments-info"
            //GET_FUNDING_RATE_HISTORY = "/v5/market/funding/history"
            //GET_PUBLIC_TRADING_HISTORY = "/v5/market/recent-trade"
            //GET_OPEN_INTEREST = "/v5/market/open-interest"
            //GET_HISTORICAL_VOLATILITY = "/v5/market/historical-volatility"
            //GET_INSURANCE = "/v5/market/insurance"
            //GET_RISK_LIMIT = "/v5/market/risk-limit"
            //GET_OPTION_DELIVERY_PRICE = "/v5/market/delivery-price"
            //GET_LONG_SHORT_RATIO = "/v5/market/account-ratio"
        }

        internal static class Wallet
        {
            public const string Withdraw = "/v5/asset/withdraw/create";
        }

        internal static class Account
        {
            public const string Info = "/v5/account/info";
            public const string Balance = "/v5/account/wallet-balance";

            //UPGRADE_TO_UNIFIED_ACCOUNT = "/v5/account/upgrade-to-uta"
            //GET_BORROW_HISTORY = "/v5/account/borrow-history"
            //GET_COLLATERAL_INFO = "/v5/account/collateral-info"
            //GET_COIN_GREEKS = "/v5/asset/coin-greeks"
            //GET_FEE_RATE = "/v5/account/fee-rate"
            //GET_TRANSACTION_LOG = "/v5/account/transaction-log"
            //SET_MARGIN_MODE = "/v5/account/set-margin-mode"
            //SET_MMP = "/v5/account/mmp-modify"
            //RESET_MMP = "/v5/account/mmp-reset"
            //GET_MMP_STATE = "/v5/account/mmp-state"
        }

        internal static class Spot
        {
            public const string Borrow = "/v5/spot-cross-margin-trade/loan";

            // spot margin trade

            // UTA endpoints
            //TOGGLE_MARGIN_TRADE = "/v5/spot-margin-trade/switch-mode"
            //SET_LEVERAGE = "/v5/spot-margin-trade/set-leverage"
            //VIP_MARGIN_DATA = "/v5/spot-margin-trade/data"
            //STATUS_AND_LEVERAGE = "/v5/spot-margin-trade/state"
            // normal mode (non-UTA) endpoints
            //NORMAL_GET_VIP_MARGIN_DATA = "/v5/spot-cross-margin-trade/data"
            //NORMAL_GET_MARGIN_COIN_INFO = "/v5/spot-cross-margin-trade/pledge-token"
            //NORMAL_GET_BORROWABLE_COIN_INFO = "/v5/spot-cross-margin-trade/borrow-token"
            //NORMAL_GET_INTEREST_QUOTA = "/v5/spot-cross-margin-trade/loan-info"
            //NORMAL_GET_LOAN_ACCOUNT_INFO = "/v5/spot-cross-margin-trade/account"
            //NORMAL_BORROW = "/v5/spot-cross-margin-trade/loan"
            //NORMAL_REPAY = "/v5/spot-cross-margin-trade/repay"
            //NORMAL_GET_BORROW_ORDER_DETAIL = "/v5/spot-cross-margin-trade/orders"
            //NORMAL_GET_REPAYMENT_ORDER_DETAIL = "/v5/spot-cross-margin-trade/repay-history"
            //NORMAL_TOGGLE_MARGIN_TRADE = "/v5/spot-cross-margin-trade/switch"

            // spot_leverage_token

            //GET_LEVERAGED_TOKEN_INFO = "/v5/spot-lever-token/info"
            //GET_LEVERAGED_TOKEN_MARKET = "/v5/spot-lever-token/reference"
            //PURCHASE = "/v5/spot-lever-token/purchase"
            //REDEEM = "/v5/spot-lever-token/redeem"
            //GET_PURCHASE_REDEMPTION_RECORDS = "/v5/spot-lever-token/order-record"
        }

        internal static class Trade
        {
            public const string PlaceOrder = "/v5/order/create";
            public const string CancelOrder = "/v5/order/cancel";
            public const string GetOpenOrders = "/v5/order/realtime";

            //AMEND_ORDER = "/v5/order/amend"
            //CANCEL_ALL_ORDERS = "/v5/order/cancel-all"
            //GET_ORDER_HISTORY = "/v5/order/history"
            //BATCH_PLACE_ORDER = "/v5/order/create-batch"
            //BATCH_AMEND_ORDER = "/v5/order/amend-batch"
            //BATCH_CANCEL_ORDER = "/v5/order/cancel-batch"
            //GET_BORROW_QUOTA = "/v5/order/spot-borrow-check"
            //SET_DCP = "/v5/order/disconnected-cancel-all"
        }

        internal static class User
        {
            //CREATE_SUB_UID = "/v5/user/create-sub-member"
            //CREATE_SUB_API_KEY = "/v5/user/create-sub-api"
            //GET_SUB_UID_LIST = "/v5/user/query-sub-members"
            //FREEZE_SUB_UID = "/v5/user/frozen-sub-member"
            //GET_API_KEY_INFORMATION = "/v5/user/query-api"
            //MODIFY_MASTER_API_KEY = "/v5/user/update-api"
            //MODIFY_SUB_API_KEY = "/v5/user/update-sub-api"
            //DELETE_MASTER_API_KEY = "/v5/user/delete-api"
            //DELETE_SUB_API_KEY = "/v5/user/delete-sub-api"
            //GET_AFFILIATE_USER_INFO = "/v5/user/aff-customer-info"
            //GET_UID_WALLET_TYPE = "/v5/user/get-member-type"
        }

        internal static class PreUpgrade
        {
            //GET_PRE_UPGRADE_ORDER_HISTORY = "/v5/pre-upgrade/order/history"
            //GET_PRE_UPGRADE_TRADE_HISTORY = "/v5/pre-upgrade/execution/list"
            //GET_PRE_UPGRADE_CLOSED_PNL = "/v5/pre-upgrade/position/closed-pnl"
            //GET_PRE_UPGRADE_TRANSACTION_LOG = "/v5/pre-upgrade/account/transaction-log"
            //GET_PRE_UPGRADE_OPTION_DELIVERY_RECORD = "/v5/pre-upgrade/asset/delivery-record"
            //GET_PRE_UPGRADE_USDC_SESSION_SETTLEMENT = "/v5/pre-upgrade/asset/settlement-record"
        }

        internal static class Position
        {
            //GET_POSITIONS = "/v5/position/list"
            //SET_LEVERAGE = "/v5/position/set-leverage"
            //SWITCH_MARGIN_MODE = "/v5/position/switch-isolated"
            //SET_TP_SL_MODE = "/v5/position/set-tpsl-mode"
            //SWITCH_POSITION_MODE = "/v5/position/switch-mode"
            //SET_RISK_LIMIT = "/v5/position/set-risk-limit"
            //SET_TRADING_STOP = "/v5/position/trading-stop"
            //SET_AUTO_ADD_MARGIN = "/v5/position/set-auto-add-margin"
            //GET_EXECUTIONS = "/v5/execution/list"
            //GET_CLOSED_PNL = "/v5/position/closed-pnl"
        }

        internal static class Asset
        {
            //GET_COIN_EXCHANGE_RECORDS = "/v5/asset/exchange/order-record"
            //GET_OPTION_DELIVERY_RECORD = "/v5/asset/delivery-record"
            //GET_USDC_CONTRACT_SETTLEMENT = "/v5/asset/settlement-record"
            //GET_SPOT_ASSET_INFO = "/v5/asset/transfer/query-asset-info"
            //GET_ALL_COINS_BALANCE = "/v5/asset/transfer/query-account-coins-balance"
            //GET_SINGLE_COIN_BALANCE = "/v5/asset/transfer/query-account-coin-balance"
            //GET_TRANSFERABLE_COIN = "/v5/asset/transfer/query-transfer-coin-list"
            //CREATE_INTERNAL_TRANSFER = "/v5/asset/transfer/inter-transfer"
            //GET_INTERNAL_TRANSFER_RECORDS = (
            //    "/v5/asset/transfer/query-inter-transfer-list"
            //    )
            //    GET_SUB_UID = "/v5/asset/transfer/query-sub-member-list"
            //ENABLE_UT_FOR_SUB_UID = "/v5/asset/transfer/save-transfer-sub-member"
            //CREATE_UNIVERSAL_TRANSFER = "/v5/asset/transfer/universal-transfer"
            //GET_UNIVERSAL_TRANSFER_RECORDS = (
            //    "/v5/asset/transfer/query-universal-transfer-list"
            //    )
            //    GET_ALLOWED_DEPOSIT_COIN_INFO = "/v5/asset/deposit/query-allowed-list"
            //SET_DEPOSIT_ACCOUNT = "/v5/asset/deposit/deposit-to-account"
            //GET_DEPOSIT_RECORDS = "/v5/asset/deposit/query-record"
            //GET_SUB_ACCOUNT_DEPOSIT_RECORDS = (
            //    "/v5/asset/deposit/query-sub-member-record"
            //    )
            //    GET_INTERNAL_DEPOSIT_RECORDS = "/v5/asset/deposit/query-internal-record"
            //GET_MASTER_DEPOSIT_ADDRESS = "/v5/asset/deposit/query-address"
            //GET_SUB_DEPOSIT_ADDRESS = "/v5/asset/deposit/query-sub-member-address"
            //GET_COIN_INFO = "/v5/asset/coin/query-info"
            //GET_WITHDRAWAL_RECORDS = "/v5/asset/withdraw/query-record"
            //GET_WITHDRAWABLE_AMOUNT = "/v5/asset/withdraw/withdrawable-amount"
            //WITHDRAW = "/v5/asset/withdraw/create"
            //CANCEL_WITHDRAWAL = "/v5/asset/withdraw/cancel"
        }
    }
}