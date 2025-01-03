using RimWorld;
using UnityEngine;
using Verse;

namespace FabledCervidae
{
    public class Comp_GlowingEyes : ThingComp
    {
        private bool _shouldDrawEyes;
        private float _fadeProgress;
        private Graphic _eyeGraphic;
        private Vector2 _drawSize;
        private readonly MaterialPropertyBlock _mpb = new();
        
        private const float FADE_SPEED = 0.01f;
        
        public CompProperties_GlowingEyes Props => (CompProperties_GlowingEyes)props;

        public override void CompTick()
        {
            base.CompTick();
            if (parent is not Pawn parentPawn) return;
            bool shouldDraw = ShouldDrawEyes(parentPawn);

            if (Props.shouldFadeInOut)
            {
                _fadeProgress = shouldDraw switch
                {
                    true when parentPawn.pather.MovingNow => 
                        Mathf.Clamp(_fadeProgress + FADE_SPEED, 0f, 1f),
                    false => 0f,
                    _ => _fadeProgress
                };
            }
            else
            {
                _fadeProgress = shouldDraw ? 1f : 0f;
            }

            _shouldDrawEyes = shouldDraw || _fadeProgress > 0f;
            if (_shouldDrawEyes)
            {
                CacheEyeGraphic(parentPawn);
            }
        }

        private bool ShouldDrawEyes(Pawn pawn)
        {
            if (!pawn.Awake() 
                && !Props.drawWhileSleeping 
                && pawn.GetPosture() != PawnPosture.Standing) 
                return false;
            
            if (!Props.drawAtNight) return true;
            float currentSunGlow = GenCelestial.CurCelestialSunGlow(pawn.Map);
            return !(currentSunGlow > Props.sunlightThreshold);
        }

        private void CacheEyeGraphic(Pawn pawn)
        {
            PawnKindDef pawnKind = pawn.kindDef;
            PawnKindLifeStage curLSI = pawnKind.lifeStages[pawn.ageTracker.CurLifeStageIndex];
            string eyeGraphicPath = null;

            switch (pawn.gender)
            {
                case Gender.Female:
                    _drawSize = curLSI.femaleGraphicData.Graphic.drawSize;
                    eyeGraphicPath = Props.eyeGraphicFemale;
                    break;
                case Gender.Male:
                    _drawSize = curLSI.bodyGraphicData.Graphic.drawSize;
                    eyeGraphicPath = Props.eyeGraphicMale;
                    break;
                case Gender.None:
                    return;
            }

            _eyeGraphic = GraphicDatabase.Get<Graphic_Multi>(
                eyeGraphicPath,
                ShaderDatabase.TransparentPostLight,
                new Vector2(_drawSize.x, _drawSize.y),
                Props.eyeColor);
        }

        public override void PostDraw()
        {
            base.PostDraw();
            if (_shouldDrawEyes 
                && _eyeGraphic != null 
                && parent is Pawn parentPawn)
            {
                DrawGlowingEyeGraphic(parentPawn, _eyeGraphic, 
                    parentPawn.Rotation, _drawSize, _fadeProgress);
            }
        }

        private void DrawGlowingEyeGraphic(Pawn pawn, Graphic eyeGraphic, 
            Rot4 eyeRotation, Vector2 eyeDrawSize, float eyeFadeProgress)
        {
            PawnKindLifeStage curLifeStage = pawn.ageTracker.CurKindLifeStage;
            GraphicData graphicData = pawn.gender == Gender.Female
                ? curLifeStage?.femaleGraphicData ?? curLifeStage?.bodyGraphicData
                : curLifeStage?.bodyGraphicData;

            if (graphicData == null) return;

            Vector3 offset = eyeRotation.AsInt switch
            {
                0 => graphicData.drawOffsetNorth ?? Vector3.zero,
                1 => graphicData.drawOffsetEast ?? Vector3.zero,
                2 => graphicData.drawOffsetSouth ?? Vector3.zero,
                3 => graphicData.drawOffsetWest ?? Vector3.zero,
                _ => Vector3.zero
            };

            Vector3 drawPos = pawn.Drawer.DrawPos + offset;
            drawPos.y += 0.0075f;

            Material fadedMat = FadedMaterialPool.FadedVersionOf(
                eyeGraphic.MatAt(eyeRotation), eyeFadeProgress);
            Matrix4x4 matrix = Matrix4x4.TRS(
                drawPos, Quaternion.identity, 
                new Vector3(eyeDrawSize.x, 1f, eyeDrawSize.y));
            _mpb.Clear();
            
            _mpb.SetColor(ShaderPropertyIDs.Color, 
                new Color(Props.eyeColor.r, Props.eyeColor.g, Props.eyeColor.b, eyeFadeProgress));
            Graphics.DrawMesh(MeshPool.plane10, matrix, fadedMat, 0, null, 0, _mpb);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref _fadeProgress, "_fadeProgress", 0f);
            Scribe_Values.Look(ref _shouldDrawEyes, "_shouldDrawEyes", false);
        }
    }
}