using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityStandardAssets.Utility;
using TMPro;
using UnityEngine.Events;

namespace Redsilver2.Core.Settings
{
    public class SettingsManager : MonoBehaviour
    {
        [SerializeField] private Volume     globalPostProcessVolume;
        [SerializeField] private FPSCounter fpsCounter;
        [SerializeField] private GameObject blackBars;

        private ResolutionSetting       resolutionSetting;
        private FullScreenModeSetting   fullScreenModeSetting;
        private FrameRateLimitSetting   frameRateLimitSetting;  
        private VsyncSetting            vsyncSetting;
        private PostProcessingSetting   postProcessingSetting;
        private ShadowResolutionSetting shadowResolutionSetting;
        private ShadowQualitySetting    shadowQualitySetting;

        public static SettingsManager Instance;

        public FPSCounter FPSCounter => fpsCounter;
        public Volume GlobalPostProcessVolume => globalPostProcessVolume;
        public GameObject BlackBars => blackBars;

        private void Awake()
        {
            if(Instance == null)
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
            resolutionSetting       = new ResolutionSetting();
            fullScreenModeSetting   = new FullScreenModeSetting();
            frameRateLimitSetting   = new FrameRateLimitSetting();
            vsyncSetting            = new VsyncSetting();
            postProcessingSetting   = new PostProcessingSetting();
            shadowResolutionSetting = new ShadowResolutionSetting();
            shadowQualitySetting    = new ShadowQualitySetting();
        }

        public void SetSettingUI(Button nextButton, Button previousButton, SettingType settingType)
        {
            switch(settingType)
            {
                case SettingType.Resolution:
                    resolutionSetting.Init(nextButton, previousButton);
                    break;

                case SettingType.FullScreenMode:
                    fullScreenModeSetting.Init(nextButton, previousButton);
                    break;

                case SettingType.FrameRateLimit:
                    frameRateLimitSetting.Init(nextButton, previousButton);
                    break;

                case SettingType.Vsync:
                    vsyncSetting.Init(nextButton, previousButton);
                    break;

                case SettingType.PostProcessing:
                    postProcessingSetting.Init(nextButton, previousButton);
                    break;

                case SettingType.ShadowResolution:
                    shadowResolutionSetting.Init(nextButton, previousButton);
                    break;

                case SettingType.ShadowQuality:
                    shadowQualitySetting.Init(nextButton, previousButton);
                    break;
            }
        }

        public void AddSettingUITextEvent(UnityAction<string> action, SettingType settingType)
        {
            switch (settingType)
            {
                case SettingType.Resolution:
                    resolutionSetting.AddOnTextValueChangedEvent(action);
                    break;

                case SettingType.FullScreenMode:
                    fullScreenModeSetting.AddOnTextValueChangedEvent(action);
                    break;

                case SettingType.FrameRateLimit:
                    frameRateLimitSetting.AddOnTextValueChangedEvent(action);
                    break;

                case SettingType.Vsync:
                    vsyncSetting.AddOnTextValueChangedEvent(action);
                    break;

                case SettingType.PostProcessing:
                    postProcessingSetting.AddOnTextValueChangedEvent(action);
                    break;

                case SettingType.ShadowResolution:
                    shadowResolutionSetting.AddOnTextValueChangedEvent(action);
                    break;

                case SettingType.ShadowQuality:
                    shadowQualitySetting.AddOnTextValueChangedEvent(action);
                    break;
            }
        }

        public void RemoveSettingUITextEvent(UnityAction<string> action, SettingType settingType)
        {
            switch (settingType)
            {
                case SettingType.Resolution:
                    resolutionSetting.RemoveOnTextValueChangedEvent(action);
                    break;

                case SettingType.FullScreenMode:
                    fullScreenModeSetting.RemoveOnTextValueChangedEvent(action);
                    break;

                case SettingType.FrameRateLimit:
                    frameRateLimitSetting.RemoveOnTextValueChangedEvent(action);
                    break;

                case SettingType.Vsync:
                    vsyncSetting.RemoveOnTextValueChangedEvent(action);
                    break;

                case SettingType.PostProcessing:
                    postProcessingSetting.RemoveOnTextValueChangedEvent(action);
                    break;

                case SettingType.ShadowResolution:
                    shadowResolutionSetting.RemoveOnTextValueChangedEvent(action);
                    break;

                case SettingType.ShadowQuality:
                    shadowQualitySetting.RemoveOnTextValueChangedEvent(action);
                    break;
            }
        }

        private void OnDisable()
        {
            PlayerPrefs.Save();
        }

    }
}
