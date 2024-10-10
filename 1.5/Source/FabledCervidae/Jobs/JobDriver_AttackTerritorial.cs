using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace FabledCervidae
{
    public class JobDriver_AttackTerritorial : JobDriver
    {
        private ModExtension_TerritorialFighting _modExt;
        private AnimationDef _fightingAnimation;
        
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }
        
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(TargetIndex.A);

            _modExt = pawn.def.GetModExtension<ModExtension_TerritorialFighting>();
            _fightingAnimation = FCDefOf.FC_FightingCervidsAnimation;
            
            yield return MoveToTargetToil();
            yield return AttackToil();
            yield return PostCombatToil();
        }

        private Toil MoveToTargetToil()
        {
            Toil moveToTargetToil = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            moveToTargetToil.FailOnDespawnedOrNull(TargetIndex.A);
            return moveToTargetToil;
        }

        private Toil AttackToil()
        {
            Toil attackToil = Toils_Combat.FollowAndMeleeAttack(TargetIndex.A, TargetIndex.None, PerformAttack);
            attackToil.defaultDuration = _modExt.disputeDuration;
            Action originalTickAction = attackToil.tickAction;
            
            attackToil.tickAction = () =>
            {
                originalTickAction?.Invoke();
                SetCombatAnimation(pawn, TargetA.Thing);
                
                if (attackToil.actor.jobs.curDriver.ticksLeftThisToil <= 0)
                {
                    pawn.jobs.curDriver.ReadyForNextToil();
                }
            };
            return attackToil;
        }
        
        private Toil PostCombatToil()
        {
            return Toils_General.Do(() =>
            {
                if (TargetA.Thing is Pawn targetPawn)
                {
                    ResolvePostCombat(targetPawn);
                }
                ResolvePostCombat(pawn);
                EndJobWith(JobCondition.Succeeded);
            });
        }

        private void PerformAttack()
        {
            Thing target = job.GetTarget(TargetIndex.A).Thing;
            if (target != null)
            {
                pawn.meleeVerbs.TryMeleeAttack(target, job.verbToUse);
            }
        }

        private void SetCombatAnimation(Pawn attacker, Thing target)
        {
            if (attacker.Drawer.renderer.CurAnimation != _fightingAnimation)
            {
                attacker.Drawer.renderer.SetAnimation(_fightingAnimation);
            }
            if (target is Pawn targetPawn && targetPawn.Drawer.renderer.CurAnimation != _fightingAnimation)
            {
                targetPawn.Drawer.renderer.SetAnimation(_fightingAnimation);
            }
        }

        private void ResolvePostCombat(Pawn animal)
        {
            ClearCombatStance(animal);
            SetRelaxedState(animal);
        }

        private static void ClearCombatStance(Pawn animal)
        {
            if (animal?.mindState?.meleeThreat == null) return;
            animal.mindState.meleeThreat = null;
            animal.jobs.StopAll();
        }

        private static void SetRelaxedState(Pawn animal)
        {
            animal?.Drawer?.renderer?.SetAnimation(null);
            
            if (animal?.jobs == null) return;
            Job walkAwayJob = JobMaker.MakeJob(JobDefOf.GotoWander);
            animal.jobs.StartJob(walkAwayJob, JobCondition.Succeeded, null, resumeCurJobAfterwards: false);
        }

        public override bool IsContinuation(Job j)
        {
            return job.GetTarget(TargetIndex.A) == j.GetTarget(TargetIndex.A);
        }
    }
}