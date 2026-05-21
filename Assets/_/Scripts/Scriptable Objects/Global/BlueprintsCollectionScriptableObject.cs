using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _.Scripts.Scriptable_Objects.Global
{
    [CreateAssetMenu(fileName = "Blueprints Collection", menuName = "Core Fragmenter/Blueprints Collection", order = 0)]
    public class BlueprintsCollectionScriptableObject : ScriptableObject
    {
        [SerializeField] private List<BlueprintScriptableObject> _blueprints;

        public IReadOnlyCollection<BlueprintScriptableObject> Blueprints => _blueprints.AsReadOnly();

        public BlueprintScriptableObject Get(string identity = "")
        {
            var result = Blueprints.FirstOrDefault(x => x.Identity.Equals(identity));

            if (result == null)
                throw new KeyNotFoundException($"Trade config {identity} not found!");

            return result;
        }
    }
}