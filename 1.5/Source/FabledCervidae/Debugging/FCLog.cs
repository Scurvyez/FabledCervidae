using UnityEngine;
using Verse;

namespace FabledCervidae
{
    public static class FCLog
    {
        private static readonly Color ErrorMsgCol = new (0.4f, 0.54902f, 1.0f);
        private static readonly Color WarningMsgCol = new (0.70196f, 0.4f, 1.0f);
        private static readonly Color MessageMsgCol = new (0.4f, 1.0f, 0.54902f);
        
        public static void Error(string msg)
        {
            Log.Error("[Fabled Cervidae] ".Colorize(ErrorMsgCol) + msg);
        }

        public static void Warning(string msg)
        {
            Log.Warning("[Fabled Cervidae] ".Colorize(WarningMsgCol) + msg);
        }

        public static void Message(string msg)
        {
            Log.Message("[Fabled Cervidae] ".Colorize(MessageMsgCol) + msg);
        }
    }
}