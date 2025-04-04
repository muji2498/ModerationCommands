using HarmonyLib;
using Moderation.Handlers;
using Moderation.Utils;

namespace Moderation.Patches;

public class FactionHQPatches
{
    [HarmonyPatch(typeof(FactionHQ), nameof(FactionHQ.RpcDeclareEndGame))]
    [HarmonyWrapSafe]
    public class RpcDeclareEndGame
    {
        static void Postfix()
        {
            IncidentManager.ClearIncidents();
            PlayerUtils.ClearKickedPlayers();
        }
    }
}