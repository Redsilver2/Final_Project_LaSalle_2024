using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Redsilver2.Core.Settings
{
    sealed class ShadowResolutionSetting : SelectableButtonSetting
    {
        private const string SHADOW_QUALITY_KEY = "SHADOW_QUALITY_KEY";

        public ShadowResolutionSetting() : base()
        {
            SetSaveKeyName(SHADOW_QUALITY_KEY);
            maxSelectedIndex = Enum.GetValues(typeof(ShadowQuality)).Length - 1;
        }

        protected override void OnValueChangedEvent(int index)
        {
            ShadowResolution  shadowResolution = (ShadowResolution)index;
            QualitySettings.shadowResolution = shadowResolution;
            InvokeTextToDisplay(shadowResolution.ToString()); 
        }
    }
}
