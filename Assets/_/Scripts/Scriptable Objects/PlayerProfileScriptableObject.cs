using System;
using System.Collections.Generic;
using UnityEngine;

namespace _.Scripts.Scriptable_Objects
{
    [CreateAssetMenu(fileName = "PlayerProfile", menuName = "CoreFragmenter/Player Profile")]
    public class PlayerProfileScriptableObject : ScriptableObject
    {
        [Serializable]
        public struct ResourceAmount
        {
            [SerializeField] private ResourceConfigScriptableObject _resource;
            [SerializeField] private double _currentAmount;

            public ResourceConfigScriptableObject Resource => _resource;
            public double CurrentAmount => _currentAmount;

            public ResourceAmount(ResourceConfigScriptableObject resource, double currentAmount)
            {
                _resource = resource;
                _currentAmount = currentAmount;
            }
        }

        [Header("Player")]
        [SerializeField] private string _playerName;
        [SerializeField] private int _currentStage;
        [SerializeField] private int _prestigeCount;

        [Header("Progress")]
        [SerializeField] private List<BotBlueprintScriptableObject> _unlockedBlueprints = new();
        [SerializeField] private List<ResourceAmount> _inventory = new();

        public string PlayerName => _playerName;
        public int CurrentStage => _currentStage;
        public int PrestigeCount => _prestigeCount;
        public IReadOnlyList<BotBlueprintScriptableObject> UnlockedBlueprints => _unlockedBlueprints;
        public IReadOnlyList<ResourceAmount> Inventory => _inventory;

        public double GetResourceCount(ResourceConfigScriptableObject targetResource)
        {
            if (targetResource == null)
                return 0d;

            for (int i = 0; i < _inventory.Count; i++)
            {
                if (_inventory[i].Resource == targetResource)
                    return _inventory[i].CurrentAmount;
            }

            return 0d;
        }

        public void AddResource(ResourceConfigScriptableObject targetResource, double amount)
        {
            if (targetResource == null || amount == 0d)
                return;

            for (int i = 0; i < _inventory.Count; i++)
            {
                if (_inventory[i].Resource != targetResource)
                    continue;

                _inventory[i] = new ResourceAmount(targetResource, _inventory[i].CurrentAmount + amount);
                return;
            }

            _inventory.Add(new ResourceAmount(targetResource, amount));
        }

        public bool SpendResource(ResourceConfigScriptableObject targetResource, double amount)
        {
            if (targetResource == null || amount <= 0d)
                return false;

            for (int i = 0; i < _inventory.Count; i++)
            {
                if (_inventory[i].Resource != targetResource)
                    continue;

                if (_inventory[i].CurrentAmount < amount)
                    return false;

                _inventory[i] = new ResourceAmount(targetResource, _inventory[i].CurrentAmount - amount);
                return true;
            }

            return false;
        }
    }
}
