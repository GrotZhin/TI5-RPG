using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocalizationManager : MonoBehaviour
{
    private bool active = false;

    public void NextLocale()
    {
        if (active) return;
        
        int totalLocales = LocalizationSettings.AvailableLocales.Locales.Count;
        int currentLocaleID = GetCurrentLocaleIndex();
        
        int nextLocaleID = (currentLocaleID + 1) % totalLocales;
        
        StartCoroutine(SetLocale(nextLocaleID));
    }

    public void PreviousLocale()
    {
        if (active) return;

        int totalLocales = LocalizationSettings.AvailableLocales.Locales.Count;
        int currentLocaleID = GetCurrentLocaleIndex();
        
        int previousLocaleID = currentLocaleID - 1;
        if (previousLocaleID < 0)
        {
            previousLocaleID = totalLocales - 1;
        }

        StartCoroutine(SetLocale(previousLocaleID));
    }

    private int GetCurrentLocaleIndex()
    {
        var currentLocale = LocalizationSettings.SelectedLocale;
        var localesList = LocalizationSettings.AvailableLocales.Locales;
        
        int index = localesList.IndexOf(currentLocale);
        
        return index >= 0 ? index : 0;
    }

    IEnumerator SetLocale(int LocaleID)
    {
        active = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[LocaleID];
        active = false;
    }
}
