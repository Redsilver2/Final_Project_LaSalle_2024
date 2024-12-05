using UnityEngine;
using UnityEngine.Events;

namespace Redsilver2.Core.Quests
{
    [System.Serializable]
    public class QuestProgression 
    {
        [SerializeField] private QuestProgressionData progressionData;
        [SerializeField] private UnityEvent<float>    onQuestProgressionUpdated;
        private float progressionValue;

        public QuestProgressionData ProgressionData => progressionData;

        public void Reset()
        {
            progressionValue = 0;
        }

        public void Progress(float progressionIncrement)
        {
            if (progressionData != null && !IsDone())
            {
                progressionValue += progressionIncrement;
                Debug.LogWarning("?????? " + progressionValue);
                onQuestProgressionUpdated.Invoke(progressionValue / progressionData.MaxProgressionValue);
            }
        }

        public string GetDescription()
        {
            if(progressionData == null)
            {
                return string.Empty;
            }

            string color = "white";

            if(progressionData.MaxProgressionValue == 0f || progressionValue / progressionData.MaxProgressionValue >= 1f)
            {
                color = "green";
            }

            return $"<color={color}>- {progressionData.GetDescription(Mathf.FloorToInt(progressionValue))}</color>\n";
        }

        public bool IsDone()
        {
            if(progressionData != null)
            {
                if(progressionValue == progressionData.MaxProgressionValue)
                {
                    return true;
                }
            }

            return false;
        }

        public void AddOnQuestProgressionUpdatedEvent(UnityAction<float> action)
        {
            onQuestProgressionUpdated.AddListener(action);
        }
        public void RemoveOnQuestProgressionUpdatedEvent(UnityAction<float> action)
        {
            onQuestProgressionUpdated.RemoveListener(action);
        }
    }
}