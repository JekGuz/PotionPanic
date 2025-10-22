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

    public static void Apply(string code)
    {
        var ci = new CultureInfo(code);
        CultureInfo.CurrentUICulture = ci;
        CultureInfo.CurrentCulture = ci;
        AppResources.Culture = ci;

        CurrentCode = code;
        Preferences.Set(PrefKey, code);
    }

    // Удобный «следующий язык» для Switch (en→ru→et→en…)
    public static string Next()
        => CurrentCode == "en" ? "ru" : (CurrentCode == "ru" ? "et" : "en");
}
