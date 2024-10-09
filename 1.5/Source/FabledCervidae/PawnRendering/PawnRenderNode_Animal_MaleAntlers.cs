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
            if (!pawn.TryGetComp(out Comp_MaleAntlers compAntler) || pawn.gender != Gender.Male) 
                return null;

            Graphic defaultGraphic = pawn.ageTracker.CurKindLifeStage.bodyGraphicData.Graphic;
            Vector2 finalDrawSize = pawn.ageTracker.CurKindLifeStage.bodyGraphicData.drawSize;

            if (pawn.ageTracker.CurLifeStage == FCDefOf.AnimalAdult)
            {
                return GetAntlerGraphic(pawn, compAntler, defaultGraphic, finalDrawSize, true);
            }
            
            return pawn.ageTracker.CurLifeStage == FCDefOf.AnimalJuvenile 
                ? GetAntlerGraphic(pawn, compAntler, defaultGraphic, finalDrawSize, false) 
                : null;
        }

        private static Graphic GetAntlerGraphic(Pawn pawn, Comp_MaleAntlers compAntler, Graphic defaultGraphic, Vector2 finalDrawSize, bool isAdult)
        {
            if (isAdult)
            {
                if (compAntler.AdultAntlerVariant == 0)
                {
                    compAntler.AdultAntlerVariant = Rand.RangeInclusive(1, compAntler.Props.adultAntlerVariantCount);
                }
                string adultAntlerGraphicPath = $"{defaultGraphic.path}AntlersAdult{compAntler.AdultAntlerVariant}";
                return GetFinalAntlerGraphic(pawn, adultAntlerGraphicPath, finalDrawSize);
            }

            if (compAntler.JuvenileAntlerVariant == 0)
            {
                compAntler.JuvenileAntlerVariant = Rand.RangeInclusive(1, compAntler.Props.juvenileAntlerVariantCount);
            }
            string juvenileAntlerGraphicPath = $"{defaultGraphic.path}AntlersJuvenile{compAntler.JuvenileAntlerVariant}";
            return GetFinalAntlerGraphic(pawn, juvenileAntlerGraphicPath, finalDrawSize);
        }

        private static Graphic GetFinalAntlerGraphic(Pawn pawn, string antlerGraphicPath, Vector2 finalDrawSize)
        {
            if (pawn.Drawer.renderer.CurRotDrawMode != RotDrawMode.Dessicated)
                return GraphicDatabase.Get<Graphic_Multi>(antlerGraphicPath, ShaderDatabase.Transparent, finalDrawSize,
                    Color.white);
            
            Graphic defaultDesGraphic = pawn.ageTracker.CurKindLifeStage.dessicatedBodyGraphicData.Graphic;
            Vector2 finalDesDrawSize = pawn.ageTracker.CurKindLifeStage.dessicatedBodyGraphicData.drawSize;

            string desiccatedAntlerGraphicPath = defaultDesGraphic.path.EndsWith("west")
                ? $"{antlerGraphicPath}_east"
                : $"{antlerGraphicPath}_west";

            return GraphicDatabase.Get<Graphic_Multi>(desiccatedAntlerGraphicPath, ShaderDatabase.Transparent, finalDesDrawSize, Color.white);
        }
    }
}