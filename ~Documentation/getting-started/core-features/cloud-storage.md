---
icon: cloud
---

# Cloud Storage

The **Cloud Storage** feature in the Yandex Games SDK for Unity allows you to save and retrieve player data on Yandex’s cloud server. This enables the persistence of game progress, allowing players to access their data across devices and sessions.

**Overview**

With Cloud Storage, you can:

* **Save player data**: Store game progress, scores, or custom data for each player.
* **Load player data**: Retrieve previously saved data to resume game progress.
* **Flush data**: Commit pending saves to ensure data consistency.

**Basic Usage Examples**

Below are quick examples for saving and loading data in the cloud without the need for complex structures or JSON conversions.

***

#### **1. Saving Data to the Cloud**

To save data, specify a **key** and the **data object** you want to store. The SDK automatically handles data serialization.

```csharp
// Define the player data structure
[System.Serializable]
public struct PlayerData
{
    public string playerName;
    public int playerLevel;
}

// Example of saving data
void SavePlayerData()
{
    PlayerData playerData = new PlayerData { playerName = "TestPlayer", playerLevel = 5 };
    
    YandexGamesSDK.Instance.CloudStorage.Save("playerProfile", playerData, (success, error) =>
    {
        if (success)
        {
            Debug.Log("Player data saved successfully.");
        }
        else
        {
            Debug.LogError($"Failed to save player data: {error}");
        }
    });
}
```

* **Key**: `"playerProfile"` is used as the identifier for the data.
* **Data**: `playerData` is stored directly; no JSON conversion is needed.
* **Callback**: The callback checks if the save operation was successful.

***

#### **2. Loading Data from the Cloud**

To load data, use the same **key** that was used to save it.

```csharp
void LoadPlayerData()
{
    YandexGamesSDK.Instance.CloudStorage.Load<PlayerData>("playerProfile", (success, data, error) =>
    {
        if (success)
        {
            Debug.Log($"Loaded player data: Name={data.playerName}, Level={data.playerLevel}");
            ApplyLoadedData(data);
        }
        else
        {
            Debug.LogError($"Failed to load player data: {error}");
        }
    });
}

void ApplyLoadedData(PlayerData data)
{
    // Example usage of loaded data (e.g., update UI or game state)
    Debug.Log($"Welcome back, {data.playerName}! Level: {data.playerLevel}");
}
```

* **Key**: The `"playerProfile"` key is used to retrieve the data.
* **Data Type**: Specify `PlayerData` the type for the loaded data.
* **Callback**: If data loads successfully, it’s passed to `ApplyLoadedData` for further use.

***

#### **3. Flushing Data to Ensure Consistency**

Flushing data ensures that all pending saves are committed to the cloud immediately. This is useful to minimize data loss in case of unexpected game closures.

```csharp
void FlushData()
{
    YandexGamesSDK.Instance.CloudStorage.FlushData((success, error) =>
    {
        if (success)
        {
            Debug.Log("Data flushed successfully.");
        }
        else
        {
            Debug.LogError($"Failed to flush data: {error}");
        }
    });
}
```

Flushing data is recommended after critical saves, such as level completion or other important milestones.

***

#### **Additional Tips**

1. **Unique Keys**: Use unique keys for different types of data to prevent conflicts (e.g., `"playerProfile"`, `"gameSettings"`).
2. **Data Integrity**: Regularly save and flush data at key game points to maintain data integrity.

> For further details on the JavaScript version of this method, refer to the [Yandex Games SDK JavaScript documentation](https://yandex.ru/dev/games/doc/en/sdk/sdk-player).
