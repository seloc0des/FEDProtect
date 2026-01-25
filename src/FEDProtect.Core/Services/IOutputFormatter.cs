using FEDProtect.Core.Models;

namespace FEDProtect.Core.Services;

public interface IOutputFormatter
{
    string FormatAsMarkdown(HardeningReport report, ThreatProfile profile);
    string FormatAsJson(HardeningReport report);
}
