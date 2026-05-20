using System;
using System.Collections.Generic;
using System.Linq;
using _.Scripts.Scriptable_Objects;
using _.Scripts.User_Interface.Controls;
using _.Scripts.User_Interface.Events;
using UnityEngine;
using UnityEngine.UIElements;

namespace _.Scripts.User_Interface
{
    public class TradingWindowViewModel : AbstractUserInterfaceViewModel
    {
        private const string CatalogTabClass = "trade-window__tab--active";
        private const string HiddenClass     = "trade-window__tab-panel--hidden";

        [SerializeField] private TradeConfigScriptableObject[] _tradeConfigs = Array.Empty<TradeConfigScriptableObject>();
        [SerializeField] private VisualTreeAsset               _tradeWindowLayout;

        private readonly Dictionary<string, int>                          _cart        = new();
        private readonly Dictionary<string, TradeConfigScriptableObject> _catalogById = new();

        private VisualElement _root;
        private VisualElement _catalogPanel;
        private VisualElement _cartPanel;
        private VisualElement _catalogList;
        private VisualElement _cartList;
        private Label         _catalogEmptyLabel;
        private Label         _cartEmptyLabel;

        private Label _catalogDeliveryTimeValue;
        private Label _catalogBaseCostValue;
        private Label _catalogSurchargeValue;

        private Label _cartDeliveryTimeValue;
        private Label _cartBaseCostValue;
        private Label _cartSurchargeValue;

        private Button _catalogTabButton;
        private Button _cartTabButton;
        private Button _confirmOrderButton;
        private Button _closeButton;

        public Func<TradeOrderSummary> GetOrderSummary { get; set; }

        protected override void SoftInitialize()
        {
            _root = Document.rootVisualElement;

            if (_tradeWindowLayout != null)
            {
                _root.Clear();
                _tradeWindowLayout.CloneTree(_root);
            }

            CacheElements();
            WireButtons();
            BuildCatalog(_tradeConfigs);

            TradeEvents.TradeAdded      += OnTradeAdded;
            TradeEvents.TradeRemoved    += OnTradeRemoved;
            TradeEvents.QuantityChanged += OnQuantityChanged;
            TradeEvents.WindowClosed    += OnWindowClosed;
            TradeEvents.OrderConfirmed  += OnOrderConfirmed;
            TradeEvents.OrderUpdated    += RefreshAll;

            RefreshAll();
            SetVisible(false);
        }

        private void OnDestroy()
        {
            TradeEvents.TradeAdded      -= OnTradeAdded;
            TradeEvents.TradeRemoved    -= OnTradeRemoved;
            TradeEvents.QuantityChanged -= OnQuantityChanged;
            TradeEvents.WindowClosed    -= OnWindowClosed;
            TradeEvents.OrderConfirmed  -= OnOrderConfirmed;
            TradeEvents.OrderUpdated    -= RefreshAll;
        }

        public override void Show()
        {
            base.Show();
            SetVisible(true);
            RefreshAll();
        }

        public override void Hide()
        {
            base.Hide();
            SetVisible(false);
        }

        public void SetCatalog(IEnumerable<TradeConfigScriptableObject> trades) => BuildCatalog(trades);

        public void ClearCart()
        {
            _cart.Clear();
            TradeEvents.RaiseOrderUpdated();
        }

        private void BuildCatalog(IEnumerable<TradeConfigScriptableObject> trades)
        {
            _catalogById.Clear();

            if (trades != null)
            {
                foreach (var trade in trades)
                {
                    if (trade == null)
                        continue;

                    _catalogById[TradeIdUtility.GetId(trade)] = trade;
                }
            }

            RefreshCatalog();
        }

        private void RefreshAll()
        {
            RefreshCatalog();
            RefreshCart();
            RefreshSummaries();
        }

        private void ShowCatalogTab() => SetActiveTab(true);

        private void ShowCartTab() => SetActiveTab(false);

        private void CacheElements()
        {
            _catalogPanel             = _root.Q<VisualElement>("catalog-panel");
            _cartPanel                = _root.Q<VisualElement>("cart-panel");
            _catalogList              = _root.Q<VisualElement>("catalog-list");
            _cartList                 = _root.Q<VisualElement>("cart-list");
            _catalogEmptyLabel        = _root.Q<Label>("catalog-empty");
            _cartEmptyLabel           = _root.Q<Label>("cart-empty");
            _catalogDeliveryTimeValue = _root.Q<Label>("catalog-delivery-time-value");
            _catalogBaseCostValue     = _root.Q<Label>("catalog-base-cost-value");
            _catalogSurchargeValue    = _root.Q<Label>("catalog-surcharge-value");
            _cartDeliveryTimeValue    = _root.Q<Label>("cart-delivery-time-value");
            _cartBaseCostValue        = _root.Q<Label>("cart-base-cost-value");
            _cartSurchargeValue       = _root.Q<Label>("cart-surcharge-value");
            _catalogTabButton         = _root.Q<Button>("catalog-tab");
            _cartTabButton            = _root.Q<Button>("cart-tab");
            _confirmOrderButton       = _root.Q<Button>("confirm-order-button");
            _closeButton              = _root.Q<Button>("close-button");
        }

        private void WireButtons()
        {
            _catalogTabButton.clicked   += () => SetActiveTab(true);
            _cartTabButton.clicked      += () => SetActiveTab(false);
            _confirmOrderButton.clicked += () => TradeEvents.RaiseOrderConfirmed();
            _closeButton.clicked        += () => TradeEvents.RaiseWindowClosed();
        }

