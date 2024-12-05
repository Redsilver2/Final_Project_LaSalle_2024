using Redsilver2.Core.Counters;
using Redsilver2.Core.Events;
using Redsilver2.Core.Items;
using Redsilver2.Core.Motion;
using Redsilver2.Core.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Redsilver2.Core.Player
{
    [RequireComponent(typeof(InteractionManager))]
    public class PlayerInventory : PlayerInventoryEvents
    {
        [SerializeField] private ItemInspection itemInspection;

        [Space]
        [SerializeField] private PlayerHandMotionHandler lightSourceMotionHandler;
        [SerializeField] private PlayerHandMotionHandler itemMotionHandler;

        [Space]
        [SerializeField] private AudioSource lightAudioSource;
        [SerializeField] private AudioSource itemAudioSource;

        [Space]
        [SerializeField] private EquippableItem[] starterItems;
        [SerializeField] private int starterItemIndex = 0;

        [Space]
        [SerializeField] private int maxCarryCapacity = 5;

        private int  currentEquippedItemIndex = -1;
        private bool canSwitchItem = true;

        private PlayerControls.InventoryActions controls;

        private LightSourceItem equippedLightSource = null;
        private EquippableItem  equippedItem            = null;

        private List<EquippableItem> items;
        public AudioSource LightAudioSource => lightAudioSource;
        public AudioSource ItemAudioSource => itemAudioSource;

        public PlayerHandMotionHandler LightSourceItemMotionHandler => lightSourceMotionHandler;
        public PlayerHandMotionHandler ItemMotionHandler => itemMotionHandler;

        public LightSourceItem EquippedLightSource => equippedLightSource;
        public EquippableItem EquippedItem => equippedItem;

        protected override void Awake()
        {
            base.Awake();

            PlayerController player = PlayerController.Instance;
            controls = GameManager.Instance.GetComponent<InputManager>().PlayerControls.Inventory;
            items         = new List<EquippableItem>();

            player.AddOnStateChangedEvent(isEnabled =>
            {
                enabled = isEnabled;

                if(!isEnabled)
                {
                    equippedLightSource?.UnEquip();
                    equippedItem?.UnEquip();
                }
                else
                {
                    if(equippedLightSource != null)
                    {
                        equippedLightSource.gameObject.SetActive(true);
                        equippedLightSource.Equip();
                    }

                    if (equippedItem != null)
                    {
                        equippedItem.gameObject.SetActive(true);
                        equippedItem.Equip();
                    }
                }
            });

            itemInspection.Init();

 

            AddOnItemRemovedEvent((item, inventory, canDrop) =>
            {
                if (items.Contains(item))
                {
                    items.Remove(item);
                }

                if(item == equippedItem)
                {
                    equippedItem = null;
                }
            });

            controls.Slot01.performed += OnSlot01InputPerformed;
            controls.Slot02.performed += OnSlot02InputPerformed;
            controls.Slot03.performed += OnSlot03InputPerformed;
            controls.Slot04.performed += OnSlot04InputPerformed;
            controls.Drop.performed   += OnDropItemInputPerformed;
            controls.Inspect.performed += OnInspectInputPerformed;

            StartCoroutine(SetStarterItems());
        }

        public void AddItemInpesctionObject(GameObject gameObject)
        {
            itemInspection.SetObjectInspectionParent(gameObject);
        }

        public void AddItem(EquippableItem item)
        {
            InvokeOnItemAddedEvent(this, item);
        }

        public void RemoveItem(EquippableItem item, bool canDrop)
        {
            InvokeOnRemoveItemEvent(this, item, canDrop);
        }

        public bool Contains(EquippableItem item)
        {
            return items.Contains(item);
        }

        public void AddEquippableItemToItemsList(EquippableItem item)
        {
            items?.Add(item);
        }

        public EquippableItem[] GetEquippableItems()
        {
            if (items.Count > 0)
            {
                return items.ToArray();
            }

            return new EquippableItem [0];
        }
        public bool IsFull() => items.Count == maxCarryCapacity;

        public void SetEquippedLightSource(LightSourceItem equippedLightSource)
        {
            this.equippedLightSource = equippedLightSource;
        }

        public void SetEquippedItem(EquippableItem equippedItem)
        {
            this.equippedItem = equippedItem;
        }

        public int GetItemIndex(Item item)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if(items[i] == item)
                {
                    return i;
                }
            }

            return -1;
        }

        public void ChangeEquippedItem(uint index)
        {
            if (index < items.Count && index != currentEquippedItemIndex && canSwitchItem)
            {
                currentEquippedItemIndex = (int)index;
                StartCoroutine(ChangeItemCoroutine(currentEquippedItemIndex));
            }
        }
        private IEnumerator ChangeItemCoroutine(int index)
        {
            float waitTime;
            EquippableItem nextEquippedItem = items[index];
            canSwitchItem = false; 

            if (equippedItem != null) 
            {
                canSwitchItem = false;
                waitTime      = equippedItem.GetAnimationLenght("Hide");
                equippedItem.UnEquip();

                yield return Counter.WaitForSeconds(waitTime);
              
                equippedItem.gameObject.SetActive(false);
                equippedItem = null;
            }

            if (nextEquippedItem != null)
            {
                equippedItem = nextEquippedItem;
                equippedItem.gameObject.SetActive(true);

                waitTime = equippedItem.GetAnimationLenght("Show");
                equippedItem.Equip();
                yield return Counter.WaitForSeconds(waitTime);
            }

            canSwitchItem = true;
        }

        private IEnumerator SetStarterItems()
        {
            foreach (EquippableItem item in starterItems)
            {
                while (!item.didAwake) { yield return null; }
                item.Interact();
            }
        }

        private void OnDropItemInputPerformed(InputAction.CallbackContext context)
        {
            if (context.performed && equippedItem != null)
            {
                RemoveItem(equippedItem, true);
                equippedItem = null;
            }
        }

        private void OnSlot01InputPerformed(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                ChangeEquippedItem(0);
            }
        }
        private void OnSlot02InputPerformed(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                ChangeEquippedItem(1);
            }
        }

        private void OnSlot03InputPerformed(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                ChangeEquippedItem(2);
            }
        }
        private void OnSlot04InputPerformed(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                ChangeEquippedItem(3);
            }
        }
        private void OnInspectInputPerformed(InputAction.CallbackContext context)
        {
            if (context.performed && canSwitchItem)
            {
                if (equippedItem != null)
                {
                    itemInspection.Enable(this, equippedItem);
                }
            }
        }


        private void OnLoadSingleSceneEvent(int levelIndex)
        {
            controls.Slot01.performed -= OnSlot01InputPerformed;
            controls.Slot02.performed -= OnSlot02InputPerformed;
            controls.Slot03.performed -= OnSlot03InputPerformed;
            controls.Slot04.performed -= OnSlot04InputPerformed;
            controls.Drop.performed   -= OnDropItemInputPerformed;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SceneLoaderManager.AddOnLoadSingleSceneEvent(OnLoadSingleSceneEvent);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            SceneLoaderManager.RemoveOnLoadSingleSceneEvent(OnLoadSingleSceneEvent);
        }
    }

}
