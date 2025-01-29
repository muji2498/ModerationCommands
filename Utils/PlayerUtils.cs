using System;
using System.Collections.Generic;
using Mirage.SteamworksSocket;
using Moderation.Handlers;
using NuclearOption.Networking;

namespace Moderation.Utils;

public class PlayerUtils
{
    private static readonly List<Player> _kickedPlayers = new();
    
    private static readonly Dictionary<Player, int> _unitsKilled = new();
    
    public static ulong GetSteamId(Player callingPlayer)
    {
        return callingPlayer.Owner.Address is SteamEndPoint endpoint ? endpoint.Connection.SteamID.m_SteamID : 0;
    }

    public static void AddUnitKilled(Player player, Unit victim)
    {

        if (!_unitsKilled.ContainsKey(player))
        {
            _unitsKilled[player] = 1;
            LogUnitIncident(player, victim);
            return;
        }
        _unitsKilled[player]++;
        LogUnitIncident(player, victim);
    }

    public static int GetUnitsKilled(Player player)
    {
        if (!_unitsKilled.ContainsKey(player))
            return 0;
        
        return _unitsKilled[player];
    }

    public static void ClearUnitKilled()
    {
        _unitsKilled.Clear();
    }
    
    public static void ApplyKick(bool isPlayerDamage, Player damagerPlayer, Unit victimUnit)
    {
        if (_kickedPlayers.Contains(damagerPlayer)) return;
        
        var isKickEnabled = isPlayerDamage ? ModerationPlugin.Config.FriendlyFirePlayerKick.Value : ModerationPlugin.Config.FriendlyFireUnitKick.Value;
        if (!isKickEnabled) return;
        
        var incidentCount = IncidentManager.RecordDamageIncident(damagerPlayer, victimUnit);
        var maxIncidents = ModerationPlugin.Config.FriendlyFireMaxIncidents.Value;

        if (incidentCount >= maxIncidents)
        {
            KickPlayer(damagerPlayer, victimUnit, isPlayerDamage);
        }
        else
        {
            LogIncident(damagerPlayer, incidentCount, isPlayerDamage);
        }
    }
    
    private static void KickPlayer(Player damagerPlayer, Unit victimUnit, bool isPlayerDamage)
    {
        var incidentCount = IncidentManager.GetIncidentCount(damagerPlayer);
        IncidentManager.ClearIncidentForPlayer(damagerPlayer);
        NetworkManagerNuclearOption.i.KickPlayerAsync(damagerPlayer);
        
        var unixTimestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        var steamId = GetSteamId(damagerPlayer);
        var message = isPlayerDamage
            ? $"<t:{unixTimestamp}:F> Player: `{damagerPlayer.PlayerName}({steamId})` has been kicked for killing player `{victimUnit.unitName}`."
            : $"<t:{unixTimestamp}:F> Player: `{damagerPlayer.PlayerName}({steamId})` has been kicked for killing unit `{victimUnit.unitName}`.";
        DiscordWebhookHandler.SendToWebhook(message);
        ModerationPlugin.Logger.LogInfo($"Player {damagerPlayer.PlayerName} was kicked for hitting the friendly fire limit. Incident count: {incidentCount}");
        _kickedPlayers.Add(damagerPlayer);
    }
    
    
    private static void LogIncident(Player damagerPlayer, int incidentCount, bool isPlayerDamage)
    {
        var incidentType = isPlayerDamage ? "player" : "unit";
        var shouldLog = incidentCount % 50 == 0;
        if (shouldLog) ModerationPlugin.Logger.LogInfo($"Player {damagerPlayer.PlayerName} was logged for a friendly fire ({incidentType}) incident. Incident count: {incidentCount}.");
    }

    private static void LogUnitIncident(Player player, Unit victim)
    {
        var unixTimestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        var steamId = GetSteamId(player);
        DiscordWebhookHandler.SendToWebhook($"<t:{unixTimestamp}:F> `{player.PlayerName}({steamId})` killed a friendly unit: `{victim.unitName}`. {_unitsKilled[player]}/{ModerationPlugin.Config.FriendlyUnitThreshold.Value}.");
    }

    public static void ClearKickedPlayers()
    {
        _kickedPlayers.Clear();
    }
}