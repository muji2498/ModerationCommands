using System.Collections.Generic;

namespace Banlist.Data;

public class IncidentManager
{
    private static Dictionary<Player, int> _incidents = new Dictionary<Player, int>();

    public static int RecordDamageIncident(Player damager)
    {
        if (!_incidents.ContainsKey(damager))
        {
            _incidents.Add(damager, 0);
        }
        
        _incidents[damager]++;
        return _incidents[damager];
    }

    public static int GetIncidentCount(Player damager)
    {
        return _incidents[damager];
    }

    public static void ClearIncidents()
    {
        _incidents.Clear();
    }
}