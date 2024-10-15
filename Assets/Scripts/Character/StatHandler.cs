using UnityEngine;
using UnityEngine.Events;

public abstract class StatHandler : MonoBehaviour
{
    [Header("Stat Settings")]
    [SerializeField] protected float maxValue;

    protected float currentValue;
    private UnityEvent<float> onStatPercentageValueChanged = new UnityEvent<float>();

    public float CurrentValue => currentValue;
    public float StatPercentage => currentValue / maxValue;

    protected virtual void Start()
    {
        currentValue = maxValue;
    }

    public void Decrease(float statReduceAmount)
    {
        currentValue -= statReduceAmount;

        if (currentValue <= 0f)
        {
            currentValue = 0f;
        }

        onStatPercentageValueChanged?.Invoke(StatPercentage);
    }
    public void Increase(float statIncreaseAmount)
    {
        currentValue += statIncreaseAmount;

        if (currentValue > maxValue)
        {
            currentValue = maxValue;
        }

        onStatPercentageValueChanged?.Invoke(StatPercentage);
    }
    
    public void AddStatPercentageValueChangedEvent(UnityAction<float> action) 
    {
        Debug.Log(onStatPercentageValueChanged);
        onStatPercentageValueChanged.AddListener(action);
    }  
    public void RemoveStatPercentageValueChangedEvent(UnityAction<float> action)
    {
        onStatPercentageValueChanged.RemoveListener(action);
    }
}
