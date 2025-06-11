using UnityEngine;
using Verse;

namespace FabledCervidae
{
    public class Comp_SexualDimorphism : ThingComp
    {
        public CompProperties_SexualDimorphism Props => (CompProperties_SexualDimorphism) props;
        
        public Color NewColor;
        public Color NewColorTwo;
        private readonly float _alpha = 1f;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            if (respawningAfterLoad || parent is not Pawn pawn) return;
            switch (pawn.gender)
            {
                case Gender.Male:
                    NewColor = new Color(
                        Props.maleRRangeOne.RandomInRange, 
                        Props.maleGRangeOne.RandomInRange, 
                        Props.maleBRangeOne.RandomInRange, _alpha);
                    NewColorTwo = new Color(
                        Props.maleRRangeTwo.RandomInRange, 
                        Props.maleGRangeTwo.RandomInRange, 
                        Props.maleBRangeTwo.RandomInRange, _alpha);
                    break;
                case Gender.Female:
                    NewColor = new Color(
                        Props.femaleRRangeOne.RandomInRange, 
                        Props.femaleGRangeOne.RandomInRange, 
                        Props.femaleBRangeOne.RandomInRange, _alpha);
                    NewColorTwo = new Color(
                        Props.femaleRRangeTwo.RandomInRange, 
                        Props.femaleGRangeTwo.RandomInRange, 
                        Props.femaleBRangeTwo.RandomInRange, _alpha);
                    break;
                case Gender.None:
                default:
                    NewColor = Color.white;
                    NewColorTwo = Color.white;
                    break;
            }
            pawn.Drawer.renderer.SetAllGraphicsDirty();
        }
        
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref NewColor, "NewColor");
            Scribe_Values.Look(ref NewColorTwo, "NewColorTwo");
        }
    }
}