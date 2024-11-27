using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Redsilver2.Core.Settings
{
    sealed class VsyncSetting : ToggleSelectableButtonSetting
    {
        private const string VSYNC_KEY = "VSYNC_KEY";

        public VsyncSetting() : base()
        {
            SetSaveKeyName(VSYNC_KEY);
        }

        protected override void OnStateChangedEvent(bool isEnabled)
        {
            QualitySettings.vSyncCount = isEnabled ? 1 : 0;
            base.OnStateChangedEvent(isEnabled);
        }
    }
}
