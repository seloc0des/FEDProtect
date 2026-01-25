using FEDProtect.Core.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FEDProtect.Core.Services;

public class ProfileManager : IProfileManager
{
    private readonly string _profilesDirectory;
    private readonly IDeserializer _deserializer;
    private readonly ILogger? _logger;

    public ProfileManager(string? profilesDirectory = null, ILogger? logger = null)
    {
        _profilesDirectory = profilesDirectory ?? FindProfilesDirectory();
        _logger = logger;
        _deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();
        
        _logger?.LogInfo($"ProfileManager initialized with directory: {_profilesDirectory}");
    }

    private static string FindProfilesDirectory()
    {
        var candidates = new[]
        {
            Path.Combine(AppContext.BaseDirectory, "profiles"),
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "profiles"),
            Path.Combine(Directory.GetCurrentDirectory(), "profiles"),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "profiles"),
        };

        foreach (var candidate in candidates)
        {
            var fullPath = Path.GetFullPath(candidate);
            if (Directory.Exists(fullPath))
            {
                return fullPath;
            }
        }

        return Path.Combine(AppContext.BaseDirectory, "profiles");
    }

    public async Task<ThreatProfile> LoadProfileAsync(string profileName)
    {
        _logger?.LogInfo($"Loading profile: {profileName}");
        var profilePath = Path.Combine(_profilesDirectory, $"{profileName}.yaml");
        
        if (!File.Exists(profilePath))
        {
            _logger?.LogError($"Profile not found: {profileName}");
            throw new FileNotFoundException($"Profile not found: {profileName}");
        }

        var yamlContent = await File.ReadAllTextAsync(profilePath);
        var profile = _deserializer.Deserialize<ThreatProfile>(yamlContent);

        if (profile == null)
        {
            _logger?.LogError($"Failed to deserialize profile: {profileName}");
            throw new InvalidOperationException($"Failed to deserialize profile: {profileName}");
        }

        await ValidateProfileAsync(profile);
        _logger?.LogInfo($"Profile loaded successfully: {profile.Name} (v{profile.Version})");
        return profile;
    }

    public async Task<List<string>> ListAvailableProfilesAsync()
    {
        _logger?.LogDebug($"Listing profiles from: {_profilesDirectory}");
        
        if (!Directory.Exists(_profilesDirectory))
        {
            _logger?.LogWarning($"Profiles directory not found: {_profilesDirectory}");
            return new List<string>();
        }

        var yamlFiles = Directory.GetFiles(_profilesDirectory, "*.yaml");
        var profiles = yamlFiles
            .Select(Path.GetFileNameWithoutExtension)
            .Where(name => !string.IsNullOrEmpty(name))
            .Select(name => name!)
            .ToList();

        _logger?.LogInfo($"Found {profiles.Count} profile(s)");
        return await Task.FromResult(profiles);
    }

    public Task ValidateProfileAsync(ThreatProfile profile)
    {
        if (string.IsNullOrWhiteSpace(profile.Name))
        {
            throw new InvalidOperationException("Profile must have a name");
        }

        if (profile.Capabilities == null || profile.Capabilities.Count == 0)
        {
            throw new InvalidOperationException("Profile must have at least one capability");
        }

        foreach (var capability in profile.Capabilities)
        {
            if (string.IsNullOrWhiteSpace(capability.Id))
            {
                throw new InvalidOperationException($"Capability in profile '{profile.Name}' is missing an ID");
            }

            if (capability.Mitigations == null || capability.Mitigations.Count == 0)
            {
                throw new InvalidOperationException($"Capability '{capability.Id}' must have at least one mitigation");
            }
        }

        return Task.CompletedTask;
    }
}
