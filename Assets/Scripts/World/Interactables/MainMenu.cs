using Redsilver2.Core.SceneManagement;
using Redsilver2.Core.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace Redsilver2.Core
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private GraphicRaycaster raycaster;
        [SerializeField] private GameObject mainMenuParent;

        [SerializeField] private AnimationController animationController;
        [SerializeField] private SelectableSettingUI[] selectableButtonSettings;
        private SceneLoaderManager sceneLoader;

        #if UNITY_EDITOR
        private void OnValidate()
        {
           SelectableSettingUI.SetArray(ref selectableButtonSettings);    
        }
        #endif

        private void Start()
        {
            animationController.Init(mainMenuParent, false);
            sceneLoader = SceneLoaderManager.Instance;

            GameManager.SetCursorVisibility(true);
            GameManager.Instance.GetComponent<InputManager>().PlayerControls.Enable();
            SelectableSettingUI.SetSelectableSettingsUIState(selectableButtonSettings, true);
        }

        public void LoadLevel(int levelIndex)
        {
           if(sceneLoader != null)
           {
               sceneLoader.LoadSingleScene(levelIndex);    
           }
        }

        public void QuitGame()
        {
            if (raycaster != null)
            {
                raycaster.enabled = false;
            }

            if (sceneLoader != null)
            {
                sceneLoader.QuitApplication();
            }
        }

        public void PlayAnimation(string stateName)
        {
            animationController.PlayAnimation(stateName);
        }

        private void OnLoadSingleSceneEvent(int index)
        {
            if (raycaster != null)
            {
                raycaster.enabled = false;
            }
        }

        private void OnEnable()
        {
            SceneLoaderManager.AddOnLoadSingleSceneEvent(OnLoadSingleSceneEvent);
        }

        private void OnDisable()
        {
            SelectableSettingUI.SetSelectableSettingsUIState(selectableButtonSettings, false);
            SceneLoaderManager.RemoveOnLoadSingleSceneEvent(OnLoadSingleSceneEvent);
        }
    }

}
