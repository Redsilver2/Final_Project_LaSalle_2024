using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Redsilver2.Core.Settings
{
    sealed class FullScreenModeSetting : SelectableButtonSetting
    {
        private const string FULLSCREEN_MODE_KEY = "FULLSCREEN_MODE_KEY"; 

        public FullScreenModeSetting() : base()
        {
            SetSaveKeyName(FULLSCREEN_MODE_KEY);
            maxSelectedIndex = Enum.GetValues(typeof(FullScreenMode)).Length - 1;
        }

        protected override void OnValueChangedEvent(int index)
        {
            FullScreenMode fullScreenMode = (FullScreenMode)index;
            Resolution resolution         = Screen.currentResolution;
            Screen.SetResolution(resolution.width, resolution.height, fullScreenMode);
            InvokeTextToDisplay(fullScreenMode.ToString());
        }
    }
}
