using Plugin.Maui.Audio;

namespace PotionPanic.Services;

public sealed class MusicService
{
    private readonly IAudioManager _audioManager;
    private IAudioPlayer? _player;

    public bool IsPlaying => _player?.IsPlaying ?? false;

    public MusicService(IAudioManager audioManager)
        => _audioManager = audioManager;

    private async Task EnsurePlayerAsync()
    {
        if (_player != null) return;

        // файл лежит в Resources/Raw/bg.mp3
        using var stream = await FileSystem.OpenAppPackageFileAsync("bg.mp3");
        _player = _audioManager.CreatePlayer(stream);
        _player.Loop = true;
        _player.Volume = 0.35; // громкость 0.1
    }

    public async Task PlayAsync()
    {
        await EnsurePlayerAsync();
        _player?.Play();
    }

    public void Pause() => _player?.Pause();

    public void Stop()
    {
        _player?.Stop();
        _player?.Dispose();
        _player = null;
    }

    public void SetVolume(double volume01)
    {
        if (_player == null) return;
        _player.Volume = Math.Clamp(volume01, 0.0, 1.0);
    }

    public async Task ToggleAsync()
    {
        await EnsurePlayerAsync();
        if (_player!.IsPlaying) _player.Pause();
        else _player.Play();
    }
}
