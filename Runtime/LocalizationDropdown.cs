using TMPro;
using UnityEngine;
using Zenject;

namespace ArcaneOnyx.Localization
{
    public class LocalizationDropdown : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown dropdown;

        [Inject]
        private LocalizationManager localizationManager;
        
        private void Start()
        {
            var localizationOptions = localizationManager.GetLocalizationOptions();
            dropdown.AddOptions(localizationOptions);
            dropdown.value = localizationOptions.IndexOf(localizationManager.ActiveLanguage.Name);
            dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        }

        private void OnDropdownValueChanged(int value)
        {
            localizationManager.ChangeActiveLanguage(dropdown.options[value].text);
        }
    }
}