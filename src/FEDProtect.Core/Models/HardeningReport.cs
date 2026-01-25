namespace FEDProtect.Core.Models;

public class HardeningReport
{
    public string ProfileName { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public List<Capability> Threats { get; set; } = new();
    public List<string> AppliedMitigations { get; set; } = new();
    public string FirefoxProfilePath { get; set; } = string.Empty;
    public List<SecurityCheckResult> SecurityChecks { get; set; } = new();
}

public class SecurityCheckResult
{
    public string CheckName { get; set; } = string.Empty;
    public bool Passed { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
}
