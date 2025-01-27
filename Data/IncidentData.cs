using System;

namespace Moderation.Data;

public class IncidentData
{
    public Player Player { get; set; }
    public Unit DamagedUnit { get; set; }
    public int KillCount { get; set; }
    public int DamageCount { get; set; }
    public DateTime LastReported { get; set; }
}