using Redsilver2.Core.Subtitles;
using UnityEngine;

namespace Redsilver2.Core.Events
{
    [CreateAssetMenu(menuName = "Events/Subtitle")]
    public class SubtitleEvent : GameEvent
    {
        [SerializeField] private Subtitle subtitle;

        public override void Execute()
        {
            SubtitleManager subtitleManager  = SubtitleManager.Instance;

            if (subtitleManager != null) 
            {
                subtitleManager.PlaySubtitle(subtitle);
            }
        }
    }
}
