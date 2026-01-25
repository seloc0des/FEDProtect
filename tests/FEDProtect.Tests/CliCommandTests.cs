using FEDProtect.Core.Models;
using FEDProtect.Core.Services;
using Xunit;

namespace FEDProtect.Tests;

public class CliCommandTests
{
    [Fact]
    public void ValidateProfile_DetectsEmptyName()
    {
        var profile = new ThreatProfile { Name = "" };
        var errors = ValidateProfileStructure(profile);
        
        Assert.Contains(errors, e => e.Contains("name"));
    }

    [Fact]
    public void ValidateProfile_DetectsNoCapabilities()
    {
        var profile = new ThreatProfile 
        { 
            Name = "Test",
            Capabilities = new List<Capability>()
        };
        var errors = ValidateProfileStructure(profile);
        
        Assert.Contains(errors, e => e.Contains("capability"));
    }

    [Fact]
    public void ValidateProfile_DetectsCapabilityWithoutId()
    {
        var profile = new ThreatProfile 
        { 
            Name = "Test",
            Capabilities = new List<Capability>
            {
                new Capability { Id = "", Name = "Test Threat" }
            }
        };
        var errors = ValidateProfileStructure(profile);
        
        Assert.Contains(errors, e => e.Contains("id"));
    }

    [Fact]
    public void ValidateProfile_DetectsMitigationWithoutAction()
    {
        var profile = new ThreatProfile 
        { 
            Name = "Test",
            Capabilities = new List<Capability>
            {
                new Capability 
                { 
                    Id = "test", 
                    Name = "Test Threat",
                    Mitigations = new List<Mitigation>
                    {
                        new Mitigation { Id = "m1", Action = "" }
                    }
                }
            }
        };
        var errors = ValidateProfileStructure(profile);
        
        Assert.Contains(errors, e => e.Contains("action"));
    }

    [Fact]
    public void ValidateProfile_PassesWithValidProfile()
    {
        var profile = CreateValidProfile();
        var errors = ValidateProfileStructure(profile);
        
        Assert.Empty(errors);
    }

    [Fact]
    public void ValidateProfile_WarnsOnMissingDescription()
    {
        var profile = CreateValidProfile();
        profile.Description = "";
        var warnings = GetProfileWarnings(profile);
        
        Assert.Contains(warnings, w => w.Contains("description"));
    }

    [Fact]
    public void ValidateProfile_WarnsOnMissingVersion()
    {
        var profile = CreateValidProfile();
        profile.Version = "";
        var warnings = GetProfileWarnings(profile);
        
        Assert.Contains(warnings, w => w.Contains("version"));
    }

    [Fact]
    public void ValidateProfile_WarnsOnCapabilityWithoutSources()
    {
        var profile = CreateValidProfile();
        profile.Capabilities[0].Sources = new List<Source>();
        var warnings = GetProfileWarnings(profile);
        
        Assert.Contains(warnings, w => w.Contains("sources"));
    }

    [Fact]
    public async Task ApplyHardening_GeneratesChecklistForProfile()
    {
        var profile = CreateValidProfile();
        var applier = new HardeningApplier();
        
        var report = await applier.GenerateChecklistAsync(profile);
        
        Assert.NotNull(report);
        Assert.NotEmpty(report.Threats);
    }

    [Fact]
    public async Task ApplyHardening_GeneratesFirefoxConfigForProfile()
    {
        var profile = CreateValidProfile();
        profile.Countermeasures = new Countermeasures
        {
            BrowserProfile = new BrowserProfile
            {
                Name = "Test Browser",
                Settings = new List<string> { "privacy.resistFingerprinting=true" },
                Extensions = new List<BrowserExtension>
                {
                    new BrowserExtension { Name = "uBlock Origin", Description = "Ad blocker", Url = "https://example.com" }
                },
                Notes = "Test notes"
            }
        };
        var applier = new HardeningApplier();
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        try
        {
            var outputPath = await applier.GenerateFirefoxConfigAsync(profile, tempDir);
            
            Assert.True(File.Exists(outputPath));
            var content = await File.ReadAllTextAsync(outputPath);
            Assert.Contains("user_pref", content);
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public async Task ApplyHardening_GeneratesUfwRulesForProfile()
    {
        var profile = CreateValidProfile();
        profile.Countermeasures = new Countermeasures
        {
            OsHardeningLinux = new OsHardeningLinux
            {
                Name = "Test Linux",
                UfwRules = new List<string> { "ufw default deny incoming", "ufw allow 443/tcp" }
            }
        };
        var applier = new HardeningApplier();
        
        var rules = await applier.GenerateUfwRulesAsync(profile);
        
        Assert.Contains("ufw default deny incoming", rules);
        Assert.Contains("ufw allow 443/tcp", rules);
    }

    private static ThreatProfile CreateValidProfile()
    {
        return new ThreatProfile
        {
            Name = "Test Profile",
            Description = "A test threat profile",
            Version = "1.0.0",
            LastUpdated = DateTime.Now,
            Capabilities = new List<Capability>
            {
                new Capability
                {
                    Id = "test_threat",
                    Name = "Test Threat",
                    Severity = "high",
                    Description = "A test threat description",
                    Sources = new List<Source>
                    {
                        new Source { Title = "Test Source", Url = "https://example.com" }
                    },
                    Mitigations = new List<Mitigation>
                    {
                        new Mitigation
                        {
                            Id = "test_mitigation",
                            Action = "Test Action",
                            Description = "Test mitigation description",
                            Difficulty = "easy",
                            Effectiveness = "high"
                        }
                    }
                }
            }
        };
    }

    private static List<string> ValidateProfileStructure(ThreatProfile profile)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(profile.Name))
            errors.Add("Profile must have a 'name' field");

        if (profile.Capabilities == null || profile.Capabilities.Count == 0)
        {
            errors.Add("Profile must have at least one capability");
            return errors;
        }

        foreach (var capability in profile.Capabilities)
        {
            if (string.IsNullOrWhiteSpace(capability.Id))
                errors.Add($"Capability is missing 'id' field");

            if (string.IsNullOrWhiteSpace(capability.Name))
                errors.Add($"Capability '{capability.Id}' is missing 'name' field");

            if (capability.Mitigations == null || capability.Mitigations.Count == 0)
            {
                errors.Add($"Capability '{capability.Id}' must have at least one mitigation");
            }
            else
            {
                foreach (var mitigation in capability.Mitigations)
                {
                    if (string.IsNullOrWhiteSpace(mitigation.Action))
                        errors.Add($"Mitigation '{mitigation.Id}' is missing 'action' field");
                }
            }
        }

        return errors;
    }

    private static List<string> GetProfileWarnings(ThreatProfile profile)
    {
        var warnings = new List<string>();

        if (string.IsNullOrWhiteSpace(profile.Description))
            warnings.Add("Profile should have a 'description' field");

        if (string.IsNullOrWhiteSpace(profile.Version))
            warnings.Add("Profile should have a 'version' field");

        if (profile.Capabilities != null)
        {
            foreach (var capability in profile.Capabilities)
            {
                if (capability.Sources == null || capability.Sources.Count == 0)
                    warnings.Add($"Capability '{capability.Id}' has no sources cited");
            }
        }

        return warnings;
    }
}
