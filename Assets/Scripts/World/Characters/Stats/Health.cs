using UnityEngine;
using UnityEngine.Events;

namespace Redsilver2.Core.Stats
{
    public class Health : LimitedRecoverableStat, IHealable, IDamageable
    {
        private UnityEvent<Health> onHealed  = new UnityEvent<Health>();
        private UnityEvent<Health> onDamaged = new UnityEvent<Health>();
        private UnityEvent<Health> onDeath   = new UnityEvent<Health>();

        protected override void Start()
        {
            base.Start();
            AddOnValueChangedEvent((handler, isValueIncreasing) =>
            {
                if(isValueIncreasing)
                {
                    onHealed?.Invoke(this);
                }
                else
                {
                    if(handler.CurrentValue <= 0f)
                    {
                        onDeath?.Invoke(this);
                    }
                    else
                    {
                        onDamaged?.Invoke(this);
                    }
                }
            });
        }

        public void Heal(float healAmount)
        {
            Increase(healAmount);
        }

        public void Damage(float damageAmount)
        {
            Decrease(damageAmount);
        }

        public void AddOnHealedEvent(UnityAction<Health> action)
        {
            onHealed?.AddListener(action);
        }
        public void RemoveOnHealedEvent(UnityAction<Health> action)
        {
            onHealed?.RemoveListener(action);
        }

        public void AddOnDamagedEvent(UnityAction<Health> action)
        {
            onDamaged?.AddListener(action);
        }
        public void RemoveOnDamagedEvent(UnityAction<Health> action)
        {
            onDamaged?.RemoveListener(action);
        }

        public void AddOnDeathEvent(UnityAction<Health> action)
        {
            onDeath?.AddListener(action);
        }
        public void RemoveOnDeathEvent(UnityAction<Health> action)
        {
            onDeath?.RemoveListener(action);
        }
    }
}
