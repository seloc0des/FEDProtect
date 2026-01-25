using FEDProtect.Core.Models;
using System.Text;

namespace FEDProtect.Core.Services;

public class HardeningApplier : IHardeningApplier
{
    public Task<HardeningReport> GenerateChecklistAsync(ThreatProfile profile)
    {
        var report = new HardeningReport
        {
            ProfileName = profile.Name,
            GeneratedAt = DateTime.UtcNow,
            Threats = profile.Capabilities,
            AppliedMitigations = new List<string>()
        };

        foreach (var capability in profile.Capabilities)
        {
            foreach (var mitigation in capability.Mitigations)
            {
                report.AppliedMitigations.Add($"{capability.Name}: {mitigation.Description}");
            }
        }

        return Task.FromResult(report);
    }

    public async Task<string> GenerateFirefoxConfigAsync(ThreatProfile profile, string outputPath)
    {
        if (profile.Countermeasures?.BrowserProfile == null)
        {
            throw new InvalidOperationException("Profile does not contain browser configuration");
        }

        var browserProfile = profile.Countermeasures.BrowserProfile;
        var sb = new StringBuilder();

        sb.AppendLine("// FEDProtect Firefox Profile");
        sb.AppendLine($"// Generated from: {profile.Name}");
        sb.AppendLine($"// Date: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        sb.AppendLine("// WARNING: Review these settings before applying");
        sb.AppendLine();

        foreach (var setting in browserProfile.Settings)
        {
            sb.AppendLine($"user_pref(\"{setting.Split('=')[0]}\", {setting.Split('=')[1]});");
        }

        sb.AppendLine();
        sb.AppendLine("// Recommended Extensions:");
        foreach (var extension in browserProfile.Extensions)
        {
            sb.AppendLine($"// - {extension.Name}: {extension.Description}");
            sb.AppendLine($"//   {extension.Url}");
        }

        sb.AppendLine();
        sb.AppendLine($"// Notes: {browserProfile.Notes}");

        var userJsPath = Path.Combine(outputPath, "user.js");
        await File.WriteAllTextAsync(userJsPath, sb.ToString());

        return userJsPath;
    }

    public Task<List<string>> GenerateUfwRulesAsync(ThreatProfile profile)
    {
        if (profile.Countermeasures?.OsHardeningLinux == null)
        {
            return Task.FromResult(new List<string>());
        }

        return Task.FromResult(profile.Countermeasures.OsHardeningLinux.UfwRules);
    }
}
