using System.Linq;
using CommandMod;
using CommandMod.CommandHandler;
using Moderation.Handlers;
using NuclearOption.Networking;

namespace Moderation.Commands;

public class PlayerCommands
{
    [ConsoleCommand("kick", Roles.Owner | Roles.Admin | Roles.Moderator)]
    public static void KickPlayer(string[] args, CommandObjects arg2)
    {
        if (args.Length < 1)
        {
            Wrapper.ChatManager.TargetReceiveMessage(arg2.Player.Owner, "Usage: [kick playername|steamid]", arg2.Player, false);
            return;
        }
        var callingPlayer = arg2.Player;
        var targetPlayer = args[0];
        var playersToKick = CommandMod.Utils.IdentifyPlayer(targetPlayer);
        if (playersToKick == null || playersToKick.Count == 0)
        {
            Wrapper.ChatManager.TargetReceiveMessage(callingPlayer.Owner, "Player not found.", callingPlayer, false);
            return;
        }

        if (playersToKick.Count == 1)
        {
            var playerToKick = playersToKick.First();
            NetworkManagerNuclearOption.i.KickPlayerAsync(playerToKick);
            Wrapper.ChatManager.TargetReceiveMessage(callingPlayer.Owner, $"Player {playerToKick.PlayerName} kicked.", callingPlayer, false);
            return;
        }
        
        PlayerModerationHandler.AddToSelection(callingPlayer,"kick", playersToKick);
    }
    
    [ConsoleCommand("ban", Roles.Owner | Roles.Admin | Roles.Moderator)]
    public static void Ban(string[] args, CommandObjects context)
    {
        if (args.Length < 1)
        {
            Wrapper.ChatManager.TargetReceiveMessage(context.Player.Owner, "Usage: [ban playername|steamid]", context.Player, false);
            return;
        }

        var callingPlayer = context.Player;
        var targetPlayer = args[0];
        
        var playersToBan = CommandMod.Utils.IdentifyPlayer(targetPlayer);
        if (playersToBan == null || playersToBan.Count == 0)
        {
            Wrapper.ChatManager.TargetReceiveMessage(context.Player.Owner, "Player not found.", callingPlayer, false);
            return;
        }

        if (playersToBan.Count == 1)
        {
            var playerToBan = playersToBan.First();
            PlayerModerationHandler.BanPlayer(playerToBan.SteamID, callingPlayer);
            return;
        }
        
        PlayerModerationHandler.AddToSelection(callingPlayer, "ban", playersToBan);
    }
    
    [ConsoleCommand("unban",  Roles.Owner | Roles.Admin | Roles.Moderator)]
    public static void Unban(string[] args, CommandObjects context)
    {
        if (args.Length < 1)
        {
            Wrapper.ChatManager.TargetReceiveMessage(context.Player.Owner, "Usage: [unban steamid]", context.Player, false);
            return;
        }

        var callingPlayer = context.Player;
        var targetPlayer = args[0];

        if (!ulong.TryParse(targetPlayer, out var steamId))
        {
            Wrapper.ChatManager.TargetReceiveMessage(context.Player.Owner, "Please use a numeric value.", context.Player, false);
            return;
        }
        PlayerModerationHandler.UnbanPlayer(steamId, callingPlayer);
    }

    [ConsoleCommand("select", Roles.Owner | Roles.Admin | Roles.Moderator)]
    public static void Select(string[] args, CommandObjects context)
    {
        var callingPlayer = context.Player;
        if (args.Length < 1)
        {
            Wrapper.ChatManager.TargetReceiveMessage(callingPlayer.Owner, "Usage: [select number|cancel]", callingPlayer, false);
            return;
        }

        lock (PlayerModerationHandler.PendingSelections)
        {
            if (!PlayerModerationHandler.PendingSelections.TryGetValue(callingPlayer, out var pendingSelection))
            {
                Wrapper.ChatManager.TargetReceiveMessage(callingPlayer.Owner,
                    "No selections. Use [kick|ban] commands first.", callingPlayer, false);
                return;
            }
            
            var argument = args[0];
            if (argument == "cancel")
            {
                PlayerModerationHandler.PendingSelections.Remove(callingPlayer);
                Wrapper.ChatManager.TargetReceiveMessage(callingPlayer.Owner, "Selection cancelled.", callingPlayer,
                    false);
                return;
            }

            if (!int.TryParse(argument, out int number) || number < 0 || number > pendingSelection.Players.Count)
            {
                // number passed in
                Wrapper.ChatManager.TargetReceiveMessage(callingPlayer.Owner, "Invalid selection number.",
                    callingPlayer, false);
                return;
            }

            var playerToSelect = pendingSelection.Players[number - 1];
            switch (pendingSelection.SelectionType.ToLower())
            {
                case "kick":
                    NetworkManagerNuclearOption.i.KickPlayerAsync(playerToSelect);
                    Wrapper.ChatManager.TargetReceiveMessage(callingPlayer.Owner, $"Player {playerToSelect.PlayerName} kicked.",
                        callingPlayer, false);
                    break;
                case "ban":
                    PlayerModerationHandler.BanPlayer(playerToSelect.SteamID, callingPlayer);
                    break;
            }

            PlayerModerationHandler.PendingSelections.Remove(callingPlayer);
        }
    }
}