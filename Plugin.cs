using BepInEx;
using BepInEx.Logging;
using CommandMod;
using HarmonyLib;

namespace Moderation;

[BepInPlugin("me.muj.moderation", "Moderation", "2.0.0")]
[BepInDependency("me.muj.commandmod")]
public partial class ModerationPlugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    
    public new static Config Config { get; private set; }

    private void Awake()
    {
        Config = new Config(base.Config);
        
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin me.muj.moderation is loaded!");
        
        var harmony = new Harmony("me.muj.moderation");
        harmony.PatchAll();
    }
}