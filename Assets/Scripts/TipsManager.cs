using Redsilver2.Core.Counters;
using Redsilver2.Core.SceneManagement;
using System.Collections;
using TMPro;
using UnityEngine;
namespace Redsilver2.Core
{
    public class TipsManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI tipDisplayer;
        [SerializeField] private string[] starterTips;

        private IEnumerator enumerator;
        public static TipsManager Instance { get; private set; }

        protected void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }

            SceneLoaderManager.AddOnSingleLevelLoadedEvent(levelIndex =>
            {
                if(levelIndex == 1)
                {
                    PlayTip(starterTips);
                }
                else if(levelIndex == 0)
                {
                    if (tipDisplayer != null) 
                    {
                        if (enumerator != null) StopCoroutine(enumerator);
                        tipDisplayer.canvasRenderer.SetAlpha(0f);
                    }
                }
            });

            if(SceneLoaderManager.SelectedSingleLevelIndex == 1)
            {
                PlayTip(starterTips);
            }

            if(tipDisplayer != null)
            {
                tipDisplayer.canvasRenderer.SetAlpha(0f);
            }
        }

        public void PlayTip(string contentToDisplay)
        {
            if (tipDisplayer != null) 
            {
                if(enumerator != null) StopCoroutine(enumerator);
                enumerator = TipCoroutine(contentToDisplay, tipDisplayer.canvasRenderer);
                StartCoroutine(enumerator);
            }
        }

        public void PlayTip(string[] contentToDisplay)
        {
            if (tipDisplayer != null)
            {
                if (enumerator != null) StopCoroutine(enumerator);
                enumerator = TipCoroutine(contentToDisplay, tipDisplayer.canvasRenderer);
                StartCoroutine(enumerator);
            }
        }


        private IEnumerator TipCoroutine(string[] contentToDisplay, CanvasRenderer renderer)
        {
            foreach(string s in contentToDisplay)
            {
                yield return TipCoroutine(s, renderer);
            }
        }

        private IEnumerator TipCoroutine(string contentToDisplay, CanvasRenderer renderer)
        {
            if(renderer.GetAlpha() != 0f)
            {
                yield return renderer.Fade(false, 0.5f);
            }

            tipDisplayer.text = $"<color=red>Gameplay Tip</color>\n{contentToDisplay}";

            yield return renderer.Fade(true, 1.5f);
            yield return Counter.WaitForSeconds(3f);
            yield return renderer.Fade(false, 1.5f);
        }
    }
}
