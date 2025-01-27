using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Moderation.Data;
using Moderation.Utils;
using UnityEngine;

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
        lock (_lock)
        {
            if (!_incidents.ContainsKey(damager))
            {
                _incidents[damager] = new IncidentData
                {
                    Player = damager,
                    DamagedUnit = unit,
                    KillCount = 0,
                    DamageCount = 0,
                    LastReported = DateTime.UtcNow
                };
            }
            
            var incident = _incidents[damager];

            // if kick on kill
            if (ModerationPlugin.Config.KickOnKill.Value)
            {
                incident.KillCount++;
                // kill events don't get spammed so just send it
                SendIncidentReport(incident);
            }
            // if kick on damage
            else
            {
                incident.DamageCount++;
            }
            return _incidents[damager].KillCount + incident.DamageCount;
        }
    }
    
    private static void SendBatchReport(object state)
    {
        var friendlyFireOnUnits = _incidents.Where(kvp => kvp.Value.DamageCount > 0);
        foreach (var pair in friendlyFireOnUnits)
        {
            var unixTimestamp = new DateTimeOffset(pair.Value.LastReported).ToUnixTimeSeconds();
            var message = $"Friendly Fire Incident: <t:{unixTimestamp}:F> Player: `{pair.Key.PlayerName}({PlayerUtils.GetSteamId(pair.Key)})` - ";
            message += $"Damaged: `{pair.Value.DamagedUnit.unitName}` `{pair.Value.DamageCount}` time(s).";
            DiscordWebhookHandler.SendToWebhook(message);
            _incidents.Remove(pair.Key);
        }
    }

    private static void SendIncidentReport(IncidentData incident)
    {
        var unixTimestamp = new DateTimeOffset(incident.LastReported).ToUnixTimeSeconds();
        var message = $"Friendly Fire Incident: <t:{unixTimestamp}:F> Player: `{incident.Player.PlayerName}({PlayerUtils.GetSteamId(incident.Player)})` - ";
        message += $"Killed: `{incident.DamagedUnit.unitName}`.";
        
        DiscordWebhookHandler.SendToWebhook(message);
    }

    public static int GetIncidentCount(Player damager)
    {
        return _incidents[damager].KillCount + _incidents[damager].DamageCount;
    }

    public static void ClearIncidents()
    {
        _incidents.Clear();
    }
}