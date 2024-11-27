using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Redsilver2.Core.Settings
{
    sealed class FrameRateLimitSetting : SelectableButtonSetting
    {
        private const string FRAMERATE_LIMIT_KEY = "FRAMERATE_LIMIT_KEY";
        private int[] framerateLimitValues = new int[] { 30, 60, 75, 120, 144, 165, 240, 360, 9999 };

        public FrameRateLimitSetting() : base()
        {
            SetSaveKeyName(FRAMERATE_LIMIT_KEY);
            maxSelectedIndex = framerateLimitValues.Length - 1;
        }

        protected override void OnValueChangedEvent(int index)
        {
            int framerateLimit = framerateLimitValues[index];
            string text = $"{framerateLimit} FPS";

            if(framerateLimit == 9999)
            {
                text = "Unlimited";
            }

            Application.targetFrameRate = framerateLimit;   
            InvokeTextToDisplay(text);
        }
    }
}