using Redsilver2.Core.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Redsilver2.Core
{
    public class PauseManager : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        [SerializeField] private GameObject pauseMenu;

        [Space]
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button quitButton;

        private bool lastCursorVisbility;

        private SceneLoaderManager sceneLoaderManager;
        private static UnityEvent<bool> onGamePaused = new UnityEvent<bool>();

        public static bool IsGamePaused { get; private set; }
        public static PauseManager Instance { get; private set; }

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
        }

        private void Start()
        {
            sceneLoaderManager = SceneLoaderManager.Instance;

            onGamePaused.AddListener(isGamePaused =>
            {
                if (pauseMenu)
                {
                    pauseMenu.SetActive(isGamePaused);
                }

                if (isGamePaused)
                {
                    lastCursorVisbility = Cursor.visible;
                    GameManager.SetCursorVisibility(true);
                }
                else
                {
                    if (lastCursorVisbility)
                    {
                        GameManager.SetCursorVisibility(false);
                    }
                }
            });


            if (resumeButton != null)
            {
                resumeButton.onClick.AddListener(() =>
                {
                    IsGamePaused = false;
                    onGamePaused.Invoke(IsGamePaused);
                });
            }

            if (quitButton != null)
            {
                quitButton.onClick.AddListener(() =>
                {
                    resumeButton.onClick.Invoke();
                    sceneLoaderManager.QuitApplication();
                });
            }

            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.AddListener(() =>
                {
                    resumeButton.onClick.Invoke();
                    sceneLoaderManager.LoadSingleScene(0);
                });
            }

            if (pauseMenu != null)
            {
                pauseMenu.SetActive(false);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !SceneLoaderManager.IsLoadingSingleScene && SceneManager.GetActiveScene().buildIndex != 0)
            {
                IsGamePaused = !IsGamePaused;
                onGamePaused.Invoke(IsGamePaused);
            }
        }

        public static void AddOnGamePausedEvent(UnityAction<bool> action) 
        {
            onGamePaused.AddListener(action);
        }
        public static void RemoveOnGamePausedEvent(UnityAction<bool> action)
        {
            onGamePaused.RemoveListener(action);
        }
    }
}
