using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Redsilver2.Core.Interactables
{
    [RequireComponent(typeof(AudioSource))]
    public class ColorSequencePuzzle : Puzzle
    {
        [Space]
        [SerializeField] private int maxAmountOfPatterns = 4;
      
        private int numbersOfPatterns = 1;
        private List<ColorSequenceButton> buttons;
        private Queue<ColorSequenceButton> buttonPatterns;

        protected override void Awake()
        {
            base.Awake();
            buttons = new List<ColorSequenceButton>();
            buttonPatterns = new Queue<ColorSequenceButton>();

            AddOnPuzzleReset(() =>
            {
                numbersOfPatterns = 1;
                StopAllCoroutines();

                foreach (ColorSequenceButton button in buttons)
                {
                    if (button != null)
                    {
                        button.ResetColor();
                    }
                }
            });

            if (clampedController != null) 
            {
                clampedController.AddOnPathFollowCompletedEvent(() =>
                {
                    if (isInteracting)
                    {
                        Debug.Log(":o");
                        GenerateRandomPattern();
                    }
                    else
                    {
                        ResetPuzzle();
                    }
                });
            }
        }

        

 
        private void GenerateRandomPattern()
        {
            if (!isPuzzleCompleted)
            {
                buttonPatterns.Clear();

                for (int i = 0; i < numbersOfPatterns; i++)
                {
                    buttonPatterns.Enqueue(buttons[Random.Range(0, buttons.Count)]);
                }

                StartCoroutine(ShowPattern(buttonPatterns.ToArray()));
            }
        }

        public void VerifyPattern(ColorSequenceButton button)
        {
            ColorSequenceButton colorSequenceButton = buttonPatterns.Dequeue();

            if(button == null)
            {
                return;
            }

            if (colorSequenceButton == button)
            {
                Debug.Log("??????????");
                if (buttonPatterns.Count == 0)
                {
                    if (numbersOfPatterns == maxAmountOfPatterns)
                    {
                        Progress(1000f);
                    }
                    else
                    {
                        numbersOfPatterns++;
                        GenerateRandomPattern();
                    }
                }
                
            }
            else
            {
                ResetPuzzle();
            }
        }

        private IEnumerator ShowPattern(ColorSequenceButton[] colorSequenceButtons)
        {
            if (colorSequenceButtons != null)
            {
                for (int i = 0;i < colorSequenceButtons.Length; i++)
                {
                   yield return colorSequenceButtons[i].ShowColors(1f);
                }
            }
        }

        public void AddButton(ColorSequenceButton button)
        {
            if(button != null)
            {
                buttons.Add(button);
            }
        }
    }
}
