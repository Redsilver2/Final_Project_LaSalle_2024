using Redsilver2.Core.Saveable;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

namespace Redsilver2.Core.Quests
{
    [System.Serializable]
    public class Quest : ISaveable
    {
        [SerializeField] private string questName;
        [SerializeField] private QuestProgression[] questsProgressions;

        [SerializeField] private UnityEvent onQuestStarted = new UnityEvent();
        [SerializeField] private UnityEvent onQuestFinished = new UnityEvent();

        private bool isActivated = false;
        private bool isCompleted = false;

        public string QuestName => questName;

        public void Reset()
        {
            isActivated = false;
            isCompleted = false;

            foreach (QuestProgression _progression in questsProgressions)
            {
                _progression.Reset();
            }

            if (isActivated && !isCompleted)
            {
                foreach (QuestProgressionData data in questsProgressions.Select(x => x.ProgressionData).ToArray())
                {
                    data.Disable();
                }
            }
        }

        private bool IsDone()
        {
            bool result = true;

            for(int i = 0; i < questsProgressions.Length; ++i)
            {
                if (!questsProgressions[i].IsDone())
                {
                    result = false;
                    break;
                }
            }

            return result;
        }

        public string GetString(float questNameSize)
        {
            string result = $"<size={questNameSize}>{questName}</size>\n\n";

            for(int i = 0; i < questsProgressions.Length; i++)
            {
                result += questsProgressions[i].GetDescription();
            }

            return result;
        }

        public void Enable(QuestManager questManager)
        {
            if(!isActivated && !isCompleted && questManager != null)
            {
                onQuestStarted.AddListener(OnQuestStartedEvent(questManager));
                onQuestFinished.AddListener(OnQuestFinishedEvent(questManager));
                onQuestStarted.Invoke();
            }
        }

        private UnityAction OnQuestStartedEvent(QuestManager questManager)
        {
            return () =>
            {
                isActivated = true;

                foreach (QuestProgression _progression in questsProgressions)
                {
                    QuestProgressionData progressionData = _progression.ProgressionData;

                    if (_progression != null)
                    {
                        progressionData.Enable(_progression);
                        _progression.AddOnQuestProgressionUpdatedEvent(value =>
                        {
                            if (questManager != null)
                            {
                                questManager.UpdateActifQuestsDisplayer(this);
                                questManager.ShowcaseQuests();
                            }

                            if (_progression.IsDone() && !isCompleted)
                            {
                                if (IsDone())
                                {
                                    isCompleted = true;
                                    onQuestFinished.Invoke();
                                }
                            }
                        });
                    }
                }

                if (questManager != null)
                {
                    Debug.LogWarning("Started 2");
                    questManager.UpdateActifQuestsDisplayer(this);
                    questManager.ShowcaseQuests();
                }

            };
        }

        private UnityAction OnQuestFinishedEvent(QuestManager questManager)
        {
            return () =>
            {
                if (questManager != null)
                {
                    Debug.LogWarning("Wtf lol");
                    questManager.UpdateActifQuestsDisplayer(this);
                    questManager.ShowcaseQuests();
                    questManager.RemoveActifQuest(this);

                    foreach (QuestProgressionData data in questsProgressions.Select(x => x.ProgressionData).ToArray())
                    {
                        data.Disable();
                    }
                }
            };
        }

        public void Save()
        {

        }

        public void Load()
        {

        }

        public void AddOnQuestStartedEvent(UnityAction action)
        {
            onQuestStarted.AddListener(action);
        }
        public void AddOnQuestFinishedEvent(UnityAction action)
        {
            onQuestFinished.AddListener(action);
        }

        public void RemoveOnQuestStartedEvent(UnityAction action)
        {
            onQuestStarted.RemoveListener(action);
        }
        public void RemoveOnQuestFinishedEvent(UnityAction action)
        {
            onQuestFinished.RemoveListener(action);
        }
    }
}
