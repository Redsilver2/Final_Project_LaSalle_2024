using Redsilver2.Core.Items;
using Redsilver2.Core.Player;
using UnityEngine.Events;

namespace Redsilver2.Core.Events
{
    public abstract class PlayerInventoryEvents : GameObjectEvents
    {
        private UnityEvent<EquippableItem, int> onEquippedItemChanged;
        private UnityEvent<EquippableItem, PlayerInventory>      onItemAdded;
        private UnityEvent<EquippableItem, PlayerInventory, bool>      onItemRemoved;


        protected override void Awake()
        {
            onEquippedItemChanged = new UnityEvent<EquippableItem, int>();
            onItemAdded           = new UnityEvent<EquippableItem, PlayerInventory>();
            onItemRemoved         = new UnityEvent<EquippableItem, PlayerInventory, bool>();

            base.Awake();
        }

        public void AddOnEquippedItemChangedEvent(UnityAction<EquippableItem, int> action)
        {
            onEquippedItemChanged?.AddListener(action);
        }
        public void RemoveOnEquippedItemChangedEvent(UnityAction<EquippableItem, int> action)
        {
            onEquippedItemChanged?.AddListener(action);
        }

        public void AddOnItemAddedEvent(UnityAction<EquippableItem, PlayerInventory> action)
        {
            onItemAdded?.AddListener(action);
        }
        public void RemoveOnItemAddedEvent(UnityAction<EquippableItem, PlayerInventory> action)
        {
            onItemAdded?.AddListener(action);
        }

        public void AddOnItemRemovedEvent(UnityAction<EquippableItem, PlayerInventory, bool> action)
        {
            onItemRemoved?.AddListener(action);
        }
        public void RemoveOnItemRemovedEvent(UnityAction<EquippableItem, PlayerInventory, bool> action)
        {
            onItemRemoved?.AddListener(action);
        }

        protected void InvokeOnEquippedItemChangedEvent(EquippableItem equippable, int index)
        {
            onEquippedItemChanged?.Invoke(equippable, index);
        }
        protected void InvokeOnItemAddedEvent(PlayerInventory inventory, EquippableItem equippable)
        {
            if (equippable != null)
            {
                onItemAdded?.Invoke(equippable, inventory);
            }
        }
        protected void InvokeOnRemoveItemEvent(PlayerInventory inventory, EquippableItem equippable, bool canDrop)
        {
            onItemRemoved?.Invoke(equippable, inventory, canDrop);
        }
    }
}
