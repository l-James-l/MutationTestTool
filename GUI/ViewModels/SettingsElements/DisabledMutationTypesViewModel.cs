using Models;
using Models.Enums;
using Models.Events;
using Mutator.MutationImplementations;
using System.Collections.ObjectModel;

namespace GUI.ViewModels.SettingsElements;


/// <summary>
/// This view model is responsible for managing the settings related to the disabled mutation types. 
/// It groups the implemented mutations by their category and allows the user to enable or disable specific mutations,
/// as well as entire categories of mutations at once.
/// 
/// All the classes for this pattern are in this file, as they are closely related and not used anywhere else in the codebase. 
/// The main class is the DisabledMutationTypesViewModel, which contains a collection of MutationTypeCategoryViewModel,
/// each representing a category of mutations. Each MutationTypeCategoryViewModel contains a collection of SpecificMutationViewModel,
/// representing the specific mutations within that category.
/// </summary>
public class DisabledMutationTypesViewModel : ViewModelBase
{
    private readonly IMutationSettings _settings;

    public DisabledMutationTypesViewModel(IEnumerable<IMutationImplementation> implementedMutations, IMutationSettings settings, 
        IEventAggregator eventAggregator)
    {
        IEnumerable<IGrouping<MutationCategory, IMutationImplementation>> categories = implementedMutations.GroupBy(m => m.Category);
        Dictionary<MutationCategory, IEnumerable<SpecificMutation>> dictionary = categories.ToDictionary(key => key.Key, value => value.Select(x => x.Mutation));

        foreach ((MutationCategory category, IEnumerable<SpecificMutation> specificMutations) in dictionary)
        {
            MutationCategories.Add(new MutationTypeCategoryViewModel(category, specificMutations, settings));
        }

        eventAggregator.GetEvent<DarwingOperationStatesChangedEvent>().Subscribe(_ => RefreshStates(), ThreadOption.UIThread, true, x => x is DarwingOperation.LoadSolution);
        _settings = settings;
    }

    private void RefreshStates()
    {
        foreach (MutationTypeCategoryViewModel category in MutationCategories)
        {
            foreach (SpecificMutationViewModel mutation in category.Mutations)
            {
                mutation.Enabled = !_settings.DisabledMutationTypes.Contains(mutation.Mutation);
            }
        }
    }

    /// <summary>
    /// Binding property for the categories of mutations. Each category contains the specific mutations that belong to it.
    /// </summary>
    public ObservableCollection<MutationTypeCategoryViewModel> MutationCategories { get; set; } = [];
}

/// <summary>
/// A view model representing a category of mutations, containing the specific mutations that belong to that category.
/// </summary>
public class MutationTypeCategoryViewModel : ViewModelBase
{
    public MutationTypeCategoryViewModel(MutationCategory category, IEnumerable<SpecificMutation> mutations, IMutationSettings settings)
    {
        Name = category.ToReadableString();
        Description = category.GetDescription();

        foreach (SpecificMutation mutation in mutations)
        {
            Mutations.Add(new SpecificMutationViewModel(mutation, settings));
        }

        EnableAllCommand = new DelegateCommand(EnableAll);
        DisableAllCommand = new DelegateCommand(DisableAll);
    }

    /// <summary>
    /// Binding property for the name of the category
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Binding property for the description of the category.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Binding property for the specific mutations that belong to this category. Each specific mutation can be enabled or disabled individually.
    /// </summary>
    public ObservableCollection<SpecificMutationViewModel> Mutations { get; set; } = [];

    /// <summary>
    /// Binding command for enabling all mutations in this category at once.
    /// </summary>
    public DelegateCommand EnableAllCommand { get; }

    private void EnableAll()
    {
        foreach (SpecificMutationViewModel mutationVm in Mutations)
        {
            mutationVm.Enabled = true;
        }
    }

    /// <summary>
    /// Binding command for disabling all mutations in this category at once.
    /// </summary>
    public DelegateCommand DisableAllCommand { get; }

    private void DisableAll()
    {
        foreach (SpecificMutationViewModel mutationVm in Mutations)
        {
            mutationVm.Enabled = false;
        }
    }
}

/// <summary>
/// VM representing a specific mutation type, which can be enabled or disabled by the user.
/// It also contains the name and description of the mutation for display purposes in the GUI.
/// </summary>
public class SpecificMutationViewModel : ViewModelBase
{
    public SpecificMutation Mutation { get; }
    private readonly IMutationSettings _settings;

    public SpecificMutationViewModel(SpecificMutation mutation, IMutationSettings settings)
    {
        _settings = settings;
        Mutation = mutation;
        Name = mutation.ToReadableString();
        Description = mutation.GetDescription();
        Enabled = !settings.DisabledMutationTypes.Contains(mutation);
    }

    /// <summary>
    /// Binding property for the name of the specific mutation.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Binding property for the description of the specific mutation, which is retrieved from the MutationDescription attribute of the mutation type.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Binding property for the enabled state of the specific mutation.
    /// When this property is set, it updates the mutation settings accordingly, 
    /// adding or removing the mutation from the list of disabled mutation types.
    /// </summary>
    public bool Enabled
    {
        get;
        set
        {
            SetProperty(ref field, value);
            if (value && _settings.DisabledMutationTypes.Contains(Mutation))
            {
                _settings.DisabledMutationTypes.Remove(Mutation);
            }
            else if (!value && !_settings.DisabledMutationTypes.Contains(Mutation))
            {
                _settings.DisabledMutationTypes.Add(Mutation);
            }
        }
    }
}
