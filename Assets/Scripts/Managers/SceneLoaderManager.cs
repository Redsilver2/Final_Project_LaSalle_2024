using Redsilver2.Core.Audio;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Redsilver2.Core.SceneManagement
{
    public class  SceneLoaderManager: MonoBehaviour
    {
        [SerializeField] private CanvasRenderer loadingScreenBackground;
        [SerializeField] private float loadingScreenAlphaLerpDuration;

        private static UnityEvent<int> onSingleSceneLoaded = new UnityEvent<int>();
        private static UnityEvent<int> onLoadSingleScene   = new UnityEvent<int>();

        public float LoadingScreenAlphaLerpDuration => loadingScreenAlphaLerpDuration;
       
        public static int SelectedSingleLevelIndex { get; private set; }
        public static bool IsLoadingSingleScene { get; private set; }
        public static SceneLoaderManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }

            if (loadingScreenBackground != null)
            {
                loadingScreenBackground.SetAlpha(0f);
                loadingScreenBackground.gameObject.SetActive(false);
            }

            SelectedSingleLevelIndex = SceneManager.GetActiveScene().buildIndex;
        }

        public void LoadSingleScene(int levelIndex)
        {
            if(levelIndex < 0)
            {
                levelIndex = 0;
            }
            else if(levelIndex >= SceneManager.sceneCountInBuildSettings)
            {
                levelIndex = SceneManager.sceneCountInBuildSettings - 1;
            }

            if (!IsLoadingSingleScene && levelIndex != SelectedSingleLevelIndex)
            {
                IsLoadingSingleScene = true;
                SelectedSingleLevelIndex = levelIndex;

                loadingScreenBackground.gameObject.SetActive(true);
                StartCoroutine(LoadSingleSceneCoroutine());
            }
        }



        private IEnumerator LoadSingleSceneCoroutine()
        {
            Debug.LogWarning("Level loading... " + SelectedSingleLevelIndex);
            onLoadSingleScene.Invoke(SelectedSingleLevelIndex);

            StartCoroutine(AudioManager.LerpAudioListenerVolume(true, loadingScreenAlphaLerpDuration));
            yield return loadingScreenBackground.FadeCanvasRenderer(true, loadingScreenAlphaLerpDuration);

            AsyncOperation operation = SceneManager.LoadSceneAsync(SelectedSingleLevelIndex);
            operation.allowSceneActivation = false;

            while (operation.progress < 0.9f)
            {
                Debug.LogWarning($"{operation.progress}/0.9f ({operation.progress/0.9f})");
                yield return null;
            }

            operation.allowSceneActivation = true;

            Debug.LogWarning($"Level Load Completed");
            onSingleSceneLoaded.Invoke(SelectedSingleLevelIndex);

            StartCoroutine(AudioManager.LerpAudioListenerVolume(false, loadingScreenAlphaLerpDuration));
            yield return loadingScreenBackground.FadeCanvasRenderer(false, loadingScreenAlphaLerpDuration);

            IsLoadingSingleScene = false;
            loadingScreenBackground.gameObject.SetActive(false);
        }

        private IEnumerator QuitApplicationCoroutine()
        {
            IsLoadingSingleScene = true;
            onLoadSingleScene.Invoke(-1);

            StartCoroutine(AudioManager.LerpAudioListenerVolume(true, loadingScreenAlphaLerpDuration));
            yield return loadingScreenBackground.FadeCanvasRenderer(true, loadingScreenAlphaLerpDuration);
            Application.Quit();
        }

        public void QuitApplication()
        {
            if (IsLoadingSingleScene)
            {
                return;
            }

            if(loadingScreenBackground != null)
            {
                loadingScreenBackground.gameObject.SetActive(true);
            }

            StartCoroutine(QuitApplicationCoroutine());
        }
        public static void AddOnLoadSingleSceneEvent(UnityAction<int> action)
        {
            onLoadSingleScene.AddListener(action);
        }
        public static void RemoveOnLoadSingleSceneEvent(UnityAction<int> action)
        {
            onLoadSingleScene.RemoveListener(action);
        }

        public static void AddOnSingleLevelLoadedEvent(UnityAction<int> action)
        {
            onSingleSceneLoaded.AddListener(action);
        }
        public static void RemoveOnSingleLevelLoadedEvent(UnityAction<int> action)
        {
            onSingleSceneLoaded.RemoveListener(action);
        }

    }
}
