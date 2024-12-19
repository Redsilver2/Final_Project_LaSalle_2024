using Redsilver2.Core.Counters;
using Redsilver2.Core.SceneManagement;
using Redsilver2.Core.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Redsilver2.Core.Quests
{
    public class QuestManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI questDisplayer;
        [SerializeField] private float questNameSize = 50;

        private bool isShowingQuest = false;
        private bool isShowingAllQuests = false;

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

            actifQuests = new List<Quest>();
            startQuestCoroutines = new Queue<IEnumerator>();

            SceneLoaderManager.AddOnLoadSingleSceneEvent(OnLoadSingleSceneEvent);
        }

        private void Start()
        {
            if (questDisplayer != null)
            {
                questDisplayer.text = string.Empty;
                questDisplayer.canvasRenderer.SetAlpha(0);
            }
        }

        private void OnLoadSingleSceneEvent(int levelIndex)
        {
            CanvasRenderer canvasRenderer = questDisplayer.canvasRenderer;

            isShowingQuest = false;
            isShowingAllQuests = false;

            StopAllCoroutines();

            if (canvasRenderer != null) canvasRenderer.SetAlpha(0f);
            foreach (Quest quest in actifQuests) quest.Reset();
            foreach (Quest quest in mainQuests)  quest.Reset();

            startQuestCoroutines.Clear();
            actifQuests.Clear();    
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
                yield return canvasRenderer.Fade(true, 2f);
                yield return Counter.WaitForSeconds(3f);
                yield return canvasRenderer.Fade(false, 2f);
            }
        }

        public void UpdateActifQuestsDisplayer(Quest quest)
        {
            if (questDisplayer != null)
            {
                questDisplayer.text = quest.GetString(questNameSize);
            }
        }

        public void StartMainQuest(int index, float waitTime) 
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

                StartQuest(mainQuests[index], waitTime);
            }
        }

        public Quest GetMainQuest(string questName)
        {
            return mainQuests.Where(x => x.QuestName.ToLower().Contains(questName.ToLower())).First();
        }

        public void StartQuest(Quest quest, float waitTime)
        {
            if (startQuestCoroutines.Count > 0)
            {
                waitTime = 0f;
            }

            if (quest != null && !actifQuests.Contains(quest))
            {
                startQuestCoroutines.Enqueue(StartQuestCoroutine(quest, waitTime));

                if (startQuestCoroutines.Count == 1 && isShowingAllQuests != true)
                {
                    StartCoroutine(startQuestCoroutines.Dequeue());
                }
            }
        }

        private IEnumerator StartQuestCoroutine(Quest quest, float waitTime)
        {
            isShowingAllQuests = true;
            yield return Counter.WaitForSeconds(waitTime);

            quest.Enable(this);
            actifQuests.Add(quest);

            if (startQuestCoroutines.Count > 0)
            {
                Debug.LogWarning("3");
                StartCoroutine(startQuestCoroutines.Dequeue());
            }
            else
            {
                Debug.LogWarning("4");
                isShowingAllQuests = false;
            }
        }

        public bool ContainsQuest(Quest quest) => actifQuests.Contains(quest);
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
    }
}
