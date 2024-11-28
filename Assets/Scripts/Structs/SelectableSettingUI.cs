using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Redsilver2.Core.Settings
{
    [System.Serializable]
    public class SelectableSettingUI
    {
        [SerializeField] private Button nextButton;
        [SerializeField] private Button prevButton;
        [SerializeField] private TextMeshProUGUI valueDisplayer;

        [SerializeField]  protected SettingType settingType;
        private bool isEnabled;


        public SelectableSettingUI(SettingType settingType)
        {
            this.settingType     = settingType;
            this.nextButton      = null;
            this.prevButton      = null;
            this.valueDisplayer  = null;
            this.isEnabled       = false;
        }

        public void Enable()
        {
            if (!isEnabled)
            {
                SettingsManager settingsManager = SettingsManager.Instance;
                isEnabled = true;

                Debug.Log(settingType.ToString());

                if (settingsManager != null)
                {
                    settingsManager.AddSettingUITextEvent(OnTextValueUpdatedEvent, settingType);
                    settingsManager.SetSettingUI(nextButton, prevButton, settingType);
                }
            }
        }
        public void Disable() 
        { 
            if (isEnabled)
            {
                SettingsManager settingsManager = SettingsManager.Instance;
                isEnabled = false;

                if (settingsManager != null)
                {
                    settingsManager.RemoveSettingUITextEvent(OnTextValueUpdatedEvent, settingType);
                }
            }
        }

        private void OnTextValueUpdatedEvent(string text)
        {
            if(valueDisplayer != null)
            {
                valueDisplayer.text = text;
            }
        }

        public static void SetArray(ref SelectableSettingUI[] datas)
        {
            SettingType[] settingTypes = (SettingType[])Enum.GetValues(typeof(SettingType));

            if (datas.Length != settingTypes.Length)
            {
                SelectableSettingUI[] results = new SelectableSettingUI[settingTypes.Length];

                for (int i = 0; i < settingTypes.Length; i++)
                {
                    SettingType settingType = settingTypes[i];
                    bool foundSameUI = false;

                    for (int j = 0; j < datas.Length; j++)
                    {
                        if(datas[j].settingType == settingType)
                        {
                            results[i] = datas[j];
                            foundSameUI = true;
                            break;
                        }
                    }

                    if (!foundSameUI)
                    {
                        results[i] = new SelectableSettingUI(settingType);    
                    }
                }

                datas = results;
            }
        }

        public static void SetSelectableSettingsUIState(SelectableSettingUI[] datas, bool isEnabled)
        {
            foreach (SelectableSettingUI ui in datas)
            {
                if (isEnabled)
                {
                    ui.Enable();
                }
                else
                {
                    ui.Disable();
                }
            }
        }
    }
}
