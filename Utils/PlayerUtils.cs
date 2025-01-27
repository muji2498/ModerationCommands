using Mirage.SteamworksSocket;

namespace Moderation.Utils;

public class PlayerUtils
{
    public static ulong GetSteamId(Player callingPlayer)
    {
        return callingPlayer.Owner.Address is SteamEndPoint endpoint ? endpoint.Connection.SteamID.m_SteamID : 0;
    }
}