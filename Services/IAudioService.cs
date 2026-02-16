using System;
using System.Collections.Generic;
using System.Text;

namespace MAUI_Weather_App.Services
{
    public interface IAudioService
    {
        void PlaySuccess();
        void PlayFailure();
    }
}
