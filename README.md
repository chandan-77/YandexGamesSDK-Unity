# YandexGamesSDK-Unity

## Overview

YandexGamesSDK-Unity is an advanced integration package designed for Unity developers to seamlessly connect their WebGL projects with Yandex Games services. This comprehensive plugin enhances your development workflow by providing robust authentication, leaderboard management, advertisement integration, cloud storage functionality, and a built-in local server management utility for efficient testing and development.

## Key Features

### Authentication and User Profiles
- Manage Yandex Games authentication directly from Unity
- Handle both anonymous and registered users
- Access player profiles and avatar data
- Support for mock profiles in local environments

### Leaderboard Management
- Submit and retrieve scores
- Get player-specific leaderboard data
- Efficient leaderboard data handling with dedicated classes

### Advertisements Integration
- Implement fullscreen ads and rewarded videos
- Customizable callback functions for ad lifecycle events (open, close, error, reward completion)

### Cloud and Local Storage
- Securely save and load player data using Yandex Cloud Storage
- Automatic fallback to local storage for testing during development

### Local Server Development
- Run a local server for development and testing
- Simulate a live Yandex environment
- Automatic server start after WebGL builds
- Real-time server logs and status monitoring

### Yandex Games SDK Dashboard
- Unity editor window for easy configuration and management
- Create and manage configuration assets
- Toggle development options and mock data
- Control local server directly from Unity

## Technical Requirements

- Unity Version: 2020.3 or higher
- Build Target: WebGL only
- Node.js and npm (for local server setup)

## Installation

You can easily add the YandexGamesSDK-Unity package to your project using Unity's Package Manager with Git URL:

1. Open your Unity project
2. Go to Window > Package Manager
3. Click the "+" button in the top-left corner
4. Select "Add package from git URL"
5. Enter the following URL:
   ```
   https://github.com/your-repo-url.git?path=/Assets/Plugins/YandexGamesSDK
   ```
   (Replace `your-repo-url` with the actual URL of your GitHub repository)
6. Click "Add"

Unity will now download and import the YandexGamesSDK-Unity package into your project.

## Setting Up the SDK

After installation:

1. Open the Yandex Games SDK Dashboard:
   - Navigate to `Menu > Yandex Games SDK > Dashboard`
2. Create a Configuration Asset:
   - In the dashboard, click on "Create Config Asset"
3. Configure your settings:
   - Enter your Yandex Application ID
   - Set up other preferences as needed

## Setting Up Local Development Server

1. Open the Yandex Games SDK Dashboard in Unity
2. Specify the WebGL build path and desired server port
3. Click "Start Server" to launch the local development environment

## Development Workflow

1. Configure mock data (user profiles, leaderboard entries) via the SDK Dashboard
2. Develop your game using the SDK's features
3. Build your WebGL project
4. Use the LocalServerManager to automatically run a server after the build
5. Test your game in a web browser, simulating the Yandex Games environment

## Key Components

### AuthModule

```csharp
// Example: Authenticate a user
YandexGamesSDK.Auth.Authenticate((success) => {
    if (success) {
        Debug.Log("User authenticated successfully");
    }
});
```

### LeaderboardModule

```csharp
// Example: Submit a score
YandexGamesSDK.Leaderboards.SubmitScore("leaderboardName", score, (success) => {
    if (success) {
        Debug.Log("Score submitted successfully");
    }
});
```

### AdvertisementModule

```csharp
// Example: Show a fullscreen ad
YandexGamesSDK.Ads.ShowFullscreenAd((result) => {
    if (result == AdResult.Closed) {
        Debug.Log("Ad closed by user");
    }
});
```

### StorageModule

```csharp
// Example: Save data to cloud/local storage
YandexGamesSDK.Storage.Save("key", "value", (success) => {
    if (success) {
        Debug.Log("Data saved successfully");
    }
});
```

## Troubleshooting

- **Package Not Found**: Ensure you've entered the correct Git URL in the Package Manager.
- **Node.js/npm Not Found**: Ensure Node.js is correctly installed and added to the system PATH.
- **Server Not Starting**: Verify build path and server port settings in YandexGamesSDKConfig.
- **Advertisements or Leaderboard Issues**: Check Yandex App ID and enable verbose logging for detailed console messages.
- **Permission Errors**: Ensure system permissions allow executing Node.js processes.

## License

YandexGamesSDK-Unity is distributed under the MIT License. See the LICENSE file for more information.

## Support

For additional help, check our documentation or open an issue on our GitHub repository.
