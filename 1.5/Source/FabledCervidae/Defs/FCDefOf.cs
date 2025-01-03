using RimWorld;
using Verse;

namespace FabledCervidae
{
    [DefOf]
    public class FCDefOf
    {
        public static ThingDef FC_Scire;
        public static ThingDef FC_Mirelung;
        public static ThingDef FC_Infernihart;
        public static ThingDef FC_Auravine;
        public static LifeStageDef AnimalJuvenile;
        public static LifeStageDef AnimalAdult;
        public static JobDef FC_AttackTerritorial;
        public static AnimationDef FC_FightingCervidsAnimation;
        public static BodyDef FC_QuadrupedAnimalWithHoovesAndAntlers;
        
        [MayRequireBiotech]
        public static GeneDef FC_Immunities_Scire;
        [MayRequireBiotech]
        public static GeneDef FC_Immunities_Mirelung;
        [MayRequireBiotech]
        public static GeneDef FC_Immunities_Infernihart;
        [MayRequireBiotech]
        public static GeneDef FC_Immunities_Auravine;
        
        static FCDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(FCDefOf));
        }
    }
}