using System.Collections.Generic;
using UnityEngine;
using BepInEx.Configuration;
using System;
using System.Linq;
using HarmonyLib;
using Hazel;
using System.Reflection;
using static System.Drawing.Color;
using System.Text;
using TheTsutinaRoles.CustomOption;
using TheTsutinaRoles.Roles;
using TheTsutinaRoles.Patch;

namespace TheTsutinaRoles.CustomOption
{
    public class CustomOptions
    {
        public static string[] rates = new string[] { "0%", "10%", "20%", "30%", "40%", "50%", "60%", "70%", "80%", "90%", "100%" };

        public static string[] rates4 = new string[] {"0%","25%","50%","75%","100%"};

        public static string[] presets = new string[] { "preset1", "preset2", "preset3", "preset4", "preset5", "preset6", "preset7", "preset8", "preset9", "preset10" };
        public static CustomOption presetSelection;

        public static CustomOption specialOptions;
        public static CustomOption hideSettings;

        public static CustomOption crewmateRolesCountMax;
        public static CustomOption impostorRolesCountMax;
        public static CustomOption neutralRolesCountMax;

        public static CustomOption IsDebugMode;

        public static CustomOption DisconnectNotPCOption;

        public static CustomRoleOption LighterOption;
        public static CustomOption LighterPlayerCount;
        public static CustomOption LighterCoolTime;
        public static CustomOption LighterDurationTime;
        public static CustomOption LighterUpVision;
     
        public static CustomRoleOption SheriffOption;
        public static CustomOption SheriffPlayerCount;
        public static CustomOption SheriffCoolTime;
        public static CustomOption SheriffMadMateKill;
        public static CustomOption SheriffNeutralKill;
        public static CustomOption SheriffLoversKill;
        public static CustomOption SheriffKillMaxCount;

        public static CustomRoleOption SerialKillerOption;
        public static CustomOption SerialKillerPlayerCount;
        public static CustomOption SerialKillerSuicideTime;
        public static CustomOption SerialKillerKillTime;
        public static CustomOption SerialKillerIsMeetingReset;

        private static string[] GuesserCount = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15" };
        public static string[] LevelingerTexts = new string[] { };
        private static string[] VultureDeadBodyCount = new string[] { "1", "2", "3", "4", "5", "6" };
        public static List<float> CrewPlayers = new List<float> { 1f,1f,15f,1f};
        public static List<float> ImpostorPlayers = new List<float> { 1f, 1f, 5f, 1f };
        public static List<float> QuarreledPlayers = new List<float> { 1f,1f,7f,1f};
        // public static CustomOption ;

        internal static Dictionary<byte, byte[]> blockedRolePairings = new Dictionary<byte, byte[]>();

        public static CustomRoleOption AmnesiacOption { get; private set; }
        public static object RoleClass { get; private set; }
        public static object ConfigRoles { get; private set; }

        public static string cs(Color c, string s)
        {
            return string.Format("<color=#{0:X2}{1:X2}{2:X2}{3:X2}>{4}</color>", ToByte(c.r), ToByte(c.g), ToByte(c.b), ToByte(c.a), ModTranslation.getString(s));
        }


        public static byte ToByte(float f)
        {
            f = Mathf.Clamp01(f);
            return (byte)(f * 255);
        }

        public static void Load()
        {
            var Levedatas = new List<string>(){ "optionOff", "LevelingerSettingKeep", "PursuerName", "TeleporterName", "SidekickName", "SpeedBoosterName", "MovingName" };
            var LeveTransed = new List<string>();
            foreach (string data in Levedatas)
            {
                LeveTransed.Add(ModTranslation.getString(data));
            }
            LevelingerTexts = LeveTransed.ToArray();
            presetSelection = CustomOption.Create(0, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "SettingpresetSelection"), presets, null, true);

            specialOptions = new CustomOptionBlank(null);
            hideSettings = CustomOption.Create(2, cs(Color.white, "SettingsHideSetting"), false, specialOptions);

            crewmateRolesCountMax = CustomOption.Create(3, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "SettingMaxCrewRole"), 0f, 0f, 15f, 1f);
            neutralRolesCountMax = CustomOption.Create(4, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "SettingMaxNeutralRole"), 0f, 0f, 15f, 1f);
            impostorRolesCountMax = CustomOption.Create(5, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "SettingMaxImpoRole"), 0f, 0f, 3f, 1f);

            if (ConfigRoles.DebugMode.Value) {
                IsDebugMode = CustomOption.Create(159, "デバッグモード", false, null, isHeader: true);
            }

            DisconnectNotPCOption = CustomOption.Create(168, cs(Color.white, "PC以外はキックする"), true,null,isHeader:true);

            MapOptions.MapOption.LoadOption();

            LighterOption = new CustomRoleOption(
                14,
                "LighterName",
                RoleClass.Lighter.color,
                1);
            LighterPlayerCount = CustomOption.Create(15, cs(Color.white, "SettingPlayerCountName"), CrewPlayers[0], CrewPlayers[1], CrewPlayers[2], CrewPlayers[3], LighterOption);
            LighterCoolTime = CustomOption.Create(16, ModTranslation.getString("LigtherCoolDownSetting"), 30f, 2.5f, 60f, 2.5f, LighterOption, format: "unitSeconds");
            LighterDurationTime = CustomOption.Create(17, ModTranslation.getString("LigtherDurationSetting"), 10f, 1f, 20f, 0.5f, LighterOption, format: "unitSeconds");
            LighterUpVision = CustomOption.Create(204, ModTranslation.getString("LighterUpVisionSetting"), 0.25f, 0f, 5f, 0.25f, LighterOption);

            SheriffOption = new CustomRoleOption(26, "SheriffName", RoleClass.Sheriff.color, 1);
            SheriffPlayerCount = CustomOption.Create(27, cs(Color.white, "SettingPlayerCountName"), CrewPlayers[0], CrewPlayers[1], CrewPlayers[2], CrewPlayers[3], SheriffOption);
            SheriffCoolTime = CustomOption.Create(28, ModTranslation.getString("SheriffCoolDownSetting"), 30f, 2.5f, 60f, 2.5f, SheriffOption, format: "unitSeconds");
            SheriffNeutralKill = CustomOption.Create(173, ModTranslation.getString("SheriffIsKillMadMateSetting"), false, SheriffOption);
            SheriffLoversKill = CustomOption.Create(258, ModTranslation.getString("SheriffIsKillLoversSetting"), false, SheriffOption);
            SheriffMadMateKill = CustomOption.Create(29, ModTranslation.getString("SheriffIsKillNeutralSetting"), false, SheriffOption);
            SheriffKillMaxCount = CustomOption.Create(30, ModTranslation.getString("SheriffMaxKillCountSetting"), 1f, 1f, 20f, 1, SheriffOption, format: "unitSeconds");
            


            TheTsutinaRolesPlugin.Logger.LogInfo("設定のidのMax:"+CustomOption.Max);
        }
    }
}