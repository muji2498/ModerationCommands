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
        NetworkManagerNuclearOption.i.KickPlayerAsync(damagerPlayer);
        
        var unixTimestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        var steamId = GetSteamId(damagerPlayer);
        var message = isPlayerDamage
            ? $"<t:{unixTimestamp}:F> Player: `{damagerPlayer.PlayerName}({steamId})` has been kicked for killing `{victimUnit.unitName}`."
            : $"<t:{unixTimestamp}:F> Player: `{damagerPlayer.PlayerName}({steamId})` has been kicked for damaging `{victimUnit.unitName}`.";
        DiscordWebhookHandler.SendToWebhook(message);
        ModerationPlugin.Logger.LogInfo($"Player {damagerPlayer.PlayerName} was kicked for hitting the friendly fire limit. Incident count: {IncidentManager.GetIncidentCount(damagerPlayer)}");
        
        _kickedPlayers.Add(damagerPlayer);
    }
    
    
    private static void LogIncident(Player damagerPlayer, int incidentCount, bool isPlayerDamage)
    {
        var incidentType = isPlayerDamage ? "player" : "unit";
        ModerationPlugin.Logger.LogInfo($"Player {damagerPlayer.PlayerName} was logged for a friendly fire ({incidentType}) incident. Incident count: {incidentCount}.");
    }
}