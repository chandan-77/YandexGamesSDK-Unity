using System;
using UnityEngine.Scripting;

namespace YandexGames.Types.IAP
{
    [Serializable]
    public class PurchaseProductResponse
    {
        [field: Preserve]
        public YGPurchasedProduct purchaseData;
        [field: Preserve]
        public string signature;
    }
}