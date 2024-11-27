using Redsilver2.Core.Counters;
using Redsilver2.Core.Motion;
using Redsilver2.Core.Player;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Redsilver2.Core.Items
{
    [RequireComponent(typeof(Animator))]
    public abstract class EquippableItem : Item
    {
        [Space]
        [SerializeField] private Sprite pickupSprite;

        [Space]
        [SerializeField]  protected PlayerHandMotionSetting motionSetting;

        [Space]
        [SerializeField] private Quaternion dropRotation;
        [SerializeField] private float   dropRaycastLenght = 2f;
        [SerializeField] private float   landingPositionOffsetY = 0f;

        [Space]
        [SerializeField] protected AnimationController itemAnimationController;

        [Space]
        [SerializeField] private AudioClip equipClip;
        [SerializeField] private AudioClip unequipClip;
        [SerializeField] private AudioClip landClip;

        private IEnumerator dropCoroutine;
        private IEnumerator enableControlsCoroutine;

        protected bool isEquipped = false;
        protected bool isControlsSet = false;

        private UnityEvent<bool> onHidingStarted;
        private UnityEvent<bool> onHidingCompleted;

        private InputManager inputManager;


        public bool IsEquipped => isEquipped;
        public PlayerHandMotionSetting MotionSetting => motionSetting;

        protected override void Awake()
        {
            base.Awake();
            inputManager = GameManager.Instance.GetComponent<InputManager>();

            inventory.AddOnItemAddedEvent(OnItemAddedEvent);
            inventory.AddOnItemRemovedEvent(OnItemRemovedEvent);

            itemAnimationController.Init(gameObject);
            isControlsSet = false;
            Physics.IgnoreCollision(GetComponent<Collider>(), PlayerController.Instance.GetComponent<Collider>());
        }

        public override void Interact()
        {
            if (inventory != null)
            {
                if (!inventory.IsFull())
                {
                    StopDrop();
                    inventory.AddItem(this);
                    base.Interact();
                }
            }
        }

        public virtual void Equip()
        {
            PlaySound(equipClip);
            itemAnimationController.Enable();
            itemAnimationController.PlayAnimation("Show");

            enableControlsCoroutine = EnableControlsCoroutine();
            StartCoroutine(enableControlsCoroutine);

        }

        public virtual void UnEquip()
        {
            if (enableControlsCoroutine != null)
            {
                StopCoroutine(enableControlsCoroutine);
            }

            DisableControls();
            PlaySound(unequipClip);
            itemAnimationController.PlayAnimation("Hide");
        }

        public float GetAnimationLenght(string keyword) => itemAnimationController.GetAnimationLenght(keyword);
        
        protected virtual void Drop()
        {
            StopDrop();

            if(enableControlsCoroutine != null)
            {
               StopCoroutine(enableControlsCoroutine);
            }

            dropCoroutine = DropCoroutine();
            StartCoroutine(dropCoroutine);  
        }

        public void StopDrop()
        {
            if (enableControlsCoroutine != null)
            {
                StopCoroutine(enableControlsCoroutine);
            }

            if (dropCoroutine != null)
            {
                StopCoroutine(dropCoroutine);   
            }
        }

        private IEnumerator DropCoroutine()
        {
            Vector3 currentPosition = transform.position;

            DisableControls();
            itemAnimationController.Disable();

            transform.SetParent(null);
            transform.rotation = dropRotation;
            transform.position = currentPosition;

            while (true)
            {
                transform.position += Vector3.down * (weight / 4) * Time.deltaTime;
                Debug.DrawRay(transform.position, Vector3.down, Color.yellow, 10f);

                if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo, dropRaycastLenght, LayerMask.GetMask("Ground")) && hitInfo.collider != null)
                {
                    break;
                }

                yield return null;
            }

            PlaySound(landClip);
        }

        public override Sprite GetIcon()
        {
            return pickupSprite;
        }
       
        private IEnumerator EnableControlsCoroutine()
        {
            float duration = itemAnimationController.GetAnimationLenght("Show");
            yield return Counter.WaitForSeconds(duration);
            EnableControls();
        }

        public void EnableControls()
        {
            if (!isControlsSet)
            {
                isControlsSet = true;
                SetControls(inputManager.PlayerControls, true);
            }
        }
       
        public void DisableControls()
        {
            if (isControlsSet)
            {
                isControlsSet = false;
                SetControls(inputManager.PlayerControls, false);
            }
        }

        protected abstract void SetControls(PlayerControls controls, bool isSettingControls);

        protected virtual void OnItemAddedEvent(EquippableItem item, PlayerInventory inventory)
        {
            if (item == this && !inventory.Contains(this) && !inventory.IsFull())
            {
                PlayerHandMotionHandler itemMotionHandler = inventory.ItemMotionHandler;
                
                inventory.AddEquippableItemToItemsList(this);
                transform?.SetParent(itemMotionHandler.transform);

                if (inventory.EquippedItem == null)
                {
                    inventory.SetEquippedItem(this);

                    if (PlayerController.Instance.enabled == true)
                    {
                        itemMotionHandler.SethandMotionSetting(motionSetting);
                        SetSource(inventory.ItemAudioSource);
                        Equip();
                    }
                }
                else
                {
                    gameObject.SetActive(false);
                    transform.localRotation = Quaternion.identity;
                    transform.localPosition = Vector3.zero;
                }
            }
        }

        protected virtual void OnItemRemovedEvent(EquippableItem item, PlayerInventory inventory, bool canDrop)
        {
            if (item == this && canDrop)
            {
                item.Drop();
            }
        }



        public void AddOnHidingStartedEvent(UnityAction<bool> action)
        {
            onHidingStarted?.AddListener(action);
        }
        public void RemoveOnHidingStartedEvent(UnityAction<bool> action)
        {
            onHidingStarted?.RemoveListener(action);
        }
        public void AddOnHidingCompletedEvent(UnityAction<bool> action)
        {
            onHidingCompleted?.AddListener(action);
        }
        public void RemoveOnHidingCompletedEvent(UnityAction<bool> action)
        {
            onHidingCompleted?.RemoveListener(action);
        }
    }
}
