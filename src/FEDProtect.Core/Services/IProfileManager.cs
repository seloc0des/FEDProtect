using FEDProtect.Core.Models;

namespace FEDProtect.Core.Services;

public interface IProfileManager
{
    Task<ThreatProfile> LoadProfileAsync(string profileName);
    Task<List<string>> ListAvailableProfilesAsync();
    Task ValidateProfileAsync(ThreatProfile profile);
}
