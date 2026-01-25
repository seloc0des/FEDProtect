using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using FEDProtect.UI.ViewModels;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace FEDProtect.UI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        DataContextChanged += (_, _) =>
        {
            if (DataContext is MainWindowViewModel vm)
            {
                vm.SaveFileDialogHandler = ShowSaveFileDialogAsync;
                vm.SelectFolderDialogHandler = ShowSelectFolderDialogAsync;
            }
        };
    }

    private async Task<IStorageFile?> ShowSaveFileDialogAsync()
    {
        var options = new FilePickerSaveOptions
        {
            Title = "Save Checklist",
            SuggestedFileName = "checklist.md",
            FileTypeChoices = new[]
            {
                new FilePickerFileType("Markdown") { Patterns = new[] { "*.md" } },
                new FilePickerFileType("All Files") { Patterns = new[] { "*" } }
            }
        };
        
        return await StorageProvider.SaveFilePickerAsync(options);
    }

    private async Task<IStorageFolder?> ShowSelectFolderDialogAsync()
    {
        var options = new FolderPickerOpenOptions
        {
            Title = "Select Output Folder",
            AllowMultiple = false
        };
        
        var result = await StorageProvider.OpenFolderPickerAsync(options);
        return result.Count > 0 ? result[0] : null;
    }
}

public class SeverityColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string severity)
        {
            return severity.ToLower() switch
            {
                "high" => new SolidColorBrush(Color.Parse("#D32F2F")),
                "medium" => new SolidColorBrush(Color.Parse("#F57C00")),
                "low" => new SolidColorBrush(Color.Parse("#1976D2")),
                _ => new SolidColorBrush(Color.Parse("#666666"))
            };
        }
        return new SolidColorBrush(Color.Parse("#666666"));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class PassFailConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool passed)
        {
            // Return appropriate type based on target
            if (targetType == typeof(Color) || targetType == typeof(IBrush))
            {
                return passed ? Color.Parse("#4CAF50") : Color.Parse("#F44336");
            }
            return passed ? "PASS" : "FAIL";
        }
        return targetType == typeof(Color) ? Color.Parse("#666666") : "?";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
