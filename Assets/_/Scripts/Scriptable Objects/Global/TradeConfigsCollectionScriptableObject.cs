using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _.Scripts.Scriptable_Objects.Global
{
    [CreateAssetMenu(fileName = "Trade Configs Collection", menuName = "Core Fragmenter/Trade Configs Collection", order = 0)]
    public class TradeConfigsCollectionScriptableObject : ScriptableObject
    {
        [SerializeField] private List<TradeConfigScriptableObject> _tradeConfigs;

        public IReadOnlyCollection<TradeConfigScriptableObject> TradeConfigs => _tradeConfigs.AsReadOnly();

        public TradeConfigScriptableObject Get(string identity = "")
        {
            var result = TradeConfigs.FirstOrDefault(x => x.Identity.Equals(identity));

            if (result == null)
                throw new KeyNotFoundException($"Trade config {identity} not found!");

            return result;
        }
    }
}