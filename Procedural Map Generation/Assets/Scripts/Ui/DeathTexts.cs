using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class DeathTexts: MonoBehaviour
{
    [Header("Localization Settings")]
    [Tooltip("The name of your String Table Collection")]
    [SerializeField] private string tableName = "RandomQuotes";

    [Tooltip("List of entry keys from the String Table")]
    [SerializeField] private List<string> entryKeys = new List<string>();

    [Header("Behavior")]
    [SerializeField] private bool randomizeOnEnable = true;

    private TextMeshProUGUI _textComponent;
    private string _currentKey;

    private void Awake()
    {
        _textComponent = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;

        if (randomizeOnEnable)
        {
            SetRandomText();
        }
    }

    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }
    public void SetRandomText()
    {
        if (entryKeys.Count > 1)
        {
            string newKey;
            do
            {
                newKey = entryKeys[Random.Range(0, entryKeys.Count)];
            } while (newKey == _currentKey);
            
            _currentKey = newKey;
        }
        else
        {
            _currentKey = entryKeys[0];
        }

        UpdateTextDisplay();
    }
    private void UpdateTextDisplay()
    {
        if (string.IsNullOrEmpty(_currentKey)) return;

        var stringOperation = LocalizationSettings.StringDatabase.GetLocalizedStringAsync(tableName, _currentKey);

        if (stringOperation.IsDone)
        {
            _textComponent.text = stringOperation.Result;
        }
        else
        {
            stringOperation.Completed += (op) =>
            {
                if (_textComponent != null)
                {
                    _textComponent.text = op.Result;
                }
            };
        }
    }
    private void OnLocaleChanged(Locale newLocale)
    {
        UpdateTextDisplay();
    }
}
