using Redsilver2.Core.Counters;
using Redsilver2.Core.Hittable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Redsilver2.Core.Items
{
    public abstract class MeleeWeapon : EquippableItem
    {
        [Space]
        [SerializeField] private Transform hitPosition;

        [Space]
        [SerializeField] private uint   maxAmountOfHitRays;
        [SerializeField] private float  swingDuration;

        [Space]
        [SerializeField] private float     maxHitRange = 10f;

        [Space]
        [SerializeField] private float damagePerHit = 10;
        private bool canSwing = true;

        private IEnumerator swingCoroutine;
        private IEnumerator resetSwingCoroutine;

        public static Dictionary<Collider, IHittable> hittableInstances = new Dictionary<Collider, IHittable>();

        protected override void Awake()
        {
            base.Awake();
        }


        public override void Equip()
        {
            base.Equip();
            canSwing = true;
        }

        private void Swing(InputAction.CallbackContext context) 
        {
            if (context.performed && canSwing)
            {
                canSwing = false;
                itemAnimationController.PlayAnimation("Swing");

                StopSwing();
                
                swingCoroutine = SwingCoroutine();
                resetSwingCoroutine = ResetSwing();
               
                StartCoroutine(swingCoroutine);
                StartCoroutine(resetSwingCoroutine);

            }
        }

        private void StopSwing()
        {
            if(swingCoroutine != null)
            {
                StopCoroutine(swingCoroutine);
            }

            if(resetSwingCoroutine != null)
            {
                StopCoroutine(resetSwingCoroutine);
            }
        }

        private IEnumerator ResetSwing()
        {
            Debug.Log("???");
            yield return Counter.WaitForSeconds(itemAnimationController.GetAnimationLenght("Swing"));
            Debug.Log("!!!");

            itemAnimationController.PlayAnimation("Idle");
            yield return Counter.WaitForSeconds(itemAnimationController.GetAnimationLenght("Idle") + 0.1f);
            canSwing = true;

        }

        private IEnumerator SwingCoroutine()
        {
            uint currentNumberOfHitRaysCasted = 0;
            float waitTime = swingDuration / maxAmountOfHitRays;

            List<Collider> collidersHit = new List<Collider>();

            if (hitPosition != null)
            {
                while (currentNumberOfHitRaysCasted != maxAmountOfHitRays)
                {
                    Debug.DrawRay(hitPosition.position, hitPosition.forward, Color.green, 5f);

                    if (Physics.Raycast(hitPosition.position, hitPosition.forward, out RaycastHit hitInfo, maxHitRange) && hitInfo.collider != null && !collidersHit.Contains(hitInfo.collider))
                    {
                        Collider collider = hitInfo.collider;
                        collidersHit.Add(collider);
                        OnHitColliderEvent(collider, damagePerHit);
                    }

                    currentNumberOfHitRaysCasted++;
                    yield return Counter.WaitForSeconds(waitTime);
                }
            }
        }

        public abstract void OnHitColliderEvent(Collider collider, float damage);

        protected override void SetControls(PlayerControls controls, bool isSettingControls)
        {
            if (isSettingControls)
            {
                controls.Item.LeftClickAction.performed += Swing;
                controls.Item.Enable();
            }
            else
            {
                controls.Item.LeftClickAction.performed -= Swing;
                controls.Item.Disable();
            }
        }

        public static void AddHittableInstance(Collider collider, IHittable hittable)
        {
            if (collider != null && !hittableInstances.ContainsKey(collider))
            {
                hittableInstances.Add(collider, hittable);
            }
        }
        public static void RemoveHittableInstance(Collider collider)
        {
            if (hittableInstances.ContainsKey(collider))
            {
                hittableInstances.Remove(collider); 
            }
        }
        public static void ClearHittableInstances()
        {
            hittableInstances.Clear();
        }


    }
}
