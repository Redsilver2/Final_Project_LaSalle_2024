using Redsilver2.Core.Counters;
using Redsilver2.Core.SceneManagement;
using Redsilver2.Core.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Redsilver2.Core.Quests
{
    public class QuestManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI questDisplayer;
        [SerializeField] private float questNameSize = 50;

        private bool isShowingQuests = false;

        [Space]
        [SerializeField] private Quest[] mainQuests;
        private IEnumerator questCoroutine;


        private Queue<IEnumerator> startQuestCoroutines;
        private List<Quest>        actifQuests;
        public static QuestManager Instance {  get; private set; }

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        private void Start()
        {
            actifQuests = new List<Quest>();

            if(questDisplayer != null)
            {
                questDisplayer.text = string.Empty;
                questDisplayer.canvasRenderer.SetAlpha(0);
            }

            startQuestCoroutines = new Queue<IEnumerator>();
        }

        private void OnSingleSceneLoadedEvent(int levelIndex)
        {
            StopAllCoroutines();
            FadeQuestShowcase();

            if (levelIndex == 0)
            {
                actifQuests.Clear();
            }
        }

        public void FadeQuestShowcase()
        {
            if (questCoroutine != null)
            {
                StopCoroutine(questCoroutine);
            }

            questCoroutine = questDisplayer.canvasRenderer.FadeCanvasRenderer(false, 2f);
            StartCoroutine(questCoroutine);
        }

        public void ShowcaseQuests()
        {
            if(questCoroutine != null)
            {
                StopCoroutine(questCoroutine);  
            }

            questCoroutine = ShowcaseQuestsCoroutine();
            StartCoroutine(questCoroutine); 
        }



        public IEnumerator ShowcaseQuestsCoroutine()
        {
            if (questDisplayer != null)
            {
                CanvasRenderer canvasRenderer = questDisplayer.canvasRenderer;
                yield return canvasRenderer.FadeCanvasRenderer(true, 2f);
                yield return Counter.WaitForSeconds(3f);
                yield return canvasRenderer.FadeCanvasRenderer(false, 2f);
            }
        }

        public void UpdateActifQuestsDisplayer()
        {
            if (questDisplayer != null)
            {
                questDisplayer.text = string.Empty;

                foreach (Quest quest in actifQuests)
                {
                    if (quest != null)
                    {
                        questDisplayer.text += "\n" + quest.GetString(questNameSize);
                    }
                }
            }
        }

        public void ActivateMainQuest(float waitTime, int index) 
        {
            if (mainQuests.Length > 0)
            {
                if (index < 0)
                {
                    index = 0;
                }
                else if (index >= mainQuests.Length)
                {
                    index = mainQuests.Length - 1;
                }

                StartCoroutine(StartQuestCoroutine(mainQuests[index], waitTime));
            }
        }

        public void StartQuest(Quest quest, float waitTime)
        {
            if (startQuestCoroutines.Count > 0)
            {
                waitTime = 0f;
            }

            startQuestCoroutines.Enqueue(StartQuestCoroutine(quest, waitTime));

            if (startQuestCoroutines.Count == 1 && isShowingQuests == false)
            {
                StartCoroutine(startQuestCoroutines.Dequeue());
            }
        }

        private IEnumerator StartQuestCoroutine(Quest quest, float waitTime)
        {
            isShowingQuests = true;

            if (quest != null && !actifQuests.Contains(quest))
            {
                yield return Counter.WaitForSeconds(waitTime);
                quest.Enable(this);

                if(startQuestCoroutines.Count > 0)
                {
                    StartCoroutine(startQuestCoroutines.Dequeue());
                }
                else
                {
                    isShowingQuests = false;
                }
            }
        }

        public void AddActifQuest(Quest quest)
        {
            if (quest != null && !ContainsActifQuest(quest))
            {
                actifQuests.Add(quest); 
            }
        }

        public void RemoveActifQuest(Quest quest)
        {
            if (quest != null && ContainsActifQuest(quest))
            {
                actifQuests.Remove(quest);
            }
        }

        private bool ContainsActifQuest(Quest quest)
        {
            bool result = false;

            foreach (Quest actifQuest in actifQuests)
            {
                if(actifQuest != null)
                {
                    if(actifQuest.QuestName == quest.QuestName)
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }

        private void OnEnable()
        {
            SceneLoaderManager.AddOnSingleLevelLoadedEvent(OnSingleSceneLoadedEvent);
        }

        private void OnDisable()
        {

            SceneLoaderManager.RemoveOnSingleLevelLoadedEvent(OnSingleSceneLoadedEvent);
        }
    }
}
