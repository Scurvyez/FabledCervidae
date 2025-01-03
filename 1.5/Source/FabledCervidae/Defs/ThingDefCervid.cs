using Verse;

namespace FabledCervidae
{
    public class ThingDefCervid : ThingDef
    {
        public override void ResolveReferences()
        {
            base.ResolveReferences();
            if (!ModsConfig.BiotechActive) return;

            TaggedString textToAppend = this switch
            {
                _ when this == FCDefOf.FC_Scire => "FC_BioScireDesc".Translate(),
                _ when this == FCDefOf.FC_Mirelung => "FC_BioMirelungDesc".Translate(),
                _ when this == FCDefOf.FC_Infernihart => "FC_BioInfernihartDesc".Translate(),
                _ when this == FCDefOf.FC_Auravine => "FC_BioAuravineDesc".Translate(),
                _ => null
            };
            
            description += "\n\n" + textToAppend;
            
            if (!Rand.Chance(0.001f)) return;
            description += "\n\n" + "FC_EasterEgg".Translate();
        }
    }
}