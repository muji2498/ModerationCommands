using System;
using BepInEx.Logging;
using CommandMod.CommandHandler;
using HarmonyLib;
using Mirage;
using Mirage.RemoteCalls;
using Moderation;
using Moderation.Utils;

namespace CommandMod.Patches;

public class ChatManagerPatch
{
    [HarmonyPatch(typeof(ChatManager), nameof(ChatManager.TargetReceiveMessage))]
    public class TargetReceiveMessage
    {
        private static bool SafeShouldInvokeLocally(NetworkBehaviour behaviour, RpcTarget target, INetworkPlayer player, bool excludeOwner) {
            try {
                return ClientRpcSender.ShouldInvokeLocally(behaviour, target, player, excludeOwner);
            } catch (Exception) {
                return false;
            }
        }

        static bool Prefix(ChatManager __instance, INetworkPlayer _, string message, Player player, bool allChat)
        {
            if (SafeShouldInvokeLocally(__instance, RpcTarget.Player, _, false))
            {
                try
                {
                    var unixTimestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                    var steamId = PlayerUtils.GetSteamId(player);
                    var discordMessage = $"[<t:{unixTimestamp}:F>] Player: `{player.PlayerName}({steamId})` sent: {message}";
                    ModerationPlugin.ModerationLogs.SendToWebhook(discordMessage);
                    return true;
                }
                catch
                {
                    return true;
                }
            }

            return false;
        }
    }
}