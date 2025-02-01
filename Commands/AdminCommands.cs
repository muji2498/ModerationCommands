using CommandMod;
using CommandMod.CommandHandler;
using CommandMod.Extensions;
using UnityEngine;

namespace Moderation.Commands;

public class AdminCommands
{
    [ConsoleCommand("setfps", Roles.Owner)]
    public static void SetFPS(string[] args, CommandObjects context)
    {
        if (args.Length < 1)
        {
            context.Player.SendChatMessage("Usage: [setfps amount]");
            return;
        }

        if (!int.TryParse(args[0], out int amount))
        {
            context.Player.SendChatMessage("Invalid amount. please use a numeric value");
            return;
        }
        
        QualitySettings.vSyncCount = 0; 
        PlayerPrefs.SetInt("Vsync", 0);
        Application.targetFrameRate = amount;
    }
}