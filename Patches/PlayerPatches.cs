using System;
using HarmonyLib;
using Moderation.Utils;
using NuclearOption.Networking;

namespace Moderation.Patches;

public class PlayerPatches
{
    [HarmonyPatch(typeof(Player), "NameChanged")]
    [HarmonyWrapSafe]
    public class Player_NameChanged
    {
        static void Postfix(Player __instance)
        {
            if (!ModerationPlugin.Config.PlayerJoinLeaveMessages.Value) return;
            
            var unixTimestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            var steamId = PlayerUtils.GetSteamId(__instance);
            var discordMessage = $"[<t:{unixTimestamp}:F>] Player: `{__instance.PlayerName}({steamId})` joined the server.";
            ModerationPlugin.ModerationLogs.SendToWebhook(discordMessage);
        }
    }
    
    [HarmonyPatch(typeof(Player), "OnStopClient")]
    [HarmonyWrapSafe]
    public class Player_OnStopClient
    {
        static void Postfix(Player __instance)
        {
            if (!ModerationPlugin.Config.PlayerJoinLeaveMessages.Value) return;
            
            var unixTimestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            var discordMessage = $"[<t:{unixTimestamp}:F>] Player: `{__instance.PlayerName}` left the server.";
            ModerationPlugin.ModerationLogs.SendToWebhook(discordMessage);
        }
    }
}