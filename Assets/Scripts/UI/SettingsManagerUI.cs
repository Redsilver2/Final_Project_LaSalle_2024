using UnityEngine;

namespace Redsilver2.Core.Settings
{
    public class SettingsManagerUI : MonoBehaviour
    {
        [SerializeField] private SelectableSettingUI[] settingsUI;

        #if UNITY_EDITOR
        private void OnValidate()
        {
            SelectableSettingUI.SetArray(ref settingsUI);
        }
        #endif

        private void Start()
        {
            for (int i = 0; i < settingsUI.Length; i++)
            {
                settingsUI[i].SetSettingType((SettingType)i);
                settingsUI[i].Enable();
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < settingsUI.Length; i++)
            {
                settingsUI[i].Disable();
            }
        }
    }
}
