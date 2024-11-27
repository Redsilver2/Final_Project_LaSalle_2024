using UnityEngine;

namespace Redsilver2.Core.Subtitles
{
    [System.Serializable]
    public struct SubtitleData
    {
        [SerializeField] private string characterName;

        [Space]
        [SerializeField][TextArea(5, 5)] private string context;

        [Space]
        [SerializeField] private float startTime;
        [SerializeField] private float endTime;

        public float StartTime => startTime;
        public float EndTime   => endTime;
        public string Context => context;
        public string CharacterName => characterName;   
        public float Duration => Mathf.Abs(endTime - startTime);
    }
}
