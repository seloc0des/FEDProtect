using FEDProtect.Core.Services;
using Spectre.Console;
using System.CommandLine;

namespace FEDProtect.CLI;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("FEDProtect - Protect protesters from surveillance");

        var listProfilesCommand = new Command("list-profiles", "List available threat profiles");
        var showProfileCommand = new Command("show-profile", "Show details of a threat profile");
        var generateChecklistCommand = new Command("generate-checklist", "Generate a security checklist");
        var createBrowserProfileCommand = new Command("create-browser-profile", "Create a hardened Firefox profile");
        var checkOsSecurityCommand = new Command("check-os-security", "Run OS security checks");
        var validateProfileCommand = new Command("validate-profile", "Validate a threat profile YAML file");
        var applyCommand = new Command("apply", "Apply hardening from a threat profile");

        var showProfileArg = new Argument<string>("profile-name", "Name of the threat profile");
        showProfileCommand.AddArgument(showProfileArg);
        
        var checklistProfileArg = new Argument<string>("profile-name", "Name of the threat profile");
        var checklistOutputOption = new Option<string?>("--output", "Output file path");
        generateChecklistCommand.AddArgument(checklistProfileArg);
        generateChecklistCommand.AddOption(checklistOutputOption);

        var browserProfileArg = new Argument<string>("profile-name", "Name of the threat profile");
        var browserOutputOption = new Option<string>("--output", () => ".", "Output directory");
        createBrowserProfileCommand.AddArgument(browserProfileArg);
        createBrowserProfileCommand.AddOption(browserOutputOption);

        var osCheckProfileArg = new Argument<string>("profile-name", "Name of the threat profile");
        checkOsSecurityCommand.AddArgument(osCheckProfileArg);

        var validateProfilePathArg = new Argument<string>("profile-path", "Path to the YAML profile file");
        validateProfileCommand.AddArgument(validateProfilePathArg);

        var applyProfileArg = new Argument<string>("profile-name", "Name of the threat profile");
        var dryRunOption = new Option<bool>("--dry-run", "Preview changes without applying");
        var confirmOption = new Option<bool>("--confirm", "Apply changes with confirmation");
        applyCommand.AddArgument(applyProfileArg);
        applyCommand.AddOption(dryRunOption);
        applyCommand.AddOption(confirmOption);

        listProfilesCommand.SetHandler(ListProfilesHandler);
        showProfileCommand.SetHandler(ShowProfileHandler, showProfileArg);
        generateChecklistCommand.SetHandler(GenerateChecklistHandler, checklistProfileArg, checklistOutputOption);
        createBrowserProfileCommand.SetHandler(CreateBrowserProfileHandler, browserProfileArg, browserOutputOption);
        checkOsSecurityCommand.SetHandler(CheckOsSecurityHandler, osCheckProfileArg);
        validateProfileCommand.SetHandler(ValidateProfileHandler, validateProfilePathArg);
        applyCommand.SetHandler(ApplyHardeningHandler, applyProfileArg, dryRunOption, confirmOption);

        rootCommand.AddCommand(listProfilesCommand);
        rootCommand.AddCommand(showProfileCommand);
        rootCommand.AddCommand(generateChecklistCommand);
        rootCommand.AddCommand(createBrowserProfileCommand);
        rootCommand.AddCommand(checkOsSecurityCommand);
        rootCommand.AddCommand(validateProfileCommand);
        rootCommand.AddCommand(applyCommand);

        return await rootCommand.InvokeAsync(args);
    }

    static async Task ListProfilesHandler()
    {
        var profileManager = new ProfileManager();
        var profiles = await profileManager.ListAvailableProfilesAsync();

        AnsiConsole.MarkupLine("[bold green]Available Threat Profiles:[/]");
        AnsiConsole.WriteLine();

        if (profiles.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No profiles found. Add YAML profiles to the profiles/ directory.[/]");
            return;
        }

        var table = new Table();
        table.AddColumn("Profile Name");
        
        foreach (var profile in profiles)
        {
            table.AddRow(profile);
        }

        AnsiConsole.Write(table);
    }

    static async Task ShowProfileHandler(string profileName)
    {
        try
        {
            var profileManager = new ProfileManager();
            var profile = await profileManager.LoadProfileAsync(profileName);

            AnsiConsole.MarkupLine($"[bold green]{profile.Name}[/]");
            AnsiConsole.MarkupLine($"[dim]{profile.Description}[/]");
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"Version: {profile.Version}");
            AnsiConsole.MarkupLine($"Last Updated: {profile.LastUpdated:yyyy-MM-dd}");
            AnsiConsole.WriteLine();

            AnsiConsole.MarkupLine("[bold yellow]Threats:[/]");
            foreach (var capability in profile.Capabilities)
            {
                var severityColor = capability.Severity.ToLower() switch
                {
                    "high" => "red",
                    "medium" => "yellow",
                    "low" => "blue",
                    _ => "white"
                };

                AnsiConsole.MarkupLine($"  [{severityColor}]■[/] {capability.Name} [{severityColor.ToUpper()}]");
                AnsiConsole.MarkupLine($"    {capability.Description}");
                AnsiConsole.MarkupLine($"    Mitigations: {capability.Mitigations.Count}");
                AnsiConsole.WriteLine();
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        }
    }

    static async Task GenerateChecklistHandler(string profileName, string? output)
    {
        try
        {
            var profileManager = new ProfileManager();
            var profile = await profileManager.LoadProfileAsync(profileName);

            var applier = new HardeningApplier();
            var report = await applier.GenerateChecklistAsync(profile);

            var formatter = new OutputFormatter();
            var markdown = formatter.FormatAsMarkdown(report, profile);

            var outputPath = output ?? $"{profileName}-checklist.md";
            await File.WriteAllTextAsync(outputPath, markdown);

            AnsiConsole.MarkupLine($"[green]✓[/] Checklist generated: {outputPath}");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        }
    }

    static async Task CreateBrowserProfileHandler(string profileName, string outputDir)
    {
        try
        {
            var profileManager = new ProfileManager();
            var profile = await profileManager.LoadProfileAsync(profileName);

            var applier = new HardeningApplier();
            var configPath = await applier.GenerateFirefoxConfigAsync(profile, outputDir);

            AnsiConsole.MarkupLine($"[green]✓[/] Firefox profile generated: {configPath}");
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[yellow]Next steps:[/]");
            AnsiConsole.MarkupLine("1. Open Firefox and navigate to about:profiles");
            AnsiConsole.MarkupLine("2. Create a new profile (e.g., 'activism')");
            AnsiConsole.MarkupLine($"3. Copy {configPath} to the profile directory");
            AnsiConsole.MarkupLine("4. Restart Firefox with the new profile");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        }
    }

    static async Task CheckOsSecurityHandler(string profileName)
    {
        try
        {
            var profileManager = new ProfileManager();
            var profile = await profileManager.LoadProfileAsync(profileName);

            var osChecker = new OsChecker();
            
            AnsiConsole.MarkupLine("[bold yellow]Running security checks...[/]");
            AnsiConsole.WriteLine();

            var results = await osChecker.RunAllChecksAsync(profile);

            var table = new Table();
            table.AddColumn("Check");
            table.AddColumn("Status");
            table.AddColumn("Message");

            foreach (var result in results)
            {
                var status = result.Passed ? "[green]✓ PASS[/]" : "[red]✗ FAIL[/]";
                table.AddRow(result.CheckName, status, result.Message);
            }

            AnsiConsole.Write(table);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        }
    }

    static async Task ValidateProfileHandler(string profilePath)
    {
        try
        {
            if (!File.Exists(profilePath))
            {
                AnsiConsole.MarkupLine($"[red]Error: File not found: {profilePath}[/]");
                return;
            }

            AnsiConsole.MarkupLine($"[yellow]Validating profile: {profilePath}[/]");
            AnsiConsole.WriteLine();

            var yamlContent = await File.ReadAllTextAsync(profilePath);
            var deserializer = new YamlDotNet.Serialization.DeserializerBuilder()
                .WithNamingConvention(YamlDotNet.Serialization.NamingConventions.UnderscoredNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();

            var profile = deserializer.Deserialize<FEDProtect.Core.Models.ThreatProfile>(yamlContent);

            if (profile == null)
            {
                AnsiConsole.MarkupLine("[red]Error: Failed to parse YAML[/]");
                return;
            }

            var errors = new List<string>();
            var warnings = new List<string>();

            if (string.IsNullOrWhiteSpace(profile.Name))
                errors.Add("Profile must have a 'name' field");

            if (string.IsNullOrWhiteSpace(profile.Description))
                warnings.Add("Profile should have a 'description' field");

            if (string.IsNullOrWhiteSpace(profile.Version))
                warnings.Add("Profile should have a 'version' field");

            if (profile.Capabilities == null || profile.Capabilities.Count == 0)
            {
                errors.Add("Profile must have at least one capability");
            }
            else
            {
                foreach (var cap in profile.Capabilities)
                {
                    if (string.IsNullOrWhiteSpace(cap.Id))
                        errors.Add($"Capability is missing 'id' field");
                    
                    if (string.IsNullOrWhiteSpace(cap.Name))
                        errors.Add($"Capability '{cap.Id}' is missing 'name' field");

                    if (cap.Sources == null || cap.Sources.Count == 0)
                        warnings.Add($"Capability '{cap.Id}' has no sources cited");

                    if (cap.Mitigations == null || cap.Mitigations.Count == 0)
                        errors.Add($"Capability '{cap.Id}' must have at least one mitigation");
                    else
                    {
                        foreach (var mit in cap.Mitigations)
                        {
                            if (string.IsNullOrWhiteSpace(mit.Id))
                                errors.Add($"Mitigation in '{cap.Id}' is missing 'id' field");
                            if (string.IsNullOrWhiteSpace(mit.Description))
                                warnings.Add($"Mitigation '{mit.Id}' should have a description");
                        }
                    }
                }
            }

            if (errors.Count > 0)
            {
                AnsiConsole.MarkupLine("[red]Validation FAILED[/]");
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine("[red]Errors:[/]");
                foreach (var error in errors)
                {
                    AnsiConsole.MarkupLine($"  [red]✗[/] {error}");
                }
            }

            if (warnings.Count > 0)
            {
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine("[yellow]Warnings:[/]");
                foreach (var warning in warnings)
                {
                    AnsiConsole.MarkupLine($"  [yellow]![/] {warning}");
                }
            }

            if (errors.Count == 0)
            {
                AnsiConsole.MarkupLine("[green]Validation PASSED[/]");
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine($"  Profile: {profile.Name}");
                AnsiConsole.MarkupLine($"  Capabilities: {profile.Capabilities?.Count ?? 0}");
                AnsiConsole.MarkupLine($"  Total Mitigations: {profile.Capabilities?.Sum(c => c.Mitigations?.Count ?? 0) ?? 0}");
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        }
    }

    static async Task ApplyHardeningHandler(string profileName, bool dryRun, bool confirm)
    {
        try
        {
            var profileManager = new ProfileManager();
            var profile = await profileManager.LoadProfileAsync(profileName);

            AnsiConsole.MarkupLine($"[bold green]Profile: {profile.Name}[/]");
            AnsiConsole.MarkupLine($"[dim]{profile.Description}[/]");
            AnsiConsole.WriteLine();

            var actions = new List<(string Category, string Action, string Command)>();

            if (profile.Countermeasures?.BrowserProfile != null)
            {
                actions.Add(("Browser", "Create Firefox hardened profile", "Generate user.js config file"));
            }

            if (profile.Countermeasures?.OsHardeningLinux != null)
            {
                foreach (var rule in profile.Countermeasures.OsHardeningLinux.UfwRules)
                {
                    actions.Add(("Firewall", "UFW Rule", rule));
                }
            }

            if (actions.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No automated actions available for this profile.[/]");
                AnsiConsole.MarkupLine("Use 'generate-checklist' for manual recommendations.");
                return;
            }

            var table = new Table();
            table.AddColumn("Category");
            table.AddColumn("Action");
            table.AddColumn("Command/Details");

            foreach (var (category, action, command) in actions)
            {
                table.AddRow(category, action, command);
            }

            AnsiConsole.MarkupLine("[bold yellow]Proposed Actions:[/]");
            AnsiConsole.WriteLine();
            AnsiConsole.Write(table);
            AnsiConsole.WriteLine();

            if (dryRun)
            {
                AnsiConsole.MarkupLine("[cyan]--dry-run specified. No changes will be made.[/]");
                return;
            }

            if (!confirm)
            {
                AnsiConsole.MarkupLine("[yellow]Use --confirm to apply these changes, or --dry-run to preview.[/]");
                return;
            }

            AnsiConsole.MarkupLine("[bold]Applying hardening...[/]");
            AnsiConsole.WriteLine();

            foreach (var (category, action, command) in actions)
            {
                var proceed = AnsiConsole.Confirm($"Apply: {category} - {action}?", true);
                
                if (!proceed)
                {
                    AnsiConsole.MarkupLine($"  [yellow]Skipped:[/] {action}");
                    continue;
                }

                if (category == "Browser")
                {
                    var applier = new HardeningApplier();
                    var path = await applier.GenerateFirefoxConfigAsync(profile, ".");
                    AnsiConsole.MarkupLine($"  [green]Created:[/] {path}");
                }
                else if (category == "Firewall")
                {
                    AnsiConsole.MarkupLine($"  [cyan]Run manually:[/] {command}");
                    AnsiConsole.MarkupLine($"  [dim](Firewall commands require sudo and are not auto-executed for safety)[/]");
                }
            }

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[green]Hardening complete.[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        }
    }
}
