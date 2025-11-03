using Microsoft.Maui.Storage;

namespace PotionPanic.Services;

public sealed class GameSessionService
{
    // PlayerName — имя игрока (по умолчанию "Player").
    public string PlayerName { get; private set; } = "Player";
    // SessionStartUtc — время начала
    public DateTime SessionStartUtc { get; private set; } = DateTime.UtcNow;


    // StartNew запускает новую сессию
    public void StartNew(string? name)
    {
        PlayerName = string.IsNullOrWhiteSpace(name) ? "Player" : name.Trim();
        SessionStartUtc = DateTime.UtcNow;
        Preferences.Set("pp.player", PlayerName);
    }

    // Загружает прошлое сохранённое имя из настроек и запускает новую сессию с этим именем
    public void LoadOrStart()
    {
        var saved = Preferences.Get("pp.player", "Player");
        StartNew(saved);
    }
}
