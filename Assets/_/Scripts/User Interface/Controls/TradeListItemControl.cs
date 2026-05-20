using _.Scripts.Scriptable_Objects;
using _.Scripts.User_Interface.Events;
using static _.Scripts.User_Interface.TradeIdUtility;
using UnityEngine;
using UnityEngine.UIElements;

namespace _.Scripts.User_Interface.Controls
{
    public class TradeListItemControl : VisualElement
    {
        private readonly string _tradeId;

        public TradeListItemControl(TradeConfigScriptableObject trade)
        {
            _tradeId = TradeIdUtility.GetId(trade);

            var asset = Resources.Load<VisualTreeAsset>("User Interface Templates/TradeListItemControl");
            if (asset == null)
            {
                Debug.LogError($"{nameof(TradeListItemControl)}: VisualTreeAsset not found.");
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

            this.Q<Button>("add-button").clicked += () => TradeEvents.RaiseTradeAdded(_tradeId);
        }

        private static void ApplyResource(VisualElement icon, Label amountLabel, ResourceConfigScriptableObject resource, double amount)
        {
            if (resource?.ResourceIcon != null)
                icon.style.backgroundImage = new StyleBackground(resource.ResourceIcon);
            else
                icon.style.backgroundImage = StyleKeyword.Null;

            amountLabel.text = FormatAmount(amount);

            var nameLabel = icon.parent.Q<Label>("resource-name");
            if (nameLabel != null)
                nameLabel.text = resource != null ? resource.ResourceName : string.Empty;
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
