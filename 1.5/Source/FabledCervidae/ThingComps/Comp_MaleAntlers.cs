using Verse;

namespace FabledCervidae
{
    public class Comp_MaleAntlers : ThingComp
    {
        public CompProperties_MaleAntlers Props => (CompProperties_MaleAntlers) props;
        
        private int _adultAntlerVariant;
        private int _juvenileAntlerVariant;

        public int AdultAntlerVariant
        {
            get => _adultAntlerVariant;
            set => _adultAntlerVariant = value;
        }

        public int JuvenileAntlerVariant
        {
            get => _juvenileAntlerVariant;
            set => _juvenileAntlerVariant = value;
        }
        
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref _adultAntlerVariant, "adultAntlerVariant", 0);
            Scribe_Values.Look(ref _juvenileAntlerVariant, "juvenileAntlerVariant", 0);
        }
    }
}