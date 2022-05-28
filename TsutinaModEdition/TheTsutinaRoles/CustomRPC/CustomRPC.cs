using HarmonyLib;
using Hazel;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using System.Collections;
using BepInEx.IL2CPP.Utils;
using InnerNet;
using TheTsutinaRoles.CustomOption;

namespace TheTsutinaRoles.CustomRPC
{
    public enum RoleId
    {
        DefaultRole,
        //RoleId
    }

    public enum CustomRPC
    {
        ShareOptions = 91,
        ShareWinner = 92,
    }
    public static class RPCProcedure
    {
        public static void ShareOptions(int numberOfOptions, MessageReader reader)
        {
            try
            {
                for (int i = 0; i < numberOfOptions; i++)
                {
                    uint optionId = reader.ReadPackedUInt32();
                    uint selection = reader.ReadPackedUInt32();
                    CustomOption.CustomOption option = CustomOption.CustomOption.options.FirstOrDefault(option => option.id == (int)optionId);
                    option.updateSelection((int)selection);
                }
            }
            catch (Exception e)
            {
                TheTsutinaRolesPlugin.Logger.LogError("Error while deserializing options: " + e.Message);
            }
        }
        [HarmonyPatch(typeof(KillAnimation), nameof(KillAnimation.CoPerformKill))]
        class KillAnimationCoPerformKillPatch
        {
            public static bool hideNextAnimation = false;

            public static void Prefix(KillAnimation __instance, [HarmonyArgument(0)] ref PlayerControl source, [HarmonyArgument(1)] ref PlayerControl target)
            {
                if (hideNextAnimation)
                    source = target;
                hideNextAnimation = false;
            }
        }
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
        class RPCHandlerPatch
        {
            static void Postfix([HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader)
            {
                    byte packetId = callId;
                    switch (packetId)
                    {

                    // Main Controls

                        case (byte)CustomRPC.ShareOptions:
                            RPCProcedure.ShareOptions((int)reader.ReadPackedUInt32(), reader);
                            break;
                }
            }
        }
        
    }
}
