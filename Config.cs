using BepInEx.Configuration;

namespace CommandMod;

public class Config(ConfigFile file)
{
    public ConfigFile File { get; } = file;
    
    // General
    public ConfigEntry<bool> Enabled { get; } = file.Bind("Moderation", "Enabled", true, "Toggle whether the plugin is enabled");
    public ConfigEntry<bool> KickOnKill { get; } = file.Bind("Moderation", "KickOnKill", true, "When true players will be kicked if they kill a player, if false they will be kicked when they damage a unit or player.");
    public ConfigEntry<bool> FriendlyFirePlayerKick { get; } = file.Bind("Moderation", "FriendlyPlayerFireKick", false, "When enabled players will be auto kicked if they damage/kill a friendly player.");
    public ConfigEntry<bool> FriendlyFireUnitKick { get; } = file.Bind("Moderation", "FriendlyUnitFireKick", false, "When enabled players will be auto kicked if they damage/kill a friendly unit.");
    public ConfigEntry<int> FriendlyFireMaxIncidents { get; } = file.Bind("Moderation", "FriendlyFireMaxIncidents", 10, "Number of incidents before player is kicked.");
    
    // Discord Webhooks
    public ConfigEntry<string> DiscordWebhook { get; } = file.Bind("Moderation", "DiscordWebhook", "", "This is the url of the Discord Webhook.");
    public ConfigEntry<string> MessagePrefix { get; } = file.Bind("Moderation", "MessagePrefix", "Tickets:", "This is the prefix the message will have.");
}