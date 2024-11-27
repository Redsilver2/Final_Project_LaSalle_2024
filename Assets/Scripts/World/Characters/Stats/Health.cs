using UnityEngine;
using UnityEngine.Events;


namespace Redsilver2.Core.Stats
{
    public class Health : LimitedRecoverableStat, IHealable, IDamageable
    {
        private UnityEvent<Health> onHealed;
        private UnityEvent<Health> onDamaged;

        protected override void Awake()
        {
            base.Awake();
            onHealed = new UnityEvent<Health>();
            onDamaged = new UnityEvent<Health>();
        }

        public void Heal(float healAmount)
        {
            Increase(healAmount);
            onHealed.Invoke(this);
        }

        public void Damage(float damageAmount)
        {
            Decrease(damageAmount);

            if (currentValue > 0)
            {
                onDamaged.Invoke(this);
            }
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
    }
}
