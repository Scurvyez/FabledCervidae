using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace FabledCervidae
{
    public class FCMod : Mod
    {
        public static FCSettings Settings;

        private const string DEFNAME_PREFIX = "FC_";
        private List<PawnKindDef> _allFcAnimals = [];
        
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
            _allFcAnimals = (from currentDef in DefDatabase<PawnKindDef>.AllDefs
                where currentDef.defName.Contains(DEFNAME_PREFIX)
                orderby currentDef.defName
                select currentDef).ToList();

            Settings.animalToggle ??= new Dictionary<string, bool>();
            
            foreach (PawnKindDef pkd in _allFcAnimals)
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