namespace FEDProtect.Core.Models;

public class ThreatProfile
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
    public List<string> Regions { get; set; } = new();
    public List<string> Contexts { get; set; } = new();
    public List<Capability> Capabilities { get; set; } = new();
    public Countermeasures? Countermeasures { get; set; }
    public string LegalNotes { get; set; } = string.Empty;
    public List<string> Sources { get; set; } = new();
    public List<string> Tags { get; set; } = new();
}
