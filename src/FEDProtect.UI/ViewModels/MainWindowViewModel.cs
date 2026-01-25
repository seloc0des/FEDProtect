using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using FEDProtect.Core.Models;
using FEDProtect.Core.Services;
using ReactiveUI;

namespace FEDProtect.UI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IProfileManager _profileManager;
    private readonly IHardeningApplier _hardeningApplier;
    private readonly IOsChecker _osChecker;
    private readonly IOutputFormatter _outputFormatter;

    private string _selectedProfile = string.Empty;
    private ThreatProfile? _currentProfile;
    private string _statusMessage = "Ready";
    private bool _isLoading;
    private string _profileDescription = string.Empty;
    private string _profileVersion = string.Empty;
    private string _profileLastUpdated = string.Empty;
    private string _profileTags = string.Empty;
    private Capability? _selectedCapability;
    private string _errorMessage = string.Empty;
    private bool _hasError;
    
    public Func<Task<IStorageFile?>>? SaveFileDialogHandler { get; set; }
    public Func<Task<IStorageFolder?>>? SelectFolderDialogHandler { get; set; }
    public Action<string, string>? ShowErrorDialogHandler { get; set; }

    public MainWindowViewModel(
        IProfileManager profileManager,
        IHardeningApplier hardeningApplier,
        IOsChecker osChecker,
        IOutputFormatter outputFormatter)
    {
        _profileManager = profileManager;
        _hardeningApplier = hardeningApplier;
        _osChecker = osChecker;
        _outputFormatter = outputFormatter;

        AvailableProfiles = new ObservableCollection<string>();
        Capabilities = new ObservableCollection<Capability>();
        SecurityCheckResults = new ObservableCollection<SecurityCheckResult>();
        Mitigations = new ObservableCollection<Mitigation>();

        LoadProfilesCommand = ReactiveCommand.CreateFromTask(LoadProfilesAsync);
        LoadProfileCommand = ReactiveCommand.CreateFromTask(LoadProfileAsync);
        GenerateChecklistCommand = ReactiveCommand.CreateFromTask(GenerateChecklistAsync);
        CreateBrowserProfileCommand = ReactiveCommand.CreateFromTask(CreateBrowserProfileAsync);
        RunSecurityChecksCommand = ReactiveCommand.CreateFromTask(RunSecurityChecksAsync);
        DismissErrorCommand = ReactiveCommand.Create(DismissError);

        LoadProfilesCommand.Execute().Subscribe();
    }

    public ObservableCollection<string> AvailableProfiles { get; }
    public ObservableCollection<Capability> Capabilities { get; }
    public ObservableCollection<SecurityCheckResult> SecurityCheckResults { get; }
    public ObservableCollection<Mitigation> Mitigations { get; }

    public string SelectedProfile
    {
        get => _selectedProfile;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedProfile, value);
            if (!string.IsNullOrEmpty(value))
            {
                LoadProfileCommand.Execute().Subscribe();
            }
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => this.RaiseAndSetIfChanged(ref _statusMessage, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }

    public string ProfileDescription
    {
        get => _profileDescription;
        set => this.RaiseAndSetIfChanged(ref _profileDescription, value);
    }

    public string ProfileVersion
    {
        get => _profileVersion;
        set => this.RaiseAndSetIfChanged(ref _profileVersion, value);
    }

    public string ProfileLastUpdated
    {
        get => _profileLastUpdated;
        set => this.RaiseAndSetIfChanged(ref _profileLastUpdated, value);
    }

    public string ProfileTags
    {
        get => _profileTags;
        set => this.RaiseAndSetIfChanged(ref _profileTags, value);
    }

    public Capability? SelectedCapability
    {
        get => _selectedCapability;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedCapability, value);
            UpdateMitigations();
        }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }

    public bool HasError
    {
        get => _hasError;
        set => this.RaiseAndSetIfChanged(ref _hasError, value);
    }

    public ReactiveCommand<Unit, Unit> LoadProfilesCommand { get; }
    public ReactiveCommand<Unit, Unit> LoadProfileCommand { get; }
    public ReactiveCommand<Unit, Unit> GenerateChecklistCommand { get; }
    public ReactiveCommand<Unit, Unit> CreateBrowserProfileCommand { get; }
    public ReactiveCommand<Unit, Unit> RunSecurityChecksCommand { get; }
    public ReactiveCommand<Unit, Unit> DismissErrorCommand { get; }

    private void UpdateMitigations()
    {
        Mitigations.Clear();
        if (_selectedCapability?.Mitigations != null)
        {
            foreach (var mitigation in _selectedCapability.Mitigations)
            {
                Mitigations.Add(mitigation);
            }
        }
    }

    private void ShowError(string message)
    {
        ErrorMessage = message;
        HasError = true;
        ShowErrorDialogHandler?.Invoke("Error", message);
    }

    private void DismissError()
    {
        HasError = false;
        ErrorMessage = string.Empty;
    }

    private async Task LoadProfilesAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Loading profiles...";

            var profiles = await _profileManager.ListAvailableProfilesAsync();
            AvailableProfiles.Clear();
            
            foreach (var profile in profiles)
            {
                AvailableProfiles.Add(profile);
            }

            StatusMessage = $"Loaded {profiles.Count} profile(s)";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading profiles: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadProfileAsync()
    {
        if (string.IsNullOrEmpty(SelectedProfile))
            return;

        try
        {
            IsLoading = true;
            StatusMessage = $"Loading {SelectedProfile}...";

            _currentProfile = await _profileManager.LoadProfileAsync(SelectedProfile);
            
            Capabilities.Clear();
            foreach (var capability in _currentProfile.Capabilities)
            {
                Capabilities.Add(capability);
            }

            ProfileDescription = _currentProfile.Description;
            ProfileVersion = $"Version: {_currentProfile.Version}";
            ProfileLastUpdated = $"Updated: {_currentProfile.LastUpdated:yyyy-MM-dd}";
            ProfileTags = _currentProfile.Tags?.Count > 0 
                ? string.Join(", ", _currentProfile.Tags) 
                : string.Empty;

            StatusMessage = $"Loaded: {_currentProfile.Name}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task GenerateChecklistAsync()
    {
        if (_currentProfile == null)
        {
            StatusMessage = "Please select a profile first";
            return;
        }

        try
        {
            IsLoading = true;
            StatusMessage = "Generating checklist...";

            var report = await _hardeningApplier.GenerateChecklistAsync(_currentProfile);
            var markdown = _outputFormatter.FormatAsMarkdown(report, _currentProfile);

            string outputPath;
            if (SaveFileDialogHandler != null)
            {
                var file = await SaveFileDialogHandler();
                if (file == null)
                {
                    StatusMessage = "Save cancelled";
                    return;
                }
                outputPath = file.Path.LocalPath;
            }
            else
            {
                outputPath = $"{SelectedProfile}-checklist.md";
            }
            
            await File.WriteAllTextAsync(outputPath, markdown);
            StatusMessage = $"Checklist saved to: {outputPath}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task CreateBrowserProfileAsync()
    {
        if (_currentProfile == null)
        {
            StatusMessage = "Please select a profile first";
            return;
        }

        try
        {
            IsLoading = true;
            StatusMessage = "Creating Firefox profile...";

            string outputDir = ".";
            if (SelectFolderDialogHandler != null)
            {
                var folder = await SelectFolderDialogHandler();
                if (folder == null)
                {
                    StatusMessage = "Save cancelled";
                    return;
                }
                outputDir = folder.Path.LocalPath;
            }
            
            var outputPath = await _hardeningApplier.GenerateFirefoxConfigAsync(_currentProfile, outputDir);
            StatusMessage = $"Firefox profile saved to: {outputPath}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task RunSecurityChecksAsync()
    {
        if (_currentProfile == null)
        {
            StatusMessage = "Please select a profile first";
            return;
        }

        try
        {
            IsLoading = true;
            StatusMessage = "Running security checks...";

            var results = await _osChecker.RunAllChecksAsync(_currentProfile);
            
            SecurityCheckResults.Clear();
            foreach (var result in results)
            {
                SecurityCheckResults.Add(result);
            }

            var passed = results.Count(r => r.Passed);
            StatusMessage = $"Checks complete: {passed}/{results.Count} passed";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
