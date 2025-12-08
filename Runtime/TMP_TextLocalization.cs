using TMPro;
using UnityEngine;
using Zenject;

namespace ArcaneOnyx.Localization
{
    public class TMP_TextLocalization : MonoBehaviour
    {
        [SerializeField, LocalizationSelector] private LocalizationKey localizationKey;

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
    }
}