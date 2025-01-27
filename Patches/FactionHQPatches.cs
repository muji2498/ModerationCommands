using System;
using Banlist.Data;
using HarmonyLib;

namespace Banlist.Patches;

public class FactionHQPatches
{
    [HarmonyPatch(typeof(FactionHQ), nameof(FactionHQ.RpcDeclareEndGame))]
    public class RpcDeclareEndGame
    {
        static void Postfix()
        {
            IncidentManager.ClearIncidents();
        }
    }
}