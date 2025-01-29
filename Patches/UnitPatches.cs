using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Moderation.Utils;

namespace Moderation.Patches;

public class UnitPatches
{
    [HarmonyPatch(typeof(Unit), nameof(Unit.ReportKilled))]
    public class ReportKilled
    {
        static bool Prefix(Unit __instance)
        {
            
            if (!ModerationPlugin.Config.Enabled.Value) return true;
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
            var isPlayerDamage = killedUnit.player != null;

            if (isPlayerDamage) 
            {
                PlayerUtils.ApplyKick(isPlayerDamage, damagerPlayer, __instance);
                return true;
            }
            
            PlayerUtils.AddUnitKilled(damagerPlayer, __instance);

            // if its unit
            var unitsKilled = PlayerUtils.GetUnitsKilled(damagerPlayer);
            if (ModerationPlugin.Config.FriendlyUnitThreshold.Value == unitsKilled)
            {
                PlayerUtils.ApplyKick(isPlayerDamage, damagerPlayer, __instance);
                return true;
            }
            return true;
        }
    }
}