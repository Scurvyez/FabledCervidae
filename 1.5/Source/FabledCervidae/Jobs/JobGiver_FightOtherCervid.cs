using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace FabledCervidae
{
    public class JobGiver_FightOtherCervid : ThinkNode_JobGiver
    {
        private ModExtension_TerritorialFighting _modExt;
        
        protected override Job TryGiveJob(Pawn pawn)
        {
            _modExt = pawn.def.GetModExtension<ModExtension_TerritorialFighting>();
            
            if (_modExt == null) return null;
            float fightChance = _modExt.fightChance > 0f ? _modExt.fightChance : 1f;
            
            if (!Rand.Chance(fightChance)) return null;
            if (pawn.ageTracker.CurLifeStage != FCDefOf.AnimalAdult 
                || pawn.gender != Gender.Male || pawn.DeadOrDowned
                || pawn.Faction == Faction.OfPlayer) return null;

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
            return pawn.def.HasModExtension<ModExtension_TerritorialFighting>() 
                   && pawn.ageTracker.CurLifeStage == FCDefOf.AnimalAdult
                   && pawn.gender == Gender.Male && !pawn.DeadOrDowned
                   && pawn.Faction != Faction.OfPlayer;
        }
    }
}