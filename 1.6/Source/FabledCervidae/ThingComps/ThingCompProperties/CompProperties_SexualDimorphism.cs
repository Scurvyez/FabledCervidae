using Verse;

namespace FabledCervidae
{
    public class CompProperties_SexualDimorphism : CompProperties
    {
        public FloatRange maleRRangeOne = new ();
        public FloatRange maleGRangeOne = new ();
        public FloatRange maleBRangeOne = new ();
        public FloatRange maleRRangeTwo = new ();
        public FloatRange maleGRangeTwo = new ();
        public FloatRange maleBRangeTwo = new ();

        public FloatRange femaleRRangeOne = new ();
        public FloatRange femaleGRangeOne = new ();
        public FloatRange femaleBRangeOne = new ();
        public FloatRange femaleRRangeTwo = new ();
        public FloatRange femaleGRangeTwo = new ();
        public FloatRange femaleBRangeTwo = new ();

        public CompProperties_SexualDimorphism()
        {
            compClass = typeof(Comp_SexualDimorphism);
        }
    }
}