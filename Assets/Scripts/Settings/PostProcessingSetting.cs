using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Redsilver2.Core.Settings
{
    sealed class PostProcessingSetting : ToggleSelectableButtonSetting
    {
        private const string POST_PROCESSING_KEY = "POST_PROCESSING_KEY";

        public PostProcessingSetting() : base() 
        {
            SetSaveKeyName(POST_PROCESSING_KEY);
        }

        protected override void OnStateChangedEvent(bool isEnabled)
        {
            Volume volume = SettingsManager.Instance.GlobalPostProcessVolume;
            Debug.LogWarning("Volume: " + volume);

            if (volume != null)
            {
                volume.enabled = isEnabled;
            }

            base.OnStateChangedEvent(isEnabled);
        }
    }
}
