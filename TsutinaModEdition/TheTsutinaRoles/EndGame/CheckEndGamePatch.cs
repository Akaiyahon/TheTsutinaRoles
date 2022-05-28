using System;

namespace TheTsutinaRoles.EndGame
{
    public partial class EndGameManagerSetUpPatch
    {
        partial class EndGame
    {
            class CheckEndGamePatch
    {
        public static void Postfix(ExileController __instance)
        {
            try
            {
                WrapUpClass.WrapUpPostfix(__instance.exiled);
            }
            catch (Exception e)
            {
                TheTsutinaRolesPlugin.Logger.LogInfo("CHECKERROR:" + e);
            }
        }
    }
}
