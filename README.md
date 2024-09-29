# YandexGamesSDK-Unity

## Overview

YandexGamesSDK-Unity is a powerful Unity plugin designed to seamlessly integrate Yandex Games services into your Unity projects, with a focus on WebGL builds. This SDK empowers developers to create engaging, feature-rich games for the Yandex platform while working within the familiar Unity environment.

### Key Features

- **User Authentication**: Easily integrate Yandex user profiles, including support for anonymous users.
- **Leaderboards**: Submit scores, retrieve leaderboard data, and access player-specific entries.
- **Advertisements**: Implement fullscreen ads and rewarded videos with customizable event callbacks.
- **Cloud Storage**: Securely save and load user data with cloud storage support.
- **Local Development Environment**: Test your game using a local server for efficient debugging.
- **Yandex Games SDK Dashboard**: A Unity editor window for easy configuration and management of SDK features.

## Getting Started

### Prerequisites

- Unity 2020.3 or higher
- Node.js and npm (for local server setup)
- WebGL Build Target support

### Installation

1. Clone the repository:
   ```
   git clone <repository-url>
   ```
2. Import the SDK into your Unity project:
   - Drag the cloned folder into your project's `Assets` directory.

### Configuration

1. Open the Yandex Games SDK Dashboard in Unity:
   - Navigate to `Tools > Yandex Games SDK > Dashboard`
2. Create a new configuration asset:
   - Click "Create Configuration Asset" in the dashboard.
3. Set up your project:
   - Enter your Yandex Application ID.
   - Configure advertisement settings.
   - Set up cloud storage preferences.

## Development Workflow

### Local Server Setup

1. In the SDK Dashboard, specify your WebGL build path and server port.
2. Click "Start Local Server" to launch the development environment.

### Using Mock Data

1. Enable "Use Mock Data" in the SDK Dashboard for testing without a live Yandex connection.
2. Customize mock user profiles and leaderboard entries as needed.

### Building and Testing

1. Make your changes in Unity.
2. Build your WebGL project.
3. If enabled, the local server will automatically start after the build.
4. Test your game in a web browser, simulating the Yandex Games environment.

## Key Components

### Authentication Module

```csharp
// Example usage
YandexGamesSDK.Auth.Authenticate((success) => {
    if (success) {
        Debug.Log("User authenticated successfully");
    }
});
```

### Leaderboards Module

```csharp
// Example usage
YandexGamesSDK.Leaderboards.SetScore("leaderboardName", score, (success) => {
    if (success) {
        Debug.Log("Score submitted successfully");
    }
});
```

### Advertisement Module

```csharp
// Example usage
YandexGamesSDK.Ads.ShowFullscreenAd((result) => {
    if (result == AdResult.Closed) {
        Debug.Log("Ad closed by user");
    }
});
```

### Cloud Storage Module

```csharp
// Example usage
YandexGamesSDK.CloudStorage.Save("key", "value", (success) => {
    if (success) {
        Debug.Log("Data saved successfully");
    }
});
```

## Troubleshooting

- **Authentication Issues**: Enable Verbose Logging in the SDK Dashboard for detailed error information.
- **Local Server Problems**: Ensure Node.js and npm are correctly installed and updated.
- **WebGL Build Errors**: Verify that your Unity version supports WebGL builds and that all necessary modules are installed.

## Contributing

We welcome contributions! Please fork the repository, make your changes, and submit a pull request.

## License

YandexGamesSDK-Unity is distributed under the MIT License. See the LICENSE file for more information.

## Support

For additional help, check out our [FAQ Section](link-to-faq) or open an issue on our GitHub repository.
