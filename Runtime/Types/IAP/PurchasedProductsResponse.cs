using System;
using UnityEngine.Scripting;

namespace YandexGames.Types.IAP
{
    [Serializable]
    public class PurchasedProductsResponse
    {
        [field: Preserve]
        public ProductListResponse[] purchasedProducts;
        [field: Preserve]
        public string signature;
    }
}