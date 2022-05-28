﻿using HarmonyLib;
using TheTsutinaRoles.Roles;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TheTsutinaRoles.Roles;

namespace TheTsutinaRoles.Patch
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
    class LightPatch
    {
        public static bool Prefix(ref float __result, ShipStatus __instance, [HarmonyArgument(0)] GameData.PlayerInfo player)
        {
            ISystemType systemType = __instance.Systems.ContainsKey(SystemTypes.Electrical) ? __instance.Systems[SystemTypes.Electrical] : null;
            if (systemType == null) return true;
            SwitchSystem switchSystem = systemType.TryCast<SwitchSystem>();
            if (switchSystem == null) return true;

            float num = (float)switchSystem.Value / 255f;

            if (player == null || player.IsDead)
                __result = __instance.MaxLightRadius;
            else if (Clergyman.IsLightOutVision())
                __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, num) * RoleClass.Clergyman.DownImpoVision;
            else if(player.Object.isRole(CustomRPC.RoleId.CountChanger) && CountChanger.GetRoleType(player.Object) == TeamRoleType.Crewmate)
                __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, num) * PlayerControl.GameOptions.ImpostorLightMod;
            else if (player.Role.IsImpostor || RoleHelpers.IsImpostorLight(player.Object))
                __result = __instance.MaxLightRadius * PlayerControl.GameOptions.ImpostorLightMod;
            else if (RoleClass.Lighter.LighterPlayer.IsCheckListPlayerControl(PlayerControl.LocalPlayer) && RoleClass.Lighter.IsLightOn)
                __result = Mathf.Lerp(__instance.MaxLightRadius * RoleClass.Lighter.UpVision, __instance.MaxLightRadius * RoleClass.Lighter.UpVision, num);
            else
                __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, num) * PlayerControl.GameOptions.CrewLightMod;
            return false;
        }
    }
}
