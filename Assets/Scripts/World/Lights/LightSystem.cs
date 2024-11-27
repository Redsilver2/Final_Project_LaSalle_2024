using System.Linq;
using Redsilver2.Core.Events;
using Redsilver2.Core.Player;
using System;
using UnityEngine;

namespace Redsilver2.Core.Lights
{
    public class LightSystem : GameObjectEvents
    {
        [SerializeField] private Light _light;

        [Space]
        [SerializeField] private LightQualityLevelData[] lightQualityLevelDatas;

        private LightManager lightManager;
        private LightQualityLevelData selectedQualityLevelData;

        #if UNITY_EDITOR
        private void OnValidate()
        {
            LightQualityLevel[] qualityLevels = (LightQualityLevel[])Enum.GetValues(typeof(LightQualityLevel));

            if(lightQualityLevelDatas == null)
            {
                lightQualityLevelDatas = new LightQualityLevelData[0];
            }

            if(lightQualityLevelDatas.Length != qualityLevels.Length)
            {
                LightQualityLevelData[] result = new LightQualityLevelData[qualityLevels.Length];

                for(int i = 0; i < qualityLevels.Length; i++)
                {
                    string qualityLevelName = qualityLevels[i].ToString();
                    bool foundQualityLevel  = false;

                    for(int j = 0; j < lightQualityLevelDatas.Length; j++)
                    {
                        if (lightQualityLevelDatas[j].qualityName.ToLower() == qualityLevelName.ToLower())
                        {
                            result[i] = lightQualityLevelDatas[j];
                            foundQualityLevel = true;
                            break;
                        }
                    }

                    if (!foundQualityLevel)
                    {
                        result[i] = new LightQualityLevelData(qualityLevelName);
                    }
                }

                lightQualityLevelDatas = result;
            }
        }
#endif

        protected override void Awake()
        {
             base.Awake();

            lightManager = LightManager.Instance;
            lightManager.AddLightSystem(this);

            AddOnStateChangedEvent(isEnabled =>
            {
                if(isEnabled)
                {
                    lightManager.AddLightSystem(this);
                }
                else
                {
                    lightManager.RemoveLightSystem(this);
                }
            });
        }

        public void UpdateLightQualityLevel()
        {
            Transform playerTransform    = PlayerController.Instance.transform;
            float     distance           = Mathf.Infinity;

            if (playerTransform != null)
            {
               distance = Vector3.Distance(transform.position, playerTransform.position); 
            }

            if (_light != null)
            {
                Debug.Log(distance);

                if (distance < lightQualityLevelDatas[0].MaxCheckDistance)
                {
                    LightQualityLevelData data = lightQualityLevelDatas.Single(x => x.IsCloseToPlayer(distance));

                    if (selectedQualityLevelData != data)
                    {
                        selectedQualityLevelData = data;
                        data.SetLightQuality(_light);
                    }
                }
                else
                {
                    selectedQualityLevelData = null;
                    _light.enabled = false;
                }
            }
        }
    }
}
