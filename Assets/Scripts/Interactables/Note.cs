using Redsilver2.Core.Player;
using UnityEngine;

namespace Redsilver2.Core.Interactables
{
    public class Note : Interactable
    {
        [SerializeField] private string title;
        [SerializeField] private string[] pageContents;

        [Space]
        [SerializeField] private AudioClip openNoteSound;
        [SerializeField] private AudioClip closeNoteSound;

        protected override void Awake()
        {
            NoteUI noteUI = Camera.main.GetComponent<NoteUI>();

            base.Awake();

            if (noteUI != null)
            {
                AddOnInteractEvent(isInteracting =>
                {
                    if (isInteracting)
                    {
                        noteUI.SetUIVisibility(true);
                        noteUI.Open(title, pageContents, openNoteSound, Interact);
                    }
                    else
                    {
                        noteUI.Close(closeNoteSound);
                    }
                });

            }
        }
    }
}
