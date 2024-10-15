using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Redsilver2.Core.Audio
{
    [Serializable]
    public class Footstep
    {
        [HideInInspector] public string groundTag = string.Empty;
        [SerializeField]  private AudioClip[] groundClips;
        [SerializeField]  private AudioClip[] landClips;
       
        private int previousLandIndex   = -1;
        private int previousGroundIndex = -1;

        public Footstep(string groundTag)
        {
            this.groundTag     = groundTag;
            this.groundClips         = new AudioClip[0];
            this.previousGroundIndex = -1;
        }
        public Footstep(string groundTag, AudioClip[] clips)
        {
            this.groundTag     = groundTag;
            this.groundClips         = clips;
            this.previousGroundIndex = -1;

        }

        public  AudioClip GetUniqueRandomClip(bool isGettingGroundClip)
        {
            AudioClip result;

            if (isGettingGroundClip)
            {
                result = GetUniqueRandomClip(ref previousGroundIndex, groundClips);
            }
            else
            {
                result = GetUniqueRandomClip(ref previousLandIndex, landClips);
            }

            return result;
        }
        private AudioClip GetUniqueRandomClip(ref int previousIndex, AudioClip[] clips)
        {
            int index = previousIndex;

            for (int i = 0; i < 999; i++)
            {
                if(index != previousIndex)
                {
                    previousIndex = index;
                    break;
                }

                index = Random.Range(0, clips.Length);
            }

            return clips[index];
        }

        public static void SetArray(ref Footstep[] footsteps)
        {
            GroundTag[] groundTags = (GroundTag[])Enum.GetValues(typeof(GroundTag));
            Footstep [] result     = new Footstep[groundTags.Length]; 

            for (int i = 0; i < groundTags.Length;i++)
            {
                string tag = groundTags[i].ToString();
                bool isSimilarTag = false;

                if (footsteps != null)
                {
                    for (int j = 0; j < footsteps.Length; j++)
                    {
                        Footstep currentFootstep = footsteps[j];

                        if (currentFootstep.groundTag == tag)
                        {
                            result[i] = currentFootstep;
                            isSimilarTag = true;
                            break;
                        }
                    }
                }

                if (!isSimilarTag)
                {
                    result[i] = new Footstep(tag);
                }
            }

            footsteps = result;
        }
    }
}
