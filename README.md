# MAUI Weather App

Cross-platform weather application built with **.NET MAUI** following MVVM architecture and service abstraction principles.

The project is designed as a clean, educational example of how to structure a real-world MAUI application without platform-specific dependencies.

---

## Features

- Search weather by city
- Get weather using device location
- Success / failure audio feedback
- Cross-platform services (no Android-only code)
- Clean MVVM separation
- API-ready architecture (no hardcoded secrets)
- Ready for public open-source use

---

## Tech Stack

- .NET MAUI
- MVVM pattern
- Dependency Injection
- Plugin.Maui.Audio
- Secure Storage (prepared)
- OpenWeather API (user provided key)

---

## Project Structure

```
MAUI_Weather_App
│
├── Models        → API response models
├── Services      → Weather / Location / Audio abstractions
├── ViewModels    → Business logic
├── Views         → UI pages
├── Platforms     → Empty (cross-platform approach)
└── Resources     → Sounds & assets
```


---

## Requirements

- Visual Studio 2022/2026 with .NET MAUI workload
- Android Emulator or Windows Machine

---

## Setup

### 1. Clone the repository

git clone https://github.com/USERNAME/MAUI_Weather_App.git
cd MAUI_Weather_App


Get a free API key

-Create an account: https://openweathermap.org/api
-Generate a key (Current Weather Data API)
-Wait ~2–10 minutes for activation

Configure API Key

-This application uses the OpenWeather Current Weather API.
-You must insert your own key before running the app at OpenWeatherService.

