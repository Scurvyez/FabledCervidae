using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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
            
            harmony.Patch(original: AccessTools.Method(typeof(BiomeDef), "CommonalityOfAnimal"),
                postfix: new HarmonyMethod(typeof(Patches), nameof(BiomeDefCommonalityOfAnimal_Postfix)));

            harmony.Patch(original: AccessTools.Method(typeof(GenRecipe), "MakeRecipeProducts"),
                postfix: new HarmonyMethod(typeof(Patches), nameof(MakeRecipeProducts_Postfix)));

            harmony.Patch(original: AccessTools.Method(typeof(Pawn_RelationsTracker), "RelationsTrackerTick"),
                postfix: new HarmonyMethod(typeof(Patches), nameof(RelationsTrackerTick_Postfix)));
            
            harmony.Patch(original: AccessTools.Method(typeof(Pawn_GeneTracker), "GeneTrackerTick"),
                postfix: new HarmonyMethod(typeof(Patches), nameof(GeneTrackerTick_Postfix)));
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
            if (!__instance.HasComp(typeof(Comp_DiseaseImmunity)) 
                || __instance.IsCorpse) return;
            
            CompProperties_DiseaseImmunity compProps 
                = __instance.GetCompProperties<CompProperties_DiseaseImmunity>();
            
            if (compProps == null) return;
            if (compProps.ownImmunities.NullOrEmpty()) return;
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
        
        public static void BiomeDefCommonalityOfAnimal_Postfix(PawnKindDef animalDef, ref float __result)
        {
            if (FCMod.Settings.animalToggle == null
                || !FCMod.Settings.animalToggle
                    .TryGetValue(animalDef.defName, out bool toggle))
                return;
            
            if (toggle)
            {
                __result = 0f;
            }
        }
        
        public static void MakeRecipeProducts_Postfix(ref IEnumerable<Thing> __result, 
            Thing dominantIngredient)
        { 
            if (dominantIngredient is not Corpse corpse) return;
            
            List<Thing> newResult = __result.ToList();
            Pawn innerPawn = corpse.InnerPawn;
            RaceProperties raceProps = innerPawn.def?.race;
            
            if (innerPawn.ageTracker?.CurLifeStage == FCDefOf.AnimalAdult && 
                innerPawn.gender == Gender.Male &&
                raceProps?.body == FCDefOf.FC_QuadrupedAnimalWithHoovesAndAntlers)
            {
                ModExtension_ButcherDrops ext 
                    = innerPawn.def?.GetModExtension<ModExtension_ButcherDrops>();
                
                if (ext == null || !Rand.Chance(ext.dropChance)) return;
                Thing antler = ThingMaker.MakeThing(ext.adultAntler);
                antler.stackCount = Rand.Chance(ext.extraDropChance) ? 2 : 1;
                newResult.Add(antler);
            }
            
            __result = newResult;
        }
        
        private static void RelationsTrackerTick_Postfix(Pawn ___pawn)
        {
            if (!ModsConfig.BiotechActive 
                || !___pawn.IsHashIntervalTick(2500)) return;

            if (___pawn.RaceProps?.body 
                != FCDefOf.FC_QuadrupedAnimalWithHoovesAndAntlers) return;
            Pawn masterColonist = ___pawn.playerSettings?.Master;
            bool hasBondedMaster = PatchesHelper.HasBondedColonist(___pawn);
            
            if (masterColonist == null || !hasBondedMaster) return;
            if (PatchesHelper.CervidVitalityGenes.Any(gene 
                    => masterColonist.genes.HasActiveGene(gene))) return;
            
            GeneDef geneToAdd = ___pawn.def switch
            {
                _ when ___pawn.def == FCDefOf.FC_Scire 
                    => FCDefOf.FC_Immunities_Scire,
                _ when ___pawn.def == FCDefOf.FC_Mirelung 
                    => FCDefOf.FC_Immunities_Mirelung,
                _ when ___pawn.def == FCDefOf.FC_Infernihart
                    => FCDefOf.FC_Immunities_Infernihart,
                _ when ___pawn.def == FCDefOf.FC_Auravine 
                    => FCDefOf.FC_Immunities_Auravine,
                _ 
                    => null
            };
            
            if (geneToAdd == null) return;
            masterColonist.genes.AddGene(geneToAdd, true);
        }
        
        private static void GeneTrackerTick_Postfix(Pawn ___pawn)
        {
            if (!___pawn.IsHashIntervalTick(2500)) return;
            List<DirectPawnRelation> relations = ___pawn.relations.DirectRelations;
            List<Gene> ownGenes = ___pawn.genes?.GenesListForReading;
            
            bool hasTargetBodyType = relations?.Any(relation => 
                relation.otherPawn.RaceProps.body 
                == FCDefOf.FC_QuadrupedAnimalWithHoovesAndAntlers) ?? false;

            if (hasTargetBodyType || ownGenes == null) return;
            foreach (Gene gene in ownGenes.Where(g 
                         => PatchesHelper.CervidVitalityGenes.Contains(g.def)).ToList())
            {
                ___pawn.genes.RemoveGene(gene);
            }
        }
    }
}