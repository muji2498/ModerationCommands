using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Moderation.Handlers;

namespace Moderation;

[BepInPlugin("me.muj.moderation", "Moderation", "3.0.1")]
[BepInDependency("me.muj.commandmod", "2.0.1")]
public partial class ModerationPlugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    
    public new static Config Config { get; private set; }

    public static DiscordWebhookHandler FriendlyFireLogs;
    public static DiscordWebhookHandler ModerationLogs;
    public static DiscordWebhookHandler TicketLogs;

    private void Awake()
    {
        Config = new Config(base.Config);

        FriendlyFireLogs = new DiscordWebhookHandler(Config.FriendlyFireWebhook.Value);
        ModerationLogs = new DiscordWebhookHandler(Config.ModerationWebhook.Value);
        TicketLogs = new DiscordWebhookHandler(Config.TicketWebhook.Value);
        
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin me.muj.moderation is loaded!");
        
        var harmony = new Harmony("me.muj.moderation");
        harmony.PatchAll();
    }
}