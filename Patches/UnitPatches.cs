using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Moderation.Utils;

namespace Moderation.Patches;

public class UnitPatches
{
    [HarmonyPatch(typeof(Unit), nameof(Unit.ReportKilled))]
    [HarmonyWrapSafe]
    public class ReportKilled
    {
        static bool Prefix(Unit __instance)
        {
            if (!ModerationPlugin.Config.Enabled.Value) return true;
            if (__instance == null) return true;
            
            var damageCredit = (Dictionary<int, float>) AccessTools.Field(typeof(Unit), "damageCredit").GetValue(__instance);
            if (damageCredit == null || damageCredit.Count == 0) return true;
            
            // sort by the highest value
            var highestDamager = damageCredit.OrderByDescending(kvp => kvp.Value).FirstOrDefault();
            if (highestDamager.Value == 0) return true;
            
            // the highest damager and the killed persons unit
            if(!UnitRegistry.TryGetPersistentUnit(__instance.persistentID, out var killedUnit)) return false;
            if (!UnitRegistry.TryGetPersistentUnit(highestDamager.Key, out var highestDamagerUnit)) return false;
            if (highestDamagerUnit == null) return true;
            if (highestDamagerUnit.player == null) return true; // ignore if the unit was killed by another unit
            if (__instance.unitName.ToLower().Contains("container"))
            {
                __instance.TryGetComponent<Container>(out var containerComponent);
                if (containerComponent != null)
                {
                    // skip if the highest killer player owns the container
                    if (containerComponent.ownerID == highestDamagerUnit.id) return true;
                } 
            }
            // skip if afv6 aa
            if (IsAFV6AAKilled(__instance)) return true;
            
            // not the same hq so move on
            if (highestDamagerUnit.GetHQ() != killedUnit.GetHQ()) return true;
            
            var damagerPlayer = highestDamagerUnit.player;
            var isPlayerKilled = (killedUnit.player != null || __instance is Aircraft);
            PlayerUtils.ShouldKick(isPlayerKilled, damagerPlayer, __instance);
            return true;
        }
        
        // weird bug where afv6 ss count as player kills, possible game bug where they get tracked as players or aircrafts?? 
        private static bool IsAFV6AAKilled(Unit __instance)
        {
            if (!ModerationPlugin.Config.AFV6AAPatch.Value) return false;
            if (__instance.unitName.ToLower().Contains("afv6 aa"))
            {
                return true;
            }
            return false;
        }
    }
}