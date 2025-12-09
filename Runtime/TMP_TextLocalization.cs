using TMPro;
using UnityEngine;
using Zenject;

namespace ArcaneOnyx.Localization
{
    public class TMP_TextLocalization : MonoBehaviour
    {
        [SerializeField, LocalizationKeySelector] private string localizationKey;

        private TextMeshProUGUI label;
        
        [Inject]
        private LocalizationManager localizationManager;
        
        private void Awake()
        {
            label = GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            UpdateText();
        }
       
        private void OnEnable()
        {
            localizationManager.OnLanguageChanged += UpdateText;
        }

        private void OnDisable()
        {
            localizationManager.OnLanguageChanged -= UpdateText;
        }

        private void UpdateText()
        {
            if (label == null) return;
            label.text = localizationManager.GetLocalization(localizationKey);
        }

        public void UpdateLocalizationKey(string key)
        {
            localizationKey = key;
            UpdateText();
        }
    }
}