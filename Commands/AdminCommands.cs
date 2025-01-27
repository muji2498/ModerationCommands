using CommandMod;
using CommandMod.CommandHandler;
using UnityEngine;

namespace Moderation.Commands;

public class AdminCommands
{
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
}