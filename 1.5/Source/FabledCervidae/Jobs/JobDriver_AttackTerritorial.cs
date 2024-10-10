using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace FabledCervidae
{
    public class JobDriver_AttackTerritorial : JobDriver
    {
        private ModExtension_Territorial _modExt;
        
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }
        
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(TargetIndex.A);
            
            _modExt = pawn.def.GetModExtension<ModExtension_Territorial>();
            AnimationDef animationDef = FCDefOf.FC_FightingCervidsAnimation;

            Toil moveToTargetToil = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            moveToTargetToil.FailOnDespawnedOrNull(TargetIndex.A);
            yield return moveToTargetToil;

            Toil attackToil = Toils_Combat.FollowAndMeleeAttack(TargetIndex.A, TargetIndex.None, AttackAction);
            attackToil.defaultDuration = _modExt.disputeDuration;
            Action originalTickAction = attackToil.tickAction;
            
            attackToil.tickAction = delegate
            {
                originalTickAction?.Invoke();

                if (pawn.Drawer.renderer.CurAnimation != animationDef)
                {
                    pawn.Drawer.renderer.SetAnimation(animationDef);
                }
                if (TargetA.Thing is Pawn targetPawn && targetPawn.Drawer.renderer.CurAnimation != animationDef)
                {
                    targetPawn.Drawer.renderer.SetAnimation(animationDef);
                }
                
                if (attackToil.actor.jobs.curDriver.ticksLeftThisToil > 0) return;
                pawn.jobs.curDriver.ReadyForNextToil();
            };
            yield return attackToil;

            yield return Toils_General.Do(delegate
            {
                if (TargetA.Thing is Pawn targetPawn)
                {
                    ClearCombatStance(targetPawn);
                    ChillTheFuckOut(targetPawn);
                }
                
                ClearCombatStance(pawn);
                ChillTheFuckOut(pawn);
                EndJobWith(JobCondition.Succeeded);
            });
        }
        
        private void AttackAction()
        {
            Thing target = job.GetTarget(TargetIndex.A).Thing;
            if (target != null)
            {
                pawn.meleeVerbs.TryMeleeAttack(target, job.verbToUse);
            }
        }

        public override bool IsContinuation(Job j)
        {
            return job.GetTarget(TargetIndex.A) == j.GetTarget(TargetIndex.A);
        }
        
        private static void ClearCombatStance(Pawn animal)
        {
            if (animal?.mindState?.meleeThreat == null) return;
            animal.mindState.meleeThreat = null;
            animal.jobs.StopAll();
        }
        
        private static void ChillTheFuckOut(Pawn animal)
        {
            if (animal?.jobs == null) return;
            animal.Drawer.renderer.SetAnimation(null);
            Job walkAwayJob = JobMaker.MakeJob(JobDefOf.GotoWander);
            animal.jobs.StartJob(walkAwayJob, JobCondition.Succeeded, null, resumeCurJobAfterwards: false);
        }
    }
}