namespace FEDProtect.Core.Models;

public class Countermeasures
{
    public BrowserProfile? BrowserProfile { get; set; }
    public OsHardeningLinux? OsHardeningLinux { get; set; }
    public OsHardeningAndroid? OsHardeningAndroid { get; set; }
    public NetworkConfig? NetworkConfig { get; set; }
}

public class BrowserProfile
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Settings { get; set; } = new();
    public List<BrowserExtension> Extensions { get; set; } = new();
    public string Notes { get; set; } = string.Empty;
}

public class BrowserExtension
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}

public class OsHardeningLinux
{
    public string Name { get; set; } = string.Empty;
    public List<SecurityCheck> Checks { get; set; } = new();
    public List<string> UfwRules { get; set; } = new();
    public string Notes { get; set; } = string.Empty;
}

public class OsHardeningAndroid
{
    public string Name { get; set; } = string.Empty;
    public List<SecurityCheck> Checks { get; set; } = new();
    public List<string> Actions { get; set; } = new();
    public string Notes { get; set; } = string.Empty;
}

public class SecurityCheck
{
    public string Check { get; set; } = string.Empty;
    public string Command { get; set; } = string.Empty;
    public string? Expected { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class NetworkConfig
{
    public string Name { get; set; } = string.Empty;
    public List<TorSetup> TorSetup { get; set; } = new();
    public List<CircumventionTool> Circumvention { get; set; } = new();
    public string Notes { get; set; } = string.Empty;
}

public class TorSetup
{
    public string Description { get; set; } = string.Empty;
    public string Download { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public string Effectiveness { get; set; } = string.Empty;
}

public class CircumventionTool
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
