using GUI.ViewModels.SettingsElements;

namespace GUI.ViewModels;

public interface ISettingsViewModel
{

}

public class SettingsViewModel : ViewModelBase, ISettingsViewModel
{
    public SettingsViewModel(ProjectTypeCollectionSettings projectTypeSettings, GeneralSettingsViewModel generalSettingsViewModel,
        DisabledMutationTypesViewModel disabledMutationTypesViewModel)
    {
        ProjectTypeSettings = projectTypeSettings;
        GeneralSettingsViewModel = generalSettingsViewModel;
        DisabledMutationTypesViewModel = disabledMutationTypesViewModel;
    }

    public ProjectTypeCollectionSettings ProjectTypeSettings { get; }
    public GeneralSettingsViewModel GeneralSettingsViewModel { get; }
    public DisabledMutationTypesViewModel DisabledMutationTypesViewModel { get; }
}

