using UnityEngine;

namespace Redsilver2.Core.Quests
{
    [CreateAssetMenu(fileName = "New Quest Objectif", menuName = "Quest Objectif")]
    public class QuestObjectifData : ScriptableObject
    {
        [SerializeField] private string questObjectifDescription;
        [SerializeField] private string questObjectifItemName;
        [SerializeField] private int    maxQuestObjectifCount;

        public string QuestObjectifDescription => questObjectifDescription;
        public string QuestObjectifItemName    => questObjectifItemName;
        public int    MaxQuestObjectifCount    => maxQuestObjectifCount;


        public void Complete(string itemName, ref int objectifCount, out bool isCompleted)
        {
            if(itemName.ToLower() == questObjectifItemName.ToLower())
            {
                objectifCount++;

                if(objectifCount > maxQuestObjectifCount)
                {
                    objectifCount = maxQuestObjectifCount;
                }

                isCompleted = true;
            }
            else
            {
                isCompleted = false;
            }
        }

    }
}
