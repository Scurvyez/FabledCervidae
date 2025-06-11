using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace FabledCervidae
{
    public class FCSettings : ModSettings
    {
        public Dictionary<string, bool> animalToggle = new ();
        
        private static Vector2 _scrollPosition = Vector2.zero;
        private List<string> _animalKeys;
        private List<bool> _animalValues;
        
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref animalToggle, "animalToggle", 
                LookMode.Value, LookMode.Value, ref _animalKeys, ref _animalValues);
        }
        
        public void DoWindowContents(Rect inRect)
        {
            List<string> keyNames = animalToggle.Keys.ToList().OrderBy(x => x).ToList();
            Listing_Standard ls = new ();

            float totalWidth = inRect.width;
            float spacing = 10f;
            float itemWidth = (totalWidth / 4f) - spacing;
            float itemHeight = itemWidth + 200f;

            Rect viewRect = new(0f, 0f, totalWidth, itemHeight);
            Widgets.BeginScrollView(inRect, ref _scrollPosition, viewRect);
            ls.Begin(viewRect);

            for (int i = 0; i < keyNames.Count; i++)
            {
                string key = keyNames[i];
                bool state = animalToggle[key];
                float xPos = i * (itemWidth + spacing);
                
                Rect labelRect = new(xPos, 0f, itemWidth * 0.6f, 24f);
                Rect checkboxRect = new(xPos + (itemWidth - 24f), 0f, 24f, 24f);
                Rect textureRect = new(xPos, labelRect.yMax + 10f, itemWidth, itemWidth);

                PawnKindDef pawnKindDef = PawnKindDef.Named(key);
                Texture2D texture = ContentFinder<Texture2D>.Get(
                    $"FabledCervidae/UI/{pawnKindDef.defName}", false);

                Widgets.Label(labelRect, 
                    "Disable " + pawnKindDef.LabelCap.Colorize(FCLog.MessageMsgCol));
                TooltipHandler.TipRegion(labelRect, "FC_AnimalToggleTooltip".Translate());
                Widgets.Checkbox(checkboxRect.x, checkboxRect.y, ref state, 24f);
                
                if (texture != null)
                {
                    GUI.DrawTexture(textureRect, texture);
                }
                
                animalToggle[key] = state;
            }

            ls.End();
            Widgets.EndScrollView();
            Write();
        }
    }
}