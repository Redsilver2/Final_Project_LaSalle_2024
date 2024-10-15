using Redsilver2.Core.Items;
using UnityEngine.Events;

namespace Redsilver2.Core.Events
{
    public abstract class PlayerInventoryEvents : GameObjectEvents
    {
        private UnityEvent<EquippableItem, int> onEquippedItemChanged;
        private UnityEvent<EquippableItem>      onItemAdded;
        private UnityEvent<EquippableItem>      onItemRemoved;


        protected override void Start()
        {
            onEquippedItemChanged = new UnityEvent<EquippableItem, int>();
            onItemAdded           = new UnityEvent<EquippableItem>();
            onItemRemoved         = new UnityEvent<EquippableItem>();

            base.Start();
        }

        public void AddOnEquippedItemChangedEvent(UnityAction<EquippableItem, int> action)
        {
            onEquippedItemChanged?.AddListener(action);
        }
        public void RemoveOnEquippedItemChangedEvent(UnityAction<EquippableItem, int> action)
        {
            onEquippedItemChanged?.AddListener(action);
        }

        public void AddOnItemAddedEvent(UnityAction<EquippableItem> action)
        {
            onItemAdded?.AddListener(action);
        }
        public void RemoveOnItemAddedEvent(UnityAction<EquippableItem> action)
        {
            onItemAdded?.AddListener(action);
        }

        public void AddOnItemRemovedEvent(UnityAction<EquippableItem> action)
        {
            onItemRemoved?.AddListener(action);
        }
        public void RemoveOnItemRemovedEvent(UnityAction<EquippableItem> action)
        {
            onItemRemoved?.AddListener(action);
        }

        protected void InvokeOnEquippedItemChangedEvent(EquippableItem equippable, int index)
        {
            onEquippedItemChanged?.Invoke(equippable, index);
        }
        protected void InvokeOnItemAddedEvent(EquippableItem equippable)
        {
            if (equippable != null)
            {
                onItemAdded?.Invoke(equippable);
            }
        }
        protected void InvokeOnRemoveItemEvent(EquippableItem equippable)
        {
            onItemRemoved?.Invoke(equippable);
        }
    }
}
