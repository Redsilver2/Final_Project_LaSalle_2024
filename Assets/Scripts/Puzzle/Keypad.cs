using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Redsilver2.Core.Interactables
{
    [RequireComponent(typeof(AudioSource))]
    public class Keypad : Puzzle
    {
        [SerializeField] private string code;
        [SerializeField] private TextMeshProUGUI codeDisplayer;

        [Space]
        [SerializeField] private AudioClip[] clickKeypadButtonSounds;

        private UnityEvent<bool> onCodeInputConfirmed;
        private string currentCode;

        private AudioSource audioSource;

        protected override void Awake()
        {
            base.Awake();
            onCodeInputConfirmed = new UnityEvent<bool>();
            audioSource = GetComponent<AudioSource>();

            AddOnPuzzleCompleted(() =>
            {
                onCodeInputConfirmed.Invoke(true);
            });
        }

        public void PlaySound()
        {
            if (clickKeypadButtonSounds.Length > 0) {
                PlaySound(clickKeypadButtonSounds[Random.Range(0, clickKeypadButtonSounds.Length)]);
            }
        }

        public void PlaySound(AudioClip clip)
        {
            if (clip != null && audioSource != null) 
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
        }

        public void InputCode(char code)
        {
            if (!isPuzzleCompleted)
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
            if (currentCode.ToLower().Contains(code.ToLower()))
            {
                Progress(1000f);
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
    }
}
