using System;
using NuclearOption.Networking;

namespace Moderation.Data;

public class IncidentData
{
    public Player Player { get; set; }
    public int UnitIncidents { get; set; }
    public int PlayerIncidents { get; set; }
    public DateTime LastReported { get; set; }
}