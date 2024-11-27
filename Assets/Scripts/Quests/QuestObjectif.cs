using UnityEngine;
using UnityEngine.Events;

namespace Redsilver2.Core.Quests
{
    public abstract class QuestObjectif : MonoBehaviour
    {
        [SerializeField] private string questObjectifName;
        [SerializeField] private QuestObjectifData [] questObjectifDatas;
        private int[] questsObjectifCountTrackers;

        private bool isCompleted = false;

        private UnityEvent onObjectifCountChanged;
        private UnityEvent onCompleted;

        private void Start()
        {
            questsObjectifCountTrackers = new int[questsObjectifCountTrackers.Length];
        }

        public void Complete(string objectifName)
        {
            int objectifsCompleted = 0;

            for(int i = 0; i < questObjectifDatas.Length; i++)
            {
                QuestObjectifData objectifData = questObjectifDatas[i];
                objectifData.Complete(objectifName, ref questsObjectifCountTrackers[i], out bool isCompleted);

                if (isCompleted)
                {
                    objectifsCompleted++;
                }    
            }

            if(objectifsCompleted == questObjectifDatas.Length - 1)
            {
                onCompleted.Invoke();
            }
        }

        public string GetDetails()
        {
            string result = $"{questObjectifName}\n\n";

            for (int i = 0; i < questObjectifDatas.Length; i++)
            {
                QuestObjectifData objectifData = questObjectifDatas[i];

                if(objectifData == null)
                {
                    continue;
                }

                result += $"- {objectifData.QuestObjectifItemName}";

                if(objectifData.MaxQuestObjectifCount > 1)
                {
                    result += $"({questsObjectifCountTrackers[i]}/{objectifData.MaxQuestObjectifCount})";
                }

                result += "\n";
            }

            return result;
        }
    }
}
