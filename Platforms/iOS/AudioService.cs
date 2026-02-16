using AVFoundation;
using Foundation;
using MAUI_Weather_App.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAUI_Weather_App.Platforms
{
    public class AudioService : IAudioService
    {
        private static AVAudioPlayer? _player;

        public void PlaySuccess() => Play("success");
        public void PlayFailure() => Play("failure");

        private void Play(string fileName)
        {
            try
            {
                var url = NSBundle.MainBundle.GetUrlForResource(fileName, "mp3");
                if (url == null) return;
                _player = AVAudioPlayer.FromUrl(url);
                _player?.PrepareToPlay();
                _player?.Play();
            }
            catch
            {
                // ignore audio errors
            }
        }
    }
}