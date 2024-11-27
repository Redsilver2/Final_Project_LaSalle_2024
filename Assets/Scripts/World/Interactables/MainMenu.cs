using Redsilver2.Core.SceneManagement;
using Redsilver2.Core.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace Redsilver2.Core
{
    [RequireComponent(typeof(Animator))]
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private GraphicRaycaster raycaster;
        [SerializeField] private AnimationController animationController;
        private SceneLoaderManager sceneLoader;

        private void Start()
        {
            sceneLoader = SceneLoaderManager.Instance;
            SceneLoaderManager.AddOnLoadSingleSceneEvent(OnLoadSingleSceneEvent);

            GameManager.SetCursorVisibility(true);
            GameManager.Instance.GetComponent<InputManager>().PlayerControls.Enable();
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

            SceneLoaderManager.RemoveOnLoadSingleSceneEvent(OnLoadSingleSceneEvent);
        }
    }

}
