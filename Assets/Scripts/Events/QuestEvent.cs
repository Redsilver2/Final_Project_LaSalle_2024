using Redsilver2.Core.Quests;
using UnityEngine;

namespace Redsilver2.Core.Events 
{
    [CreateAssetMenu(menuName = "Events/Quest", fileName = "New Quest")]
    public class QuestEvent : GameEvent
    {
        [SerializeField] private Quest quest;
        [SerializeField] private float waitTime;
        public override void Execute()
        {
            QuestManager questManager = QuestManager.Instance;

            if (questManager != null) 
            {
                questManager.StartQuest(quest, waitTime);
            }
        }
    }

}
