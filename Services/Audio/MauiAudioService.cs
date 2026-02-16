using System;
using System.Threading.Tasks;
using Plugin.Maui.Audio;
using Microsoft.Maui.Storage;

namespace MAUI_Weather_App.Services.Audio
{
    public class MauiAudioService : MAUI_Weather_App.Services.IAudioService
    {
        private readonly IAudioManager _audioManager;
        private IAudioPlayer? _success;
        private IAudioPlayer? _failure;

        public MauiAudioService(IAudioManager audioManager)
        {
            _audioManager = audioManager ?? throw new ArgumentNullException(nameof(audioManager));
            // Async init so ctor doesn't block
            _ = InitAsync();
        }

        private async Task InitAsync()
        {
            try
            {
                var successCandidates = new[]
                {
                    "Resources/Raw/success.wav",
                    "Resources/Raw/success.mp3",
                    "success.wav",
                    "success.mp3",
                    "Raw/success.wav",
                    "Raw/success.mp3"
                };

                var failureCandidates = new[]
                {
                    "Resources/Raw/failure.wav",
                    "Resources/Raw/failure.mp3",
                    "failure.wav",
                    "failure.mp3",
                    "Raw/failure.wav",
                    "Raw/failure.mp3"
                };

                _success = await CreatePlayerFromCandidatesAsync(successCandidates);
                System.Diagnostics.Debug.WriteLine(_success != null ? "Audio: success player loaded" : "Audio: success player NOT loaded");

                _failure = await CreatePlayerFromCandidatesAsync(failureCandidates);
                System.Diagnostics.Debug.WriteLine(_failure != null ? "Audio: failure player loaded" : "Audio: failure player NOT loaded");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Audio init error: {ex.GetType().Name}: {ex.Message}");
            }
        }

        private async Task<IAudioPlayer?> CreatePlayerFromCandidatesAsync(string[] candidates)
        {
            foreach (var candidate in candidates)
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine($"Audio: trying candidate '{candidate}'");
                    using var stream = await FileSystem.OpenAppPackageFileAsync(candidate);
                    if (stream == null)
                    {
                        System.Diagnostics.Debug.WriteLine($"Audio: stream null for {candidate}");
                        continue;
                    }

                    var player = _audioManager.CreatePlayer(stream);
                    if (player != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"Audio: created player from {candidate}");
                        return player;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Audio: candidate {candidate} failed: {ex.GetType().Name}: {ex.Message}");
                    // try next candidate
                }
            }

            return null;
        }

        public void PlaySuccess()
        {
            try { _success?.Play(); }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Audio PlaySuccess error: {ex.Message}"); }
        }

        public void PlayFailure()
        {
            try { _failure?.Play(); }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Audio PlayFailure error: {ex.Message}"); }
        }
    }
}
