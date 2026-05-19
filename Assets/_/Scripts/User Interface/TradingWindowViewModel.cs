using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using _.Scripts.Scriptable_Objects;
using UnityEngine;
using UnityEngine.UIElements;

namespace _.Scripts.User_Interface
{
    public class TradingWindowViewModel : AbstractUserInterfaceViewModel
    {
        [Serializable]
        public struct ResourceStack
        {
            [SerializeField] private ResourceConfigScriptableObject _resource;
            [SerializeField] private double                     _amount;

            public ResourceConfigScriptableObject Resource => _resource;
            public double                         Amount   => _amount;

            public ResourceStack(ResourceConfigScriptableObject resource, double amount)
            {
                _resource = resource;
                _amount   = amount;
            }
        }

        [Serializable]
        public class TradingOfferDefinition
        {
            [SerializeField] private string _offerId;
            [SerializeField] private string _displayName;

            [SerializeField] private ResourceConfigScriptableObject _outputResource;
            [SerializeField] private double                         _outputAmount;

            [SerializeField] private List<ResourceStack> _requiredInputs = new();

            [SerializeField] private long   _bitsOfferPrice;
            [SerializeField] private float  _deliveryTimeHours;
            [SerializeField] private long   _bitsDeliveryPrice;

            public string OfferId => string.IsNullOrEmpty(_offerId) ? _displayName : _offerId;
            public string DisplayName => string.IsNullOrEmpty(_displayName) ? OfferId : _displayName;

            public ResourceStack Output => new(_outputResource, _outputAmount);

            public IReadOnlyList<ResourceStack> RequiredInputs => _requiredInputs;

            public long  BitsOfferPrice    => _bitsOfferPrice;
            public float DeliveryTimeHours => _deliveryTimeHours;
            public long  BitsDeliveryPrice => _bitsDeliveryPrice;

            public static TradingOfferDefinition FromTradeConfig(TradeConfigScriptableObject config)
            {
                var inputs = new List<ResourceStack>();

                if (config.InputResource != null)
                    inputs.Add(new ResourceStack(config.InputResource, config.InputAmount));

                return new TradingOfferDefinition
                {
                    _displayName       = config.TradeName,
                    _outputResource    = config.OutputResource,
                    _outputAmount      = config.OutputAmount,
                    _requiredInputs    = inputs
                };
            }
        }

        public sealed class TradingOffer
        {
            public TradingOffer(TradingOfferDefinition definition)
            {
                Definition = definition ?? throw new ArgumentNullException(nameof(definition));
                OfferId    = Definition.OfferId;
            }

            public TradingOfferDefinition Definition { get; }
            public string                 OfferId    { get; }
        }

        public sealed class BasketEntry
        {
            public BasketEntry(TradingOffer offer, int quantity = 1)
            {
                Offer    = offer    ?? throw new ArgumentNullException(nameof(offer));
                Quantity = Mathf.Max(1, quantity);
            }

            public TradingOffer Offer    { get; }
            public int          Quantity { get; set; }
        }

        [SerializeField] private TradeConfigScriptableObject[] _tradeConfigs = Array.Empty<TradeConfigScriptableObject>();
        [SerializeField] private TradingOfferDefinition[]        _customOffers = Array.Empty<TradingOfferDefinition>();

        private readonly List<TradingOffer>  _catalog = new();
        private readonly List<BasketEntry>   _basket  = new();
        private readonly HashSet<string>     _basketOfferIds = new();

        private VisualElement _container;
        private VisualElement _offersPanel;
        private VisualElement _basketPanel;
        private VisualElement _offersList;
        private VisualElement _basketList;
        private Label         _offersEmptyLabel;
        private Label         _basketEmptyLabel;
        private Label         _deliveryTimeValue;
        private Label         _bitsOfferValue;
        private Label         _bitsDeliveryValue;
        private Button        _offersTabButton;
        private Button        _basketTabButton;
        private Button        _submitOfferButton;
        private Button        _closeButton;

