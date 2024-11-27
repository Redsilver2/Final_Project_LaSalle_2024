using Redsilver2.Core.Counters;
using Redsilver2.Core.SceneManagement;
using Redsilver2.Core.UI;
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

        [Space]
        [SerializeField] private Quest[] mainQuests;

        private IEnumerator questCoroutine;


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

            SceneLoaderManager.AddOnSingleLevelLoadedEvent(OnSingleSceneLoadedEvent);
            SceneLoaderManager.AddOnLoadSingleSceneEvent(OnLoadSingleSceneEvent);
        }

        private void OnSingleSceneLoadedEvent(int levelIndex)
        {
            if (levelIndex != 0)
            {
                ActivateMainQuest(1f, 0);
            }
        }
        private void OnLoadSingleSceneEvent(int levelIndex)
        {
            FadeQuestShowcase();
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

                StartCoroutine(ActivateMainQuestCoroutine(waitTime, index));
            }
        }

        private IEnumerator ActivateMainQuestCoroutine(float waitTime, int index)
        {
            yield return Counter.WaitForSeconds(waitTime);
            mainQuests[index].Enable(this);
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
    }
}
