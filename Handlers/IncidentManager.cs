using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Moderation.Data;
using Moderation.Utils;

namespace Moderation.Handlers;

public class IncidentManager
{
    private static readonly Dictionary<Player, IncidentData> _incidents = new();
    private static readonly object _lock = new();

    private static readonly Timer ReportTimer;
    private const int ReportInterval = 2000;

    static IncidentManager()
    {
        ReportTimer = new Timer(SendBatchReport, null, ReportInterval, ReportInterval);
    }

    public static int RecordDamageIncident(Player damager, Unit unit)
    {
        if (damager == null || unit == null)
        {
            ModerationPlugin.Logger.LogError("RecordDamageIncident: damager or unit is null.");
            return 0;
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
            
            var incidentData = _incidents[damager];
            
            if (!incidentData.UnitIncidents.ContainsKey(unit))
            {
                incidentData.UnitIncidents[unit] = new UnitIncident();
            }

            var unitIncident = incidentData.UnitIncidents[unit];
            unitIncident.HasBeenReported = false;
            
            // if kick on kill
            if (ModerationPlugin.Config.KickOnKill.Value)
            {
                unitIncident.KillCount++;
                // kill events don't get spammed so just send it
                SendIncidentReport(damager, unit, unitIncident);
            }
            // if kick on damage
            else
            {
                unitIncident.DamageCount++;
            }
            return incidentData.UnitIncidents.Sum(kvp => kvp.Value.KillCount + kvp.Value.DamageCount);
        }
    }
    
    private static void SendBatchReport(object state)
    {
        lock (_lock)
        {
            foreach (var playerIncident in _incidents.ToList()) // go through the player incidents
            {
                foreach (var unitIncident in playerIncident.Value.UnitIncidents) // go through their unit incidents
                {
                    var incident = unitIncident.Value;
                    if (!incident.HasBeenReported && incident.DamageCount > 0)
                    {
                        var unixTimestamp = new DateTimeOffset(playerIncident.Value.LastReported).ToUnixTimeSeconds();
                        var message = $"Friendly Fire Incident: <t:{unixTimestamp}:F> Player: `{playerIncident.Key.PlayerName}({PlayerUtils.GetSteamId(playerIncident.Key)})` - ";
                        message += $"Damaged: `{unitIncident.Key.unitName}` `{incident.DamageCount}` time(s).";
                        DiscordWebhookHandler.SendToWebhook(message);
                        
                        incident.HasBeenReported = true;
                    }
                }
            }
        }
    }

    private static void SendIncidentReport(Player player, Unit unit, UnitIncident incident)
    {
        var unixTimestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
        var message = $"Friendly Fire Incident: <t:{unixTimestamp}:F> Player: `{player.PlayerName}({PlayerUtils.GetSteamId(player)})` - ";
        message += $"Killed: `{unit.unitName}`.";
        
        DiscordWebhookHandler.SendToWebhook(message);
    }

    public static int GetIncidentCount(Player damager)
    {
        lock (_lock)
        {
            return _incidents[damager].UnitIncidents.Sum(kvp => kvp.Value.KillCount + kvp.Value.DamageCount);
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