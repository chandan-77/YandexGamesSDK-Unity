using AOT;
using System;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Modules.Abstractions;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Networking;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Logging;
using  YandexGames.Types.IAP;
using UnityEngine;

namespace PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Modules.Purchases
{
    public class PurchaseModule : YGModuleBase, IPurchaseModule
    {
        #region Static Callbacks

        private static Action<bool, PurchaseProductResponse, string> s_purchaseProductCallback;
        private static Action<bool, string> s_consumeProductCallback;
        private static Action<bool, ProductListResponse, string> s_productCatalogCallback;
        private static Action<bool, PurchasedProductsResponse, string> s_purchasedProductsCallback;

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void HandlePurchaseProductResponse(string jsonResponse)
        {
            var response = JsonConvert.DeserializeObject<JSResponse<PurchaseProductResponse>>(jsonResponse);
            if (response.status && response.data != null)
            {
                s_purchaseProductCallback?.Invoke(true, response.data, null);
            }
            else
            {
                s_purchaseProductCallback?.Invoke(false, null, response.error);
            }
        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void HandleConsumeProductResponse(string jsonResponse)
        {
            var response = JsonConvert.DeserializeObject<JSResponse<object>>(jsonResponse);
            s_consumeProductCallback?.Invoke(response.status, response.error);
        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void HandleProductCatalogResponse(string jsonResponse)
        {
            var response = JsonConvert.DeserializeObject<JSResponse<ProductListResponse>>(jsonResponse);
            if (response.status && response.data != null)
            {
                s_productCatalogCallback?.Invoke(true, response.data, null);
            }
            else
            {
                s_productCatalogCallback?.Invoke(false, null, response.error);
            }
        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void HandlePurchasedProductsResponse(string jsonResponse)
        {
            var response = JsonConvert.DeserializeObject<JSResponse<PurchasedProductsResponse>>(jsonResponse);
            if (response.status && response.data != null)
            {
                s_purchasedProductsCallback?.Invoke(true, response.data, null);
            }
            else
            {
                s_purchasedProductsCallback?.Invoke(false, null, response.error);
            }
        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void HandleErrorResponse(string errorJson)
        {
            var response = JsonConvert.DeserializeObject<JSResponse<object>>(errorJson);
            s_purchaseProductCallback?.Invoke(false, null, response.error);
            s_consumeProductCallback?.Invoke(false, response.error);
            s_productCatalogCallback?.Invoke(false, null, response.error);
            s_purchasedProductsCallback?.Invoke(false, null, response.error);
        }

        #endregion

        #region DllImports

        [DllImport("__Internal")]
        private static extern void PurchaseApi_PurchaseProduct(string productId, string developerPayload,
            Action<string> successCallback, Action<string> errorCallback);

        [DllImport("__Internal")]
        private static extern void PurchaseApi_ConsumeProduct(string purchaseToken,
            Action<string> successCallback, Action<string> errorCallback);

        [DllImport("__Internal")]
        private static extern void PurchaseApi_GetProductCatalog(Action<string> successCallback,
            Action<string> errorCallback);

        [DllImport("__Internal")]
        private static extern void PurchaseApi_GetPurchasedProducts(Action<string> successCallback,
            Action<string> errorCallback);

        #endregion

        public override void Initialize()
        {
            YGLogger.Debug("Purchase Module initialized");
        }

        public void PurchaseProduct(string productId, Action<bool, PurchaseProductResponse, string> callback = null, string developerPayload = "")
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            if (!YandexGamesSDK.IsInitialized)
            {
                YGLogger.Error("SDK is not initialized. Call Initialize first.");
                callback?.Invoke(false, null, "SDK is not initialized");
                return;
            }

            s_purchaseProductCallback = callback;
            PurchaseApi_PurchaseProduct(productId, developerPayload, HandlePurchaseProductResponse, HandleErrorResponse);
#else
            YGLogger.Debug("Purchases are only available in WebGL builds.");
            callback?.Invoke(false, null, "Purchases are only available in WebGL builds.");
#endif
        }

        public void ConsumeProduct(string purchaseToken, Action<bool, string> callback = null)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            if (!YandexGamesSDK.IsInitialized)
            {
                YGLogger.Error("SDK is not initialized. Call Initialize first.");
                callback?.Invoke(false, "SDK is not initialized");
                return;
            }

            s_consumeProductCallback = callback;
            PurchaseApi_ConsumeProduct(purchaseToken, HandleConsumeProductResponse, HandleErrorResponse);
#else
            YGLogger.Debug("Purchases are only available in WebGL builds.");
            callback?.Invoke(false, "Purchases are only available in WebGL builds.");
#endif
        }

        public void GetProductCatalog(Action<bool, ProductListResponse, string> callback = null)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            if (!YandexGamesSDK.IsInitialized)
            {
                YGLogger.Error("SDK is not initialized. Call Initialize first.");
                callback?.Invoke(false, null, "SDK is not initialized");
                return;
            }

            s_productCatalogCallback = callback;
            PurchaseApi_GetProductCatalog(HandleProductCatalogResponse, HandleErrorResponse);
#else
            YGLogger.Debug("Purchases are only available in WebGL builds.");
            callback?.Invoke(false, null, "Purchases are only available in WebGL builds.");
#endif
        }

        public void GetPurchasedProducts(Action<bool, PurchasedProductsResponse, string> callback = null)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            if (!YandexGamesSDK.IsInitialized)
            {
                YGLogger.Error("SDK is not initialized. Call Initialize first.");
                callback?.Invoke(false, null, "SDK is not initialized");
                return;
            }

            s_purchasedProductsCallback = callback;
            PurchaseApi_GetPurchasedProducts(HandlePurchasedProductsResponse, HandleErrorResponse);
#else
            YGLogger.Debug("Purchases are only available in WebGL builds.");
            callback?.Invoke(false, null, "Purchases are only available in WebGL builds.");
#endif
        }

        private void OnDestroy()
        {
            s_purchaseProductCallback = null;
            s_consumeProductCallback = null;
            s_productCatalogCallback = null;
            s_purchasedProductsCallback = null;
        }
    }
} 