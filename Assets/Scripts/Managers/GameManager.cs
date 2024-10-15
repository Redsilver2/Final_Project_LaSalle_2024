using UnityEngine;

namespace Redsilver2.Core
{
    public class GameManager : MonoBehaviour
    {
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

        private void Start()
        {

        }

        public static void SetCursorVisibility(bool isVisible)
        {
            Cursor.visible   = isVisible;
            Cursor.lockState = isVisible ? CursorLockMode.Confined : CursorLockMode.Locked;
        }
    }
}
