# YandexGamesSDK-Unity

[![Documentation](https://img.shields.io/badge/documentation-website-blue)](https://yandex-games-docs.playables.studio/)

## Documentation

For detailed documentation, tutorials, and integration guides, please visit our [documentation website](https://yandex-games-docs.playables.studio/).

## Overview

YandexGamesSDK-Unity is an advanced integration package that connects Unity WebGL projects with Yandex Games services. This repository contains the core SDK implementation and is intended for developers who want to contribute to the project or understand its technical details.

## Technical Requirements

- Unity Version: 2020.3 or higher
- Build Target: WebGL only
- Node.js and npm (for local server setup)

## Repository Structure

```
Assets/
├── Plugins/
│   └── YandexGamesSDK/
│       ├── Runtime/         # Core SDK implementation
│       ├── Editor/          # Unity editor extensions
│       └── LocalServer/     # Development server utilities
```

## Development Setup

1. Clone the repository:
   ```bash
   git clone https://github.com/playables-studio/YandexGamesSDK-Unity.git
   ```

2. Open the project in Unity 2020.3 or higher

3. Install Node.js dependencies for local development server:
   ```bash
   cd Assets/Plugins/YandexGamesSDK/LocalServer
   npm install
   ```

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Coding Standards

- Follow Unity's C# coding conventions
- Maintain XML documentation for public APIs
- Include unit tests for new features
- Update relevant documentation when making changes

## Building from Source

1. Open the project in Unity
2. Set build target to WebGL
3. Build the project using Unity's standard build process

## Testing

- Unit tests are located in the `Tests` folder
- Run tests through Unity Test Runner
- Use the LocalServerManager for integration testing

## License

YandexGamesSDK-Unity is distributed under the MIT License. See the [LICENSE](LICENSE) file for more information.

## Support

- For SDK usage questions, refer to our [documentation](https://yandex-games-docs.playables.studio/)
- For bugs and feature requests, open an issue
- For contribution questions, start a discussion

## Core Modules Reference

For developers working with the source code:

### AuthModule
Handles user authentication and session management.

### LeaderboardModule
Manages leaderboard submissions and retrievals.

### AdvertisementModule
Controls advertisement display and lifecycle events.

### StorageModule
Handles cloud and local storage operations.

## Troubleshooting Development Issues

- **Package Not Found**: Verify Git submodules are initialized
- **Node.js/npm Not Found**: Check system PATH configuration
- **Server Not Starting**: Check port availability and permissions
- **Build Errors**: Ensure Unity version compatibility
