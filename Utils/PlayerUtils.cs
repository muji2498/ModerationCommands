using System;
using System.Collections.Generic;
using Mirage.SteamworksSocket;
using Moderation.Handlers;
using NuclearOption.Networking;

namespace Moderation.Utils;

public class PlayerUtils
{
    private static readonly List<Player> _kickedPlayers = new();
    
    public static ulong GetSteamId(Player callingPlayer)
    {
        return callingPlayer.Owner.Address is SteamEndPoint endpoint ? endpoint.Connection.SteamID.m_SteamID : 0;
    }
    
    public static void ShouldKick(bool isPlayerKilled, Player damagerPlayer, Unit victimUnit)
    {
        if (_kickedPlayers.Contains(damagerPlayer)) return;
        
        var isKickEnabled = isPlayerKilled 
            ? ModerationPlugin.Config.FriendlyFirePlayerKick.Value 
            : ModerationPlugin.Config.FriendlyFireUnitKick.Value;
        if (!isKickEnabled) return;
        
        var (playerIncidents, unitIncidents) = IncidentManager.RecordKillIncident(isPlayerKilled, damagerPlayer, victimUnit);
        
        var maxPlayerIncidents = ModerationPlugin.Config.PlayerMaxIncidents.Value;
        var maxUnitIncidents = ModerationPlugin.Config.UnitMaxIncidents.Value;
        var unitThreshold = ModerationPlugin.Config.FriendlyUnitThreshold.Value;

        bool shouldKick = false;
        bool kickDueToPlayer = false;

        if (ModerationPlugin.Config.FriendlyFirePlayerKick.Value && playerIncidents >= maxPlayerIncidents)
        {
            shouldKick = true;
            kickDueToPlayer = true;
        }

        // 5 - 2 = 3 == 3
        if (ModerationPlugin.Config.FriendlyFireUnitKick.Value 
            && (unitIncidents - unitThreshold) >= maxUnitIncidents)
        {
            shouldKick = true;
        }
        else if (!isPlayerKilled)
        {
            // send this to discord when under threshold
            if (unitIncidents <= unitThreshold)
                LogUnitIncident(damagerPlayer, victimUnit);
        }
        
        if (shouldKick)
        {
            KickPlayer(damagerPlayer, victimUnit, kickDueToPlayer);
        }
        else
        {
            var incidentCount = isPlayerKilled
                ? playerIncidents
                : unitIncidents;
            LogIncidentToConsole(damagerPlayer, incidentCount, isPlayerKilled);
        }
    }
    
    private static void KickPlayer(Player damagerPlayer, Unit victimUnit, bool isPlayerDamage)
    {
        var incidentCount = IncidentManager.GetIncidentCount(damagerPlayer, isPlayerDamage);
        IncidentManager.ClearIncidentForPlayer(damagerPlayer);
        NetworkManagerNuclearOption.i.KickPlayerAsync(damagerPlayer);
        
        var unixTimestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        var steamId = GetSteamId(damagerPlayer);
        var typeString = isPlayerDamage ? "Player" : "Unit";
        var message = $"[<t:{unixTimestamp}:F>] Player: `{damagerPlayer.PlayerName}({steamId})` has been kicked for killing {typeString} `{victimUnit.unitName}`.";
        ModerationPlugin.FriendlyFireLogs.SendToWebhook(message);
        ModerationPlugin.Logger.LogInfo($"Player {damagerPlayer.PlayerName} was kicked for hitting the friendly fire limit. Incident count: {incidentCount}");
        _kickedPlayers.Add(damagerPlayer);
    }
    
    
    private static void LogIncidentToConsole(Player damagerPlayer, int incidentCount, bool isPlayerDamage)
    {
        var incidentType = isPlayerDamage ? "player" : "unit";
        ModerationPlugin.Logger.LogInfo($"Player {damagerPlayer.PlayerName} was logged for a friendly fire ({incidentType}) incident. Incident count: {incidentCount}.");
    }

    private static void LogUnitIncident(Player player, Unit victim)
    {
        var unixTimestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        var steamId = GetSteamId(player);
        ModerationPlugin.FriendlyFireLogs.SendToWebhook($"[<t:{unixTimestamp}:F>] Player: `{player.PlayerName}({steamId})` - Killed: `{victim.unitName}` | `{IncidentManager.GetIncidentCount(player, false)}/{ModerationPlugin.Config.FriendlyUnitThreshold.Value}`");
    }

    public static void ClearKickedPlayers()
    {
        _kickedPlayers.Clear();
    }
}