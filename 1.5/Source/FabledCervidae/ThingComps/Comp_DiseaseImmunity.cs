using RimWorld;
using Verse;

namespace FabledCervidae
{
    public class Comp_DiseaseImmunity : ThingComp
    {
        public CompProperties_DiseaseImmunity Props => (CompProperties_DiseaseImmunity) props;
        
        public override void CompTick()
        {
            base.CompTick();
            if (!parent.IsHashIntervalTick(Props.tickInterval)) return;
            TryRemoveHediffsFromAnimal();
            //TryRemoveHediffsFromMaster();
            TryGiveGeneToMaster();
        }
        
        private void TryRemoveHediffsFromAnimal()
        {
            Pawn pawn = parent as Pawn;
            
            if (pawn?.health?.hediffSet == null) return;
            foreach (HediffDef hediffDef in Props.ownImmunities)
            {
                Hediff hediffToRemove = pawn.health.hediffSet.GetFirstHediffOfDef(hediffDef);
                if (hediffToRemove != null)
                {
                    pawn.health.RemoveHediff(hediffToRemove);
                }
            }
        }
        
        private void TryGiveGeneToMaster()
        {
            Pawn animal = parent as Pawn;
            
            if (animal?.training == null) return;
            Pawn master = animal.playerSettings.RespectedMaster;
            
            if (master.genes.HasActiveGene(FCDefOf.FC_Immunities_Scire)) return;
            master.genes.AddGene(FCDefOf.FC_Immunities_Scire, false);
        }
        
        private void TryRemoveHediffsFromMaster()
        {
            Pawn animal = parent as Pawn;
            
            if (animal?.training == null) return;
            Pawn master = animal.playerSettings.RespectedMaster;
            
            if (master?.health?.hediffSet == null) return;
            foreach (HediffDef hediffDef in Props.masterImmunities)
            {
                Hediff hediffToRemove = master.health.hediffSet.GetFirstHediffOfDef(hediffDef);
                if (hediffToRemove != null)
                {
                    master.health.RemoveHediff(hediffToRemove);
                }
            }
        }
    }
}