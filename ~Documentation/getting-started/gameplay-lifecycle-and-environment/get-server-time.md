---
icon: hourglass-end
---

# Get Server Time

The **Get Server Time** feature in the Yandex Games SDK allows your game to retrieve the current server time from Yandex. This can be useful for time-based events, daily rewards, or synchronizing gameplay elements that depend on an accurate and unified clock.

**Overview**

Using the **Get Server Time** method allows you to:

* **Sync Timed Events**: Create time-dependent features like daily login rewards, timed challenges, or cooldowns.
* **Standardize Time Across Devices**: Ensure that all players experience the same time-based events, regardless of their local device time.

***

#### **Example Usage**

Here’s a basic example of retrieving the current server time in Unity and displaying it in the console.

```csharp
void FetchServerTime()
{
    YandexGamesSDK.Instance.GetServerTime((success, serverTime, error) =>
    {
        if (success)
        {
            Debug.Log($"Current server time: {serverTime}");
            HandleServerTime(serverTime);
        }
        else
        {
            Debug.LogError($"Failed to retrieve server time: {error}");
        }
    });
}

void HandleServerTime(DateTime serverTime)
{
    // Use the server time as needed (e.g., for timed events or daily rewards)
    Debug.Log($"Server time processed: {serverTime}");
}
```

* **Callback Parameters**:
  * **success**: Indicates if the server time was retrieved successfully.
  * **serverTime**: The current time from the Yandex server as an `DateTime` object.
  * **error**: Contains an error message if the request fails.
* **Example Use Case**:
  * Call `FetchServerTime()` when the game starts to sync daily events.
  * Use `HandleServerTime(serverTime)` to process or display the server time in-game.

***

#### **Best Practices**

1. **Use for Important Timed Events**: Call `GetServerTime` for critical events that need consistent timing, such as daily logins, special offers, or challenge timers.
2. **Handle Errors Gracefully**: If server time retrieval fails, consider fallback options, such as showing an error message or using a default timer until the next request.
3. **Reduce Frequency of Calls**: To avoid unnecessary server requests, call `GetServerTime` only when needed (e.g., at the start of a session) and cache the time if applicable.

> For further details on the JavaScript version of this method, refer to the [Yandex Games SDK JavaScript documentation](https://yandex.ru/dev/games/doc/en/sdk/sdk-server-time).
