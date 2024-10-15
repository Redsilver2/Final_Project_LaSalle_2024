using UnityEngine;
using UnityEngine.Events;

namespace Redsilver2.Core.Stats
{
    public abstract class StatHandler : MonoBehaviour
    {
        [Header("Base Stat Handler")]
        [SerializeField] protected float maxValue;
        protected float currentValue;

        private UnityEvent<StatHandler, bool> onValueChanged = new UnityEvent<StatHandler, bool>();

        public float CurrentValue => currentValue;
        public float MaxValue => maxValue;
        public float PercentageValue => currentValue / maxValue;

        protected virtual void Start()
        {
            currentValue = maxValue;
        }

        protected void Decrease(float amount)
        {
            currentValue -= amount;

            if (currentValue < 0)
            {
                currentValue = 0;
            }

            onValueChanged?.Invoke(this, false);
        }
        protected void Increase(float amount)
        {
            currentValue += amount;

            if (currentValue > maxValue)
            {
                currentValue = maxValue;
            }

            onValueChanged?.Invoke(this, true);
        }

        public void AddOnValueChangedEvent(UnityAction<StatHandler, bool> action)
        {
            onValueChanged?.AddListener(action);
        }
        public void RemoveOnValueChangedEvent(UnityAction<StatHandler, bool> action)
        {
            onValueChanged?.AddListener(action);
        }
    }
}