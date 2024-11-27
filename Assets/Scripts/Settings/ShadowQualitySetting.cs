using System;
using UnityEngine;

namespace Redsilver2.Core.Settings
{
    sealed class ShadowQualitySetting : SelectableButtonSetting
    {
        private const string SHADOW_QUALITY_KEY = "SHADOW_QUALITY_KEY";

        public ShadowQualitySetting() : base()
        {
            SetSaveKeyName(SHADOW_QUALITY_KEY);
            maxSelectedIndex = Enum.GetValues(typeof(ShadowQuality)).Length - 1;
        }

        protected override void OnValueChangedEvent(int index)
        {
            ShadowQuality shadowQuality = (ShadowQuality)index;
            QualitySettings.shadows = shadowQuality;
            InvokeTextToDisplay(shadowQuality.ToString());
        }
    }
}
