using UnityEngine;
using Verse;

namespace FabledCervidae
{
    public class PawnRenderNode_Animal_SexualDimorphism : PawnRenderNode_AnimalPart
    {
        public PawnRenderNode_Animal_SexualDimorphism(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) 
            : base(pawn, props, tree)
        {
            
        }

        public override Graphic GraphicFor(Pawn pawn)
        {
            Graphic defaultGraphic = pawn.ageTracker.CurKindLifeStage.bodyGraphicData.Graphic;
            Vector2 finalDrawSize = pawn.ageTracker.CurKindLifeStage.bodyGraphicData.drawSize;

            if (!pawn.TryGetComp(out Comp_SexualDimorphism comp)) return base.GraphicFor(pawn);
            return GraphicDatabase.Get<Graphic_Multi>(defaultGraphic.path, ShaderDatabase.CutoutComplex,
                finalDrawSize, comp.newColor, comp.newColorTwo);
        }
    }
}