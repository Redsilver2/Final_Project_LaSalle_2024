using UnityEngine;

namespace Redsilver2.Core.Interactables
{
    public interface IDescribable
    {
        string GetName();
        Sprite GetInteractionSprite();

    }
}
