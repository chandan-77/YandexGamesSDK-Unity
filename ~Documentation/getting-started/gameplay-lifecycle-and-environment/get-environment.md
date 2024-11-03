---
icon: chart-gantt
---

# Get Environment

The **Get Environment** feature in the Yandex Games SDK provides detailed information about the player’s environment. This data includes language preferences, platform details, and other contextual information that can help you optimize and personalize the player experience.

**Overview**

The `EnvironmentData` class offers a structured way to access key environmental properties, including:

* **Language and Localization**: `I18n.lang` provides the player’s language setting.
* **Browser and Platform Data**: Understand if the player is using a specific platform like Telegram or a Yandex domain.
* **App and Domain Info**: Access the game’s `app.id`, current domain, and additional URL information.

Using this data effectively allows you to:

* Localize the game experience based on language.
* Tailor gameplay and UI based on platform or device type.
* Collect contextual insights for analytics and optimization.

**Structure of EnvironmentData**

The `EnvironmentData` class has the following properties:

* **`app`**: Contains application-specific data.
  * **`id`**: The application ID for your game.
* **`browser`**: Provides information on the browser language.
  * **`lang`**: The browser language, useful for localizing content.
* **`data`**: Holds URLs and domain data.
  * **`baseUrl`**: The main URL used to serve the game.
  * **`secondDomain`**: A secondary domain that may be used as a fallback.
* **`i18n`**: Handles internationalization settings.
  * **`lang`**: The player’s preferred language.
  * **`tld`**: The top-level domain, useful for region-based content.
* **`isTelegram`**: Indicates if the game is being played on Telegram.
* **`domain`**: The primary domain serving the game.

***

#### **Example Usage**

Below is an example of retrieving environment data and using it to adjust language settings and display relevant domain information.

```csharp
using UnityEngine;

void FetchEnvironmentData()
{
    YandexGamesSDK.Instance.GetEnvironment((success, environmentData, error) =>
    {
        if (success)
        {
            Debug.Log("Environment data retrieved successfully:");
            Debug.Log($"App ID: {environmentData.app.id}");
            Debug.Log($"Browser Language: {environmentData.browser.lang}");
            Debug.Log($"Game Language: {environmentData.i18n.lang}");
            Debug.Log($"Primary Domain: {environmentData.domain}");
            Debug.Log($"Is running on Telegram: {environmentData.isTelegram}");

            ApplyEnvironmentSettings(environmentData);
        }
        else
        {
            Debug.LogError($"Failed to retrieve environment data: {error}");
        }
    });
}

void ApplyEnvironmentSettings(EnvironmentData environmentData)
{
    // Customize game language based on player's preferred language
    if (environmentData.i18n.lang == "tr")
    {
        SetLanguageToRussian();
    }

    // Display special UI if the game is running on Telegram
    if (environmentData.isTelegram)
    {
        DisplayTelegramSpecificUI();
    }

    // Log additional URLs or domains for debugging or analytics
    Debug.Log($"Base URL: {environmentData.data.baseUrl}");
    Debug.Log($"Second Domain: {environmentData.data.secondDomain}");
}

void SetLanguageToRussian()
{
    Debug.Log("Setting game language to Russian.");
    // Code to localize the game text and UI for the Turkish language
}

void DisplayTelegramSpecificUI()
{
    Debug.Log("Enabling Telegram-specific UI features.");
    // Code to activate Telegram-specific UI or features
}
```

* **App ID and Domain**: Use `environmentData.app.id` and `environmentData.domain` for app-specific setups or analytics.
* **Language Settings**: Check `environmentData.i18n.lang` to set language preferences and ensure the game is displayed in the correct language.
* **Platform-Specific UI**: If `environmentData.isTelegram` is true, consider adjusting the UI or gameplay to cater to Telegram users.

***

#### **Use Cases**

1. **Localization and Language Support**
   * Use `i18n.lang` and `browser.lang` to automatically adjust the language, currency, or date formats.
   * Example: Set game text to Russian if `environmentData.i18n.lang == "ru"`.
2. **Platform-Specific Adjustments**
   * If the game runs on Telegram (`isTelegram == true`), show specialized UI or features tailored for Telegram users.
   * Example: Enable specific controls or layouts optimized for Telegram’s layout.
3. **Domain-Based Content and Fallback URLs**
   * Use `data.baseUrl` and `data.secondDomain` to retrieve the game’s URLs, allowing fallback options if the primary domain is unavailable.
   * Example: Use `secondDomain` as a backup for loading assets if `baseUrl` is inaccessible.
4. **App and Analytics Tracking**
   * Use `app.id` and `domain` for internal analytics or tracking player interactions across different versions or entry points of the game.
   * Example: Log `app.id` to track which version of the game is being played most frequently.

***

#### **Best Practices**

1. **Cache Environment Data**: Store the environment data at the start of the session to avoid repeated API calls.
2. **Fallbacks for Language Settings**: Provide a default language setting (like English) if `i18n.lang` does not match your supported languages.
3. **Use Domain Data for Troubleshooting**: Log `baseUrl` and `secondDomain` for easier troubleshooting and analytics tracking.

> For further details on the JavaScript version of this method, refer to the [Yandex Games SDK JavaScript documentation](https://yandex.ru/dev/games/doc/en/sdk/sdk-environment).
