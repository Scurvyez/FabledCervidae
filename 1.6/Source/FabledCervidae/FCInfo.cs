using System.Collections.Generic;
using Verse;

namespace FabledCervidae
{
    [StaticConstructorOnStartup]
    public static class FCInfo
    {
        public static readonly HashSet<string> CervidFacts =
        [
            "FC_Fact1".Translate(),
            "FC_Fact2".Translate(),
            "FC_Fact3".Translate(),
            "FC_Fact4".Translate(),
            "FC_Fact5".Translate(),
            "FC_Fact6".Translate(),
            "FC_Fact7".Translate(),
            "FC_Fact8".Translate(),
            "FC_Fact9".Translate(),
            "FC_Fact10".Translate(),
        ];
    }
}