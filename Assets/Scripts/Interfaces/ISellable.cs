using UnityEngine;

namespace Redsilver2.Core.Economy
{
    public interface ISellable
    {
        string GetName();
        string GetDescription();
        uint GetSellPrice();

        Sprite GetShopSprite();

        void Sell(Shop shop);
        void SetSellPrice(uint price);
    }
}
