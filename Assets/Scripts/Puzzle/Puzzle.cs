using UnityEngine;
using UnityEngine.Events;

namespace Redsilver2.Core.Interactables
{
    public abstract class Puzzle : Lookable
    {
        public abstract bool IsCompleted();
        public static UnityEvent<Puzzle> onPuzzleCompleted = new UnityEvent<Puzzle>();
    }
}
