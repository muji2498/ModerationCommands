using System;
using HarmonyLib;
using Mirage;
using Moderation.Utils;
using NuclearOption.Networking;

namespace Moderation.Patches;

public class ChatManagerPatch
{
    [HarmonyPatch(typeof(ChatManager), "UserCode_CmdSendChatMessage_1323305531")]
    [HarmonyWrapSafe]
    public class TargetReceiveMessage
    {
        static void Postfix(ChatManager __instance, string message, bool allChat, INetworkPlayer sender)
        {
            Player player;
            if (!sender.TryGetPlayer(out player))
                return;
            
            var unixTimestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            var steamId = PlayerUtils.GetSteamId(player);
            var discordMessage =
                $"[<t:{unixTimestamp}:F>] Player: `{player.PlayerName}({steamId})` sent: {message}";
            ModerationPlugin.ModerationLogs.SendToWebhook(discordMessage);
        }
    }
}