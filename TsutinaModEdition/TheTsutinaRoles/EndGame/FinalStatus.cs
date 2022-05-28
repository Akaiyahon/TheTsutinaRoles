using TheTsutinaRoles.Patch;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TheTsutinaRoles.EndGame
{
    class FinalStatusPatch
    {
        public static class FinalStatusData
        {
            
            public static List<Tuple<Vector3, bool>> localPlayerPositions = new List<Tuple<Vector3, bool>>();
            public static List<DeadPlayer> deadPlayers = new List<DeadPlayer>();
            public static Dictionary<int, FinalStatus> FinalStatuses = new Dictionary<int, FinalStatus>();

            public static void ClearFinalStatusData()
            {
                localPlayerPositions = new List<Tuple<Vector3, bool>>();
                deadPlayers = new List<DeadPlayer>();
                FinalStatuses = new Dictionary<int, FinalStatus>();
            }

        }
    }
    enum FinalStatus
    {
        Alive,
        Kill,
        Exiled,
        SheriffKill,
        SheriffMisFire,
        MeetingSheriffKill,
        MeetingSheriffMisFire,
        SelfBomb,
        BySelfBomb,
        Disconnected,
        Dead,
        Sabotage
    }
}
