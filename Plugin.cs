using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Moderation.Handlers;

namespace Moderation;

[BepInPlugin("me.muj.moderation", "Moderation", "2.0.4")]
[BepInDependency("me.muj.commandmod")]
public partial class ModerationPlugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    
    public new static Config Config { get; private set; }

    public static DiscordWebhookHandler FriendlyFireLogs;
    public static DiscordWebhookHandler ModerationLogs;

    private void Awake()
    {
        Config = new Config(base.Config);

        FriendlyFireLogs = new DiscordWebhookHandler(Config.FriendlyFireWebhook.Value);
        ModerationLogs = new DiscordWebhookHandler(Config.ModerationWebhook.Value);
        
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin me.muj.moderation is loaded!");
        
        var harmony = new Harmony("me.muj.moderation");
        harmony.PatchAll();
    }
}