using HarmonyLib;
using Moderation.Handlers;

namespace Moderation.Patches;

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