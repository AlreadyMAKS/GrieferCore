using System;
using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using Util.Commands;

namespace GrieferCore.Scripts
{
    [BepInPlugin("net.a1ready.GrieferCore.Scripts", "GrieferCore", "0.1.3.0")]   
    public class GrieferCorePlugin : BaseUnityPlugin
    {
        private static Harmony harmony;

        private void Awake()
        {
            harmony = new Harmony("com.a1ready.GrieferCore");
            harmony.PatchAll();
            
            Logger.LogInfo("GrieferCore loaded");
        }
        
    }
}