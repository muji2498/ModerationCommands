using System;
using HarmonyLib;
using Moderation.Utils;

namespace Moderation.Patches;

public class PlayerPatches
{
    [HarmonyPatch(typeof(Player), "NameChanged")]
    public class Player_NameChanged
    {
        static void Postfix(Player __instance)
        {
            var unixTimestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            var steamId = PlayerUtils.GetSteamId(__instance);
            var discordMessage = $"[<t:{unixTimestamp}:F>] Player: `{__instance.PlayerName}({steamId})` joined the server.";
            ModerationPlugin.ModerationLogs.SendToWebhook(discordMessage);
        }
    }
    
    [HarmonyPatch(typeof(Player), "OnStopClient")]
    public class Player_OnStopClient
    {
        static void Postfix(Player __instance)
        {
            var unixTimestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            var steamId = PlayerUtils.GetSteamId(__instance);
            var discordMessage = $"[<t:{unixTimestamp}:F>] Player: `{__instance.PlayerName}({steamId})` left the server.";
            ModerationPlugin.ModerationLogs.SendToWebhook(discordMessage);
        }
    }
}