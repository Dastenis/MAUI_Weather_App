using Android.Content;
using Android.Media;
using MAUI_Weather_App.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAUI_Weather_App.Platforms
{
    public class AudioService : IAudioService
    {
        private readonly SoundPool _pool;
        private readonly int _successId;
        private readonly int _failureId;

        public AudioService()
        {
            _pool = new SoundPool.Builder().SetMaxStreams(3).Build();
            var ctx = Android.App.Application.Context;
            _successId = _pool.Load(ctx, Resource.Raw.success, 1);
            _failureId = _pool.Load(ctx, Resource.Raw.failure, 1);
        }

        public void PlaySuccess() => _pool.Play(_successId, 1, 1, 0, 0, 1);
        public void PlayFailure() => _pool.Play(_failureId, 1, 1, 0, 0, 1);
    }
}