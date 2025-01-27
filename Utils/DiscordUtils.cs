namespace Moderation.Utils;

public class DiscordUtils
{
    public static string Sanitise(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return string.Empty;

        message = message.Trim();
        message = message.Replace("@", "[at]");
        message = message.Replace("`", "");
        message = message.Replace("*", "");
        message = message.Replace("_", "");
        message = message.Replace("~", "");
        message = message.Replace("|", "");
        
        return message;
    }
}