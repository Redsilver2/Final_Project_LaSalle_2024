using Redsilver2.Core.Stats;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Redsilver2.Core.Items
{
    [RequireComponent(typeof(Health))]
    public class Flashlight : EquippableItem
    {
        [Space]
        [SerializeField] private Light light;

        [Space]
        [SerializeField] private float batteryDrainPerSeconds;

        private Health batteryPower;
        private IEnumerator drainBatteryCoroutine;

        protected override void Start()
        {
            base.Start();

            batteryPower = GetComponent<Health>();
            batteryPower.AddOnValueChangedEvent((handler, isValueIncreasing) =>
            {
                Debug.Log("Battery Power: " + handler.PercentageValue);
            });

            if (light)
            {
                light.enabled = false;
            }
        }


        public override string GetName()
        {
            return $"{base.GetName()} ({(int)(batteryPower.PercentageValue * 100f)}%)";
        }

        public override Sprite GetInteractionSprite()
        {
            return null;
        }

        public override void Equip()
        {
            base.Equip();
        }

        public override void UnEquipped()
        {
            base.UnEquipped();
        }

        private void DrainBattery()
        {
            StopDrainBattery();
            drainBatteryCoroutine = DrainBatteryCoroutine();
            StartCoroutine(drainBatteryCoroutine);        
        }

        private void StopDrainBattery()
        {
            if(drainBatteryCoroutine != null)
            {
                StopCoroutine(drainBatteryCoroutine);
            }
        }

        public void Recharge(float percentage, out bool isRecharged)
        {
            float maxValue = batteryPower.MaxValue;
            isRecharged = false;

            if (percentage > 1f)
            {
                percentage = 1f;
            }
            else if(percentage < 0f)
            {
                percentage = 0f;
            }

            if(batteryPower.CurrentValue < maxValue)
            {
                batteryPower.Heal(maxValue * percentage);
                isRecharged = true;
            }

            //PlaySound();
        }
        private IEnumerator DrainBatteryCoroutine()
        {
            while (batteryPower.CurrentValue > 0f)
            {
               batteryPower.Damage(batteryDrainPerSeconds * Time.deltaTime);
               yield return null;
            }

            if(batteryPower.CurrentValue <= 0f)
            {
                light.enabled = false;
            }
        }
    }
}
