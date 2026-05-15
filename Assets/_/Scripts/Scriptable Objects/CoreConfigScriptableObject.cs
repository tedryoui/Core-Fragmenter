using System;
using System.Collections.Generic;
using UnityEngine;

namespace _.Scripts.Scriptable_Objects
{
    [CreateAssetMenu(fileName = "CoreConfig", menuName = "CoreFragmenter/Core Config")]
    public class CoreConfigScriptableObject : ScriptableObject
    {
        [Serializable]
        public struct ResourceDrop
        {
            [SerializeField] private ResourceConfigScriptableObject _resource;
            [SerializeField] private double _amount;

            public ResourceConfigScriptableObject Resource => _resource;
            public double Amount => _amount;
        }

        [Header("Identity")]
        [SerializeField] private int _stageNumber;
        [SerializeField] private string _coreName;

        [Header("Combat")]
        [SerializeField] private double _maxHealth;
        [SerializeField] private bool _isBoss;
        [SerializeField] private float _bossTimer;

        [Header("Rewards")]
        [SerializeField] private List<ResourceDrop> _rewards = new();

        public int StageNumber => _stageNumber;
        public string CoreName => _coreName;
        public double MaxHealth => _maxHealth;
        public bool IsBoss => _isBoss;
        public float BossTimer => _bossTimer;
        public IReadOnlyList<ResourceDrop> Rewards => _rewards;
    }
}
