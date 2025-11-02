using Microsoft.Maui.Storage;

namespace PotionPanic.Services;

public sealed class GameSessionService
{
    public string PlayerName { get; private set; } = "Player";
    public DateTime SessionStartUtc { get; private set; } = DateTime.UtcNow;

    public void StartNew(string? name)
    {
        PlayerName = string.IsNullOrWhiteSpace(name) ? "Player" : name.Trim();
        SessionStartUtc = DateTime.UtcNow;
        Preferences.Set("pp.player", PlayerName);
    }

    public void LoadOrStart()
    {
        var saved = Preferences.Get("pp.player", "Player");
        StartNew(saved);
    }
}
