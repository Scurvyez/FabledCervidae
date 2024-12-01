using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace FabledCervidae
{
    public class FCMod : Mod
    {
        public List<PawnKindDef> AllFCAnimals = [];
        public static FCSettings Settings;
        
        private string _defNamePrefix = "FC_";
        
        public FCMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<FCSettings>();
        }
        
        public override string SettingsCategory()
        {
            return "FC_ModName".Translate();
        }
        
        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            AllFCAnimals = (from currentDef in DefDatabase<PawnKindDef>.AllDefs
                where currentDef.defName.Contains(_defNamePrefix)
                orderby currentDef.defName
                select currentDef).ToList();

            Settings.animalToggle ??= new Dictionary<string, bool>();
            
            foreach (PawnKindDef pkd in AllFCAnimals)
            {
                if (!Settings.animalToggle.ContainsKey(pkd.defName))
                {
                    Settings.animalToggle[pkd.defName] = false;
                }
            }
            
            Settings.DoWindowContents(inRect);
            Settings.Write();
        }
    }
}