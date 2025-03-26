using System;
using UnityEngine.Scripting;

namespace YandexGames.Types.IAP
{
    [Serializable]
    public class YGPurchasedProduct
    {
        [field: Preserve]
        public string developerPayload;
        [field: Preserve]
        public string productID;
        [field: Preserve]
        public string purchaseTime;
        [field: Preserve]
        public string purchaseToken;
    }
}