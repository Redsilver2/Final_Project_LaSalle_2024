using Redsilver2.Core.Saveable;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Redsilver2.Core.Settings
{
    public abstract class SelectableButtonSetting : ISaveable
    {
        private   UnityEvent<int> onIndexValueChanged;
        private UnityEvent<string> onTextValueChanged;

        private   int  selectedIndex = 0;
        protected int  maxSelectedIndex;
      
        private string saveKeyName = "";

        public SelectableButtonSetting() 
        {
            this.onIndexValueChanged = new UnityEvent<int>();
            this.onTextValueChanged = new UnityEvent<string>();

            onIndexValueChanged.AddListener(OnValueChangedEvent);
            onIndexValueChanged.AddListener(index => { Save(); });
        }


        public void Init(Button nextButton, Button previousButton)
        {
            if (nextButton != null)
            {
                nextButton.onClick.AddListener(OnClickNextButton);
            }

            if (previousButton != null)
            {
                previousButton.onClick.AddListener(OnClickPreviousButton);
            }

            Load();
        }

        protected void SetSaveKeyName(string saveKeyName)
        {
            this.saveKeyName = saveKeyName;
        }

        protected void InvokeTextToDisplay(string text)
        {
            onTextValueChanged.Invoke(text);
        }

        public void Save()
        {
            if (saveKeyName != string.Empty)
            {
                PlayerPrefs.SetInt(saveKeyName, selectedIndex);
            }
        }
        public void Load()
        {
            if (saveKeyName != string.Empty)
            {
                onIndexValueChanged.Invoke(PlayerPrefs.GetInt(saveKeyName, selectedIndex));
            }
        }

        protected abstract void OnValueChangedEvent(int index);

        private void OnClickNextButton()
        {
            selectedIndex++;

            if(selectedIndex > maxSelectedIndex)
            {
                selectedIndex = 0;
            }

            onIndexValueChanged.Invoke(selectedIndex);
        }
        private void OnClickPreviousButton()
        {
            selectedIndex--;

            if (selectedIndex < 0)
            {
                selectedIndex = maxSelectedIndex;
            }

            onIndexValueChanged.Invoke(selectedIndex);
        }

        public void AddOnIndexValueChangedEvent(UnityAction<int> action)
        {
            onIndexValueChanged.AddListener(action);
        }
        public void RemoveOnIndexValueChangedEvent(UnityAction<int> action)
        {
            onIndexValueChanged.RemoveListener(action);
        }

        public void AddOnTextValueChangedEvent(UnityAction<string> action)
        {
            onTextValueChanged.AddListener(action);
        }
        public void RemoveOnTextValueChangedEvent(UnityAction<string> action)
        {
            onTextValueChanged.RemoveListener(action);
        }
    }
}
