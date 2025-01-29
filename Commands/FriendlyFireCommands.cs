using CommandMod;
using CommandMod.CommandHandler;
using Moderation.Handlers;
using Moderation.Utils;

namespace Moderation.Commands;

public class FriendlyFireCommands
{
    [ConsoleCommand("friendlyfire", Roles.Owner | Roles.Admin)]
    public static void FriendlyFire(string[] args, CommandObjects context)
    {
        if (args.Length < 1)
        {
            var currentValue = ModerationPlugin.Config.Enabled.Value ? "Enabled" : "Disabled";
            Wrapper.ChatManager.TargetReceiveMessage(context.Player.Owner, $"Usage: friendlyfire true|false. Current Value: {currentValue}", context.Player, false);
            return;
        }
        
        if (bool.TryParse(args[0], out bool friendlyFireToggle))
        {
            ModerationPlugin.Config.Enabled.Value = friendlyFireToggle;
            var what = friendlyFireToggle ? "Enabled" : "Disabled";
            Wrapper.ChatManager.TargetReceiveMessage(context.Player.Owner, $"Friendly-fire: {what}", context.Player, false);
        }
    }

    [ConsoleCommand("friendlyfire.player", Roles.Owner | Roles.Admin)]
    public static void FriendlyFirePlayer(string[] args, CommandObjects context)
    {
        if (args.Length < 1)
        {
            var currentValue = ModerationPlugin.Config.FriendlyFirePlayerKick.Value ? "Enabled" : "Disabled";
            Wrapper.ChatManager.TargetReceiveMessage(context.Player.Owner, $"Usage: friendlyfire.player true|false. Current Value: {currentValue}", context.Player, false);
            return;
        }
        
        if (bool.TryParse(args[0], out bool toggle))
        {
            ModerationPlugin.Config.FriendlyFirePlayerKick.Value = toggle;
            var what = toggle ? "Enabled" : "Disabled";
            Wrapper.ChatManager.TargetReceiveMessage(context.Player.Owner, $"Player friendly-fire: {what}", context.Player, false);
        }
    }

    [ConsoleCommand("friendlyfire.unit", Roles.Owner | Roles.Admin)]
    public static void FriendlyFireUnit(string[] args, CommandObjects context)
    {
        if (args.Length < 1)
        {
            var currentValue = ModerationPlugin.Config.FriendlyFireUnitKick.Value ? "Enabled" : "Disabled";
            Wrapper.ChatManager.TargetReceiveMessage(context.Player.Owner, $"Usage: friendlyfire.unit true|false. Current Value: {currentValue}", context.Player, false);
            return;
        }
        
        if (bool.TryParse(args[0], out bool toggle))
        {
            ModerationPlugin.Config.FriendlyFireUnitKick.Value = toggle;
            var what = toggle ? "Enabled" : "Disabled";
            Wrapper.ChatManager.TargetReceiveMessage(context.Player.Owner, $"Unit friendly-fire: {what}", context.Player, false);
        }
    }

    [ConsoleCommand("friendlyfire.unitincidents", Roles.Owner | Roles.Admin)]
    public static void UnitMaxIncidents(string[] args, CommandObjects context)
    {
        if (args.Length < 1)
        {
            var currentValue = ModerationPlugin.Config.UnitMaxIncidents.Value;
            Wrapper.ChatManager.TargetReceiveMessage(context.Player.Owner, $"Usage: friendlyfire.unitincidents amount. Current Value: {currentValue}", context.Player, false);
            return;
        }
        
        if (int.TryParse(args[0], out int amount))
        {
            ModerationPlugin.Config.UnitMaxIncidents.Value = amount;
            Wrapper.ChatManager.TargetReceiveMessage(context.Player.Owner, $"Friendly-fire max incidents set to {amount}", context.Player, false);
        }
    }
    
    [ConsoleCommand("friendlyfire.playerincidents", Roles.Owner | Roles.Admin)]
    public static void PlayerMaxIncidents(string[] args, CommandObjects context)
    {
        if (args.Length < 1)
        {
            var currentValue = ModerationPlugin.Config.PlayerMaxIncidents.Value;
            Wrapper.ChatManager.TargetReceiveMessage(context.Player.Owner, $"Usage: friendlyfire.playerincidents amount. Current Value: {currentValue}", context.Player, false);
            return;
        }
        
        if (int.TryParse(args[0], out int amount))
        {
            ModerationPlugin.Config.PlayerMaxIncidents.Value = amount;
            Wrapper.ChatManager.TargetReceiveMessage(context.Player.Owner, $"Friendly-fire max incidents set to {amount}", context.Player, false);
        }
    }
    
    [ConsoleCommand("friendlyfire.unitthreshold", Roles.Owner | Roles.Admin)]
    public static void FriendlyFireUnitThreshold(string[] args, CommandObjects context)
    {
        if (args.Length < 1)
        {
            var currentValue = ModerationPlugin.Config.FriendlyUnitThreshold.Value;
            Wrapper.ChatManager.TargetReceiveMessage(context.Player.Owner, $"Usage: friendlyfire.unitthreshold amount. Current Value: {currentValue}", context.Player, false);
            return;
        }
        
        if (int.TryParse(args[0], out int amount))
        {
            ModerationPlugin.Config.FriendlyUnitThreshold.Value = amount;
            Wrapper.ChatManager.TargetReceiveMessage(context.Player.Owner, $"Friendly-fire unit threshold set to {amount}", context.Player, false);
        }
    }

    [ConsoleCommand("friendlyfire.reset", Roles.Owner | Roles.Admin)]
    public static void FriendlyFireMaxReset(string[] args, CommandObjects context)
    {
        IncidentManager.ClearIncidents();
        Wrapper.ChatManager.TargetReceiveMessage(context.Player.Owner, "Incidents cleared", context.Player, false);
    }
}