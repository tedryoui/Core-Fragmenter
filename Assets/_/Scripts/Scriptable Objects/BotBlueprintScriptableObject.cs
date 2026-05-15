using System;
using UnityEngine;

namespace _.Scripts.Scriptable_Objects
{
    [CreateAssetMenu(fileName = "BotBlueprint", menuName = "CoreFragmenter/Bot Blueprint")]
    public class BotBlueprintScriptableObject : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string _id;
        [SerializeField] private string _botName;
        [SerializeField] private float _productionTime;
        [SerializeField] private int _techTier;

        [Header("Combat — Base Stats")]
        [SerializeField] private double _baseDamage;
        [SerializeField] private float _baseAttackSpeed;
        [SerializeField] private float _baseCritChance;

        [Header("Production Cost")]
        [SerializeField] private ResourceConfigScriptableObject _costResourceA;
        [SerializeField] private double _baseCostA;
        [SerializeField] private ResourceConfigScriptableObject _costResourceB;
        [SerializeField] private double _baseCostB;
        [SerializeField] private float _costMultiplier = 1.15f;

        public string Id => _id;
        public string BotName => _botName;
        public float ProductionTime => _productionTime;
        public int TechTier => _techTier;
        public double BaseDamage => _baseDamage;
        public float BaseAttackSpeed => _baseAttackSpeed;
        public float BaseCritChance => _baseCritChance;
        public ResourceConfigScriptableObject CostResourceA => _costResourceA;
        public double BaseCostA => _baseCostA;
        public ResourceConfigScriptableObject CostResourceB => _costResourceB;
        public double BaseCostB => _baseCostB;
        public float CostMultiplier => _costMultiplier;

        public double GetCostA(int currentLevel) =>
            _baseCostA * Math.Pow(_costMultiplier, currentLevel);

        public double GetCostB(int currentLevel) =>
            _baseCostB * Math.Pow(_costMultiplier, currentLevel);
    }
}
