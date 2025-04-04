using System;
using HarmonyLib;
using Mirage;
using Mirage.RemoteCalls;
using Moderation;
using Moderation.Utils;

namespace CommandMod.Patches;

public class ChatManagerPatch
{
    [HarmonyPatch(typeof(ChatManager), nameof(ChatManager.TargetReceiveMessage))]
    [HarmonyWrapSafe]
    public class TargetReceiveMessage
    {
        private static bool SafeShouldInvokeLocally(NetworkBehaviour behaviour, RpcTarget target, INetworkPlayer player,
            bool excludeOwner)
        {
            try
            {
                return ClientRpcSender.ShouldInvokeLocally(behaviour, target, player, excludeOwner);
            }
            catch (Exception)
            {
                return false;
            }
        }

        static void Postfix(ChatManager __instance, INetworkPlayer _, string message, Player player, bool allChat)
        {
            if (SafeShouldInvokeLocally(__instance, RpcTarget.Player, _, false))
            {
                var unixTimestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                var steamId = PlayerUtils.GetSteamId(player);
                var discordMessage =
                    $"[<t:{unixTimestamp}:F>] Player: `{player.PlayerName}({steamId})` sent: {message}";
                ModerationPlugin.ModerationLogs.SendToWebhook(discordMessage);
            }
        }
    }
}