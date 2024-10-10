using RimWorld;
using Verse;

namespace FabledCervidae
{
    [DefOf]
    public class FCDefOf
    {
        public static LifeStageDef AnimalBaby;
        public static LifeStageDef AnimalJuvenile;
        public static LifeStageDef AnimalAdult;
        public static JobDef FC_AttackTerritorial;
        public static AnimationDef FC_FightingCervidsAnimation;
        
        static FCDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(FCDefOf));
        }
    }
}