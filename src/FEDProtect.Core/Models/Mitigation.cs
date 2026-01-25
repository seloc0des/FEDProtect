using YamlDotNet.Serialization;

namespace FEDProtect.Core.Models;

public class Mitigation
{
    public string Id { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public string Effectiveness { get; set; } = string.Empty;
    public string? Resource { get; set; }
    
    [YamlMember(Alias = "os")]
    public List<string>? OperatingSystems { get; set; }
}
