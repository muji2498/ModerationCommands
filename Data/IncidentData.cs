using System;
using System.Collections.Generic;

namespace Moderation.Data;

public class IncidentData
{
    public Player Player { get; set; }
    public Dictionary<Unit, UnitIncident> UnitIncidents { get; set; } = new();
    public DateTime LastReported { get; set; }
}

public class UnitIncident
{
    public int KillCount { get; set; }
    public int DamageCount { get; set; }

    public bool HasBeenReported { get; set; } = false;
}