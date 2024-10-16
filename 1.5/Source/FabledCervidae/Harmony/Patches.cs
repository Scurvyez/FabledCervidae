﻿using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using LudeonTK;
using RimWorld;
using UnityEngine;
using Verse;

namespace FabledCervidae
{
    [StaticConstructorOnStartup]
    public static class Patches
    {
        [TweakValue("Graphics", -3f, 3f)]
        public static float fc_DOffsetNorthX = 0f;
        [TweakValue("Graphics", -3f, 3f)]
        public static float fc_DOffsetNorthZ = 0f;

        [TweakValue("Graphics", -3f, 3f)]
        public static float fc_DOffsetEastX = 0f;
        [TweakValue("Graphics", -3f, 3f)]
        public static float fc_DOffsetEastZ = 0f;

        [TweakValue("Graphics", -3f, 3f)]
        public static float fc_DOffsetSouthX = 0f;
        [TweakValue("Graphics", -3f, 3f)]
        public static float fc_DOffsetSouthZ = 0f;

        [TweakValue("Graphics", -3f, 3f)]
        public static float fc_DOffsetWestX = 0f;
        [TweakValue("Graphics", -3f, 3f)]
        public static float fc_DOffsetWestZ = 0f;
        
        [TweakValue("Graphics", 0f, 1f)]
        public static bool fc_Debugging;
        
        static Patches()
        {
            Harmony harmony = new ("rimworld.scurvyez.fabledcervidae");
            
            harmony.Patch(original: AccessTools.Method(typeof(PawnRenderer), "ParallelGetPreRenderResults"),
                prefix: new HarmonyMethod(typeof(Patches), nameof(ParallelGetPreRenderResults_Prefix)));
            
            harmony.Patch(original: AccessTools.Method(typeof(ThingDef), "SpecialDisplayStats"),
                postfix: new HarmonyMethod(typeof(Patches), nameof(SpecialDisplayStats_Postfix)));
        }
        
        private static void ParallelGetPreRenderResults_Prefix(PawnRenderer __instance, ref Vector3 drawLoc, Pawn ___pawn)
        {
            if (__instance == null || ___pawn.kindDef == null || !___pawn.RaceProps.Animal)return;
            PawnKindLifeStage curLifeStage = ___pawn.ageTracker.CurKindLifeStage;
            
            GraphicData graphicData = ___pawn.gender == Gender.Female 
                ? curLifeStage?.femaleGraphicData ?? curLifeStage?.bodyGraphicData 
                : curLifeStage?.bodyGraphicData;

            if (graphicData?.drawOffsetNorth == null ||
                graphicData?.drawOffsetEast ==  null ||
                graphicData?.drawOffsetSouth == null ||
                graphicData?.drawOffsetWest == null) return;
            Vector3? offset = null;
            
            if (fc_Debugging)
            {
                offset = ___pawn.Rotation.AsInt switch
                {
                    0 => new Vector3(fc_DOffsetNorthX, 0, fc_DOffsetNorthZ),
                    1 => new Vector3(fc_DOffsetEastX, 0, fc_DOffsetEastZ),
                    2 => new Vector3(fc_DOffsetSouthX, 0, fc_DOffsetSouthZ),
                    3 => new Vector3(fc_DOffsetWestX, 0, fc_DOffsetWestZ),
                    _ => null
                };
            }
            else
            {
                offset = ___pawn.Rotation.AsInt switch
                {
                    0 => graphicData.drawOffsetNorth,
                    1 => graphicData.drawOffsetEast,
                    2 => graphicData.drawOffsetSouth,
                    3 => graphicData.drawOffsetWest,
                    _ => null
                };
            }

            if (!offset.HasValue) return;
            drawLoc += offset.Value;
        }
        
        private static void SpecialDisplayStats_Postfix(ThingDef __instance, ref IEnumerable<StatDrawEntry> __result)
        {
            if (!__instance.HasComp(typeof(Comp_DiseaseImmunity)) || __instance.IsCorpse) return;
            CompProperties_DiseaseImmunity compProps = __instance.GetCompProperties<CompProperties_DiseaseImmunity>();
            
            if (compProps == null) return;
            if (!compProps.ownImmunities.NullOrEmpty())
            {
                string ownImmunityLabel = "FC_OwnImmunitiesLabel".Translate();
                string ownImmunityDesc = "FC_OwnImmunitiesDesc".Translate();

                StringBuilder ownImmunityList = new ();
                foreach (HediffDef immunity in compProps.ownImmunities)
                {
                    ownImmunityList.AppendLine("   - " + immunity.LabelCap);
                }

                string reportText = ownImmunityDesc + "\n\n" + ownImmunityList;
                __result = __result.AddItem(new StatDrawEntry(
                    StatCategoryDefOf.PawnMisc,
                    ownImmunityLabel,
                    "x" + compProps.ownImmunities.Count,
                    reportText,
                    2
                ));
            }

            if (compProps.masterImmunities.NullOrEmpty()) return;
            {
                string masterImmunityLabel = "FC_MasterImmunitiesLabel".Translate();
                string masterImmunityDesc = "FC_MasterImmunitiesDesc".Translate();

                StringBuilder masterImmunityList = new ();
                foreach (HediffDef immunity in compProps.masterImmunities)
                {
                    masterImmunityList.AppendLine("   - " + immunity.LabelCap);
                }

                string reportText = masterImmunityDesc + "\n\n" + masterImmunityList;
                __result = __result.AddItem(new StatDrawEntry(
                    StatCategoryDefOf.PawnMisc,
                    masterImmunityLabel,
                    "x" + compProps.masterImmunities.Count,
                    reportText,
                    1
                ));
            }
        }
    }
}