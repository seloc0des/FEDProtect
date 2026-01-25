using FEDProtect.Core.Models;
using FEDProtect.Core.Services;
using Xunit;

namespace FEDProtect.Tests;

public class OutputFormatterTests
{
    [Fact]
    public void FormatAsMarkdown_IncludesProfileName()
    {
        var formatter = new OutputFormatter();
        var profile = CreateTestProfile();
        var report = new HardeningReport
        {
            ProfileName = profile.Name,
            GeneratedAt = DateTime.UtcNow,
            Threats = profile.Capabilities
        };
        
        var markdown = formatter.FormatAsMarkdown(report, profile);
        
        Assert.Contains(profile.Name, markdown);
        Assert.Contains("# Security Checklist", markdown);
    }

    [Fact]
    public void FormatAsMarkdown_IncludesThreatsAndMitigations()
    {
        var formatter = new OutputFormatter();
        var profile = CreateTestProfile();
        var report = new HardeningReport
        {
            ProfileName = profile.Name,
            GeneratedAt = DateTime.UtcNow,
            Threats = profile.Capabilities
        };
        
        var markdown = formatter.FormatAsMarkdown(report, profile);
        
        Assert.Contains("Test Threat", markdown);
        Assert.Contains("[HIGH]", markdown);
        Assert.Contains("Do something", markdown);
    }

    [Fact]
    public void FormatAsMarkdown_IncludesLegalNotes()
    {
        var formatter = new OutputFormatter();
        var profile = CreateTestProfile();
        profile.LegalNotes = "Do not unlock your device";
        var report = new HardeningReport
        {
            ProfileName = profile.Name,
            GeneratedAt = DateTime.UtcNow,
            Threats = profile.Capabilities
        };
        
        var markdown = formatter.FormatAsMarkdown(report, profile);
        
        Assert.Contains("## Legal Notes", markdown);
        Assert.Contains("Do not unlock your device", markdown);
    }

    [Fact]
    public void FormatAsJson_ReturnsValidJson()
    {
        var formatter = new OutputFormatter();
        var report = new HardeningReport
        {
            ProfileName = "Test",
            GeneratedAt = DateTime.UtcNow,
            Threats = new List<Capability>()
        };
        
        var json = formatter.FormatAsJson(report);
        
        Assert.Contains("\"ProfileName\"", json);
        Assert.Contains("\"Test\"", json);
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
                        }
                    }
                }
            }
        };
    }
}
