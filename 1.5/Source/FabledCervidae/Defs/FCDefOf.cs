using RimWorld;
using Verse;

namespace FabledCervidae
{
    [DefOf]
    public class FCDefOf
    {
        public static LifeStageDef AnimalJuvenile;
        public static LifeStageDef AnimalAdult;
        public static JobDef FC_AttackTerritorial;
        public static AnimationDef FC_FightingCervidsAnimation;
        public static BodyDef FC_QuadrupedAnimalWithHoovesAndAntlers;
        public static GeneDef FC_Immunities_Scire;
        
        static FCDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(FCDefOf));
        }
    }
}