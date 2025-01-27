using System.Collections.Generic;
using System.Linq;
using Banlist.Data;
using HarmonyLib;
using Moderation;
using NuclearOption.Networking;

namespace Banlist.Patches;

public class UnitPatches
{
    [HarmonyPatch(typeof(Unit), nameof(Unit.ReportKilled))]
    public class ReportKilled
    {
        static bool Prefix(Unit __instance)
        {
            if (!ModerationPlugin.Config.KickOnKill.Value) return true;
            if (__instance == null) return true;
            
            var damageCredit = (Dictionary<int, float>) AccessTools.Field(typeof(Unit), "damageCredit").GetValue(__instance);
            if (damageCredit == null || damageCredit.Count == 0) return true;
            
            // sort by the highest value
            var highestDamager = damageCredit.OrderByDescending(kvp => kvp.Value).FirstOrDefault();
            if (highestDamager.Value == 0) return true;
            
            // the highest damager and the killed persons unit
            var killedUnit = UnitRegistry.GetPersistentUnit(__instance.persistentID);
            var highestDamagerUnit = UnitRegistry.GetPersistentUnit(highestDamager.Key);
            if (highestDamagerUnit == null) return true;

            // not the same hq so move on
            if (highestDamagerUnit.HQ != killedUnit.HQ) return true;

            var damagerPlayer = highestDamagerUnit.player;
            if (damagerPlayer == null) return true;
            
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
            
            return true;
        }
    }
}