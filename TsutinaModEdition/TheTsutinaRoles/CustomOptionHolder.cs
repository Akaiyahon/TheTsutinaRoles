using System.Collections.Generic;
using UnityEngine;
using BepInEx.Configuration;
using System;
using System.Linq;
using HarmonyLib;
using Hazel;
using System.Reflection;
using System.Text;
using TheTsutinaRoles.CustomOption;

namespace TheTsutinaRoles {

    public class CustomOptionHolder {
        public static string[] rates = new string[]{"0%", "10%", "20%", "30%", "40%", "50%", "60%", "70%", "80%", "90%", "100%"};
        public static string[] presets = new string[]{"preset1", "preset2", "preset3", "preset4", "preset5" };

        public static CustomOptionHolder presetSelection;
        public static CustomOptionHolder activateRoles;
        public static CustomOptionHolder crewmateRolesCountMin;
        public static CustomOptionHolder crewmateRolesCountMax;
        public static CustomOptionHolder neutralRolesCountMin;
        public static CustomOptionHolder neutralRolesCountMax;
        public static CustomOptionHolder impostorRolesCountMin;
        public static CustomOptionHolder impostorRolesCountMax;

        public static CustomRoleOption sheriffSpawnRate;
        public static CustomOptionHolder sheriffCooldown;
        public static CustomOptionHolder sheriffNumShots;
        public static CustomOptionHolder sheriffCanKillNeutrals;
        public static CustomOptionHolder sheriffMisfireKillsTarget;

        public static CustomRoleOption lighterSpawnRate;
        public static CustomOptionHolder lighterModeLightsOnVision;
        public static CustomOptionHolder lighterModeLightsOffVision;
        public static CustomOptionHolder lighterCooldown;
        public static CustomOptionHolder lighterDuration;
        public static CustomOptionHolder lighterCanSeeNinja;

        public static CustomRoleOption serialKillerSpawnRate;
        public static CustomOptionHolder serialKillerKillCooldown;
        public static CustomOptionHolder serialKillerSuicideTimer;
        public static CustomOptionHolder serialKillerResetTimer;

        internal static Dictionary<byte, byte[]> blockedRolePairings = new Dictionary<byte, byte[]>();
        private static object noVoteIsSelfVote;
        private static object allowParallelMedBayScans;
        private static object uselessOptions;
        private static object specialOptions;
        private static object maxNumberOfMeetings;
        private static object blockSkippingInEmergencyMeetings;
        private static object restrictVents;
        private static object restrictDevices;
        private static object restrictAdmin;
        private static object restrictCameras;
        private static object playerNameDupes;
        private static object dynamicMap;
        private static object dynamicMapEnableSkeld;
        private static object dynamicMapEnableMira;
        private static object dynamicMapEnablePolus;
        private static object dynamicMapEnableAirShip;
        private static object dynamicMapEnableDleks;
        private static object disableVents;

        public static object RoleType { get; private set; }
        public static object Lighter { get; private set; }

        public static string cs(Color c, string s) {
            return string.Format("<color=#{0:X2}{1:X2}{2:X2}{3:X2}>{4}</color>", ToByte(c.r), ToByte(c.g), ToByte(c.b), ToByte(c.a), s);
        }
 
        private static byte ToByte(float f) {
            f = Mathf.Clamp01(f);
            return (byte)(f * 255);
        }

        public static void Load()
        {

            // Role Options
            activateRoles = CustomOption.Create(7, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "blockOriginal"), true, null, true);

            presetSelection = CustomOption.Create(0, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "presetSelection"), presets, null, true);

