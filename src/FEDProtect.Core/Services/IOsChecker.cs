using FEDProtect.Core.Models;

namespace FEDProtect.Core.Services;

public interface IOsChecker
{
    Task<SecurityCheckResult> CheckEncryptionAsync();
    Task<SecurityCheckResult> CheckFirewallAsync();
    Task<List<SecurityCheckResult>> RunAllChecksAsync(ThreatProfile profile);
}
