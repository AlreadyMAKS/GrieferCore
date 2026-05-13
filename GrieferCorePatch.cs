using Assets.Scripts;
using Assets.Scripts.AssetCreation;
using Assets.Scripts.Inventory;
using Assets.Scripts.Localization2;
using Assets.Scripts.Networking;
using Assets.Scripts.Objects;
using Assets.Scripts.Serialization;
using HarmonyLib;
using UnityEngine;

namespace GrieferCore.Scripts
{
    // [HarmonyPatch(typeof(Constructor), "Construct")]
    // public class Constructor_Construct_Patch
    // {
    //     static void Postfix(Constructor __instance, long creatorSteamId)
    //     {
    //         if (__instance == null) return;
    //
    //         __instance.OwnerSteamId = creatorSteamId;
    //     }
    // }
    
    [HarmonyPatch(typeof(Structure), "AttackWith")]
    class AttackWithPatch
    {
        static bool Prefix(Structure __instance, Attack attack, bool doAction, ref Thing.DelayedActionInstance __result)
        {
            
            if (!GameManager.RunSimulation)
                return true;
            
            if (!WorldAuthSystem.CanInteract(__instance.OwnerClientId, GetPlayerId()))
            {
                __result = new Thing.DelayedActionInstance
                {
                    Duration = 0,
                    IsDisabled = true,
                    ActionMessage = "Interact"
                };
                __result.AppendStateMessage("You do not own this object");

                return false;
            }
            
            return true;
        }

        static ulong GetPlayerId() => 1;
    }
    
    [HarmonyPatch(typeof(WorldManager), "AfterLoadDataFiles")]
    class AfterLoadDataFilesPatch
    {
        static void Postfix()
        {

            string worldId = WorldManager.CurrentWorldName;
            WorldAuthSystem.Init(worldId);
            
        }
    }
    
}

