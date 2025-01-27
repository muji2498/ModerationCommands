using Banlist.Data;
using HarmonyLib;
using Moderation;
using Moderation.Utils;
using NuclearOption.Networking;

namespace Banlist.Patches;

public class UnitPartPatches
{
    [HarmonyPatch(typeof(UnitPart), nameof(UnitPart.TakeDamage))]
    public class TakeDamage
    {
        static bool Prefix(UnitPart __instance, int dealerID)
        {
            if (!ModerationPlugin.Config.Enabled.Value) return true;
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
            var damagerPlayer = damager.player;
            var isPlayerDamage = victim.player != null;
            PlayerUtils.ApplyKick(isPlayerDamage, damagerPlayer);
            return true;
        }
    }
}