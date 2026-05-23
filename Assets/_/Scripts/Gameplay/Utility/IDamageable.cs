using System;

namespace _.Scripts.Gameplay.Utility
{
    public interface IDamageable
    {
        public event Action<int> OnDamageReceived;
        
        public void ReceiveDamage(int value);
    }
}