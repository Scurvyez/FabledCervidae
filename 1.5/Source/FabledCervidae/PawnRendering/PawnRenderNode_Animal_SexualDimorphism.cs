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
            if (pawn.Drawer.renderer.CurRotDrawMode == RotDrawMode.Dessicated) return base.GraphicFor(pawn);
            if (!pawn.TryGetComp(out Comp_SexualDimorphism comp)) return base.GraphicFor(pawn);

            PawnKindLifeStage curKindLifeStage = pawn.ageTracker.CurKindLifeStage;
            Graphic sexSpecificGraphic;
            Vector2 finalDrawSize;
            
            switch (pawn.gender)
            {
                case Gender.Male:
                    sexSpecificGraphic = curKindLifeStage.bodyGraphicData.Graphic;
                    finalDrawSize = curKindLifeStage.bodyGraphicData.drawSize;
                    break;
                case Gender.Female when curKindLifeStage.femaleGraphicData != null:
                    sexSpecificGraphic = curKindLifeStage.femaleGraphicData.Graphic;
                    finalDrawSize = curKindLifeStage.femaleGraphicData.drawSize;
                    break;
                default:
                    return base.GraphicFor(pawn);
            }
            
            switch (pawn.Drawer.renderer.CurRotDrawMode)
            {
                case RotDrawMode.Fresh:
                    return GraphicDatabase.Get<Graphic_Multi>(sexSpecificGraphic.path, ShaderDatabase.CutoutComplex, 
                        finalDrawSize, comp.newColor, comp.newColorTwo);
                case RotDrawMode.Rotting:
                    Color rottenColor = PawnRenderUtility.GetRottenColor(comp.newColor);
                    Color rottenColorTwo = PawnRenderUtility.GetRottenColor(comp.newColorTwo);
                    return GraphicDatabase.Get<Graphic_Multi>(sexSpecificGraphic.path, ShaderDatabase.CutoutComplex, 
                        finalDrawSize, rottenColor, rottenColorTwo);
                default:
                    return base.GraphicFor(pawn);
            }
        }
    }
}