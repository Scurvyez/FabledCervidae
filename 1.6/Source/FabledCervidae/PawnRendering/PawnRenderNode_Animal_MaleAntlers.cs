using UnityEngine;
using Verse;

namespace FabledCervidae
{
    public class PawnRenderNode_Animal_MaleAntlers : PawnRenderNode_AnimalPart
    {
        private Graphic _defaultGraphic;
        private Vector2 _finalDrawSize;
        private Graphic _defaultDesGraphic;
        private Vector2 _finalDesDrawSize;

        private string _adultAntlerGraphicPath = "";
        private string _juvenileAntlerGraphicPath = "";
        private string _desiccatedAntlerGraphicPath = "";
        
        public PawnRenderNode_Animal_MaleAntlers(Pawn pawn, 
            PawnRenderNodeProperties props, PawnRenderTree tree) 
            : base(pawn, props, tree)
        {
            
        }
        
        public override Graphic GraphicFor(Pawn pawn)
        {
            if (!pawn.TryGetComp(out Comp_MaleAntlers compAntler) 
                || pawn.gender != Gender.Male) 
                return null;

            _defaultGraphic = pawn.ageTracker.CurKindLifeStage.bodyGraphicData.Graphic;
            _finalDrawSize = pawn.ageTracker.CurKindLifeStage.bodyGraphicData.drawSize;

            if (pawn.ageTracker.CurLifeStage == FCDefOf.AnimalAdult)
            {
                return GetAntlerGraphic(
                    pawn, compAntler, _defaultGraphic, 
                    _finalDrawSize, true);
            }
            
            return pawn.ageTracker.CurLifeStage == FCDefOf.AnimalJuvenile 
                ? GetAntlerGraphic(
                    pawn, compAntler, _defaultGraphic, 
                    _finalDrawSize, false) 
                : null;
        }

        private Graphic GetAntlerGraphic(Pawn pawn, Comp_MaleAntlers compAntler, 
            Graphic defaultGraphic, Vector2 finalDrawSize, bool isAdult)
        {
            if (isAdult)
            {
                if (compAntler.AdultAntlerVariant == 0)
                {
                    compAntler.AdultAntlerVariant = 
                        Rand.RangeInclusive(1, compAntler.Props.adultAntlerVariantCount);
                }
                _adultAntlerGraphicPath = 
                    $"{defaultGraphic.path}AntlersAdult{compAntler.AdultAntlerVariant}";
                return GetFinalAntlerGraphic(pawn, _adultAntlerGraphicPath, finalDrawSize);
            }

            if (compAntler.JuvenileAntlerVariant == 0)
            {
                compAntler.JuvenileAntlerVariant = 
                    Rand.RangeInclusive(1, compAntler.Props.juvenileAntlerVariantCount);
            }
            _juvenileAntlerGraphicPath = 
                $"{defaultGraphic.path}AntlersJuvenile{compAntler.JuvenileAntlerVariant}";
            return GetFinalAntlerGraphic(pawn, _juvenileAntlerGraphicPath, finalDrawSize);
        }

        private Graphic GetFinalAntlerGraphic(Pawn pawn, 
            string antlerGraphicPath, Vector2 finalDrawSize)
        {
            if (pawn.Drawer.renderer.CurRotDrawMode != RotDrawMode.Dessicated)
                return GraphicDatabase.Get<Graphic_Multi>(
                    antlerGraphicPath, ShaderDatabase.Transparent, 
                    finalDrawSize, Color.white);
            
            _defaultDesGraphic = pawn.ageTracker.CurKindLifeStage.dessicatedBodyGraphicData.Graphic;
            _finalDesDrawSize = pawn.ageTracker.CurKindLifeStage.dessicatedBodyGraphicData.drawSize;

            _desiccatedAntlerGraphicPath = _defaultDesGraphic.path.EndsWith("west")
                ? $"{antlerGraphicPath}_east"
                : $"{antlerGraphicPath}_west";

            return GraphicDatabase.Get<Graphic_Multi>(
                _desiccatedAntlerGraphicPath, ShaderDatabase.Transparent, 
                _finalDesDrawSize, Color.white);
        }
    }
}