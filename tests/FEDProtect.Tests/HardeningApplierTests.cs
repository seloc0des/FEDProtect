using FEDProtect.Core.Models;
using FEDProtect.Core.Services;
using Xunit;

namespace FEDProtect.Tests;

public class HardeningApplierTests
{
    [Fact]
    public async Task GenerateChecklistAsync_ReturnsReportWithAllMitigations()
    {
        var applier = new HardeningApplier();
        var profile = CreateTestProfile();
        
        var report = await applier.GenerateChecklistAsync(profile);
        
        Assert.Equal(profile.Name, report.ProfileName);
        Assert.Equal(2, report.AppliedMitigations.Count);
        Assert.NotEqual(default, report.GeneratedAt);
    }

    [Fact]
    public async Task GenerateUfwRulesAsync_ReturnsEmptyList_WhenNoLinuxHardening()
    {
        var applier = new HardeningApplier();
        var profile = new ThreatProfile
        {
            Name = "Test",
            Capabilities = new List<Capability>(),
            Countermeasures = null
        };
        
        var rules = await applier.GenerateUfwRulesAsync(profile);
        
        Assert.Empty(rules);
    }

    [Fact]
    public async Task GenerateUfwRulesAsync_ReturnsRules_WhenLinuxHardeningPresent()
    {
        var applier = new HardeningApplier();
        var profile = new ThreatProfile
        {
            Name = "Test",
            Capabilities = new List<Capability>(),
            Countermeasures = new Countermeasures
            {
                OsHardeningLinux = new OsHardeningLinux
                {
                    UfwRules = new List<string>
                    {
                        "sudo ufw default deny incoming",
                        "sudo ufw enable"
                    }
                }
            }
        };
        
        var rules = await applier.GenerateUfwRulesAsync(profile);
        
        Assert.Equal(2, rules.Count);
    }

    [Fact]
    public async Task GenerateFirefoxConfigAsync_ThrowsException_WhenNoBrowserProfile()
    {
        var applier = new HardeningApplier();
        var profile = new ThreatProfile
        {
            Name = "Test",
            Capabilities = new List<Capability>(),
            Countermeasures = null
        };
        
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            applier.GenerateFirefoxConfigAsync(profile, "."));
    }

    private static ThreatProfile CreateTestProfile()
    {
        return new ThreatProfile
        {
            Name = "Test Profile",
            Description = "Test description",
            Version = "1.0",
            Capabilities = new List<Capability>
            {
                new Capability
                {
                    Id = "test_threat",
                    Name = "Test Threat",
                    Severity = "high",
                    Description = "A test threat",
                    Mitigations = new List<Mitigation>
                    {
                        new Mitigation
                        {
                            Id = "mitigation_1",
                            Action = "do_something",
                            Description = "Do something"
                        },
                        new Mitigation
                        {
                            Id = "mitigation_2",
                            Action = "do_something_else",
                            Description = "Do something else"
                        }
                    }
                }
            }
        };
    }
}
