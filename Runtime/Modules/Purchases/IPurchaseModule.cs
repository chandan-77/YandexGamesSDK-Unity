using System;
using  YandexGames.Types.IAP;

namespace PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Modules.Purchases
{
    public interface IPurchaseModule
    {
        void PurchaseProduct(string productId, Action<bool, PurchaseProductResponse, string> callback = null, string developerPayload = "");
        void ConsumeProduct(string purchaseToken, Action<bool, string> callback = null);
        void GetProductCatalog(Action<bool, ProductListResponse, string> callback = null);
        void GetPurchasedProducts(Action<bool, PurchasedProductsResponse, string> callback = null);
    }
} 