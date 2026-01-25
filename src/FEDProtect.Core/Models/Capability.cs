namespace FEDProtect.Core.Models;

public class Capability
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<Source> Sources { get; set; } = new();
    public List<Mitigation> Mitigations { get; set; } = new();
}
