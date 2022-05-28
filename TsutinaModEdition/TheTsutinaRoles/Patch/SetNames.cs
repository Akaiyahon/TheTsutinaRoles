﻿using HarmonyLib;
using TheTsutinaRoles.CustomOption;
using TheTsutinaRoles.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TheTsutinaRoles.Patch
{
    public class SetNamesClass
    {
        public static Dictionary<int, string> AllNames = new Dictionary<int, string>();
        private static string roleNames;
        private static Color roleColors;

        public static void SetPlayerNameColor(PlayerControl p, Color color)
        {
            p.nameText.color = color;
        }
        public static void SetPlayerNameText(PlayerControl p,string text)
        {
            p.nameText.text = text;
        }
        public static void resetNameTagsAndColors()
        {
            Dictionary<byte, PlayerControl> playersById = ModHelpers.allPlayersById();

            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                player.nameText.text = player.CurrentOutfit.PlayerName;
                if (PlayerControl.LocalPlayer.Data.Role.IsImpostor && (player.Data.Role.IsImpostor || player.isRole(CustomRPC.RoleId.Egoist)))
                {
                    player.nameText.color = Palette.ImpostorRed;
                }
                else
                {
                    player.nameText.color = Color.white;
                }
            }
            if (MeetingHud.Instance != null)
            {
                foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                {
                    PlayerControl playerControl = playersById.ContainsKey((byte)player.TargetPlayerId) ? playersById[(byte)player.TargetPlayerId] : null;
                    if (playerControl != null)
                    {
                        player.NameText.text = playerControl.Data.PlayerName;
                        if (PlayerControl.LocalPlayer.Data.Role.IsImpostor && (playerControl.Data.Role.IsImpostor || playerControl.isRole(CustomRPC.RoleId.Egoist)))
                        {
                            player.NameText.color = Palette.ImpostorRed;
                        }
                        else
                        {
                            player.NameText.color = Color.white;
                        }
                    }
                }
            }
            if (PlayerControl.LocalPlayer.Data.Role.IsImpostor)
            {
                List<PlayerControl> impostors = PlayerControl.AllPlayerControls.ToArray().ToList();
                impostors.RemoveAll(x => !x.Data.Role.IsImpostor && !x.isRole(CustomRPC.RoleId.Egoist));
                foreach (PlayerControl player in impostors)
                    player.nameText.color = Palette.ImpostorRed;
                if (MeetingHud.Instance != null)
                    foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                    {
                        PlayerControl playerControl = ModHelpers.playerById((byte)player.TargetPlayerId);
                        if (playerControl != null && (playerControl.Data.Role.IsImpostor || playerControl.isRole(CustomRPC.RoleId.Egoist)))
                            player.NameText.color = Palette.ImpostorRed;
                    }
            }

        }
        public static void SetPlayerRoleInfo(PlayerControl p)
        {
                    Transform playerInfoTransform = p.nameText.transform.parent.FindChild("Info");
                    TMPro.TextMeshPro playerInfo = playerInfoTransform != null ? playerInfoTransform.GetComponent<TMPro.TextMeshPro>() : null;
                    if (playerInfo == null)
                    {
                        playerInfo = UnityEngine.Object.Instantiate(p.nameText, p.nameText.transform.parent);
                        playerInfo.fontSize *= 0.75f;
                        playerInfo.gameObject.name = "Info";
                    }

                    // Set the position every time bc it sometimes ends up in the wrong place due to camoflauge
                    playerInfo.transform.localPosition = p.nameText.transform.localPosition + Vector3.up * 0.5f;

                    PlayerVoteArea playerVoteArea = MeetingHud.Instance?.playerStates?.FirstOrDefault(x => x.TargetPlayerId == p.PlayerId);
                    Transform meetingInfoTransform = playerVoteArea != null ? playerVoteArea.NameText.transform.parent.FindChild("Info") : null;
                    TMPro.TextMeshPro meetingInfo = meetingInfoTransform != null ? meetingInfoTransform.GetComponent<TMPro.TextMeshPro>() : null;
                    if (meetingInfo == null && playerVoteArea != null)
                    {
                        meetingInfo = UnityEngine.Object.Instantiate(playerVoteArea.NameText, playerVoteArea.NameText.transform.parent);
                        meetingInfo.transform.localPosition += Vector3.down * 0.1f;
                        meetingInfo.fontSize = 1.5f;
                        meetingInfo.gameObject.name = "Info";
                    }

            // Set player name higher to align in middle
            if (meetingInfo != null && playerVoteArea != null)
            {
                var playerName = playerVoteArea.NameText;
                playerName.transform.localPosition = new Vector3(0.3384f, (0.0311f + 0.0683f), -0.1f);
            }

                    var role = p.getRole();
            if (role == CustomRPC.RoleId.DefaultRole || (role == CustomRPC.RoleId.Bestfalsecharge && p.isAlive())) {
                if (p.Data.Role.IsImpostor) 
                { 
                    roleNames = "ImpostorName"; 
                    roleColors = Roles.RoleClass.ImpostorRed; 
                }
                else 
                {
                    roleNames = "CrewMateName";
                    roleColors = Roles.RoleClass.CrewmateWhite;
                }

            } else
            {
                var introdate = Intro.IntroDate.GetIntroDate(role);
                roleNames = introdate.NameKey + "Name";
                roleColors = introdate.color;
            }
                    
                    
                    string playerInfoText = "";
                    string meetingInfoText = "";
                        playerInfoText = $"{CustomOption.CustomOptions.cs(roleColors, roleNames)}";
                        meetingInfoText = $"{CustomOption.CustomOptions.cs(roleColors, roleNames)}".Trim();


            
                    playerInfo.text = playerInfoText;
                    playerInfo.gameObject.SetActive(p.Visible);
            if (meetingInfo != null) meetingInfo.text = MeetingHud.Instance.state == MeetingHud.VoteStates.Results ? "" : meetingInfoText;  p.nameText.color = roleColors;
        }
        public static void SetPlayerNameColors(PlayerControl player)
        {
            var role = player.getRole();
            if (role == CustomRPC.RoleId.DefaultRole || (role == CustomRPC.RoleId.Bestfalsecharge && player.isAlive())) return;
            SetPlayerNameColor(player, Intro.IntroDate.GetIntroDate(role).color);
        }
        public static void SetPlayerRoleNames(PlayerControl player)
        {
            SetPlayerRoleInfo(player);
        }
        public static void QuarreledSet()
        {
            if (PlayerControl.LocalPlayer.IsQuarreled() && PlayerControl.LocalPlayer.isAlive())
            {
                string suffix = ModHelpers.cs(RoleClass.Quarreled.color,"○");
                PlayerControl side = PlayerControl.LocalPlayer.GetOneSideQuarreled();
                side.nameText.text += suffix;
                PlayerControl.LocalPlayer.nameText.text += suffix;
                foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates) {
                    if (side.PlayerId == player.TargetPlayerId || PlayerControl.LocalPlayer.PlayerId == player.TargetPlayerId)
                    {
                        player.NameText.text += suffix;
                    }
                }
            }
            if (!PlayerControl.LocalPlayer.isAlive() && RoleClass.Quarreled.QuarreledPlayer != new List<List<PlayerControl>>())
            {
                string suffix = ModHelpers.cs(RoleClass.Quarreled.color, "○");
                foreach (List<PlayerControl> ps in RoleClass.Quarreled.QuarreledPlayer) {
                    foreach (PlayerControl p in ps)
                    {
                        p.nameText.text += suffix;
                        foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                        {
                            if (p.PlayerId == player.TargetPlayerId)
                            {
                                player.NameText.text += suffix;
                            }
                        }
                    }
                }
            }
        }
    }
    public class SetNameUpdate
    {
        public static void Postfix(PlayerControl __instance)
        {
            SetNamesClass.resetNameTagsAndColors();
            if ((PlayerControl.LocalPlayer.isDead() || PlayerControl.LocalPlayer.isRole(CustomRPC.RoleId.God)) && !PlayerControl.LocalPlayer.isRole(CustomRPC.RoleId.NiceRedRidingHood))
            {
                foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                {
                        SetNamesClass.SetPlayerNameColors(player);
                        SetNamesClass.SetPlayerRoleNames(player);
                    
                }
            }
            else
            {
                if (PlayerControl.LocalPlayer.isRole(CustomRPC.RoleId.MadMate) && RoleClass.MadMate.IsImpostorCheck)
                {
                    foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                    {
                        if (p.isImpostor())
                        {
                            SetNamesClass.SetPlayerNameColors(p);
                            SetNamesClass.SetPlayerRoleNames(p);
                        }
                    }
                }
                if (PlayerControl.LocalPlayer.isRole(CustomRPC.RoleId.JackalFriends) && RoleClass.JackalFriends.IsJackalCheck)
                {
                    foreach (PlayerControl p in RoleClass.Jackal.JackalPlayer)
                    {
                        SetNamesClass.SetPlayerNameColors(p);
                        SetNamesClass.SetPlayerRoleNames(p);
                    }
                    foreach (PlayerControl p in RoleClass.Jackal.SidekickPlayer)
                    {
                        SetNamesClass.SetPlayerNameColors(p);
                        SetNamesClass.SetPlayerRoleNames(p);
                    }
                }
                SetNamesClass.QuarreledSet();
                if (RoleClass.Jackal.JackalPlayer.IsCheckListPlayerControl(PlayerControl.LocalPlayer) || RoleClass.Jackal.SidekickPlayer.IsCheckListPlayerControl(PlayerControl.LocalPlayer)) {
                    foreach (PlayerControl p in RoleClass.Jackal.JackalPlayer) {
                        if (p != PlayerControl.LocalPlayer) {
                            SetNamesClass.SetPlayerNameColors(p);
                            SetNamesClass.SetPlayerRoleNames(p);
                        }
                    }
                    foreach (PlayerControl p in RoleClass.Jackal.SidekickPlayer)
                    {
                        if (p != PlayerControl.LocalPlayer)
                        {
                            SetNamesClass.SetPlayerRoleNames(p);
                            SetNamesClass.SetPlayerNameColors(p);
                        }
                    }
                }
                SetNamesClass.SetPlayerRoleNames(PlayerControl.LocalPlayer);
                SetNamesClass.SetPlayerNameColors(PlayerControl.LocalPlayer);
            }
        }
        
    }
}
