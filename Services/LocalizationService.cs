using System.Globalization;
using Microsoft.Maui.Storage;
using PotionPanic.Resources;

namespace PotionPanic.Services;

public static class LocalizationService
{
    const string PrefKey = "ui.lang";

    public static string CurrentCode { get; private set; } = "en";

    public static void InitFromPreferences()
    {
        var code = Preferences.Get(PrefKey, "en");
        Apply(code);
    }

    // Применить язык по коду (en, ru, et)
    public static void Apply(string code)
    {
        // CurrentCulture — влияет на форматы(дата / время, числа, валюты).
        // CurrentUICulture — влияет на язык интерфейса(что берётся из.resx).
        // AppResources.Culture — напрямую указывает нашему ресурcному классу, из какого .resx доставать строки.

        var lang = new CultureInfo(code);
        CultureInfo.CurrentUICulture = lang;
        CultureInfo.CurrentCulture = lang;
        AppResources.Culture = lang;

        CurrentCode = code;
        Preferences.Set(PrefKey, code);
    }

    // Удобный «следующий язык» для Switch (en→ru→et→en…)
    public static string Next()
        => CurrentCode == "en" ? "ru" : (CurrentCode == "ru" ? "et" : "en");
}
