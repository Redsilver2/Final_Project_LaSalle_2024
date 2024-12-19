using Redsilver2.Core.Items;
using Redsilver2.Core.Player;
using UnityEngine;

namespace Redsilver2.Core.Interactables
{
    public class MissingPuzzleComponent : Interactable
    {
        [Space]
        [SerializeField] private string missingPuzzleItemName = string.Empty;

        [Space]
        [SerializeField] private Transform  missingPuzzleParent;
        [SerializeField] private Vector3    missingPuzzlePosition;
        [SerializeField] private Quaternion missingPuzzleRotation;
        
        private PlayerInventory         inventory;
        private EquippableItem           missingPuzzleItem;

        protected override void Awake()
        {
            Collider collider = GetComponent<Collider>();   

            base.Awake();
            inventory = Camera.main.GetComponent<PlayerInventory>();

            if (collider == null) return; 
            AddOnInteractEvent(isInteracting =>
            {
                if (!isInteracting)
                {
                    EquippableItem[] equippableItems = inventory.GetEquippableItems();

                    if (equippableItems != null)
                    {
                        foreach (EquippableItem equippableItem in equippableItems)
                        {
                            if (equippableItem == null) continue;

                            if (equippableItem.InteractableName.ToLower().Contains(missingPuzzleItemName.ToLower()))
                            {
                                Transform transform = equippableItem.transform;
                                inventory.RemoveItem(equippableItem, false);
                                missingPuzzleItem = equippableItem;

                                transform.SetParent(missingPuzzleParent);
                                transform.SetLocalPositionAndRotation(missingPuzzlePosition, missingPuzzleRotation);

                                collider.enabled = false;
                                break;
                            }
                            else
                            {
                                this.isInteracting = false;
                            }
                        }
                    }
                    else
                    {
                        if (missingPuzzleItem != null)
                        {
                            inventory.AddItem(missingPuzzleItem);
                            collider.enabled = true;
                        }
                    }
                }
            });
        }
    }
}
