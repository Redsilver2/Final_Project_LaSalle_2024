using TMPro;
using UnityEngine;

namespace Redsilver2.Core.Interactables
{
    public class NoteComputer : Computer
    {
        [Space]
        [SerializeField]                private string title;
        [SerializeField][TextArea(5,5)] private string content;

        [Space]
        [SerializeField]                private TextMeshProUGUI titleUI;
        [SerializeField]                private TextMeshProUGUI contentUI;

        protected override void Start()
        {
            base.Start();
            
            if(title != null)
            {
                titleUI.text = title;
            }

            if(content != null)
            {
                contentUI.text = content;
            }
        }
    }
}
