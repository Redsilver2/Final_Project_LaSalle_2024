using Redsilver2.Core.Counters;
using Redsilver2.Core.UI;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Redsilver2.Core.Subtitles
{
    public class SubtitleManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI subtitleDisplayer;
        [SerializeField] private Subtitle test;

        [Space]
        [SerializeField][Range(0.1f, 1f)] private float textReadSpeed              = 1.0f;
        [SerializeField][Range(4f, 9f)] private float   maxVisibilityTimeThreshold = 4f;

        private string currentSubtitleDisplayerText;
        private bool canDisplayCharacterName = true;

        private IEnumerator writeSubtitleCoroutine;
        private IEnumerator playSubtitleCoroutine;
        private IEnumerator visibilitySubtitleCoroutine;

        public static SubtitleManager Instance { get; private set; }

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
            //subtitleDisplayer.canvasRenderer.SetAlpha(0f);
           // PlaySubtitle(test);
        }

        public void PlaySubtitle(Subtitle subtitle)
        {
            PlaySubtitle(subtitle, 0);
        }

        public void PlaySubtitle(Subtitle subtitle, int starterIndex)
        {
            if (subtitle != null)
            {
                StopSubtitle();
                playSubtitleCoroutine = PlaySubtitleCoroutine(subtitle, starterIndex);
                StartCoroutine(playSubtitleCoroutine);
            }
        }

        private void LerpSubtitleVisibility(float duration, bool isVisible)
        {
            if (visibilitySubtitleCoroutine != null)
            {
                StopCoroutine (visibilitySubtitleCoroutine);    
            }

            visibilitySubtitleCoroutine = subtitleDisplayer.canvasRenderer.FadeCanvasRenderer(isVisible, duration);
            StartCoroutine(visibilitySubtitleCoroutine);
        }


        public void WriteSubtitle(string characterName, string text, float duration)
        {
            if (writeSubtitleCoroutine != null)
            {
               StopCoroutine(writeSubtitleCoroutine);    
            }

            writeSubtitleCoroutine = WriteSubtitleCoroutine(characterName, text, duration);
            StartCoroutine(writeSubtitleCoroutine);
        }

        private void StopSubtitle()
        {
            if (visibilitySubtitleCoroutine != null)
            {
                StopCoroutine(visibilitySubtitleCoroutine);
                visibilitySubtitleCoroutine = null;
            }


            if (writeSubtitleCoroutine != null)
            {
                StopCoroutine(writeSubtitleCoroutine);
                writeSubtitleCoroutine = null;
            }

            if (playSubtitleCoroutine != null)
            {
                StopCoroutine(playSubtitleCoroutine);
                playSubtitleCoroutine = null;
            }
        }

        private IEnumerator PlaySubtitleCoroutine(Subtitle subtitle, int starterIndex)
        {
            SubtitleData[] datas         = subtitle.datas;     
            Timer timer = new Timer();

            if(starterIndex < 0)
            {
                starterIndex = 0;
            }
            else if(starterIndex >= datas.Length)
            {
                starterIndex = datas.Length - 1;
            }

            if(starterIndex > 0)
            {
                timer.Start(datas[starterIndex].StartTime);
            }
            else
            {
                timer.Start();
            }

            LerpSubtitleVisibility(0.1f, false);

            for (int i = starterIndex; i < datas.Length; i++)
            {
                SubtitleData selectedData = datas[i];
                int nextIndex             = i + 1;

                yield return Timer.WaitTimer(timer, selectedData.StartTime);

                WriteSubtitle(selectedData.CharacterName, selectedData.Context, selectedData.Duration);
                LerpSubtitleVisibility(0.75f, true);

                if (nextIndex < datas.Length)
                {
                    SubtitleData nextData = datas[nextIndex];
                    float        waitTime = nextData.StartTime - selectedData.EndTime;

                    if (waitTime >= maxVisibilityTimeThreshold)
                    {
                        yield return Counter.WaitForSeconds(maxVisibilityTimeThreshold / 2f);
                        LerpSubtitleVisibility(0.75f, false);
                    }
                }
            }

            while (!currentSubtitleDisplayerText.Contains(datas[datas.Length - 1].Context))
            {
                yield return null;
            }

            yield return Counter.WaitForSeconds(3f);
            LerpSubtitleVisibility(0.75f, false);

            if(subtitleDisplayer != null)
            {
                while (subtitleDisplayer.canvasRenderer.GetAlpha() != 0f)
                {
                    yield return null;
                }

                subtitleDisplayer.text = string.Empty;
            }
        }

        private IEnumerator WriteSubtitleCoroutine(string characterName, string text, float duration)
        {
            if(text != string.Empty && subtitleDisplayer != null)
            {
                string characterNameText;
                float  waitTime = duration / text.Length;

                currentSubtitleDisplayerText = string.Empty;

                foreach (char c in text.ToCharArray())
                {
                    if(characterName == string.Empty || !canDisplayCharacterName)
                    {
                        characterNameText = string.Empty;
                    }
                    else
                    {
                        characterNameText = characterName + ": ";
                    }

                    currentSubtitleDisplayerText += c;
                    subtitleDisplayer.text = characterNameText + currentSubtitleDisplayerText;
                    yield return Counter.WaitForSeconds(waitTime * textReadSpeed);
                }
            }
        }

        public void SetSubtitleVisility(bool isVisible)
        {
            if(subtitleDisplayer != null)
            {
                subtitleDisplayer.gameObject.SetActive(isVisible);
            }
        }
        public void SetSubtitleNameVisility(bool isVisible)
        {
            canDisplayCharacterName = isVisible;
        }
    }        
}