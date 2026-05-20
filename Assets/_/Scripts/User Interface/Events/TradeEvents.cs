using System;

namespace _.Scripts.User_Interface.Events
{
    public static class TradeEvents
    {
        public static event Action<string> TradeAdded;
        public static event Action<string> TradeRemoved;
        public static event Action<string, int> QuantityChanged;
        public static event Action OrderConfirmed;
        public static event Action WindowClosed;
        public static event Action OrderUpdated;

        public static void RaiseTradeAdded(string tradeId) => TradeAdded?.Invoke(tradeId);

        public static void RaiseTradeRemoved(string tradeId) => TradeRemoved?.Invoke(tradeId);

        public static void RaiseQuantityChanged(string tradeId, int quantity) =>
            QuantityChanged?.Invoke(tradeId, quantity);

        public static void RaiseOrderConfirmed() => OrderConfirmed?.Invoke();

        public static void RaiseWindowClosed() => WindowClosed?.Invoke();

        public static void RaiseOrderUpdated() => OrderUpdated?.Invoke();

        public static void ClearAll()
        {
            TradeAdded       = null;
            TradeRemoved     = null;
            QuantityChanged  = null;
            OrderConfirmed   = null;
            WindowClosed     = null;
            OrderUpdated     = null;
        }
    }
}
