---
icon: wave-pulse
---

# Gameplay Lifecycle Events

The **Gameplay Lifecycle Events** in the Yandex Games SDK are essential for ensuring your game functions optimally on the Yandex platform. These events are required to help Yandex collect crucial data about game loading and user interactions. Accurate usage of these events enhances game visibility, optimizes loading times, and improves player engagement.

**Overview**

Using Gameplay Lifecycle Events correctly enables you to:

* **Notify Yandex when your game is ready to play**: This ensures users experience minimal loading times and smooth transitions.
* **Track the beginning and end of gameplay sessions**: Yandex uses this data to improve recommendations and engagement metrics, helping your game reach more players.

Failure to implement these events properly can result in lower performance metrics and reduced visibility on the Yandex Games platform.

***

#### **1. Game Loading: Signaling Game Readiness**

**When to Use**

Call `SetGameplayReady` once all game resources have loaded, and the game is prepared for user interaction. This event is crucial because it lets Yandex know that your game has completed loading, enabling the platform to optimize the user experience.

**Example**

```csharp
void SignalGameReady()
{
    // Notify Yandex that the game has finished loading and is ready to be played
    YandexGamesSDK.Instance.SetGameplayReady();
    Debug.Log("Game loading complete. Ready for player interaction.");
}
```

* **Important**: Ensure that all loading screens and initialization processes are completed before calling this method. The game should be fully interactive when this is invoked.
* **Why It Matters**: Proper usage of `SetGameplayReady` allows Yandex to gather data on load times, improving platform-wide optimizations and potentially increasing game exposure.

***

#### **2. Starting Gameplay: Marking the Beginning of Player Interaction**

**When to Use**

Call `SetGameplayStart` at key moments when the player actively begins gameplay. This includes:

* Starting a new level
* Closing a pause menu
* Resuming gameplay after viewing an ad or switching back to the game from another app/tab

**Example**

```csharp
void StartGameplay()
{
    // Signal Yandex that gameplay has started or resumed
    YandexGamesSDK.Instance.SetGameplayStart();
    Debug.Log("Gameplay session started or resumed.");
}
```

* **Important**: This event should only be called when the player actively begins playing, not when they are in menus or non-interactive screens.
* **Why It Matters**: Yandex uses this data to measure player engagement accurately, which can improve your game’s ranking and recommendations on the platform.

***

#### **3. Stopping Gameplay: Pausing or Ending the Game Session**

**When to Use**

Call `SetGameplayStop` at moments when gameplay is paused or stopped. This includes:

* Pausing the game (e.g., when a menu opens)
* Completing or failing a level
* Displaying a full-screen ad
* Player switching to another browser tab or application

**Example**

```csharp
void StopGameplay()
{
    // Notify Yandex that gameplay has been paused or stopped
    YandexGamesSDK.Instance.SetGameplayStop();
    Debug.Log("Gameplay session paused or ended.");
}
```

* **Important**: Ensure this method is called immediately after gameplay stops to provide accurate data to Yandex.
* **Why It Matters**: Accurate usage of `SetGameplayStop` helps Yandex understand how players interact with your game, which is critical for engagement metrics and catalog placement.

***

#### **Best Practices for Gameplay Lifecycle Events**

1. **Call Events at the Right Time**: Ensure `SetGameplayStart` and `SetGameplayStop` are only called when gameplay genuinely starts and stops. Avoid using these events for transitions or non-interactive states.
2. **Avoid Redundant Calls**: Call `SetGameplayReady` only once, after the game is fully loaded. `SetGameplayStart` and `SetGameplayStop` should be used to toggle gameplay states without repeating calls unnecessarily.
3. **Prioritize User Experience**: Accurate lifecycle event usage helps Yandex display your game in an optimized way, benefiting players by providing a smooth and consistent experience.
4. **Testing**: Test these events thoroughly in different scenarios, such as when returning from an ad, switching between tabs, or pausing and resuming gameplay. Ensure they are triggered at the correct times.

***

#### **Frequently Asked Questions**

* **Q: What happens if I don’t use these events correctly?**\
  A: Not implementing these events can lead to inaccurate analytics, reduced visibility, and lower performance metrics for your game on Yandex Games.
* **Q: Can I use `SetGameplayStart` without `SetGameplayReady`?**\
  A: No. `SetGameplayReady` should be called first, as it indicates the game is fully loaded and ready for interaction.
* **Q: When should I call `SetGameplayStop`?**\
  A: Call `SetGameplayStop` whenever gameplay is paused or interrupted, such as when the player navigates away from the game or opens a menu.

***

> For further details on the JavaScript version of this method, refer to the [Yandex Games SDK JavaScript documentation](https://yandex.ru/dev/games/doc/en/sdk/sdk-game-events).
