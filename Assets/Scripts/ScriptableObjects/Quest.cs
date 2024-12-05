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

        [SerializeField] private UnityEvent      onQuestStarted;
        [SerializeField] private UnityEvent      onQuestFinished;

        private bool isActivated = false;
        private bool isCompleted = false;

        public string QuestName => questName;

        public void Reset()
        {
            onQuestStarted  = new UnityEvent();
            onQuestFinished = new UnityEvent();
            isActivated = false;
            isCompleted = false;

            foreach (QuestProgression _progression in questsProgressions)
            {
                _progression.Reset();
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
                if (onQuestStarted == null)
                {
                    onQuestStarted = new UnityEvent();
                }

                if (onQuestFinished == null)
                {

                }

                onQuestStarted.AddListener(() =>
                {
                    isActivated = true;

                    foreach(QuestProgression _progression in questsProgressions)    
                    {
                        QuestProgressionData progressionData = _progression.ProgressionData;

                        if (_progression != null)
                        {
                            progressionData.Enable(_progression);
                            _progression.AddOnQuestProgressionUpdatedEvent(value =>
                            {
                                if (questManager != null)
                                {
                                    questManager.UpdateActifQuestsDisplayer();
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
                        questManager.AddActifQuest(this);
                        questManager.UpdateActifQuestsDisplayer();
                        questManager.ShowcaseQuests();
                    }
                });


                onQuestFinished.AddListener(() =>
                {
                    if (questManager != null)
                    {
                        Debug.LogWarning("Wtf lol");
                        questManager.UpdateActifQuestsDisplayer();
                        questManager.ShowcaseQuests();
                        questManager.RemoveActifQuest(this);
                        
                        foreach(QuestProgressionData data in questsProgressions.Select(x => x.ProgressionData).ToArray())
                        {
                            data.Disable();
                        }
                    }
                });



                Debug.LogWarning("Started 1");
                onQuestStarted.Invoke();
            }
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
