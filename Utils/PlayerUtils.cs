using Banlist.Data;
using Mirage.SteamworksSocket;
using NuclearOption.Networking;

namespace Moderation.Utils;

public class PlayerUtils
{
    public static ulong GetSteamId(Player callingPlayer)
    {
        return callingPlayer.Owner.Address is SteamEndPoint endpoint ? endpoint.Connection.SteamID.m_SteamID : 0;
    }
    
    public static void ApplyKick(bool isPlayerDamage, Player damagerPlayer)
    {
        if (isPlayerDamage && ModerationPlugin.Config.FriendlyFirePlayerKick.Value)
        {
            var incidentCount = IncidentManager.RecordDamageIncident(damagerPlayer);
            if (incidentCount >= ModerationPlugin.Config.FriendlyFireMaxIncidents.Value)
            {
                NetworkManagerNuclearOption.i.KickPlayerAsync(damagerPlayer);
                ModerationPlugin.Logger.LogInfo($"Player {damagerPlayer.PlayerName} was kicked for hitting the friendly fire limit. Incident count: {incidentCount}");
            }
            else
            {
                ModerationPlugin.Logger.LogInfo($"Player {damagerPlayer.PlayerName} was logged for a friendly fire incident. Incident count: {incidentCount}");
            }
        }
        else if (!isPlayerDamage && ModerationPlugin.Config.FriendlyFireUnitKick.Value) // only care about unit damage
        {
            var incidentCount = IncidentManager.RecordDamageIncident(damagerPlayer);
            if (incidentCount >= ModerationPlugin.Config.FriendlyFireMaxIncidents.Value)
            {
                NetworkManagerNuclearOption.i.KickPlayerAsync(damagerPlayer);
                ModerationPlugin.Logger.LogInfo($"Player {damagerPlayer.PlayerName} was kicked for hitting the friendly fire (unit) limit. Incident count: {incidentCount}");
            }
            else
            {
                ModerationPlugin.Logger.LogInfo($"Player {damagerPlayer.PlayerName} was logged for a friendly fire (player) incident. Incident count: {incidentCount}");
            }
        }
    }
}