        private VisualElement _offerRowTemplate;
        private VisualElement _basketRowTemplate;
        private VisualElement _resourceStackTemplate;

        private enum ActiveTab
        {
            Offers,
            Basket
        }

        private ActiveTab _activeTab = ActiveTab.Offers;

        public event Action<IReadOnlyList<BasketEntry>> OfferSubmitted;
        public event Action<BasketEntry>                BasketChanged;

        public IReadOnlyList<TradingOffer> Catalog => _catalog;
        public IReadOnlyList<BasketEntry>  Basket  => _basket;

        protected override void SoftInitialize()
        {
            var root = Document.rootVisualElement;

            _container            = root.Q<VisualElement>("Container");
            _offersPanel          = root.Q<VisualElement>("offers-panel");
            _basketPanel          = root.Q<VisualElement>("basket-panel");
            _offersList           = root.Q<VisualElement>("offers-list");
            _basketList           = root.Q<VisualElement>("basket-list");
            _offersEmptyLabel     = root.Q<Label>("offers-empty");
            _basketEmptyLabel     = root.Q<Label>("basket-empty");
            _deliveryTimeValue    = root.Q<Label>("delivery-time-value");
            _bitsOfferValue       = root.Q<Label>("bits-offer-value");
            _bitsDeliveryValue    = root.Q<Label>("bits-delivery-value");
            _offersTabButton      = root.Q<Button>("offers-tab");
            _basketTabButton      = root.Q<Button>("basket-tab");
            _submitOfferButton    = root.Q<Button>("submit-offer-button");
            _closeButton          = root.Q<Button>("close-button");

            var templates = root.Q<VisualElement>("templates");
            _offerRowTemplate       = templates.Q<VisualElement>("offer-row-template");
            _basketRowTemplate      = templates.Q<VisualElement>("basket-row-template");
            _resourceStackTemplate  = templates.Q<VisualElement>("resource-stack-template");

            _offersTabButton.clicked   += () => SetActiveTab(ActiveTab.Offers);
            _basketTabButton.clicked   += () => SetActiveTab(ActiveTab.Basket);
            _submitOfferButton.clicked += SubmitOffer;
            _closeButton.clicked       += Hide;

            BuildCatalogFromSerializedData();
            RefreshAll();
            SetVisible(false);
        }

        public void SetVisible(bool isVisible)
        {
            if (_container == null)
                return;

            _container.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
        }


        public override void Show()
        {
            base.Show();
            
            SetVisible(true);
        }

        public override void Hide()
        {
            base.Hide();
            
            SetVisible(false);
        }

        public void SetCatalog(IEnumerable<TradingOfferDefinition> offers)
        {
            _catalog.Clear();

            if (offers == null)
            {
                RefreshAll();
                return;
            }

            foreach (var definition in offers)
            {
                if (definition == null || definition.Output.Resource == null)
                    continue;

                _catalog.Add(new TradingOffer(definition));
            }

            RefreshAll();
        }

        public void SetCatalogFromTradeConfigs(IEnumerable<TradeConfigScriptableObject> tradeConfigs)
        {
            if (tradeConfigs == null)
            {
                SetCatalog(null);
                return;
            }

            var definitions = tradeConfigs
                .Where(config => config != null)
                .Select(TradingOfferDefinition.FromTradeConfig)
                .ToArray();

            SetCatalog(definitions);
        }

        public void ClearBasket()
        {
            _basket.Clear();
            _basketOfferIds.Clear();
            RefreshAll();
        }

        public bool TryAddToBasket(string offerId, int quantity = 1)
        {
            var offer = _catalog.FirstOrDefault(entry => entry.OfferId == offerId);
            if (offer == null)
                return false;

            AddToBasket(offer, quantity);
            return true;
        }

