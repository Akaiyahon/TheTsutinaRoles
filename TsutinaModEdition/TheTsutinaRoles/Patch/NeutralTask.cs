﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TheTsutinaRoles.Patch
{
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.StartGame))]
    class startgamepatch {
        public static void Postfix() {
            Roles.RoleClass.clearAndReloadRoles();
        }
    }
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.OnDestroy))]
    class NeutralTask
    {
        public static void Prefix(IntroCutscene __instance)
        {
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return;
            if (Roles.RoleClass.HomeSecurityGuard.HomeSecurityGuardPlayer.IsCheckListPlayerControl(PlayerControl.LocalPlayer))
            {
                    foreach (PlayerTask task in PlayerControl.LocalPlayer.myTasks)
                    {
                        task.Complete();
                    }
                    
                }
               PlayerControlHepler.refreshRoleDescription(PlayerControl.LocalPlayer);
        }
    }
}

