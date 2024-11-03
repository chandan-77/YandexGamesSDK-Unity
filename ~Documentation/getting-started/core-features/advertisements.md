---
icon: adversal
---

# Advertisements

The **Advertisements** feature in the Yandex Games SDK for Unity allows you to monetize your game through various ad formats. Yandex provides support for rewarded ads, interstitial ads, and banner ads, enabling you to generate revenue while enhancing user engagement.

**Overview**

With Advertisements, you can:

* **Show Rewarded Ads**: Engage players by offering rewards (like in-game currency) in exchange for watching an ad.
* **Display Interstitial Ads**: Show full-screen ads at appropriate moments in your game.
* **Use Banner Ads**: Display banner ads at a fixed position on the screen.

**Basic Usage Examples**

Below are examples of using each ad type in your Unity game.

***

#### **1. Showing Rewarded Ads**

Rewarded ads offer a way to give players rewards in exchange for watching an ad. This can be used for bonuses, extra lives, or in-game currency.

```csharp
void ShowRewardedAd()
{
    YandexGamesSDK.Instance.Advertisement.ShowRewardedAd((success, adResponse, error) =>
    {
        if (success)
        {
            switch (adResponse)
            {
                case YGAdResponse.AdOpened:
                    Debug.Log("Rewarded ad opened.");
                    break;
                case YGAdResponse.AdGranted:
                    Debug.Log("Reward granted for watching ad.");
                    GrantRewardToPlayer(); // Give reward to player
                    break;
                case YGAdResponse.AdClosed:
                    Debug.Log("Rewarded ad closed.");
                    break;
                default:
                    Debug.Log($"Unknown ad response: {adResponse}");
                    break;
            }
        }
        else
        {
            Debug.LogError($"Failed to show rewarded ad: {error}");
            // Optionally, handle ad failure by showing a message or retrying.
        }
    });
}

void GrantRewardToPlayer()
{
    // Implement the logic to reward the player (e.g., add in-game currency)
    Debug.Log("Player has been rewarded!");
}
```

* **Callback**: Checks if the ad was successfully shown.
* **Ad Responses**:
  * **AdOpened**: The ad was successfully opened.
  * **AdGranted**: The ad was watched fully, and the reward can be granted.
  * **AdClosed**: The ad was closed without granting a reward.

***

#### **2. Displaying Interstitial Ads**

Interstitial ads are full-screen ads that appear at natural breaks in the game, such as level transitions.

```csharp
void ShowInterstitialAd()
{
    YandexGamesSDK.Instance.Advertisement.ShowInterstitialAd((success, adResponse, error) =>
    {
        if (success)
        {
            switch (adResponse)
            {
                case YGAdResponse.AdOpened:
                    Debug.Log("Interstitial ad opened.");
                    break;
                case YGAdResponse.AdClosed:
                    Debug.Log("Interstitial ad closed.");
                    break;
                default:
                    Debug.Log($"Unknown ad response: {adResponse}");
                    break;
            }
        }
        else
        {
            Debug.LogError($"Failed to show interstitial ad: {error}");
        }
    });
}
```

* **Ad Responses**:
  * **AdOpened**: The ad was successfully opened.
  * **AdClosed**: The ad was closed by the player.
* **Callback**: Provides feedback on whether the ad was shown successfully or if there was an error.

***

#### **3. Displaying Banner Ads**

Banner ads are displayed at a fixed position on the screen, typically at the top or bottom, and remain visible during gameplay.

```csharp
void ShowBannerAd()
{
    string position = "bottom"; // Position can be "top" or "bottom"

    YandexGamesSDK.Instance.Advertisement.ShowBannerAd(position, (success, adResponse, error) =>
    {
        if (success)
        {
            if (adResponse == YGAdResponse.AdShown)
            {
                Debug.Log("Banner ad shown successfully.");
            }
            else
            {
                Debug.Log($"Unknown banner ad response: {adResponse}");
            }
        }
        else
        {
            Debug.LogError($"Failed to show banner ad: {error}");
        }
    });
}
```

* **Position**: Specify `"top"` or `"bottom"` for the banner ad position on the screen.
* **Ad Response**:
  * **AdShown**: The banner ad is displayed.
* **Callback**: Logs whether the banner ad was successfully shown or if an error occurred.

***

#### **Additional Tips**

1. **Frequency of Ads**: Avoid showing ads too frequently, as this may disrupt gameplay. Use interstitial ads at natural breaks and limit rewarded ads to specific actions.
2. **Testing Ads**: During development, test ad functionality thoroughly to ensure a smooth experience for players.
3. **Error Handling**: Provide feedback for players if an ad fails to load (e.g., network issues) and consider offering alternative actions.

> For further details on the JavaScript version of this method, refer to the [Yandex Games SDK JavaScript documentation](https://yandex.ru/dev/games/doc/en/sdk/sdk-adv).
