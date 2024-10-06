using Verse;

namespace FabledCervidae
{
    public class Comp_MaleAntlers : ThingComp
    {
        public CompProperties_MaleAntlers Props => (CompProperties_MaleAntlers) props;
        
        private int adultAntlerVariant;
        private int juvenileAntlerVariant;

        public int AdultAntlerVariant
        {
            get => adultAntlerVariant;
            set => adultAntlerVariant = value;
        }

        public int JuvenileAntlerVariant
        {
            get => juvenileAntlerVariant;
            set => juvenileAntlerVariant = value;
        }
        
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref adultAntlerVariant, "adultAntlerVariant", 0);
            Scribe_Values.Look(ref juvenileAntlerVariant, "juvenileAntlerVariant", 0);
        }
    }
}