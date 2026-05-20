using UnityEngine;

namespace _.Scripts.Scriptable_Objects
{
    [CreateAssetMenu(fileName = "TradeConfig", menuName = "CoreFragmenter/Trade Config")]
    public class TradeConfigScriptableObject : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string _identity;
        [SerializeField] private string _tradeName;
        [SerializeField] private float _deliveryTime;
        [SerializeField] private float _baseCost;

        [Header("Input")]
        [SerializeField] private ResourceConfigScriptableObject _inputResource;
        [SerializeField] private double _inputAmount;

        [Header("Output")]
        [SerializeField] private ResourceConfigScriptableObject _outputResource;
        [SerializeField] private double _outputAmount;

        public string                         Identity       => _identity;
        public string                         TradeName      => _tradeName;
        public float                          DeliveryTime   => _deliveryTime;
        public float                          BaseCost       => _baseCost;
        public ResourceConfigScriptableObject InputResource  => _inputResource;
        public double                         InputAmount    => _inputAmount;
        public ResourceConfigScriptableObject OutputResource => _outputResource;
        public double                         OutputAmount   => _outputAmount;
    }
}
