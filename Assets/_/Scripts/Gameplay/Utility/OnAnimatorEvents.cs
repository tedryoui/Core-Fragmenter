using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _.Scripts.Gameplay.Utility
{
    public class OnAnimatorEvents : MonoBehaviour
    {
        [SerializeField, DisableIf("@true")] private Animator _animator;
        
        public event Action<Animator> onAnimatorMoved;

        private void Awake()
        {
            onAnimatorMoved = delegate(Animator animator) { };
        }

        private void OnValidate()
        {
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            if (_animator.Equals(null))
                _animator = GetComponent<Animator>();
        }

        private void OnAnimatorMove()
        {
            onAnimatorMoved?.Invoke(_animator);
        }

        private void OnAnimatorIK(int layerIndex)
        {
        }

        private void OnDidApplyAnimationProperties()
        {
        }
    }
}