using System.Collections.Generic;
using CommandMod;
using HarmonyLib;
using Moderation.Data;
using NuclearOption.Chat;
using NuclearOption.Networking;

namespace Moderation.Handlers;

public class PlayerModerationHandler
{
    public static readonly Dictionary<Player, SelectionData> PendingSelections = new();
    
    public static void AddToSelection(Player callingPlayer, string type, List<Player> players)
    {
        lock (PendingSelections)
        {
            PendingSelections.Add(callingPlayer, new SelectionData
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
    
    public static void UnbanPlayer(ulong steamID, Player callingPlayer)
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

    public static void BanPlayer(ulong steamID, Player callingPlayer)
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
}