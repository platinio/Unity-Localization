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
            var localizationLanguages = localizationManager.GetAvailableLanguages();
            dropdown.AddOptions(localizationLanguages);
            dropdown.value = localizationLanguages.IndexOf(localizationManager.ActiveLanguage.Name);
            dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        }

        private void OnDropdownValueChanged(int value)
        {
            localizationManager.ChangeActiveLanguage(dropdown.options[value].text);
        }
    }
}