        public void AddToBasket(TradingOffer offer, int quantity = 1)
        {
            if (offer == null)
                return;

            quantity = Mathf.Max(1, quantity);

            var existing = _basket.FirstOrDefault(entry => entry.Offer.OfferId == offer.OfferId);
            if (existing != null)
            {
                existing.Quantity += quantity;
            }
            else
            {
                _basket.Add(new BasketEntry(offer, quantity));
                _basketOfferIds.Add(offer.OfferId);
            }

            BasketChanged?.Invoke(existing ?? _basket[^1]);
            RefreshAll();
        }

        public void RemoveFromBasket(string offerId)
        {
            var index = _basket.FindIndex(entry => entry.Offer.OfferId == offerId);
            if (index < 0)
                return;

            _basket.RemoveAt(index);
            _basketOfferIds.Remove(offerId);
            RefreshAll();
        }

        public void SetBasketQuantity(string offerId, int quantity)
        {
            var entry = _basket.FirstOrDefault(line => line.Offer.OfferId == offerId);
            if (entry == null)
                return;

            if (quantity <= 0)
            {
                RemoveFromBasket(offerId);
                return;
            }

            entry.Quantity = quantity;
            BasketChanged?.Invoke(entry);
            RefreshAll();
        }

        private void BuildCatalogFromSerializedData()
        {
            var definitions = new List<TradingOfferDefinition>();

            if (_customOffers is { Length: > 0 })
                definitions.AddRange(_customOffers.Where(definition => definition != null));

            if (_tradeConfigs is { Length: > 0 })
                definitions.AddRange(_tradeConfigs.Where(config => config != null).Select(TradingOfferDefinition.FromTradeConfig));

            SetCatalog(definitions);
        }

        private void SetActiveTab(ActiveTab tab)
        {
            _activeTab = tab;

            var isOffersTab = tab == ActiveTab.Offers;

            _offersPanel.EnableInClassList("trading-window__tab-panel--hidden", !isOffersTab);
            _basketPanel.EnableInClassList("trading-window__tab-panel--hidden", isOffersTab);

            _offersTabButton.EnableInClassList("trading-window__tab--active", isOffersTab);
            _basketTabButton.EnableInClassList("trading-window__tab--active", !isOffersTab);
        }

        private void SubmitOffer()
        {
            if (_basket.Count == 0)
                return;

            OfferSubmitted?.Invoke(_basket.ToArray());
        }

        private void RefreshAll()
        {
            RefreshOffersList();
            RefreshBasketList();
            RefreshSummary();
            RefreshEmptyStates();
        }

        private void RefreshOffersList()
        {
            _offersList.Clear();

            foreach (var offer in _catalog)
                _offersList.Add(BuildOfferRow(offer));
        }

        private VisualElement BuildOfferRow(TradingOffer offer)
        {
            var row = CloneTemplate(_offerRowTemplate);

            ApplyResourceStack(row.Q<VisualElement>("output-icon"), row.Q<Label>("output-amount"), offer.Definition.Output);

            var inputsRow = row.Q<VisualElement>("inputs-row");
            inputsRow.Clear();

            foreach (var input in offer.Definition.RequiredInputs)
            {
                if (input.Resource == null)
                    continue;

                inputsRow.Add(BuildResourceStack(input));
            }

            var addButton = row.Q<Button>("add-to-basket-button");
            var isInBasket = _basketOfferIds.Contains(offer.OfferId);

            addButton.text = isInBasket ? "In basket" : "Add to basket";
            addButton.EnableInClassList("trading-window__add-button--in-basket", isInBasket);
            addButton.clicked += () =>
            {
                AddToBasket(offer);
                SetActiveTab(ActiveTab.Basket);
            };

            return row;
        }

        private void RefreshBasketList()
        {
            _basketList.Clear();

            foreach (var entry in _basket)
                _basketList.Add(BuildBasketRow(entry));
        }

