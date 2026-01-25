using FEDProtect.Core.Models;
using FEDProtect.Core.Services;
using Xunit;

namespace FEDProtect.Tests;

public class ProfileManagerTests
{
    [Fact]
    public async Task ListAvailableProfilesAsync_ReturnsEmptyList_WhenDirectoryDoesNotExist()
    {
        var manager = new ProfileManager("/nonexistent/path");
        var profiles = await manager.ListAvailableProfilesAsync();
        
        Assert.Empty(profiles);
    }

    [Fact]
    public async Task ValidateProfileAsync_ThrowsException_WhenProfileNameIsEmpty()
    {
        var manager = new ProfileManager();
        var profile = new ThreatProfile { Name = "" };
        
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            manager.ValidateProfileAsync(profile));
    }

    [Fact]
    public async Task ValidateProfileAsync_ThrowsException_WhenNoCapabilities()
    {
        var manager = new ProfileManager();
        var profile = new ThreatProfile 
        { 
            Name = "Test Profile",
            Capabilities = new List<Capability>()
        };
        
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            manager.ValidateProfileAsync(profile));
    }

    [Fact]
    public async Task ValidateProfileAsync_ThrowsException_WhenCapabilityHasNoMitigations()
    {
        var manager = new ProfileManager();
        var profile = new ThreatProfile 
        { 
            Name = "Test Profile",
            Capabilities = new List<Capability>
            {
                new Capability
                {
                    Id = "test_capability",
                    Name = "Test Threat",
                    Mitigations = new List<Mitigation>()
                }
            }
        };
        
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            manager.ValidateProfileAsync(profile));
    }

    [Fact]
    public async Task ValidateProfileAsync_Succeeds_WithValidProfile()
    {
        var manager = new ProfileManager();
        var profile = new ThreatProfile 
        { 
            Name = "Test Profile",
            Capabilities = new List<Capability>
            {
                new Capability
                {
                    Id = "test_capability",
                    Name = "Test Threat",
                    Mitigations = new List<Mitigation>
                    {
                        new Mitigation
                        {
                            Id = "test_mitigation",
                            Action = "test_action",
                            Description = "Test action"
                        }
                    }
                }
            }
        };
        
        await manager.ValidateProfileAsync(profile);
    }
}
