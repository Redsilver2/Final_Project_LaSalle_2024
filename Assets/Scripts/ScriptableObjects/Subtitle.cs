using System;
using UnityEngine;

namespace Redsilver2.Core.Subtitles
{
    [CreateAssetMenu(menuName = "Subtitle", fileName = "New Subtitle")]
    public class Subtitle : ScriptableObject
    {
        [SerializeField] private AudioClip audioClip;
        [SerializeField] private SubtitleData[] subtitleDatas;
        public SubtitleData[] datas => subtitleDatas;
    }
}
