using System;
using System.Collections.Generic;
using System.Text;

namespace MAUI_Weather_App.Services
{
    /// <summary>
    /// Abstracts application sound notifications.
    /// Implementations provide platform-specific audio playback behavior.
    /// </summary>
    public interface IAudioService
    {
        /// <summary>
        /// Plays the success feedback sound.
        /// Intended for positive operation completion.
        /// </summary>
        void PlaySuccess();

        /// <summary>
        /// Plays the failure feedback sound.
        /// Intended for errors or rejected operations.
        /// </summary>
        void PlayFailure();
    }
}
