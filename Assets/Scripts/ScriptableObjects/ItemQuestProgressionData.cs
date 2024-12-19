using Redsilver2.Core.Items;
using UnityEngine;

namespace Redsilver2.Core.Quests
{
    [CreateAssetMenu(menuName = "Quest Progression/Item", fileName = "New Item Quest Progression Data")]
    public class ItemQuestProgressionData : QuestProgressionData
    {
        [SerializeField] private ItemQuestProgressionType questProgressionType;

        public override void Enable(QuestProgression progression)
        {
            base.Enable(progression);
            Debug.LogWarning("Quest Event Added");

            switch (questProgressionType)
            {
                case ItemQuestProgressionType.OnInteractOnce:
                    Item.AddOnItemInteractOnceEvent(OnItemInteractEvent);
                    break;

                case ItemQuestProgressionType.OnInteract:
                    Item.AddOnItemInteractOnceEvent(OnItemInteractEvent);
                    break;

                case ItemQuestProgressionType.OnFireWeapon:
                    RangedWeapon.AddOnFireWeaponEvent(OnWeaponEvent);
                    break;

                case ItemQuestProgressionType.OnReloadWeapon:
                    RangedWeapon.AddOnReloadWeaponEvent(OnWeaponEvent);
                    break;
            }
        }

        public override void Disable()
        {
            switch (questProgressionType)
            {
                case ItemQuestProgressionType.OnInteractOnce:
                    Item.RemoveOnItemInteractOnceEvent(OnItemInteractEvent);
                    break;

                case ItemQuestProgressionType.OnInteract:
                    Item.AddOnItemInteractOnceEvent(OnItemInteractEvent);
                    break;

                case ItemQuestProgressionType.OnFireWeapon:
                    RangedWeapon.RemoveOnFireWeaponEvent(OnWeaponEvent);
                    break;

                case ItemQuestProgressionType.OnReloadWeapon:
                    RangedWeapon.RemoveOnReloadWeaponEvent(OnWeaponEvent);
                    break;
            }

            base.Disable(); 
        } 

        private void OnItemInteractEvent(Item item)
        {
            Debug.LogWarning($"{item.InteractableName.ToLower()} && {questProgressionObjectName.ToLower()} && {questProgression}");

            if (item.InteractableName.ToLower() == questProgressionObjectName.ToLower() && questProgression != null)
            {
                questProgression.Progress(1f);
            }
        }

        private void OnWeaponEvent(RangedWeapon weapon)
        {
            if (weapon.InteractableName.ToLower() == questProgressionObjectName.ToLower() && questProgression != null)
            {
                Debug.LogWarning(weapon.InteractableName);
                questProgression.Progress(1f);
            }
        }
    }
}
