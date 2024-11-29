using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redsilver2.Core.Player;
using Redsilver2.Core.SceneManagement;
using Redsilver2.Core.Counters;

namespace Redsilver2.Core.Lights {
    public class LightManager : MonoBehaviour
    {
        [SerializeField] private float lightUpdateInterval = 0.2f;    
        private IEnumerator updateLightSystemsCoroutine;

        private List<LightSystem> lightSystems;
        public static LightManager Instance { get; private set; }

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

            lightSystems = new List<LightSystem>();
        }

        private IEnumerator UpdateLightSystems()
        {
            while (true)
            {
                PlayerController player = PlayerController.Instance;

                if (lightSystems.Count > 0 && player != null)
                {
                    foreach (LightSystem system in lightSystems)
                    {
                        Debug.DrawLine(system.transform.position, player.transform.position, Color.yellow);
                        system?.UpdateLightQualityLevel();
                    }
                }

                yield return Counter.WaitForSeconds(lightUpdateInterval);
            }
        }

        private void OnLoadSingleLevel(int levelIndex)
        {
            lightSystems.Clear();

            if (levelIndex != 0)
            {
                if (updateLightSystemsCoroutine == null)
                {
                    Debug.LogWarning("Activating Light Manager");

                    updateLightSystemsCoroutine = UpdateLightSystems();
                    StartCoroutine(updateLightSystemsCoroutine);
                }
            }
            else
            {
                Debug.LogWarning("Disabling Light Manager");

                if (updateLightSystemsCoroutine != null)
                {
                    StopCoroutine(updateLightSystemsCoroutine);
                }
            }
        }

        public void AddLightSystem(LightSystem system)
        {
            if(system != null && !lightSystems.Contains(system))
            {
                lightSystems.Add(system);
            }
        }

        public void RemoveLightSystem(LightSystem system)
        {
            if(lightSystems.Contains(system))
            {
                lightSystems.Remove(system);
            }
        }

        private void OnEnable()
        {
            SceneLoaderManager.AddOnLoadSingleSceneEvent(OnLoadSingleLevel);
        }

        private void OnDisable()
        {
            SceneLoaderManager.RemoveOnLoadSingleSceneEvent(OnLoadSingleLevel);
        }
    }
}
