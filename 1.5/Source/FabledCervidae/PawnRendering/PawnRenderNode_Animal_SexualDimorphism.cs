using UnityEngine;
using Verse;

namespace FabledCervidae
{
    public class PawnRenderNode_Animal_SexualDimorphism : PawnRenderNode_AnimalPart
    {
        private readonly Shader _shader = ShaderDatabase.CutoutComplex;
        private PawnKindLifeStage _curKindLifeStage;
        private GraphicData _maleData;
        private GraphicData _femaleData;
        private Graphic _sexSpecificGraphic;
        private Vector2 _finalDrawSize;
        
        public PawnRenderNode_Animal_SexualDimorphism(Pawn pawn, 
            PawnRenderNodeProperties props, PawnRenderTree tree) 
            : base(pawn, props, tree)
        {
            
        }
        
        public override Graphic GraphicFor(Pawn pawn)
        {
            if (pawn.Drawer.renderer.CurRotDrawMode == RotDrawMode.Dessicated
                || !pawn.TryGetComp(out Comp_SexualDimorphism comp)) 
                return base.GraphicFor(pawn);

            _curKindLifeStage = pawn.ageTracker.CurKindLifeStage;
            _maleData = _curKindLifeStage.bodyGraphicData;
            _femaleData = _curKindLifeStage.femaleGraphicData;

            switch (pawn.gender)
            {
                case Gender.Male:
                    _sexSpecificGraphic = _maleData.Graphic;
                    _finalDrawSize = _maleData.drawSize;
                    break;
                case Gender.Female when _femaleData != null:
                    _sexSpecificGraphic = _femaleData.Graphic;
                    _finalDrawSize = _femaleData.drawSize;
                    break;
                case Gender.None:
                default:
                    return base.GraphicFor(pawn);
            }

            switch (pawn.Drawer.renderer.CurRotDrawMode)
            {
                case RotDrawMode.Fresh:
                    return GraphicDatabase.Get<Graphic_Multi>(
                        _sexSpecificGraphic.path, _shader,
                        _finalDrawSize, comp.NewColor, comp.NewColorTwo);
                case RotDrawMode.Rotting or RotDrawMode.Dessicated:
                    Color rottenColor = PawnRenderUtility.GetRottenColor(comp.NewColor);
                    Color rottenColorTwo = PawnRenderUtility.GetRottenColor(comp.NewColorTwo);
                    return GraphicDatabase.Get<Graphic_Multi>(
                        _sexSpecificGraphic.path, _shader,
                        _finalDrawSize, rottenColor, rottenColorTwo);
                default:
                    return base.GraphicFor(pawn);
            }
        }
    }
}