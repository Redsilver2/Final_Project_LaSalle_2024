using Redsilver2.Core.Motion;
using Redsilver2.Core.Player;
using Redsilver2.Core.Stats;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Redsilver2.Core.Items
{
    [RequireComponent(typeof(Health))]
    public abstract class LightSourceItem : EquippableItem
    {
        [SerializeField] protected Light light;
        [SerializeField] protected float lightLifeDrainPerSeconds;

        protected Health lightLife;
        private IEnumerator drainLightLifeCoroutine;

        protected bool isDrainingLightLife = false;

        protected override void Awake()
        {
            base.Awake();
            lightLife = GetComponent<Health>();
        }


        public    abstract void TurnLightState(InputAction.CallbackContext context);
        protected abstract void TurnLightOnEvent();
        protected abstract void TurnLightOffEvent();

        protected virtual IEnumerator DrainLightLife()
        {
            isDrainingLightLife = true;

            while (isDrainingLightLife && lightLife.CurrentValue > 0f)
            {
                if (!PauseManager.IsGamePaused)
                {
                   lightLife.Damage(lightLifeDrainPerSeconds * Time.deltaTime);
                }

                yield return null;
            }

            if (lightLife.CurrentValue <= 0f)
            {
                //PlayAnimation(FLASHLIGHT_EMPTY_ANIMATION, 0.2f);
                // PlaySound(flashlightEmptyClip);
                light.enabled = false;
            }

            isDrainingLightLife = false;
        }

        public void StartDrainingLightLife()
        {
            StopDrainLightLife();
            drainLightLifeCoroutine = DrainLightLife();
            StartCoroutine(drainLightLifeCoroutine);
        }

        protected override void OnItemAddedEvent(EquippableItem item, PlayerInventory inventory)
        {
            if (item == this)
            {
                LightSourceItem equippedLightSourceItem = inventory.EquippedLightSource;
                PlayerHandMotionHandler lightsourceMotionHandler = inventory.LightSourceItemMotionHandler;

                if (equippedLightSourceItem != null)
                {
                    equippedLightSourceItem.Drop();
                }
                else
                {
                    inventory.SetEquippedLightSource(this);

                    if (PlayerController.Instance.enabled == true)
                    {
                        transform?.SetParent(lightsourceMotionHandler.transform);
                        SetVisibility(true);

                        SetSource(inventory.LightAudioSource);
                        lightsourceMotionHandler.SethandMotionSetting(motionSetting);
                        Equip();
                    }
                }

            }
        }

        private void StopDrainLightLife()
        {
            if (drainLightLifeCoroutine != null)
            {
                StopCoroutine(drainLightLifeCoroutine);
            }
        }
    }

}
