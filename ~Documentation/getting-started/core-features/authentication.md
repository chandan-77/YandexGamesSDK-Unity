---
description: >-
  Securely manage user authentication with Yandex accounts, allowing players to
  log in and save their progress.
icon: user
---

# Authentication

The **Authentication** feature in the Yandex Games SDK for Unity allows players to log in using their Yandex accounts, enabling access to personalized features such as saving progress and accessing leaderboards.

**Overview**

With authentication:

* Players can securely log in using their Yandex accounts.
* You can retrieve and display player profile information (e.g., name and profile picture).
* You can enable continuity across devices by linking game data to user accounts.

**Basic Authentication Example**

Here’s a simplified example for authenticating a user in Unity. This mirrors the JavaScript version of Yandex’s authentication flow but is adapted for Unity:

```csharp
YandexGamesSDK.Instance.Authentication.AuthenticateUser(requireSignin: true, (success, message) =>
{
    if (!success)
    {
        Debug.LogError($"Authentication failed: {message}");
    }
    else
    {
        var userProfile = YandexGamesSDK.Instance.Authentication.GetUserProfile();
        Debug.Log($"Authenticated user: {userProfile.name}");
    }
});
```

* **`requireSignin: true`**: Prompts the user to sign in if not already authenticated.
* **Callback**: If `success` is `true`, the user's profile information (like their name) is accessible via `GetUserProfile()`.

> For further details on the JavaScript version of this method, refer to the [Yandex Games SDK JavaScript documentation](https://yandex.ru/dev/games/doc/en/sdk/sdk-player).
