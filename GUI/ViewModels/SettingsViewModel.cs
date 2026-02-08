using GUI.ViewModels.SettingsElements;
using Models;
using Serilog;
using System.IO;
using System.Windows;
using YamlDotNet.Serialization;

namespace GUI.ViewModels;

public interface ISettingsViewModel
{

}

public class SettingsViewModel : ViewModelBase, ISettingsViewModel
{
    private readonly IMutationSettings _settings;

    public SettingsViewModel(ProjectTypeCollectionSettings projectTypeSettings, GeneralSettingsViewModel generalSettingsViewModel,
        DisabledMutationTypesViewModel disabledMutationTypesViewModel, IMutationSettings settings)
    {
        _settings = settings;
        
        ProjectTypeSettings = projectTypeSettings;
        GeneralSettingsViewModel = generalSettingsViewModel;
        DisabledMutationTypesViewModel = disabledMutationTypesViewModel;

        SaveSettingsCommand = new DelegateCommand(SaveProfile);
    }

    public ProjectTypeCollectionSettings ProjectTypeSettings { get; }
    public GeneralSettingsViewModel GeneralSettingsViewModel { get; }
    public DisabledMutationTypesViewModel DisabledMutationTypesViewModel { get; }

    public DelegateCommand SaveSettingsCommand { get; }

    public void SaveProfile()
    {
        if (string.IsNullOrEmpty(_settings.SolutionPath))
        {
            Log.Error("No solution file path specified. Solution profile could not be saved.");
            MessageBox.Show(
                "No solution file path specified. Solution profile could not be saved.",
                "Save Failed",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            return;
        }

        string? directory = Path.GetDirectoryName(_settings.SolutionPath);
        if (directory == null)
        {
            Log.Error("Failed to determine solution directory from solution file path. Solution profile could not be saved.");
            MessageBox.Show(
                "Failed to determine solution directory from solution file path. Solution profile could not be saved.",
                "Save Failed",
                MessageBoxButton.OK, 
                MessageBoxImage.Error);
            return;
        }

        SolutionProfileData profileData = BuildNewProfileObject();
        var serializer = new SerializerBuilder().Build();
        string ymlContent = serializer.Serialize(profileData);

        string path = Path.Combine(directory, ".darwingSolutionProfile.yml");
        File.WriteAllText(path, ymlContent);

        MessageBox.Show($"Profile saved to file location: '{path}'", "Save Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private SolutionProfileData BuildNewProfileObject()
    {
        SolutionProfileData profileData = new SolutionProfileData
        {
            TestProjects = _settings.TestProjects,
            IgnoreProjects = _settings.IgnoreProjects,
            SourceCodeProjects = _settings.SourceCodeProjects,
            DisabledMutationTypes = _settings.DisabledMutationTypes,
            SingleMutantPerLine = _settings.SingleMutantPerLine,
            BuildTimeout = _settings.BuildTimeout,
            TestRunTimeout = _settings.TestRunTimeout,
            SkipTestingNoActiveMutants = _settings.SkipTestingNoActiveMutants
        };

        return profileData;
    }
}

