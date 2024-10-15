using System.Collections.Generic;
using Redsilver2.Core.Items;
using Redsilver2.Core.Stats;
using UnityEngine;
using UnityEngine.Events;

namespace Redsilver2.Core.Player
{
    public class PlayerWeight : MonoBehaviour
    {
        [SerializeField] private float maxWeight = 100f;

        [SerializeField] private float maxWeightWalkSpeed;
        [SerializeField] private float maxWeightRunSpeed;

        [Space]
        [SerializeField] private float maxWeightStaminaDecreaseSpeed    = 10f;
        [SerializeField] private float maxWeightStaminaRecoverySpeed    = 10f;
        [SerializeField] private float maxWeightStaminaRecoveryWaitTime = 10f;

        private float currentWeight;
        private IWeighable[] weighables;
        private UnityEvent<float> onWeightValueChanged = new UnityEvent<float>();


        private void Start()
        {
            Stamina stamina = GetComponent<Stamina>();

            if (stamina != null)
            {
                AddOnWeightValueChanged(value =>
                {
                    float defaultStaminaDecreaseSpeed = stamina.DefaultValueDecreaseSpeed;
                    float defaultStaminaRecoverySpeed = stamina.DefaultRecoverySpeed;
                    float defaultStaminaRecoveryWaitTime = stamina.DefaultRecoveryWaitTime;

                    float staminaDecreaseSpeed;
                    float staminaRecoverySpeed;
                    float staminaRecoveryWaitTime;

                    if (value <= 0f)
                    {
                        staminaDecreaseSpeed    = defaultStaminaDecreaseSpeed;
                        staminaRecoverySpeed    = defaultStaminaRecoverySpeed;
                        staminaRecoveryWaitTime = defaultStaminaRecoveryWaitTime;
                    }
                    else if (value >= 1f)
                    {
                        staminaDecreaseSpeed    = maxWeightStaminaDecreaseSpeed;
                        staminaRecoverySpeed    = maxWeightStaminaRecoverySpeed;
                        staminaRecoveryWaitTime = maxWeightStaminaRecoveryWaitTime;
                    }
                    else
                    {
                        staminaDecreaseSpeed = Mathf.Lerp(defaultStaminaDecreaseSpeed,
                                                          maxWeightStaminaDecreaseSpeed, value);


                        staminaRecoverySpeed = Mathf.Lerp(defaultStaminaRecoverySpeed,
                                                           maxWeightStaminaRecoverySpeed, value);

                        staminaRecoveryWaitTime = Mathf.Lerp(defaultStaminaRecoveryWaitTime,
                                                             maxWeightStaminaRecoveryWaitTime, value);
                    }

                    stamina.SetDecreaseSpeed(staminaDecreaseSpeed);
                    stamina.SetRecoverySpeed(staminaRecoverySpeed);
                    stamina.SetRecoveryWaitTime(staminaRecoveryWaitTime);
                });
            }
        }

        public bool IsExeceedingMaxWeight(float weightToAdd)
        {
            float totalWeight = currentWeight + weightToAdd;

            if (totalWeight <= maxWeight)
            {
                return false;
            }

            return true;
        }

        public void AddWeight(float weight)
        {
            currentWeight += weight;

            if(currentWeight > maxWeight)
            {
                currentWeight = maxWeight;
            }

            onWeightValueChanged?.Invoke(currentWeight / maxWeight);
        }


        public void RemoveWeight(float weight)
        {
            currentWeight -= weight;

            if(currentWeight < 0f)
            {
                currentWeight = 0f;
            }

            onWeightValueChanged?.Invoke(currentWeight / maxWeight);
        }

        public float GetDesiredMovementSpeed(PlayerController player)
        {
            float currentMovementSpeed;
            float percentage = currentWeight / maxWeight;

            if (player.IsRunning)
            {
                currentMovementSpeed = Mathf.Lerp(player.DefaultRunSpeed, maxWeightRunSpeed, percentage);
            }
            else
            {
                currentMovementSpeed = Mathf.Lerp(player.DefaultWalkSpeed, maxWeightWalkSpeed, percentage);
            }

            return currentMovementSpeed;
        }
        public void AddOnWeightValueChanged(UnityAction<float> action)
        {
            onWeightValueChanged?.AddListener(action);
        }
        public void RemoveOnWeightValueChanged(UnityAction<float> action)
        {
            onWeightValueChanged?.RemoveListener(action);
        }
    }

}
