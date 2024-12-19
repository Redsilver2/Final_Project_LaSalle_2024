using Redsilver2.Core.SceneManagement;
using UnityEngine;

namespace Redsilver2.Core.Events
{
    [CreateAssetMenu(menuName = "Events/Load Scene", fileName = "New Load Scene Event")]
    public class LoadSceneEvent : GameEvent
    {
        [SerializeField] private uint levelIndex;

        public override void Execute()
        {
            SceneLoaderManager sceneLoaderManager = SceneLoaderManager.Instance;

            if (sceneLoaderManager != null) { sceneLoaderManager.LoadSingleScene((int)levelIndex); }
        }
    }
}
