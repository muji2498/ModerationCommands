using Banlist.Data;
using HarmonyLib;
using Moderation;
using NuclearOption.Networking;

namespace Banlist.Patches;

public class UnitPartPatches
{
    [HarmonyPatch(typeof(UnitPart), nameof(UnitPart.TakeDamage))]
    public class TakeDamage
    {
        static bool Prefix(UnitPart __instance, int dealerID)
        {
            if (ModerationPlugin.Config.KickOnKill.Value) return true;
            if (__instance == null) return true;
            
            var victim = UnitRegistry.GetPersistentUnit(__instance.parentUnit.persistentID);
            var damager = UnitRegistry.GetPersistentUnit(dealerID);
            if (victim == null || damager == null) return true;
            
            var victimFaction = victim.GetFaction();
            var damagerFaction = damager.GetFaction();
            // not a friendly
            if (victimFaction != damagerFaction) return true;

            // check if unit is player or not
            var isPlayerDamage = victim.player != null;
            if (isPlayerDamage && ModerationPlugin.Config.FriendlyFirePlayerKick.Value) // only care about player damage
            {
                var incidentCount = IncidentManager.RecordDamageIncident(damager.player);
                if (incidentCount >= ModerationPlugin.Config.FriendlyFireMaxIncidents.Value)
                {
                    NetworkManagerNuclearOption.i.KickPlayerAsync(damager.player);
                    ModerationPlugin.Logger.LogInfo($"Player {damager.player.PlayerName} was kicked for hitting the friendly fire (player) limit. Incident count: {incidentCount}");
                }
                else
                {
                    ModerationPlugin.Logger.LogInfo($"Player {damager.player.PlayerName} was logged for a friendly fire (player) incident. Incident count: {incidentCount}");
                }
            }
            else if (!isPlayerDamage && ModerationPlugin.Config.FriendlyFireUnitKick.Value) // only care about unit damage
            {
                var incidentCount = IncidentManager.RecordDamageIncident(damager.player);
                if (incidentCount >= ModerationPlugin.Config.FriendlyFireMaxIncidents.Value)
                {
                    NetworkManagerNuclearOption.i.KickPlayerAsync(damager.player);
                    ModerationPlugin.Logger.LogInfo($"Player {damager.player.PlayerName} was kicked for hitting the friendly fire (unit) limit. Incident count: {incidentCount}");
                }
                else
                {
                    ModerationPlugin.Logger.LogInfo($"Player {damager.player.PlayerName} was logged for a friendly fire (player) incident. Incident count: {incidentCount}");
                }
            }
            return true;
        }
    }
}