using Verse;

namespace FabledCervidae
{
    public class CompProperties_MaleAntlers : CompProperties
    {
        public int adultAntlerVariantCount = 1;
        public int juvenileAntlerVariantCount = 1;
        
        public CompProperties_MaleAntlers()
        {
            compClass = typeof(Comp_MaleAntlers);
        }
    }
}