        private void SetActiveTab(bool showCatalog)
        {
            _catalogPanel.EnableInClassList(HiddenClass, !showCatalog);
            _cartPanel.EnableInClassList(HiddenClass, showCatalog);

            _catalogTabButton.EnableInClassList(CatalogTabClass, showCatalog);
            _cartTabButton.EnableInClassList(CatalogTabClass, !showCatalog);
        }

        private void RefreshCatalog()
        {
            _catalogList.Clear();

            foreach (var trade in _catalogById.Values.OrderBy(t => t.TradeName))
                _catalogList.Add(new TradeListItemControl(trade));

            var hasItems = _catalogById.Count > 0;
            _catalogEmptyLabel.EnableInClassList(HiddenClass, hasItems);
            _catalogList.style.display = hasItems ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void RefreshCart()
        {
            _cartList.Clear();
            var hasItems = false;

            foreach (var tradeId in _cart.Keys.ToArray())
            {
                if (!_catalogById.TryGetValue(tradeId, out var trade))
                    trade = ResolveTrade(tradeId);

                if (trade == null)
                    continue;

                var quantity = Mathf.Max(1, _cart[tradeId]);
                _cartList.Add(new CartItemControl(trade, tradeId, quantity));
                hasItems = true;
            }

            _cartEmptyLabel.EnableInClassList(HiddenClass, hasItems);
            _cartList.style.display = hasItems ? DisplayStyle.Flex : DisplayStyle.None;
            _confirmOrderButton.SetEnabled(hasItems);
        }

        private void RefreshSummaries()
        {
            var summary = ResolveOrderSummary();
            ApplySummary(_catalogDeliveryTimeValue, _catalogBaseCostValue, _catalogSurchargeValue, summary);
            ApplySummary(_cartDeliveryTimeValue, _cartBaseCostValue, _cartSurchargeValue, summary);
        }

        private TradeOrderSummary ResolveOrderSummary()
        {
            if (GetOrderSummary != null)
                return GetOrderSummary.Invoke();

            if (_cart.Count == 0)
                return default;

            double totalBaseCost          = 0d;
            float  maxDeliveryTime        = 0f;
            double totalDeliverySurcharge = 0d;

            foreach (var (tradeId, quantity) in _cart)
            {
                var trade = ResolveTrade(tradeId);
                if (trade == null)
                    continue;

                totalBaseCost         += trade.BaseCost * quantity;
                maxDeliveryTime        = Mathf.Max(maxDeliveryTime, trade.DeliveryTime);
                totalDeliverySurcharge += 0d;
            }

            return new TradeOrderSummary(totalBaseCost, maxDeliveryTime, totalDeliverySurcharge);
        }

        private TradeConfigScriptableObject ResolveTrade(string tradeId) =>
            _catalogById.TryGetValue(tradeId, out var trade)
                ? trade
                : _tradeConfigs?.FirstOrDefault(config => config != null && TradeIdUtility.GetId(config) == tradeId);

        private void OnTradeAdded(string tradeId)
        {
            _cart[tradeId] = _cart.TryGetValue(tradeId, out var quantity) ? quantity + 1 : 1;
            TradeEvents.RaiseOrderUpdated();
            ShowCartTab();
        }

        private void OnTradeRemoved(string tradeId)
        {
            if (_cart.Remove(tradeId))
                TradeEvents.RaiseOrderUpdated();
        }

        private void OnQuantityChanged(string tradeId, int quantity)
        {
            if (quantity <= 0)
            {
                OnTradeRemoved(tradeId);
                return;
            }

            _cart[tradeId] = quantity;
            TradeEvents.RaiseOrderUpdated();
        }

        private void OnWindowClosed() => Hide();

        private void OnOrderConfirmed()
        {
            Debug.Log($"[Trading] Order confirmed with {_cart.Count} trade line(s).");
            ClearCart();
            Hide();
        }

        private void SetVisible(bool isVisible)
        {
            if (_root == null)
                return;

            _root.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private static void ApplySummary(Label deliveryLabel, Label baseCostLabel, Label surchargeLabel, TradeOrderSummary summary)
        {
            var hasOrder = summary.TotalBaseCost > 0d || summary.TotalDeliveryTime > 0f || summary.TotalDeliverySurcharge > 0d;

            deliveryLabel.text  = hasOrder ? FormatDeliveryTime(summary.TotalDeliveryTime) : "—";
            baseCostLabel.text  = hasOrder ? FormatCost(summary.TotalBaseCost) : "0";
            surchargeLabel.text = hasOrder ? FormatCost(summary.TotalDeliverySurcharge) : "0";
        }

        private static string FormatCost(double value) => value.ToString("N0");

        private static string FormatDeliveryTime(float hours)
        {
            if (hours <= 0f)
                return "Instant";

            if (hours < 1f)
                return $"{Mathf.CeilToInt(hours * 60f)} min";

            if (hours < 24f)
                return hours % 1f < 0.05f ? $"{hours:0} h" : $"{hours:0.#} h";

            var days = hours / 24f;
            return days % 1f < 0.05f ? $"{days:0} d" : $"{days:0.#} d";
        }
    }

    public readonly struct TradeOrderSummary
    {
        public TradeOrderSummary(double totalBaseCost, float totalDeliveryTime, double totalDeliverySurcharge)
        {
            TotalBaseCost          = totalBaseCost;
            TotalDeliveryTime      = totalDeliveryTime;
            TotalDeliverySurcharge = totalDeliverySurcharge;
        }

        public double TotalBaseCost          { get; }
        public float  TotalDeliveryTime      { get; }
        public double TotalDeliverySurcharge { get; }
    }

    public static class TradeIdUtility
    {
        public static string GetId(TradeConfigScriptableObject trade) =>
            string.IsNullOrEmpty(trade.name) ? trade.TradeName : trade.name;
    }
}
