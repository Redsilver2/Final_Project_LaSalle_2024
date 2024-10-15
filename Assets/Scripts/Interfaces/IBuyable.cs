using UnityEngine;

namespace Redsilver2.Core.Economy
{
    public interface IBuyable
    {
        string GetName();
        string GetDescription();
       
        uint GetBuyPrice();
        bool CanBuy(uint moneyAmount);

        Sprite GetShopSprite();

        void Buy(Shop shop, out bool isBought);
        void SetBuyPrice(uint price);


    }
}
