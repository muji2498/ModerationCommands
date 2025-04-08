using BepInEx.Configuration;

namespace Moderation;

public class Config(ConfigFile file)
{
    public ConfigFile File { get; } = file;
    
    // General
    public ConfigEntry<bool> Enabled { get; } = file.Bind("Moderation", "Enabled", true, "Toggle whether the plugin is enabled");
    public ConfigEntry<bool> FriendlyFirePlayerKick { get; } = file.Bind("Moderation", "FriendlyPlayerFireKick", true, "When enabled players will be auto kicked if they damage/kill a friendly player.");
    public ConfigEntry<bool> FriendlyFireUnitKick { get; } = file.Bind("Moderation", "FriendlyUnitFireKick", true, "When enabled players will be auto kicked if they damage/kill a friendly unit.");
    public ConfigEntry<int> UnitMaxIncidents { get; } = file.Bind("Moderation", "UnitMaxIncidents", 10, "Number of incidents before player is kicked.");
    public ConfigEntry<int> PlayerMaxIncidents { get; } = file.Bind("Moderation", "PlayerMaxIncidents", 1, "Number of incidents before player is kicked.");
    public ConfigEntry<int> FriendlyUnitThreshold { get; } = file.Bind("Moderation", "FriendlyUnitThreshold", 5, "Number of units a player can kill before a incident is reported.");
    public ConfigEntry<bool> AFV6AAPatch { get; } = file.Bind("Moderation", "AFV6AAPatch", true, "If enabled, plugin will skip checks for AFV6 AA unit.");
    
    // Discord Webhooks
    public ConfigEntry<string> FriendlyFireWebhook { get; } = file.Bind("Moderation", "FriendlyFireWebhook", "", "This is the url of the friendly fire Discord Webhook.");
    public ConfigEntry<string> ModerationWebhook { get; } = file.Bind("Moderation", "ModerationWebhook", "", "This is the url of the general Discord Webhook.");
    public ConfigEntry<string> TicketWebhook { get; } = file.Bind("Moderation", "TicketWebhook", "", "This is the url of the ticket Discord Webhook.");
    public ConfigEntry<string> MessagePrefix { get; } = file.Bind("Moderation", "MessagePrefix", "Tickets:", "This is the prefix the ticket message will have.");
    public ConfigEntry<bool> PlayerJoinLeaveMessages { get;  } = file.Bind("Moderation", "PlayerJoinLeaveMessages", true, "Whether player join/leave messages should be sent to webhook.");
    public ConfigEntry<bool> SendTicketsToFriendlyFireLogs { get; } = file.Bind("Moderation", "SendTicketsToFriendlyFireLogs", false, "Whether ticket logs should be sent to friendly fire webhook or to ticket webhook.");
}