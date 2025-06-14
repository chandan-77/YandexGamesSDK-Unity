using UnityEngine;
using UnityEngine.UI;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime;
using YandexGames.Types.IAP;

public class YandexPurchaseExample : MonoBehaviour
{
    [SerializeField] private Transform shopItemsContainer;
    [SerializeField] private GameObject shopItemPrefab;
    [SerializeField] private Text currencyText;
    [SerializeField] private Text statusText;
    
    private YandexGamesSDK sdk;
    private int playerCurrency = 0;
    
    private void Awake()
    {
        // Disable the script until SDK is initialized
        enabled = false;
    }
    
    private void OnEnable()
    {
        // Get SDK instance
        sdk = YandexGamesSDK.Instance;
        
        // Perform initialization logic here
        InitializeExample();
    }

    private void InitializeExample()
    {
        if (!YandexGamesInitializer.Instance.CheckSDKAvailability())
            return;
            
        // Load available products when the shop opens
        LoadProductCatalog();
        
        // Check for any existing purchases
        CheckPurchasedProducts();
        
        // Initialize currency display
        UpdateCurrencyDisplay();
    }
    
    private void LoadProductCatalog()
    {
        sdk.Purchases.GetProductCatalog((success, response, error) =>
        {
            if (success && response?.products != null)
            {
                DisplayShopItems(response.products);
            }
            else
            {
                SetStatus($"Failed to load shop items: {error}");
            }
        });
    }
    
    private void DisplayShopItems(YGProduct[] products)
    {
        // Clear existing items
        foreach (Transform child in shopItemsContainer)
            Destroy(child.gameObject);
            
        // Display available products
        foreach (var product in products)
        {
            GameObject itemObj = Instantiate(shopItemPrefab, shopItemsContainer);
            
            // Assuming the prefab has these components
            itemObj.transform.Find("Title").GetComponent<Text>().text = product.title;
            itemObj.transform.Find("Description").GetComponent<Text>().text = product.description;
            itemObj.transform.Find("Price").GetComponent<Text>().text = product.price;
            
            // Add purchase button listener
            Button buyButton = itemObj.GetComponentInChildren<Button>();
            buyButton.onClick.AddListener(() => PurchaseItem(product.id));
        }
    }
    
    public void PurchaseItem(string productId)
    {
        if (!YandexGamesInitializer.Instance.CheckSDKAvailability())
            return;
            
        SetStatus("Processing purchase...");
        
        sdk.Purchases.PurchaseProduct(
            productId,
            callback: (success, response, error) =>
            {
                if (success && response != null)
                {
                    SetStatus($"Purchase successful! Purchase token: {response.purchasedProduct.purchaseToken}");
                    
                    // If this is a consumable item, consume it to complete the transaction
                    ConsumeProduct(response.purchasedProduct.purchaseToken);
                }
                else
                {
                    SetStatus($"Purchase failed: {error}");
                }
            },
            developerPayload: "optional-developer-payload"
        );
    }
    
    private void ConsumeProduct(string purchaseToken)
    {
        sdk.Purchases.ConsumeProduct(purchaseToken, (success, error) =>
        {
            if (success)
            {
                SetStatus("Item consumed successfully!");
                AddPurchaseRewards();
            }
            else
            {
                SetStatus($"Failed to consume item: {error}");
            }
        });
    }
    
    private void CheckPurchasedProducts()
    {
        sdk.Purchases.GetPurchasedProducts((success, response, error) =>
        {
            if (success && response?.purchasedProducts != null)
            {
                foreach (var purchase in response.purchasedProducts)
                {
                    Debug.Log($"Existing purchase: {purchase.productID}, Token: {purchase.purchaseToken}");
                    // Handle any unconsumed purchases
                    ConsumeProduct(purchase.purchaseToken);
                }
            }
            else
            {
                Debug.LogError($"Failed to check purchases: {error}");
            }
        });
    }
    
    private void AddPurchaseRewards()
    {
        // Example of adding rewards after successful purchase
        playerCurrency += 100;
        UpdateCurrencyDisplay();
    }
    
    private void UpdateCurrencyDisplay()
    {
        currencyText.text = $"Coins: {playerCurrency}";
    }
    
    private void SetStatus(string message)
    {
        Debug.Log(message);
        statusText.text = message;
    }
} 
