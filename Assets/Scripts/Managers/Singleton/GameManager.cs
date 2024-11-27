using Redsilver2.Core.Audio;
using Redsilver2.Core.Lights;
using Redsilver2.Core.SceneManagement;
using Redsilver2.Core.Settings;
using Redsilver2.Core.Subtitles;
using UnityEngine;

namespace Redsilver2.Core
{
    [RequireComponent(typeof(AudioManager))]
    [RequireComponent(typeof(LightManager))]
    [RequireComponent(typeof(SceneLoaderManager))]
    [RequireComponent(typeof(InputManager))]
    [RequireComponent(typeof(SettingsManager))]
    [RequireComponent(typeof(PauseManager))]
    [RequireComponent(typeof(SubtitleManager))]
    public class GameManager : MonoBehaviour
    {
        private int daysLeft      = 0;   
        public static GameManager Instance { get; private set; }


        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(Instance);
            }
            else
            {
                Destroy(gameObject);
            }
        }


        public void SpawnPlayer()
        {

        }

        public void DecreaseDaysLeft()
        {
            daysLeft--;

            if(daysLeft <= 0)
            {
                // Do something here
            }
        }


        public static void SetCursorVisibility(bool isVisible)
        {
            Cursor.visible   = isVisible;
            Cursor.lockState = isVisible ? CursorLockMode.Confined : CursorLockMode.Locked;
        }

    }
}
