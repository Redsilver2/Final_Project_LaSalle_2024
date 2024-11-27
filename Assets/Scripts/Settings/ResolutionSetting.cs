using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

namespace Redsilver2.Core.Settings
{
    sealed class ResolutionSetting : SelectableButtonSetting
    {
        private Resolution[] resolutions;
        private const string RESOLUTION_KEY = "RESOLUTION_KEY";

        public ResolutionSetting() : base()
        {
            resolutions = Screen.resolutions.Distinct().ToArray();
            Array.Reverse(resolutions);

            SetSaveKeyName(RESOLUTION_KEY);
            maxSelectedIndex = resolutions.Length - 1;
        }

        protected override void OnValueChangedEvent(int index)
        {
           Resolution resolution = resolutions[index];
           Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
           InvokeTextToDisplay($"{resolution.width}x{resolution.height}");
        }
    }
}
