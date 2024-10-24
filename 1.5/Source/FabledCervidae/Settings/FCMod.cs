using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace FabledCervidae
{
    public class FCMod : Mod
    {
        public List<PawnKindDef> allFCAnimals = [];
        public static FCSettings _settings;
        
        private string _defNamePrefix = "FC_";
        
        public FCMod(ModContentPack content) : base(content)
        {
            _settings = GetSettings<FCSettings>();
        }
        
        public override string SettingsCategory()
        {
            return "FC_ModName".Translate();
        }
        
        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            allFCAnimals = (from currentDef in DefDatabase<PawnKindDef>.AllDefs
                where currentDef.defName.Contains(_defNamePrefix)
                orderby currentDef.defName
                select currentDef).ToList();

            _settings.animalToggle ??= new Dictionary<string, bool>();
            
            foreach (PawnKindDef pkd in allFCAnimals)
            {
                if (!_settings.animalToggle.ContainsKey(pkd.defName))
                {
                    _settings.animalToggle[pkd.defName] = false;
                }
            }
            
            _settings.DoWindowContents(inRect);
            _settings.Write();
        }
    }
}