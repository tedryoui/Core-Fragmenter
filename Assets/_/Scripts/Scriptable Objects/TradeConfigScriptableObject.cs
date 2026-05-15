using UnityEngine;

namespace _.Scripts.Scriptable_Objects
{
    [CreateAssetMenu(fileName = "TradeConfig", menuName = "CoreFragmenter/Trade Config")]
    public class TradeConfigScriptableObject : ScriptableObject
    {
        public enum ModuleType
        {
            Factory,
            BlackMarket
        }

        [Header("Identity")]
        [SerializeField] private string _tradeId;
        [SerializeField] private string _tradeName;
        [SerializeField] private ModuleType _moduleType;

        [Header("Input")]
        [SerializeField] private ResourceConfigScriptableObject _inputResource;
        [SerializeField] private double _inputAmount;

        [Header("Output")]
        [SerializeField] private ResourceConfigScriptableObject _outputResource;
        [SerializeField] private double _outputAmount;

        public string TradeId => _tradeId;
        public string TradeName => _tradeName;
        public ModuleType Module => _moduleType;
        public ResourceConfigScriptableObject InputResource => _inputResource;
        public double InputAmount => _inputAmount;
        public ResourceConfigScriptableObject OutputResource => _outputResource;
        public double OutputAmount => _outputAmount;
    }
}
