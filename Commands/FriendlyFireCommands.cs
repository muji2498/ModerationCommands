using Banlist.Data;
using CommandMod;
using CommandMod.CommandHandler;

namespace Moderation.Commands;

public class FriendlyFireCommands
{
    [ConsoleCommand("friendlyfire", Roles.Owner | Roles.Admin)]
    public static void FriendlyFire(string[] args, CommandObjects context)
    {
        if (bool.TryParse(args[0], out bool friendlyFireToggle))
        {
            ModerationPlugin.Config.Enabled.Value = false;
            var what = friendlyFireToggle ? "Enabled" : "Disabled";
            Wrapper.ChatManager.TargetReceiveMessage(context.Player.Owner, $"friendly-fire: {what}", context.Player, false);
        }
        else
        {
            Wrapper.ChatManager.TargetReceiveMessage(context.Player.Owner, "Usage: friendlyfire true|false", context.Player, false);
        }
    }
    
    [ConsoleCommand("friendlyfire.kickonkill", Roles.Owner | Roles.Admin)]
    public static void FriendlyFireOnKill(string[] args, CommandObjects context)
    {
        if (bool.TryParse(args[0], out bool friendlyFireToggle))
        {
            ModerationPlugin.Config.KickOnKill.Value = friendlyFireToggle;
            var what = friendlyFireToggle ? "Enabled" : "Disabled";
            Wrapper.ChatManager.TargetReceiveMessage(context.Player.Owner, $"friendly fire kick on kill: {what}", context.Player, false);
        }
        else
        {
            Wrapper.ChatManager.TargetReceiveMessage(context.Player.Owner, "Usage: friendlyfire.kickonkill true|false. NOTE: When true player will be kicked when they kill, When false they will be kicked when they damage.", context.Player, false);
        }
    }
    
    [ConsoleCommand("friendlyfire.player", Roles.Owner | Roles.Admin)]
    public static void FriendlyFirePlayer(string[] args, CommandObjects context)
    {
        if (bool.TryParse(args[0], out bool toggle))
        {
            ModerationPlugin.Config.FriendlyFirePlayerKick.Value = toggle;
            var what = toggle ? "Enabled" : "Disabled";
            Wrapper.ChatManager.TargetReceiveMessage(context.Player.Owner, $"Player friendly-fire: {what}", context.Player, false);
        }
        else
        {
            Wrapper.ChatManager.TargetReceiveMessage(context.Player.Owner, "Usage: friendlyfire.player true|false", context.Player, false);
        }
    }
    
    [ConsoleCommand("friendlyfire.unit", Roles.Owner | Roles.Admin)]
    public static void FriendlyFireUnit(string[] args, CommandObjects context)
    {
        if (bool.TryParse(args[0], out bool toggle))
        {
            ModerationPlugin.Config.FriendlyFireUnitKick.Value = toggle;
            var what = toggle ? "Enabled" : "Disabled";
            Wrapper.ChatManager.TargetReceiveMessage(context.Player.Owner, $"Unit friendly fire: {what}", context.Player, false);
        }
        else
        {
            Wrapper.ChatManager.TargetReceiveMessage(context.Player.Owner, "Usage: friendlyfire.unit true|false", context.Player, false);
        }
    }
    
    [ConsoleCommand("friendlyfire.maxincidents", Roles.Owner | Roles.Admin)]
    public static void FriendlyFireMaxIncidents(string[] args, CommandObjects context)
    {
        if (int.TryParse(args[0], out int amount))
        {
            ModerationPlugin.Config.FriendlyFireMaxIncidents.Value = amount;
            Wrapper.ChatManager.TargetReceiveMessage(context.Player.Owner, $"friendly-fire max incidents set to {amount}", context.Player, false);
        }
        else
        {
            Wrapper.ChatManager.TargetReceiveMessage(context.Player.Owner, "Usage: friendlyfire.maxincidents amount", context.Player, false);
        }
    }
    
    [ConsoleCommand("friendlyfire.reset", Roles.Owner | Roles.Admin)]
    public static void FriendlyFireMaxReset(string[] args, CommandObjects context)
    {
        IncidentManager.ClearIncidents();
        Wrapper.ChatManager.TargetReceiveMessage(context.Player.Owner, "Incidents cleared", context.Player, false);
    }
}