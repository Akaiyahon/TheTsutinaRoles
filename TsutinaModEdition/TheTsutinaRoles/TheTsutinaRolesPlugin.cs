using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using HarmonyLib;

namespace TheTsutinaRoles
{
    [BepInPlugin(Id, "TheTsutinaRoles", VersionString)]
    [BepInProcess("Among Us.exe")]
    public partial class TheTsutinaRolesPlugin : BasePlugin
    {
        public const string VersionString = "1.0.0,Beta";
        public const string Id = "jp.Tsutina.TheTsutinaRoles";
        public static System.Version Version = System.Version.Parse(VersionString);
        public Harmony Harmony { get; } = new(Id);

        public ConfigEntry<string> ConfigName { get; private set; }
        public static TheTsutinaRolesPlugin Instance;
        internal static BepInEx.Logging.ManualLogSource Logger;
        public static int optionsPage = 1;

        public override void Load()
        {
            Logger = Log;
            Instance = this;

            Harmony.PatchAll();
        }
    }
}