using HarmonyLib;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TheTsutinaRoles.MapOptions
{
    [HarmonyPatch]
    public class MapOption
    {
        public static bool UseAdmin;
        public static bool UseVitalOrDoorLog;
        public static bool UseCamera;
        public static void ClearAndReload()
        {
            {


                {
                    UseAdmin = true;
                    UseVitalOrDoorLog = true;
                    UseCamera = true;
                }
            }
        }

        internal static void LoadOption()
        {
            throw new NotImplementedException();
        }
    }
}