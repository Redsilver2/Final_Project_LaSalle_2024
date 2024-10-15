using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Redsilver2.Core.Economy
{
    public class Shop : MonoBehaviour
    {
        [SerializeField] private GameObject shop;
        [SerializeField] private Transform shopParent;

        [Space]
        [SerializeField] private Button          buyButton;
        [SerializeField] private TextMeshProUGUI descriptionUI;
        [SerializeField] private TextMeshProUGUI moneyUI;

        [Space]
        [SerializeField] private AudioSource shopSource;

        [Space]
        [SerializeField] private AudioClip affordableClip;
        [SerializeField] private AudioClip unaffordableClip;

        [Space]
        [SerializeField] private AudioClip sellClip;

        private int shopItemIndex;
        private int previousShopItemIndex;

        private string textPlayerMoneyAmount = "0";
        private static uint playerMoneyAmount;

        private List<IBuyable>  buyables;
        private List<ISellable> sellables;

        private IBuyable  selectedBuyable;
        private ISellable selectedSellable;

        private IEnumerator updateMoneyUICoroutine;
       
        private UnityEvent<int> onBuyableIndexChanged   = new UnityEvent<int>();
        private UnityEvent<int> onSellableIndexChanged  = new UnityEvent<int>();

        private UnityEvent      onUpdateBuyableUI       = new UnityEvent();
        private UnityEvent      onUpdateSellableUI      = new UnityEvent();


        private void Start()
        {
            buyables  = new List<IBuyable>();
            sellables = new List<ISellable>();

            onBuyableIndexChanged.AddListener(index =>
            {
                if (index != previousShopItemIndex)
                {
                    selectedBuyable = buyables[index];
                    previousShopItemIndex = index;
                    onUpdateBuyableUI.Invoke();
                }
            });
            onUpdateBuyableUI.AddListener(() =>
            {
                bool canBuy = selectedBuyable.CanBuy(playerMoneyAmount);
                Color uiColor = canBuy ?
                                Color.white : Color.red;

                UpdateMoneyUI(3f);

                //if (shopItemBuyButton)
                //{
                //    TextMeshProUGUI shopItemBuyButtonUI = shopItemBuyButton.GetComponentInChildren<TextMeshProUGUI>();

                //    if (shopItemBuyButtonUI != null)
                //    {
                //        shopItemBuyButtonUI.color = uiColor;
                //        shopItemBuyButtonUI.text = canBuy ? "Buy" : "Can't Afford";
                //    }
                //}

                //if (shopItemIcon)
                //{
                //    shopItemIcon.color = uiColor;
                //    shopItemIcon.sprite = selectedShopItem.Icon;
                //}

                //if (shopItemDescriptionUI)
                //{
                //    shopItemDescriptionUI.color = uiColor;
                //    shopItemDescriptionUI.text = $"Name: {selectedShopItem.Name}\n\n" +
                //                                  $"Description: {selectedShopItem.Description}\n\n" +
                //                                  $"Price: {selectedShopItem.Price}$\n\n" +
                //                                  $"Quantity Left: {selectedShopItem.QuantityLeft}";
                //}

            });
        }

        private void ShowShopItem(int index)
        {
            onBuyableIndexChanged?.Invoke(index);
        }

        private void Buy()
        {
            selectedBuyable.Buy(this, out bool isBought);
            PlaySound(isBought ? affordableClip : unaffordableClip);

            if (isBought)
            {
                onUpdateBuyableUI.Invoke();
            }
        }
        public void AddBuyableItem(GameObject prefab, float price)
        {
            if(prefab != null)
            {
                if(prefab.TryGetComponent(out IBuyable buyable))
                {

                    buyables.Add(buyable);
                }
            }
        }

        private void UpdateMoneyUI(float duration)
        {
            if (updateMoneyUICoroutine != null)
            {
                StopCoroutine(updateMoneyUICoroutine);
            }

            updateMoneyUICoroutine = UpdatePlayerMoneyUICoroutine(duration);
            StartCoroutine(updateMoneyUICoroutine);
        }
        private void PlaySound(AudioClip clip)
        {
            if (shopSource != null && clip != null)
            {
                shopSource.clip = clip;
                shopSource?.Play();
            }
        }

        private IEnumerator UpdatePlayerMoneyUICoroutine(float duration)
        {
            float t = 0f;
            float value = float.Parse(textPlayerMoneyAmount);

            if (value != playerMoneyAmount)
            {
                while (t < duration && moneyUI)
                {
                    value = Mathf.Lerp(value, playerMoneyAmount, t / duration);
                    textPlayerMoneyAmount = ((int)value).ToString();
                    moneyUI.text = textPlayerMoneyAmount + "$";
                    yield return null;
                }

                if (t >= duration)
                {
                    textPlayerMoneyAmount = playerMoneyAmount.ToString();

                    if (moneyUI)
                    {
                        moneyUI.text = textPlayerMoneyAmount + "$";
                    }
                }
            }
        }
    }
}
