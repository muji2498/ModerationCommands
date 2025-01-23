using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Logging;
using CommandMod;
using CommandMod.CommandHandler;
using HarmonyLib;
using NuclearOption.Networking;
using UnityEngine;

namespace Moderation;

[BepInPlugin("me.muj.moderation", "Moderation", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    
    private static Dictionary<Player, SelectionData> _pendingSelections = new();

    public class SelectionData
    {
        public List<Player> Players { get; set; }
        public string SelectionType { get; set; }  
    }
    
    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin me.muj.moderation is loaded!");
    }
    
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
        var playersToKick = Utils.IdentifyPlayer(targetPlayer);
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
        
        AddToSelection(callingPlayer,"kick", playersToKick);
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
        
        var playersToBan = Utils.IdentifyPlayer(targetPlayer);
        if (playersToBan == null || playersToBan.Count == 0)
        {
            Wrapper.ChatManager.TargetReceiveMessage(context.Player.Owner, "Player not found.", callingPlayer, false);
            return;
        }

        if (playersToBan.Count == 1)
        {
            var playerToBan = playersToBan.First();
            BanPlayer(playerToBan.SteamID, callingPlayer);
            return;
        }
        
        AddToSelection(callingPlayer, "ban", playersToBan);
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
        UnbanPlayer(steamId, callingPlayer);
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

        lock (_pendingSelections)
        {
            if (!_pendingSelections.TryGetValue(callingPlayer, out var pendingSelection))
            {
                Wrapper.ChatManager.TargetReceiveMessage(callingPlayer.Owner,
                    "No selections. Use [kick|ban|unban] commands first.", callingPlayer, false);
                return;
            }
            
            var argument = args[0];
            if (argument == "cancel")
            {
                _pendingSelections.Remove(callingPlayer);
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
                    BanPlayer(playerToSelect.SteamID, callingPlayer);
                    break;
                case "unban":
                    UnbanPlayer(playerToSelect.SteamID, callingPlayer);
                    break;
            }

            _pendingSelections.Remove(callingPlayer);
        }
    }
    
    [ConsoleCommand("setfps", Roles.Owner)]
    public static void SetFPS(string[] args, CommandObjects context)
    {
        if (args.Length < 1)
        {
            Wrapper.ChatManager.TargetReceiveMessage(context.Player.Owner, "Usage: [setfps amount]", context.Player, false);
            return;
        }

        if (!int.TryParse(args[0], out int amount))
        {
            Wrapper.ChatManager.TargetReceiveMessage(context.Player.Owner, "Invalid amount. please use a numeric value", context.Player, false);
            return;
        }
        
        QualitySettings.vSyncCount = 0; 
        PlayerPrefs.SetInt("Vsync", 0);
        Application.targetFrameRate = amount;
    }

    private static void UnbanPlayer(ulong steamID, Player callingPlayer)
    {
        var playerIdField = AccessTools.Field(typeof(BlockList), "playerId");
        if (playerIdField == null)
        {
            Wrapper.ChatManager.TargetReceiveMessage(callingPlayer.Owner, "Could not access block list.", callingPlayer, false);
            return;
        }
        
        var bannedPlayers = playerIdField.GetValue(null) as List<ulong> ?? new List<ulong>();
        if (!bannedPlayers.Contains(steamID))
        {
            Wrapper.ChatManager.TargetReceiveMessage(callingPlayer.Owner, "Player is not banned.", callingPlayer, false);
            return;
        }
        
        bannedPlayers.Remove(steamID);
        playerIdField.SetValue(null, bannedPlayers);
        
        Wrapper.ChatManager.TargetReceiveMessage(callingPlayer.Owner, $"Player {steamID} unbanned.", callingPlayer, false);
    }

    private static void BanPlayer(ulong steamID, Player callingPlayer)
    {
        var playerIdField = AccessTools.Field(typeof(BlockList), "playerId");
        if (playerIdField == null)
        {
            Wrapper.ChatManager.TargetReceiveMessage(callingPlayer.Owner, "Could not access block list.", callingPlayer, false);
            return;
        }
        
        var bannedPlayers = playerIdField.GetValue(null) as List<ulong> ?? new List<ulong>();
        if (bannedPlayers.Contains(steamID))
        {
            Wrapper.ChatManager.TargetReceiveMessage(callingPlayer.Owner, "Player is already banned.", callingPlayer, false);
            return;
        }
        
        bannedPlayers.Add(steamID);
        playerIdField.SetValue(null, bannedPlayers);
        
        Wrapper.ChatManager.TargetReceiveMessage(callingPlayer.Owner, $"Player {steamID} banned.", callingPlayer, false);
    }

    private static void AddToSelection(Player callingPlayer, string type, List<Player> players)
    {
        lock (_pendingSelections)
        {
            _pendingSelections.Add(callingPlayer, new SelectionData
            {
                Players = players,
                SelectionType = type
            });    
        }
        
        var optionsMessage = $"({CommandMod.Plugin.Config.Prefix.Value}select) Player. ";
        for (int i = 0; i < players.Count; i++)
        {
            var player = players[i];
            optionsMessage += $"{i + 1}: {player.PlayerName},";
        }
        Wrapper.ChatManager.TargetReceiveMessage(callingPlayer.Owner, optionsMessage.TrimEnd(' ', ','), callingPlayer, false);
    }
}