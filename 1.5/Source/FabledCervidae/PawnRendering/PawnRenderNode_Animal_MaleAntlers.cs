using UnityEngine;
using Verse;

namespace FabledCervidae
{
    public class PawnRenderNode_Animal_MaleAntlers : PawnRenderNode_AnimalPart
    {
        public PawnRenderNode_Animal_MaleAntlers(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) 
            : base(pawn, props, tree)
        {
            
        }
        
        public override Graphic GraphicFor(Pawn pawn)
        {
            if (pawn.gender != Gender.Male) return null;
            Graphic defaultGraphic = pawn.ageTracker.CurKindLifeStage.bodyGraphicData.Graphic;
            Vector2 finalDrawSize = pawn.ageTracker.CurKindLifeStage.bodyGraphicData.drawSize;
            Comp_MaleAntlers compAntler = pawn.TryGetComp<Comp_MaleAntlers>();
            
            if (compAntler == null) return null;
            if (pawn.ageTracker.CurLifeStage == FCDefOf.AnimalAdult)
            {
                if (compAntler.AdultAntlerVariant == 0)
                {
                    compAntler.AdultAntlerVariant = Rand.RangeInclusive(1, compAntler.Props.adultAntlerVariantCount);
                }
                string adultAntlerGraphicPath = $"{defaultGraphic.path}AntlersAdult{compAntler.AdultAntlerVariant}";
                return GraphicDatabase.Get<Graphic_Multi>(adultAntlerGraphicPath, ShaderDatabase.Transparent, finalDrawSize, Color.white);
            }

            if (pawn.ageTracker.CurLifeStage != FCDefOf.AnimalJuvenile) return null;
            if (compAntler.JuvenileAntlerVariant == 0)
            {
                compAntler.JuvenileAntlerVariant = Rand.RangeInclusive(1, compAntler.Props.juvenileAntlerVariantCount);
            }
            string juvenileAntlerGraphicPath = $"{defaultGraphic.path}AntlersJuvenile{compAntler.JuvenileAntlerVariant}";
            return GraphicDatabase.Get<Graphic_Multi>(juvenileAntlerGraphicPath, ShaderDatabase.Transparent, finalDrawSize, Color.white);
        }
    }
}