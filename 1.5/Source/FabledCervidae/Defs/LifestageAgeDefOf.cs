using RimWorld;

namespace FabledCervidae
{
    [DefOf]
    public class FCDefOf
    {
        public static LifeStageDef AnimalBaby;
        public static LifeStageDef AnimalJuvenile;
        public static LifeStageDef AnimalAdult;
        
        static FCDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(FCDefOf));
        }
    }
}