using System.Collections.Generic;
using Verse;

namespace FabledCervidae
{
    public class CompProperties_DiseaseImmunity : CompProperties
    {
        public List<HediffDef> ownImmunities = new ();
        public int tickInterval = 2500;
        
        public CompProperties_DiseaseImmunity()
        {
            compClass = typeof(Comp_DiseaseImmunity);
        }
    }
}