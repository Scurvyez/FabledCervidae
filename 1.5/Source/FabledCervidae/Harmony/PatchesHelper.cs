using System.Collections.Generic;
using RimWorld;
using Verse;

namespace FabledCervidae
{
    public class PatchesHelper
    {
        public static bool HasBondedColonist(Pawn pawn)
        {
            return pawn.Map?.mapPawns?.FreeColonistsSpawned
                .Any(colonist => 
                    pawn.relations.DirectRelationExists(PawnRelationDefOf.Bond, colonist)) ?? false;
        }

        public static readonly List<GeneDef> CervidVitalityGenes =
        [
            FCDefOf.FC_Immunities_Scire,
            FCDefOf.FC_Immunities_Mirelung,
            FCDefOf.FC_Immunities_Infernihart,
            FCDefOf.FC_Immunities_Auravine
        ];
    }
}