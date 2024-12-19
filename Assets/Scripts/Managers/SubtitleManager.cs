using Redsilver2.Core.Counters;
using Redsilver2.Core.SceneManagement;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Redsilver2.Core.Subtitles
{
    public class SubtitleManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI subtitleDisplayer;

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
            if (subtitleDisplayer != null)
            {
                subtitleDisplayer.canvasRenderer.SetAlpha(0f);
            }
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

            visibilitySubtitleCoroutine = subtitleDisplayer.canvasRenderer.Fade(isVisible, duration);
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

        private void OnLevelLoadedEvent(int sceneIndex)
        {
            StopSubtitle();

            if (subtitleDisplayer != null)
            {
                subtitleDisplayer.text = string.Empty;
            }

            SetSubtitleVisility(false);
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

            SetSubtitleVisility(true);
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
                // text = Regex.Replace(text, "<.*?>", string.Empty); // Remove all tags.

                char[] chars = text.ToCharArray();

                for(int i = 0; i < chars.Length; i++) 
                {
                    char c = chars[i];  

                    if(characterName == string.Empty || !canDisplayCharacterName)
                    {
                        characterNameText = string.Empty;
                    }
                    else
                    {
                        characterNameText = characterName + ": ";
                    }

                    if (c == '<')
                    {
                        currentSubtitleDisplayerText += RemoveRichTextTag(chars, ref i);
                    }
                    else
                    {
                        currentSubtitleDisplayerText += c;
                        subtitleDisplayer.text = characterNameText + currentSubtitleDisplayerText;
                        yield return Counter.WaitForSeconds(waitTime * textReadSpeed);
                    }
                }
            }
        }

        private string RemoveRichTextTag(char[] chars, ref int currentIndex)
        {
            string result = string.Empty;

            for (int i = currentIndex; i < chars.Length; i++)
            {
                char c = chars[i];
                result += c;

                if (c == '>')
                {
                    currentIndex = i;
                    break;
                }
            }

            return result;
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

        private void OnEnable()
        {
            SceneLoaderManager.AddOnSingleLevelLoadedEvent(OnLevelLoadedEvent);
        }

        private void OnDisable()
        {
            SceneLoaderManager.RemoveOnSingleLevelLoadedEvent(OnLevelLoadedEvent);
        }
    }        
}
