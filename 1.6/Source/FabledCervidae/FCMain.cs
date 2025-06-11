using Verse;

namespace FabledCervidae
{
    [StaticConstructorOnStartup]
    public static class FCMain
    {
        static FCMain()
        {
            string randomFact = FCInfo.CervidFacts.RandomElement();
            FCLog.Message(randomFact);
        }
    }
}