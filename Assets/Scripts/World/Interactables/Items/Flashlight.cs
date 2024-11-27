using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Redsilver2.Core.Items
{
    public class Flashlight : LightSourceItem
    {
        [Space]
        [SerializeField] private Image fillbar;

        [Space]
        [SerializeField] private float fillbarColorLerpSpeed = 1.0f;    

        [Space]
        [SerializeField] private float batteryDrainPerSeconds;

        [Space]
        [SerializeField] private AudioClip flashlightOnClip;
        [SerializeField] private AudioClip flashlightOffClip;
        [SerializeField] private AudioClip flashlightEmptyClip;

        private bool isDrainingBattery = false;

        protected override void Awake()
        {
            base.Awake();

            lightLife.AddOnValueChangedEvent(value =>
            {
                float percentage = lightLife.PercentageValue;
                Color color = Color.red;

                if(percentage >= 0.7f && percentage <= 1f)
                {
                    color = Color.green;
                }
                else if(percentage >= 0.3f && percentage < 0.7f)
                {
                    color = Color.yellow;
                }

                if (fillbar != null)
                {
                    Debug.LogWarning(percentage);
                    fillbar.fillAmount = percentage;
                    fillbar.color      = Color.Lerp(fillbar.color, color, fillbarColorLerpSpeed * Time.deltaTime);
                }

            });

            TurnLightOffEvent();
        }


        public override string GetName()
        {
            return $"{base.GetName()} ({(int)(lightLife.PercentageValue * 100f)}%)";
        }

        public override Sprite GetIcon()
        {
            return null;
        }

        public override void Equip()
        {
            base.Equip();
            EnableControls();
        }

        public override void UnEquip()
        {
            base.UnEquip();
            DisableControls();
        }

        public override void TurnLightState(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                isDrainingBattery = !isDrainingBattery;

                if(lightLife.CurrentValue <= 0f)
                {
                    isDrainingBattery = false;
                }

                itemAnimationController.PlayAnimation(isDrainingBattery ? "Turn_On" : "Turn_Off");
            }
        }

        protected override void TurnLightOnEvent()
        {
            light.enabled = true;
            StartDrainingLightLife();
            PlaySound(flashlightOnClip);
        }
        protected override void TurnLightOffEvent()
        {
            isDrainingBattery = false;
            light.enabled = false;
            PlaySound(flashlightOffClip);
        }

        public void Recharge(float percentage, out bool isRecharged)
        {
            float maxValue = lightLife.MaxValue;
            isRecharged = false;

            if (percentage > 1f)
            {
                percentage = 1f;
            }
            else if(percentage < 0f)
            {
                percentage = 0f;
            }

            if (lightLife.CurrentValue < maxValue)
            {
                lightLife.Heal(maxValue * percentage);
                isRecharged = true;
            }
        }

        protected override void SetControls(PlayerControls controls, bool isSettingControls)
        {
            if (isSettingControls)
            {
                controls.Inventory.Flashlight.performed += TurnLightState;
            }
            else
            {
                controls.Inventory.Flashlight.performed -= TurnLightState;
            }
        }
    }

}
