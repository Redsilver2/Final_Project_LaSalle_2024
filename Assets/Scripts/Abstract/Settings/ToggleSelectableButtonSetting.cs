using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Redsilver2.Core.Settings
{
    public abstract class ToggleSelectableButtonSetting : SelectableButtonSetting
    {
        private UnityEvent<bool> onStateChanged;

        public ToggleSelectableButtonSetting() : base()
        {
            onStateChanged = new UnityEvent<bool>();
            maxSelectedIndex = 1;
            onStateChanged.AddListener(OnStateChangedEvent);
        }

        protected override void OnValueChangedEvent(int index)
        {
            bool isEnabled = true;

            if (index == 1)
            {
                isEnabled = false;
            }

            onStateChanged.Invoke(isEnabled);
        }

        protected virtual void OnStateChangedEvent(bool isEnabled)
        {
            InvokeTextToDisplay(isEnabled ? "Enabled" : "Disabled");
        }
    }
}
