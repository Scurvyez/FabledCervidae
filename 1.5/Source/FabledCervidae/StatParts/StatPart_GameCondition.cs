using System.Text;
using RimWorld;
using Verse;

namespace FabledCervidae
{
    public class StatPart_GameCondition : StatPart
    {
        private float _adjustedMoveSpeed;
        private ModExtension_GameConditionStatOffsets _ext;
        
        public override void TransformValue(StatRequest req, ref float val)
        {
            if (!ActiveFor(req.Thing)) return;
            val *= _adjustedMoveSpeed;
        }
        
        public override string ExplanationPart(StatRequest req)
        {
            if (!req.HasThing || !ActiveFor(req.Thing)) return null;
            StringBuilder explanationBuilder = new ();
            
            foreach (GameCondition activeCondition 
                     in req.Thing.Map.gameConditionManager.ActiveConditions)
            {
                foreach (GameConditionStatOffsetPair pair in _ext.pairs)
                {
                    if (pair.gameCondition == activeCondition.def)
                    {
                        explanationBuilder.AppendLine(
                            "StatsReport_MultiplierFor".Translate(activeCondition.def.LabelCap) 
                            + ": x" + pair.moveSpeedFactor.ToStringPercent()
                        );
                    }
                }
            }
            return explanationBuilder.Length > 0 
                ? explanationBuilder.ToString() 
                : null;
        }
        
        private bool ActiveFor(Thing t)
        {
            if (t?.Map == null 
                || !t.Position.IsValid 
                || t.Map.gameConditionManager == null) 
                return false;
            
            _ext = t.def.GetModExtension<ModExtension_GameConditionStatOffsets>();
            
            if (_ext == null) return false;
            foreach (GameCondition activeCondition 
                     in t.Map.gameConditionManager.ActiveConditions)
            {
                foreach (GameConditionStatOffsetPair pair in _ext.pairs)
                {
                    if (pair.gameCondition != activeCondition.def) continue;
                    _adjustedMoveSpeed = pair.moveSpeedFactor;
                    return true;
                }
            }
            return false;
        }
    }
}