using RimWorld;
using UnityEngine;
using Verse;

namespace FabledCervidae
{
    public class Comp_GlowingEyes : ThingComp
    {
        private bool shouldDrawEyes;
        private Graphic eyeGraphic;
        private Vector2 drawSize;
        private float fadeProgress;
        private const float fadeSpeed = 0.01f;
        private readonly MaterialPropertyBlock Mpb = new();
        
        public CompProperties_GlowingEyes Props => (CompProperties_GlowingEyes)props;

        public override void CompTick()
        {
            base.CompTick();
            if (parent is not Pawn parentPawn) return;
            bool shouldDraw = ShouldDrawEyes(parentPawn);

            if (Props.shouldFadeInOut)
            {
                if (shouldDraw && parentPawn.pather.MovingNow)
                {
                    fadeProgress = Mathf.Clamp(fadeProgress + fadeSpeed, 0f, 1f);
                }
                else if (!shouldDraw)
                {
                    fadeProgress = 0f;
                }
            }
            else
            {
                fadeProgress = shouldDraw ? 1f : 0f;
            }

            shouldDrawEyes = shouldDraw || fadeProgress > 0f;
            if (shouldDrawEyes)
            {
                CacheEyeGraphic(parentPawn);
            }
        }

        private bool ShouldDrawEyes(Pawn pawn)
        {
            if (!pawn.Awake() && !Props.drawWhileSleeping && pawn.GetPosture() != PawnPosture.Standing) return false;
            if (Props.drawAtNight)
            {
                float currentSunGlow = GenCelestial.CurCelestialSunGlow(pawn.Map);
                if (currentSunGlow > Props.sunlightThreshold) return false;
            }
            return true;
        }

        private void CacheEyeGraphic(Pawn pawn)
        {
            PawnKindDef pawnKind = pawn.kindDef;
            string eyeGraphicPath = null;

            switch (pawn.gender)
            {
                case Gender.Female:
                    drawSize = pawnKind.lifeStages[pawn.ageTracker.CurLifeStageIndex].femaleGraphicData.Graphic.drawSize;
                    eyeGraphicPath = Props.eyeGraphicFemale;
                    break;
                case Gender.Male:
                    drawSize = pawnKind.lifeStages[pawn.ageTracker.CurLifeStageIndex].bodyGraphicData.Graphic.drawSize;
                    eyeGraphicPath = Props.eyeGraphicMale;
                    break;
                case Gender.None:
                    return;
            }

            eyeGraphic = GraphicDatabase.Get<Graphic_Multi>(
                eyeGraphicPath,
                ShaderDatabase.TransparentPostLight,
                new Vector2(drawSize.x, drawSize.y),
                Props.eyeColor);
        }

        public override void PostDraw()
        {
            base.PostDraw();
            if (shouldDrawEyes && eyeGraphic != null && parent is Pawn parentPawn)
            {
                DrawGlowingEyeGraphic(parentPawn, eyeGraphic, parentPawn.Rotation, drawSize, fadeProgress);
            }
        }

        private void DrawGlowingEyeGraphic(Pawn pawn, Graphic eyeGraphic, Rot4 eyeRotation, Vector2 eyeDrawSize, float eyeFadeProgress)
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
            float yOffset = 0.01f;
            drawPos += new Vector3(0f, yOffset, 0f);

            Material fadedMat = FadedMaterialPool.FadedVersionOf(eyeGraphic.MatAt(eyeRotation), eyeFadeProgress);
            Matrix4x4 matrix = Matrix4x4.TRS(drawPos, Quaternion.identity, new Vector3(eyeDrawSize.x, 1f, eyeDrawSize.y));
            Mpb.Clear();
            
            Mpb.SetColor(ShaderPropertyIDs.Color, new Color(Props.eyeColor.r, Props.eyeColor.g, Props.eyeColor.b, eyeFadeProgress));
            Graphics.DrawMesh(MeshPool.plane10, matrix, fadedMat, 0, null, 0, Mpb);
        }
    }
}
