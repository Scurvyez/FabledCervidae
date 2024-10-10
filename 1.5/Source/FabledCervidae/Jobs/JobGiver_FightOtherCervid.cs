using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace FabledCervidae
{
    public class JobGiver_FightOtherCervid : ThinkNode_JobGiver
    {
        private ModExtension_Territorial _modExt;
        
        protected override Job TryGiveJob(Pawn pawn)
        {
            _modExt = pawn.def.GetModExtension<ModExtension_Territorial>();
            
            if (_modExt == null) return null;
            if (pawn.ageTracker.CurLifeStage != FCDefOf.AnimalAdult 
                || pawn.gender != Gender.Male || pawn.DeadOrDowned) return null;

            Thing target = FindCervidTarget(pawn);
            if (target == null) return null;

            Job job = JobMaker.MakeJob(FCDefOf.FC_AttackTerritorial, target);
            return job;
        }

        private Thing FindCervidTarget(Pawn pawn)
        {
            List<Pawn> nearbyCervid = pawn.Map.mapPawns.AllPawnsSpawned
                .Where(x => x != pawn && pawn.Position.InHorDistOf(x.Position, _modExt.targetAcquireRadius) &&
                            x.def == pawn.def && IsEquallyTerritorialCervid(x))
                .ToList();

            return nearbyCervid.TryRandomElement(out Pawn target) ? target : null;
        }

        private static bool IsEquallyTerritorialCervid(Pawn pawn)
        {
            return pawn.def.HasModExtension<ModExtension_Territorial>() 
                   && pawn.ageTracker.CurLifeStage == FCDefOf.AnimalAdult
                   && pawn.gender == Gender.Male && !pawn.DeadOrDowned;
        }
    }
}