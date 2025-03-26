using System;
using UnityEngine.Scripting;

namespace YandexGames.Types.IAP
{
    [Serializable]
    public class ProductListResponse
    {
        [field: Preserve]
        public YGProduct[] products;
    }
}