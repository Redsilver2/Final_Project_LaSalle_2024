using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Redsilver2.Core.Interactables
{
    public class Keypad : Puzzle
    {
        [SerializeField] private string code;
        [SerializeField] private TextMeshProUGUI codeDisplayer;

        private UnityEvent<bool> onCodeInputConfirmed;
        private string currentCode;

        private bool isPuzzleFinished = false;

        protected override void Awake()
        {
            base.Awake();
            onCodeInputConfirmed = new UnityEvent<bool>();
        }
      
        public void InputCode(char code)
        {
            if (!isPuzzleFinished)
            {
                currentCode += code;

                if (codeDisplayer != null) 
                {
                    codeDisplayer.text = currentCode;   
                }
            }
        }
        public void ConfirmCode()
        {
            if (IsCompleted())
            {
                isPuzzleFinished = true;

                if (!isPuzzleFinished)
                {
                    onCodeInputConfirmed.Invoke(true);
                }
            }
            else
            {
                onCodeInputConfirmed.Invoke(false);
            }
        }

        public void AddOnCodeInputConfirmedEvent(UnityAction<bool> action)
        {
            onCodeInputConfirmed.AddListener(action);
        }
        public void RemoveOnCodeInputConfirmedEvent(UnityAction<bool> action)
        {
            onCodeInputConfirmed.RemoveListener(action);
        }

        public override bool IsCompleted()
        {
            return currentCode.ToLower().Contains(code.ToLower());
        }
    }
}
