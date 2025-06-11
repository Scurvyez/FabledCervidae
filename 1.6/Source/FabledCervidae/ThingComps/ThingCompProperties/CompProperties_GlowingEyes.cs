using UnityEngine;
using Verse;

namespace FabledCervidae
{
    public class CompProperties_GlowingEyes : CompProperties
    {
        public string eyeGraphicMale = null;
        public string eyeGraphicFemale = null;
        public Color eyeColor = new(1f, 1f, 1f);
        public bool shouldFadeInOut;
        public bool drawWhileSleeping;
        public bool drawAtNight;
        public float sunlightThreshold = 0f;
        
        public CompProperties_GlowingEyes()
        {
            compClass = typeof(Comp_GlowingEyes);
        }
    }
}