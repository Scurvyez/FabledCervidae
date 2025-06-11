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
    }
}