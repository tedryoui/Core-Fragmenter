using _.Scripts.Scriptable_Objects;
using UnityEngine;
using UnityEngine.UIElements;

namespace _.Scripts.User_Interface.Controls
{
    public class CartItemControl : VisualElement
    {
        private readonly string                 _identity;
        private readonly int                    _quantity;
        private readonly TradingWindowViewModel _viewModel;

        public CartItemControl(TradeConfigScriptableObject trade, int quantity, TradingWindowViewModel viewModel)
        {
            _identity  = trade.Identity;
            _quantity  = Mathf.Max(1, quantity);
            _viewModel = viewModel;

            var asset = Resources.Load<VisualTreeAsset>("User Interface Templates/CartItemControl");
            if (asset == null)
            {
                Debug.LogError($"{nameof(CartItemControl)}: VisualTreeAsset not found.");
                return;
            }

            asset.CloneTree(this);
            Bind(trade);
        }

        private void Bind(TradeConfigScriptableObject trade)
        {
            this.Q<Label>("trade-name").text = trade.TradeName;

            ApplyResource(this.Q<VisualElement>("input-icon"), this.Q<Label>("input-amount"), trade.InputResource, trade.InputAmount);
            ApplyResource(this.Q<VisualElement>("output-icon"), this.Q<Label>("output-amount"), trade.OutputResource, trade.OutputAmount);

            this.Q<Label>("delivery-time").text = FormatDeliveryTime(trade.DeliveryTime);
            this.Q<Label>("base-cost").text       = trade.BaseCost.ToString("N0");

            this.Q<Label>("quantity-value").text = _quantity.ToString();

            this.Q<Button>("decrease-button").clicked += () =>
            {
                var next = _quantity - 1;
                if (next <= 0)
                    _viewModel.RemoveTrade(_identity);
                else
                    _viewModel.ChangeQuantity(_identity, next);
            };

            this.Q<Button>("increase-button").clicked += () =>
                _viewModel.ChangeQuantity(_identity, _quantity + 1);

            this.Q<Button>("remove-button").clicked += () => _viewModel.RemoveTrade(_identity);
        }

        private static void ApplyResource(VisualElement icon, Label amountLabel, ResourceConfigScriptableObject resource, double amount)
        {
            if (resource?.ResourceIcon != null)
                icon.style.backgroundImage = new StyleBackground(resource.ResourceIcon);
            else
                icon.style.backgroundImage = StyleKeyword.Null;

            amountLabel.text = FormatAmount(amount);
        }

        private static string FormatAmount(double amount) =>
            amount >= 1000d ? $"{amount:0,.0f}" : amount.ToString(amount % 1d < 0.001d ? "0" : "0.##");

        private static string FormatDeliveryTime(float hours)
        {
            if (hours <= 0f)
                return "Instant";

            if (hours < 1f)
                return $"{Mathf.CeilToInt(hours * 60f)} min";

            return hours < 24f ? $"{hours:0.#} h" : $"{hours / 24f:0.#} d";
        }
    }
}
