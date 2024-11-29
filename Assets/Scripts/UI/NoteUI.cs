using Redsilver2.Core.Player;
using Redsilver2.Core.SceneManagement;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Redsilver2.Core
{
    public class NoteUI : MonoBehaviour
    {
        [SerializeField] private GameObject uiObject;
        [SerializeField] private CanvasGroup canvasGroup;

        [SerializeField] private AudioController audioController;

        [Space]
        [SerializeField] private Button closeUIButton;
        [SerializeField] private Button nextPageButton;
        [SerializeField] private Button prevPageButton;

        [Space]
        [SerializeField] private TextMeshProUGUI titleDisplayer;
        [SerializeField] private TextMeshProUGUI pageContentDisplayer;
        [SerializeField] private TextMeshProUGUI pageIndexDisplayer;

        private int      pageIndex = 0;
        private string[] noteContents;

        private IEnumerator fadeNoteUICoroutine;
        private PlayerController playerController;

        private void Awake()
        {
            playerController = PlayerController.Instance;

            if (nextPageButton != null) 
            {
                nextPageButton.onClick.AddListener(() =>
                {
                    if (noteContents != null)
                    {
                        pageIndex++;
                        if (pageIndex >= noteContents.Length) pageIndex = 0;
                        SetPage(pageIndex);
                    }
                });
            }
            if (prevPageButton != null) 
            {
                prevPageButton.onClick.AddListener(() =>
                {
                    if (noteContents != null)
                    {
                        pageIndex--;
                        if (pageIndex < 0) pageIndex = noteContents.Length - 1;

                        SetPage(pageIndex);
                    }
                });
            }

            canvasGroup.alpha = 0f;
            SetUIVisibility(false);
        }

        public void SetUIVisibility(bool isVisible)
        {
            if (uiObject != null)
            {
                uiObject.SetActive(isVisible);
            }
        }


        public void Open(string title, string[] contents, AudioClip openNoteClip, UnityAction onCloseUIEvent)
        {
            if (closeUIButton != null) 
            {
                closeUIButton.onClick.RemoveAllListeners();
                closeUIButton.onClick.AddListener(onCloseUIEvent);  
            }

            noteContents = contents;
            playerController.enabled  = false;

            pageIndex                 = 0;
            titleDisplayer.text       = title;

            SetPage(pageIndex);
            FadeCanvasGroupAlpha(true);

            audioController.Play(openNoteClip);
            GameManager.SetCursorVisibility(true);
        }

        public void Close(AudioClip closeClip)
        {
            noteContents = null;
            FadeCanvasGroupAlpha(false);
            audioController.Play(closeClip);
        }

        private void SetPage(int pageIndex)
        {
            if(noteContents != null)
            {
                pageContentDisplayer.text = noteContents[pageIndex];
                pageIndexDisplayer.text = GetCurrentPageIndex();
            }
        }

        private void OnGamePausedEvent(bool isGamePaused)
        {
            prevPageButton.interactable = !isGamePaused;
            nextPageButton.interactable = !isGamePaused;
            closeUIButton.interactable  = !isGamePaused;
        }

        private void FadeCanvasGroupAlpha(bool isVisible)
        {
            if (fadeNoteUICoroutine != null) StopCoroutine(fadeNoteUICoroutine);
            fadeNoteUICoroutine = FadeCanvasGroupAlphaCoroutine(isVisible);
            StartCoroutine(fadeNoteUICoroutine);
        }

        private IEnumerator FadeCanvasGroupAlphaCoroutine(bool isVisible)
        {
            canvasGroup.enabled = true;

            if (isVisible)
            {
                playerController.enabled = false;
                GameManager.SetCursorVisibility(true);
            }

            yield return canvasGroup.FadeCanvasGroup(isVisible, 0.5f);

            if (!isVisible)
            {
                GameManager.SetCursorVisibility(false);
                playerController.enabled = true;
                SetUIVisibility(false);
            }

            canvasGroup.enabled = false;
        }

        private string GetCurrentPageIndex() => $"Pages {pageIndex + 1}/{noteContents.Length}";

        private void OnLoadSceneEvent(int levelIndex)
        {
            SetUIVisibility(false);
        }

        private void OnEnable()
        {
            PauseManager.AddOnGamePausedEvent(OnGamePausedEvent);
            SceneLoaderManager.AddOnLoadSingleSceneEvent(OnLoadSceneEvent);
        }

        private void OnDisable()
        {
            PauseManager.RemoveOnGamePausedEvent(OnGamePausedEvent);
            SceneLoaderManager.RemoveOnLoadSingleSceneEvent(OnLoadSceneEvent);
        }

    }
}
