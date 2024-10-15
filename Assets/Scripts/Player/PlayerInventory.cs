using Redsilver2.Core.Events;
using Redsilver2.Core.Interactables;
using Redsilver2.Core.Items;
using Redsilver2.Core.Motion;
using System.Collections.Generic;
using UnityEngine;
using IEnumerator = System.Collections.IEnumerator;

namespace Redsilver2.Core.Player
{
    public class PlayerInventory : PlayerInventoryEvents
    {
        [SerializeField] private PlayerHandMotionHandler flashlightMotionHandler;
        [SerializeField] private PlayerHandMotionHandler itemMotionHandler;

        [Space]
        [SerializeField] private Animator flashlightAnimator;
        [SerializeField] private Animator itemAnimator;

        [Space]
        [SerializeField] private AudioSource equipItemSource;
        [SerializeField] private AudioSource switchItemSource;

        [Space]
        [SerializeField] private EquippableItem[] starterItems;
        [SerializeField] private int starterItemIndex = 0;

        [Space]
        [SerializeField] private float interactionRayLenght = 5f;

        [Space]
        [SerializeField] private int maxCarryCapacity = 5;

        private Flashlight     equippedFlashlight = null;
        private EquippableItem equippedItem       = null;

        private List<EquippableItem> items;
        private PlayerWeight currentWeight;

        private PlayerControls.InventoryActions controls;

        protected override void Start()
        {
            PlayerController player = PlayerController.Instance;
            base.Start();

            currentWeight   = player.Weight;
            controls        = player.Controls.Inventory;

            items           = new List<EquippableItem>();

            player.AddOnStateChangedEvent(isEnabled =>
            {
                enabled = isEnabled;

                if(!isEnabled)
                {
                    equippedFlashlight?.UnEquipped();
                    equippedItem?.UnEquipped();
                }
                else
                {
                    equippedFlashlight?.Equip();
                    equippedItem?.Equip();
                }
            });

            AddOnItemAddedEvent(item =>
            {
                Transform transform = item.transform;
                PlayerHandMotionSetting motionSetting = item.MotionSetting;
                RuntimeAnimatorController animatorController = item.AnimatorController;

                item.SetVisibility(false);

                if (currentWeight != null)
                {
                    currentWeight.AddWeight(item.GetWeight());
                }
                
                if(item.TryGetComponent(out Flashlight flashlight))
                {
                    if(equippedFlashlight != null)
                    {
                        equippedFlashlight.Drop();
                    }

                    transform?.SetParent(flashlightMotionHandler.transform);

                    transform.localPosition = Vector3.down * item.MaxHidePositionY;
                    item.SetRotationInHand();

                    equippedFlashlight = flashlight;
                    item?.SetVisibility(true);

                    item.SetAnimator(flashlightAnimator);
                    flashlightAnimator.runtimeAnimatorController = animatorController;

                    flashlightMotionHandler.SethandMotionSetting(motionSetting);
                    equippedFlashlight.Equip();
                }
                else
                {
                    transform?.SetParent(itemMotionHandler.transform);
                    itemMotionHandler.SethandMotionSetting(motionSetting);

                    if (equippedItem == null)
                    {

                    }   
                }        
            });

            AddOnItemRemovedEvent(item =>
            {
                currentWeight?.RemoveWeight(-item.GetWeight());
            });
        }

        private void Update()
        {
            InteractionRay();
        }

        private void InteractionRay()
        {
            Ray ray = new Ray(transform.position, transform.forward);

            if(Physics.Raycast(ray, out RaycastHit hitInfo, interactionRayLenght, LayerMask.GetMask("Interactable")) && hitInfo.collider != null) 
            {
                Collider collider = hitInfo.collider;

                if (collider.TryGetComponent(out IDescribable describable))
                {
                    Debug.Log(describable.GetName());
                }

                if (controls.Interact.WasPressedThisFrame())
                {
                    if (collider.TryGetComponent(out Interactable interactable))
                    {
                        interactable?.Interact();
                    }
                }
            }
        }

        public void AddItem(EquippableItem item, out bool isAdded)
        {
            isAdded = false;

            if (!items.Contains(item) && item != null && currentWeight != null)
            {
                Debug.Log("Phase 1");

                if (!currentWeight.IsExeceedingMaxWeight(item.GetWeight()) && items.Count < maxCarryCapacity)
                {
                    Debug.Log("Phase 2");


                    items?.Add(item);
                    InvokeOnItemAddedEvent(item);
                    isAdded = true;
                }
            }
        }

        public void RemoveItem(EquippableItem item)
        {
            if (items.Contains(item) && item != null)
            {
                items.Remove(item); 
                InvokeOnRemoveItemEvent(item);
            }
        }

        private void SetItemParent()
        {

        }

        private void EquipItem(int index)
        {

        }

        private void OnDisable()
        {
           InvokeStateChangedEvent(false);
        }

        private void OnEnable()
        {
           InvokeStateChangedEvent(true);
        }
    }

}
