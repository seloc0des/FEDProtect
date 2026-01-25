using FEDProtect.Core.Models;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FEDProtect.Core.Services;

public class OsChecker : IOsChecker
{
    public async Task<SecurityCheckResult> CheckEncryptionAsync()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return await RunCommandCheckAsync(
                "dmsetup",
                "table --showkeys",
                "crypt",
                "Full-disk encryption (LUKS)"
            );
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return await RunCommandCheckAsync(
                "manage-bde",
                "-status C:",
                "Protection On",
                "BitLocker encryption (C: drive)"
            );
        }

        return new SecurityCheckResult
        {
            CheckName = "Encryption Check",
            Passed = false,
            Message = "Encryption check not implemented for this OS"
        };
    }

    public async Task<SecurityCheckResult> CheckFirewallAsync()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return await RunCommandCheckAsync(
                "ufw",
                "status",
                "Status: active",
                "UFW Firewall"
            );
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return await RunCommandCheckAsync(
                "netsh",
                "advfirewall show allprofiles state",
                "ON",
                "Windows Firewall"
            );
        }

        return new SecurityCheckResult
        {
            CheckName = "Firewall Check",
            Passed = false,
            Message = "Firewall check not implemented for this OS"
        };
    }

    public async Task<List<SecurityCheckResult>> RunAllChecksAsync(ThreatProfile profile)
    {
        var results = new List<SecurityCheckResult>();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            if (profile.Countermeasures?.OsHardeningLinux?.Checks != null)
            {
                foreach (var check in profile.Countermeasures.OsHardeningLinux.Checks)
                {
                    var parts = check.Command.Split(' ', 2);
                    var command = parts[0];
                    var args = parts.Length > 1 ? parts[1] : string.Empty;
                    var expectedOutput = check.Expected ?? string.Empty;

                    var result = await RunCommandCheckAsync(command, args, expectedOutput, check.Description);
                    results.Add(result);
                }
            }
            else
            {
                results.Add(await CheckFirewallAsync());
                results.Add(await CheckEncryptionAsync());
            }
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            results.Add(await CheckFirewallAsync());
            results.Add(await CheckEncryptionAsync());
            results.Add(await CheckWindowsDefenderAsync());
            results.Add(await CheckWindowsUpdateAsync());
            results.Add(await CheckRemoteDesktopAsync());
        }

        return results;
    }

    public async Task<SecurityCheckResult> CheckWindowsDefenderAsync()
    {
        return await RunCommandCheckAsync(
            "powershell",
            "-Command \"Get-MpComputerStatus | Select-Object -ExpandProperty RealTimeProtectionEnabled\"",
            "True",
            "Windows Defender real-time protection"
        );
    }

    public async Task<SecurityCheckResult> CheckWindowsUpdateAsync()
    {
        return await RunCommandCheckAsync(
            "powershell",
            "-Command \"(Get-Service wuauserv).Status\"",
            "Running",
            "Windows Update service"
        );
    }

    public async Task<SecurityCheckResult> CheckRemoteDesktopAsync()
    {
        return await RunCommandCheckAsync(
            "reg",
            "query \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Terminal Server\" /v fDenyTSConnections",
            "0x1",
            "Remote Desktop disabled"
        );
    }

    private async Task<SecurityCheckResult> RunCommandCheckAsync(
        string command,
        string arguments,
        string expectedOutput,
        string description)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = command,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            if (process == null)
            {
                return new SecurityCheckResult
                {
                    CheckName = description,
                    Passed = false,
                    Message = $"Failed to start {command}"
                };
            }

            var output = await process.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            var passed = string.IsNullOrEmpty(expectedOutput) || output.Contains(expectedOutput);

            return new SecurityCheckResult
            {
                CheckName = description,
                Passed = passed,
                Message = passed ? "Check passed" : "Check failed",
                Details = output + error
            };
        }
        catch (Exception ex)
        {
            return new SecurityCheckResult
            {
                CheckName = description,
                Passed = false,
                Message = $"Error running check: {ex.Message}"
            };
        }
    }
}
