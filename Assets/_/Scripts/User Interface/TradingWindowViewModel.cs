using System;
using System.Collections.Generic;
using System.Linq;
using _.Scripts.Data.Concrete;
using _.Scripts.Scriptable_Objects;
using _.Scripts.User_Interface.Controls;
using UnityEngine;
using UnityEngine.UIElements;

namespace _.Scripts.User_Interface
{
    public class TradingWindowViewModel : AbstractUserInterfaceViewModel
    {
        private const string CatalogTabClass = "trade-window__tab--active";
        private const string HiddenClass     = "trade-window__tab-panel--hidden";

        private readonly Dictionary<string, TradeConfigScriptableObject> _tradesByIdentity = new();

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

        public event Action OrderConfirmed;
        public event Action WindowClosed;
        public event Action OnClearCart;
        public event Action<string> OnTradeAdded;
        public event Action<string> OnTradeRemoved;
        public event Action<string, int> OnQuantityChanged;

        public Func<TradeEntityData.TradeOrder> GetOrder { get; set; }
        public Func<TradeOrderSummary> GetOrderSummary { get; set; }

        protected override void SoftInitialize()
        {
            _root = Document.rootVisualElement;

            CacheElements();
            WireButtons();
            RefreshCatalog();
            RefreshAll();
            SetVisible(false);
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

        public void SetCatalog(IEnumerable<TradeConfigScriptableObject> trades)
        {
            _tradesByIdentity.Clear();

            if (trades != null)
            {
                foreach (var trade in trades)
                {
                    if (trade == null || string.IsNullOrEmpty(trade.Identity))
                        continue;

                    _tradesByIdentity[trade.Identity] = trade;
                }
            }

            RefreshCatalog();
            RefreshAll();
        }

        public void ClearCart()
        {
            if (!HasOrderItems())
                return;

            OnClearCart?.Invoke();
            RefreshAll();
        }

        public void AddTrade(string identity)
        {
            if (string.IsNullOrEmpty(identity) || ResolveTrade(identity) == null)
                return;

            OnTradeAdded?.Invoke(identity);
            RefreshAll();
            ShowCartTab();
        }

        public void RemoveTrade(string identity)
        {
            if (string.IsNullOrEmpty(identity))
                return;

            OnTradeRemoved?.Invoke(identity);
            RefreshAll();
        }

        public void ChangeQuantity(string identity, int quantity)
        {
            if (string.IsNullOrEmpty(identity))
                return;

            if (quantity <= 0)
            {
                RemoveTrade(identity);
                return;
            }

            if (ResolveTrade(identity) == null)
                return;

            OnQuantityChanged?.Invoke(identity, quantity);
            RefreshAll();
        }

        public void RefreshAll()
        {
            RefreshCart();
            RefreshSummaries();
        }

        public void ClearAll()
        {
            _tradesByIdentity.Clear();

            OrderConfirmed     = null;
            WindowClosed       = null;
            OnClearCart        = null;
            OnTradeAdded       = null;
            OnTradeRemoved     = null;
            OnQuantityChanged  = null;

            GetOrder        = null;
            GetOrderSummary = null;

            RefreshAll();
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
            _confirmOrderButton.clicked += OnOrderConfirmed;
            _closeButton.clicked        += OnWindowClosed;
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

            var trades = _tradesByIdentity.Values
                .OrderBy(trade => trade.TradeName)
                .ToArray();

            foreach (var trade in trades)
                _catalogList.Add(new TradeListItemControl(trade, this));

            var hasItems = trades.Length > 0;
            _catalogEmptyLabel.EnableInClassList(HiddenClass, hasItems);
            _catalogList.style.display = hasItems ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void RefreshCart()
        {
            _cartList.Clear();
            var hasItems = false;

            foreach (var (trade, quantity) in GetOrderLines())
            {
                _cartList.Add(new CartItemControl(trade, quantity, this));
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

        private IEnumerable<(TradeConfigScriptableObject Trade, int Quantity)> GetOrderLines()
        {
            var order = GetOrder?.Invoke();
            if (order == null || !order.HasItems)
                yield break;

            foreach (var line in order.Lines)
            {
                if (!_tradesByIdentity.TryGetValue(line.Identity, out var trade))
                    continue;

                yield return (trade, line.Quantity);
            }
        }

        private bool HasOrderItems() => GetOrderLines().Any();

        private TradeOrderSummary ResolveOrderSummary()
        {
            if (GetOrderSummary != null)
                return GetOrderSummary.Invoke();

            if (!HasOrderItems())
                return default;

            double totalBaseCost          = 0d;
            float  maxDeliveryTime        = 0f;
            double totalDeliverySurcharge = 0d;

            foreach (var (trade, quantity) in GetOrderLines())
            {
                totalBaseCost         += trade.BaseCost * quantity;
                maxDeliveryTime        = Mathf.Max(maxDeliveryTime, trade.DeliveryTime);
                totalDeliverySurcharge += 0d;
            }

            return new TradeOrderSummary(totalBaseCost, maxDeliveryTime, totalDeliverySurcharge);
        }

        private TradeConfigScriptableObject ResolveTrade(string identity) =>
            string.IsNullOrEmpty(identity) || !_tradesByIdentity.TryGetValue(identity, out var trade)
                ? null
                : trade;

        private void OnWindowClosed()
        {
            WindowClosed?.Invoke();
            ClearAll();
            Hide();
        }

        private void OnOrderConfirmed()
        {
            if (!HasOrderItems())
                return;

            OrderConfirmed?.Invoke();
            Debug.Log("[Trading] Order confirmed.");
            OnClearCart?.Invoke();
            RefreshAll();
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

        private static string FormatDeliveryTime(float inputSeconds)
        {
            if (inputSeconds <= 0f)
                return "Instant";

            var timeSpan = TimeSpan.FromSeconds(inputSeconds);
            
            var hours = timeSpan.Hours;
            var minutes = timeSpan.Minutes;
            var seconds =  timeSpan.Seconds;
            
            return $"{hours:00}:{minutes:00}:{seconds:00}";
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
}
