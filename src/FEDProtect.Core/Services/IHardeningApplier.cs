using FEDProtect.Core.Models;

namespace FEDProtect.Core.Services;

public interface IHardeningApplier
{
    Task<HardeningReport> GenerateChecklistAsync(ThreatProfile profile);
    Task<string> GenerateFirefoxConfigAsync(ThreatProfile profile, string outputPath);
    Task<List<string>> GenerateUfwRulesAsync(ThreatProfile profile);
}
