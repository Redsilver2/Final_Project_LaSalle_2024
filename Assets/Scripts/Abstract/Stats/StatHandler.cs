using UnityEngine;
using UnityEngine.Events;

namespace Redsilver2.Core.Stats
{
    public abstract class StatHandler : MonoBehaviour
    {
        [Header("Base Stat Handler")]
        [SerializeField] protected float maxValue;
        protected float currentValue;

        private UnityEvent<float> onValueChanged;

        public float CurrentValue => currentValue;
        public float MaxValue => maxValue;
        public float PercentageValue => currentValue / maxValue;

        protected virtual void Awake()
        {
            currentValue = maxValue;
            onValueChanged = new UnityEvent<float>();
        }

        protected void Decrease(float amount)
        {
            currentValue -= amount;

            if (currentValue < 0)
            {
                currentValue = 0;
            }

            onValueChanged?.Invoke(currentValue / maxValue);
        }
        protected void Increase(float amount)
        {
            currentValue += amount;

            if (currentValue > maxValue)
            {
                currentValue = maxValue;
            }

            onValueChanged?.Invoke(currentValue / maxValue);
        }

        public void AddOnValueChangedEvent(UnityAction<float> action)
        {
            onValueChanged?.AddListener(action);
        }
        public void RemoveOnValueChangedEvent(UnityAction<float> action)
        {
            onValueChanged?.RemoveListener(action);
        }
    }
}