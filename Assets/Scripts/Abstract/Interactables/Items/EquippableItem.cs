using Redsilver2.Core.Player;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Redsilver2.Core.Items
{
    public abstract class EquippableItem : Item
    {
        [Space]
        [SerializeField] private float maxHidePositionY = 20f;
        [SerializeField] private float hideDuration     = 2f;

        private UnityEvent<bool> onHidingStarted;
        private UnityEvent<bool> onHidingCompleted;

        protected bool isEquipped = false;


        public float MaxHidePositionY => maxHidePositionY;
        public bool IsEquipped => isEquipped;

        public override void Interact()
        {
            if (inventory != null)
            {
                inventory.AddItem(this, out bool isAdded);
                Debug.Log("Is Added: " + isAdded);

                if (isAdded)
                {
                    base.Interact();
                }
            }
        }

        public virtual void Equip()
        {
            ChangeAnimation("Show_Flashlight", 0.2f);
        }

        public virtual void UnEquipped()
        {
            ChangeAnimation("Hide_Flashlight", 0.2f);
        }

        public void Drop()
        {
           inventory?.RemoveItem(this);
           transform.SetParent(null);
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