            // Using new id's for the options to not break compatibilty with older versions
            crewmateRolesCountMin = CustomOption.Create(300, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "crewmateRolesCountMin"), 0f, 0f, 15f, 1f, null, true);
            crewmateRolesCountMax = CustomOption.Create(301, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "crewmateRolesCountMax"), 0f, 0f, 15f, 1f);
            neutralRolesCountMin = CustomOption.Create(302, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "neutralRolesCountMin"), 0f, 0f, 15f, 1f);
            neutralRolesCountMax = CustomOption.Create(303, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "neutralRolesCountMax"), 0f, 0f, 15f, 1f);
            impostorRolesCountMin = CustomOption.Create(304, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "impostorRolesCountMin"), 0f, 0f, 15f, 1f);
            impostorRolesCountMax = CustomOption.Create(305, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "impostorRolesCountMax"), 0f, 0f, 15f, 1f);


            serialKillerSpawnRate = new CustomRoleOption(1010, "serialKiller", SerialKiller.color, 3);
            serialKillerKillCooldown = CustomOption.Create(1012, "serialKillerKillCooldown", 15f, 2.5f, 60f, 2.5f, serialKillerSpawnRate, format: "unitSeconds");
            serialKillerSuicideTimer = CustomOption.Create(1013, "serialKillerSuicideTimer", 40f, 2.5f, 60f, 2.5f, serialKillerSpawnRate, format: "unitSeconds");
            serialKillerResetTimer = CustomOption.Create(1014, "serialKillerResetTimer", true, serialKillerSpawnRate);


            sheriffSpawnRate = new CustomRoleOption(100, "sheriff", Sheriff.color, 15);
            sheriffCooldown = CustomOption.Create(101, "sheriffCooldown", 30f, 2.5f, 60f, 2.5f, sheriffSpawnRate, format: "unitSeconds");
            sheriffNumShots = CustomOption.Create(103, "sheriffNumShots", 2f, 1f, 15f, 1f, sheriffSpawnRate, format: "unitShots");
            sheriffMisfireKillsTarget = CustomOption.Create(104, "sheriffMisfireKillsTarget", false, sheriffSpawnRate);
            sheriffCanKillNeutrals = CustomOption.Create(102, "sheriffCanKillNeutrals", false, sheriffSpawnRate);

            lighterSpawnRate = new CustomRoleOption(110, "lighter", Lighter.color, 15);
            lighterModeLightsOnVision = CustomOption.Create(111, "lighterModeLightsOnVision", 2f, 0.25f, 5f, 0.25f, lighterSpawnRate, format: "unitMultiplier");
            lighterModeLightsOffVision = CustomOption.Create(112, "lighterModeLightsOffVision", 0.75f, 0.25f, 5f, 0.25f, lighterSpawnRate, format: "unitMultiplier");
            lighterCooldown = CustomOption.Create(113, "lighterCooldown", 30f, 5f, 120f, 5f, lighterSpawnRate, format: "unitSeconds");
            lighterDuration = CustomOption.Create(114, "lighterDuration", 5f, 2.5f, 60f, 2.5f, lighterSpawnRate, format: "unitSeconds");
            lighterCanSeeNinja = CustomOption.Create(115, "lighterCanSeeNinja", true, lighterSpawnRate);


            // Other options
            specialOptions = new CustomOptionBlank(null);
            maxNumberOfMeetings = CustomOption.Create(3, "maxNumberOfMeetings", 10, 0, 15, 1, specialOptions, true);
            blockSkippingInEmergencyMeetings = CustomOption.Create(4, "blockSkippingInEmergencyMeetings", false, specialOptions);
            noVoteIsSelfVote = CustomOption.Create(5, "noVoteIsSelfVote", false, specialOptions);
            allowParallelMedBayScans = CustomOption.Create(540, "parallelMedbayScans", false, specialOptions);

            restrictDevices = CustomOption.Create(510, "restrictDevices", new string[] { "optionOff", "restrictPerTurn", "restrictPerGame" }, specialOptions);
            restrictAdmin = CustomOption.Create(501, "disableAdmin", 30f, 0f, 600f, 5f, restrictDevices, format: "unitSeconds");
            restrictCameras = CustomOption.Create(502, "disableCameras", 30f, 0f, 600f, 5f, restrictDevices, format: "unitSeconds");
            restrictVents = CustomOption.Create(503, "disableVitals", 30f, 0f, 600f, 5f, restrictDevices, format: "unitSeconds");

            NewMethod();
            dynamicMap = CustomOption.Create(8, "playRandomMaps", false, uselessOptions);
            dynamicMapEnableSkeld = CustomOption.Create(531, "dynamicMapEnableSkeld", true, dynamicMap, false);
            dynamicMapEnableMira = CustomOption.Create(532, "dynamicMapEnableMira", true, dynamicMap, false);
            dynamicMapEnablePolus = CustomOption.Create(533, "dynamicMapEnablePolus", true, dynamicMap, false);
            dynamicMapEnableAirShip = CustomOption.Create(534, "dynamicMapEnableAirShip", true, dynamicMap, false);
            dynamicMapEnableDleks = CustomOption.Create(535, "dynamicMapEnableDleks", false, dynamicMap, false);

            disableVents = CustomOption.Create(504, "disableVents", false, uselessOptions);
            playerNameDupes = CustomOption.Create(522, "playerNameDupes", false, uselessOptions);

            blockedRolePairings.Add((byte)RoleType.Vampire, new[] { (byte)RoleType.Warlock });
            blockedRolePairings.Add((byte)RoleType.Warlock, new[] { (byte)RoleType.Vampire });
            blockedRolePairings.Add((byte)RoleType.Spy, new[] { (byte)RoleType.Mini });
            blockedRolePairings.Add((byte)RoleType.Mini, new[] { (byte)RoleType.Spy });
            blockedRolePairings.Add((byte)RoleType.Vulture, new[] { (byte)RoleType.Cleaner });
            blockedRolePairings.Add((byte)RoleType.Cleaner, new[] { (byte)RoleType.Vulture });
        }

        private static void NewMethod()
        {
            uselessOptions = CustomOption.Create(530, "uselessOptions", false, null, isHeader: true);
        }
    }

}
