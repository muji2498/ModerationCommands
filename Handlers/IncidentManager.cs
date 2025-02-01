using System;
using System.Collections.Generic;
using Moderation.Data;
using Moderation.Utils;

namespace Moderation.Handlers;

public class IncidentManager
{
    private static readonly Dictionary<Player, IncidentData> _incidents = new();
    private static readonly object _lock = new();

    public static (int playerIncidents, int unitIncidents) RecordKillIncident(bool isPlayerDamage, Player damager, Unit unit)
    {
        if (damager == null || unit == null)
        {
            ModerationPlugin.Logger.LogError("RecordKillIncident: damager or unit is null.");
            return (0, 0);
        }
        
        lock (_lock)
        {
            if (!_incidents.ContainsKey(damager))
            {
                _incidents[damager] = new IncidentData
                {
                    Player = damager,
                    LastReported = DateTime.UtcNow
                };
            }
            
            // update when last reported
            var incidentData = _incidents[damager];
            incidentData.LastReported = DateTime.UtcNow;

            if (isPlayerDamage)
            {
                incidentData.PlayerIncidents++;
                SendIncidentReport(damager, unit, incidentData.PlayerIncidents, true);
            }
            else
            {
                incidentData.UnitIncidents++;
                if (incidentData.UnitIncidents > ModerationPlugin.Config.FriendlyUnitThreshold.Value) // dont send incident reports when below threshold 
                    SendIncidentReport(damager, unit, incidentData.UnitIncidents, isPlayerDamage);
            }
            
            return (incidentData.PlayerIncidents, incidentData.UnitIncidents);
        }
    }

    private static void SendIncidentReport(Player player, Unit unit, int incidents, bool playerDamage)
    {
        var damageString = playerDamage ? "PlayerDamage" : "UnitDamage";
        
        var unixTimestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
        var message = $"[<t:{unixTimestamp}:F>] Player: `{player.PlayerName}({PlayerUtils.GetSteamId(player)})` - ";
        message += $"Killed: `{unit.unitName}` | Incidents Count ({damageString}): `{incidents}`";
        
        ModerationPlugin.FriendlyFireLogs.SendToWebhook(message);
    }

    public static int GetIncidentCount(Player damager, bool playerIncidents)
    {
        lock (_lock)
        {
            if (!_incidents.TryGetValue(damager, out var data)) return 0;
            return playerIncidents ? data.PlayerIncidents : data.UnitIncidents;
        }
    }

    public static void ClearIncidentForPlayer(Player damager)
    {
        _incidents.Remove(damager);
    }

    public static void ClearIncidents()
    {
        _incidents.Clear();
    }
}