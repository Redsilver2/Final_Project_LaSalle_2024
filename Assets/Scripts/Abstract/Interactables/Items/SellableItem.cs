using Redsilver2.Core.Economy;
using UnityEngine;

namespace Redsilver2.Core.Items
{
    public abstract class SellableItem : EquippableItem, ISellable
    {
        [Space]
        [SerializeField] private Sprite itemShopIcon;
        [SerializeField] private string description;

        [Space]
        [SerializeField] private uint minSellPrice;
        [SerializeField] private uint maxSellPrice;

        private uint currentSellPrice;

        protected override void Start()
        {

            base.Start();
        }

        public virtual string GetDescription()
        {
            return $"Description: {description}$";
        }

        public uint GetSellPrice()
        {
            return currentSellPrice;
        }
        public Sprite GetShopSprite()
        {
            return itemShopIcon;
        }

        public void Sell(Shop shop)
        { 

        }  

        public void SetSellPrice(uint price)
        {
            currentSellPrice = price;
        }
    }
}
