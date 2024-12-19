using UnityEngine;
using UnityEngine.Events;

namespace Redsilver2.Core.Interactables
{
    public abstract class Puzzle : Lookable
    {
        [SerializeField] private float maxPuzzleProgression;

        [Space]
        [SerializeField] private UnityEvent onResetPuzzle;
        [SerializeField] private UnityEvent onPuzzleCompleted;

        private float puzzleProgressionValue;

        protected bool isPuzzleCompleted = false;


        public void Progress(float progress)
        {
            if (isPuzzleCompleted == false)
            {
                puzzleProgressionValue += progress;

                if (puzzleProgressionValue > maxPuzzleProgression)
                {
                    puzzleProgressionValue = maxPuzzleProgression;
                    isPuzzleCompleted = true;

                }
            }
        }

        public virtual void ResetPuzzle()
        {
            onResetPuzzle.Invoke();
        }
        public bool IsCompleted() => puzzleProgressionValue >= maxPuzzleProgression;


        public void AddOnPuzzleReset(UnityAction action)
        {
            onResetPuzzle.AddListener(action);
        }
        public void RemoveOnPuzzleReset(UnityAction action)
        {
            onResetPuzzle.RemoveListener(action);
        }
        public void AddOnPuzzleCompleted(UnityAction action)
        {
            onPuzzleCompleted.AddListener(action);
        }
        public void RemoveOnPuzzleCompleted(UnityAction action)
        {
            onPuzzleCompleted.RemoveListener(action);
        }

    }
}
