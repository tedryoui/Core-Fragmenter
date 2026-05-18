using System;

namespace _.Scripts.Gameplay.Utility
{
    public interface IDamageable
    {
        public event Action OnDamageReceived;
        
        public void ReceiveDamage(int value);
    }
}