        private VisualElement BuildBasketRow(BasketEntry entry)
        {
            var row = CloneTemplate(_basketRowTemplate);
            var definition = entry.Offer.Definition;

            ApplyResourceStack(row.Q<VisualElement>("output-icon"), row.Q<Label>("output-amount"), definition.Output);

            row.Q<Label>("basket-title").text = definition.DisplayName;
            row.Q<Label>("basket-detail").text = BuildInputsSummary(definition.RequiredInputs);
            row.Q<Label>("quantity-label").text = entry.Quantity.ToString();

            var offerId = entry.Offer.OfferId;

            row.Q<Button>("decrease-quantity-button").clicked += () => SetBasketQuantity(offerId, entry.Quantity - 1);
            row.Q<Button>("increase-quantity-button").clicked += () => SetBasketQuantity(offerId, entry.Quantity + 1);
            row.Q<Button>("remove-from-basket-button").clicked += () => RemoveFromBasket(offerId);

            return row;
        }

        private VisualElement BuildResourceStack(ResourceStack stack)
        {
            var element = CloneTemplate(_resourceStackTemplate);

            ApplyResourceStack(element.Q<VisualElement>("icon"), element.Q<Label>("amount"), stack);
            element.Q<Label>("name").text = stack.Resource != null ? stack.Resource.ResourceName : string.Empty;

            return element;
        }

        private static void ApplyResourceStack(VisualElement iconElement, Label amountLabel, ResourceStack stack)
        {
            if (stack.Resource?.ResourceIcon != null)
                iconElement.style.backgroundImage = new StyleBackground(stack.Resource.ResourceIcon);
            else
                iconElement.style.backgroundImage = StyleKeyword.Null;

            amountLabel.text = FormatAmount(stack.Amount);
        }

        private static VisualElement CloneTemplate(VisualElement template) => template;

        private void RefreshSummary()
        {
            if (_basket.Count == 0)
            {
                _deliveryTimeValue.text = "—";
                _bitsOfferValue.text      = "0";
                _bitsDeliveryValue.text   = "0";
                _submitOfferButton.SetEnabled(false);
                return;
            }

            var maxDeliveryHours = 0f;
            long totalOfferBits      = 0;
            long totalDeliveryBits   = 0;

            foreach (var entry in _basket)
            {
                var definition = entry.Offer.Definition;
                maxDeliveryHours = Mathf.Max(maxDeliveryHours, definition.DeliveryTimeHours);
                totalOfferBits += definition.BitsOfferPrice * entry.Quantity;
                totalDeliveryBits += definition.BitsDeliveryPrice * entry.Quantity;
            }

            _deliveryTimeValue.text = FormatDeliveryTime(maxDeliveryHours);
            _bitsOfferValue.text      = FormatBits(totalOfferBits);
            _bitsDeliveryValue.text   = FormatBits(totalDeliveryBits);
            _submitOfferButton.SetEnabled(true);
        }

        private void RefreshEmptyStates()
        {
            var hasOffers  = _catalog.Count > 0;
            var hasBasket  = _basket.Count > 0;

            _offersEmptyLabel.EnableInClassList("trading-window__tab-panel--hidden", hasOffers);
            _offersList.parent.style.display = hasOffers ? DisplayStyle.Flex : DisplayStyle.None;

            _basketEmptyLabel.EnableInClassList("trading-window__tab-panel--hidden", hasBasket);
            _basketList.parent.style.display = hasBasket ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private static string BuildInputsSummary(IReadOnlyList<ResourceStack> inputs)
        {
            if (inputs == null || inputs.Count == 0)
                return "No inputs";

            var builder = new StringBuilder();

            for (var i = 0; i < inputs.Count; i++)
            {
                var stack = inputs[i];
                if (stack.Resource == null)
                    continue;

                if (builder.Length > 0)
                    builder.Append(" · ");

                builder.Append(stack.Resource.ResourceName);
                builder.Append(' ');
                builder.Append(FormatAmount(stack.Amount));
            }

            return builder.Length == 0 ? "No inputs" : builder.ToString();
        }

        private static string FormatAmount(double amount)
        {
            return amount >= 1000d
                ? $"{amount:0,.0f}"
                : amount.ToString(amount % 1d < 0.001d ? "0" : "0.##");
        }

        private static string FormatBits(long bits) => bits.ToString("N0");

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
}
