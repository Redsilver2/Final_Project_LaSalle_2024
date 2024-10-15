using Redsilver2.Core.Economy;
using UnityEngine;
using UnityEngine.UI;

namespace Redsilver2.Core.Interactables
{
    [RequireComponent(typeof(Shop))]
    public class ShopComputer : Computer
    {
        [SerializeField] private Button buyButton;
        [SerializeField] private Button shopButton;

    }
}
