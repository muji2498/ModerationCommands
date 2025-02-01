using CommandMod;
using CommandMod.CommandHandler;
using CommandMod.Extensions;
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
            context.Player.SendChatMessage($"Usage: friendlyfire true|false. Current Value: {currentValue}");
            return;
        }
        
        if (bool.TryParse(args[0], out bool friendlyFireToggle))
        {
            ModerationPlugin.Config.Enabled.Value = friendlyFireToggle;
            var what = friendlyFireToggle ? "Enabled" : "Disabled";
            context.Player.SendChatMessage($"Friendly-fire: {what}");
        }
    }

    [ConsoleCommand("friendlyfire.player", Roles.Owner | Roles.Admin)]
    public static void FriendlyFirePlayer(string[] args, CommandObjects context)
    {
        if (args.Length < 1)
        {
            var currentValue = ModerationPlugin.Config.FriendlyFirePlayerKick.Value ? "Enabled" : "Disabled";
            context.Player.SendChatMessage($"Usage: friendlyfire.player true|false. Current Value: {currentValue}");
            return;
        }
        
        if (bool.TryParse(args[0], out bool toggle))
        {
            ModerationPlugin.Config.FriendlyFirePlayerKick.Value = toggle;
            var what = toggle ? "Enabled" : "Disabled";
            context.Player.SendChatMessage($"Player friendly-fire: {what}");
        }
    }

    [ConsoleCommand("friendlyfire.unit", Roles.Owner | Roles.Admin)]
    public static void FriendlyFireUnit(string[] args, CommandObjects context)
    {
        if (args.Length < 1)
        {
            var currentValue = ModerationPlugin.Config.FriendlyFireUnitKick.Value ? "Enabled" : "Disabled";
            context.Player.SendChatMessage($"Usage: friendlyfire.unit true|false. Current Value: {currentValue}");
            return;
        }
        
        if (bool.TryParse(args[0], out bool toggle))
        {
            ModerationPlugin.Config.FriendlyFireUnitKick.Value = toggle;
            var what = toggle ? "Enabled" : "Disabled";
            context.Player.SendChatMessage($"Unit friendly-fire: {what}");
        }
    }

    [ConsoleCommand("friendlyfire.unitincidents", Roles.Owner | Roles.Admin)]
    public static void UnitMaxIncidents(string[] args, CommandObjects context)
    {
        if (args.Length < 1)
        {
            var currentValue = ModerationPlugin.Config.UnitMaxIncidents.Value;
            context.Player.SendChatMessage($"Usage: friendlyfire.unitincidents amount. Current Value: {currentValue}");
            return;
        }
        
        if (int.TryParse(args[0], out int amount))
        {
            ModerationPlugin.Config.UnitMaxIncidents.Value = amount;
            context.Player.SendChatMessage($"Friendly-fire max incidents set to {amount}");
        }
    }
    
    [ConsoleCommand("friendlyfire.playerincidents", Roles.Owner | Roles.Admin)]
    public static void PlayerMaxIncidents(string[] args, CommandObjects context)
    {
        if (args.Length < 1)
        {
            var currentValue = ModerationPlugin.Config.PlayerMaxIncidents.Value;
            context.Player.SendChatMessage($"Usage: friendlyfire.playerincidents amount. Current Value: {currentValue}");
            return;
        }
        
        if (int.TryParse(args[0], out int amount))
        {
            ModerationPlugin.Config.PlayerMaxIncidents.Value = amount;
            context.Player.SendChatMessage($"Friendly-fire max incidents set to {amount}");
        }
    }
    
    [ConsoleCommand("friendlyfire.unitthreshold", Roles.Owner | Roles.Admin)]
    public static void FriendlyFireUnitThreshold(string[] args, CommandObjects context)
    {
        if (args.Length < 1)
        {
            var currentValue = ModerationPlugin.Config.FriendlyUnitThreshold.Value;
            context.Player.SendChatMessage($"Usage: friendlyfire.unitthreshold amount. Current Value: {currentValue}");
            return;
        }
        
        if (int.TryParse(args[0], out int amount))
        {
            ModerationPlugin.Config.FriendlyUnitThreshold.Value = amount;
            context.Player.SendChatMessage($"Friendly-fire unit threshold set to {amount}");
        }
    }

    [ConsoleCommand("friendlyfire.reset", Roles.Owner | Roles.Admin)]
    public static void FriendlyFireMaxReset(string[] args, CommandObjects context)
    {
        IncidentManager.ClearIncidents();
        context.Player.SendChatMessage("Incidents cleared");
    }
}