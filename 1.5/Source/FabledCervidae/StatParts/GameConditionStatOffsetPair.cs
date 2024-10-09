using System.Xml;
using Verse;

namespace FabledCervidae
{
    public class GameConditionStatOffsetPair : IExposable
    {
        public GameConditionDef gameCondition;
        public float moveSpeedFactor = 1f;
        
        public void ExposeData()
        {
            Scribe_Defs.Look(ref gameCondition, "gameCondition");
            Scribe_Values.Look(ref moveSpeedFactor, "moveSpeedFactor");
        }
        
        public void LoadDataFromXmlCustom(XmlNode xmlRoot)
        {
            foreach (XmlNode childNode in xmlRoot.ChildNodes)
            {
                if (childNode.Name == "moveSpeedFactor")
                {
                    moveSpeedFactor = ParseHelper.FromString<float>(childNode.InnerText);
                }
                else if (childNode.Name == "gameCondition")
                {
                    DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, "gameCondition", childNode.InnerText);
                }
            }
        }
    }
}