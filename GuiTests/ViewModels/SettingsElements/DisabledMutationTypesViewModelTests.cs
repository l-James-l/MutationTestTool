using GUI.ViewModels.SettingsElements;
using Models;
using Models.Enums;
using Models.Events;
using Mutator.MutationImplementations;
using NSubstitute;

namespace GuiTests.ViewModels.SettingsElements;

public class DisabledMutationTypesViewModelTests
{
    private IMutationSettings _settings;
    private IEventAggregator _eventAggregator;
    private Action<DarwingOperation> _onRefreshAction = default!;
    private List<IMutationImplementation> _implementedMutations;
    private DisabledMutationTypesViewModel _sut;

    [SetUp]
    public void Setup()
    {
        _settings = Substitute.For<IMutationSettings>();
        _eventAggregator = Substitute.For<IEventAggregator>();

        // Setup internal list for the mock settings
        _settings.DisabledMutationTypes.Returns(new List<SpecificMutation>());

        // Setup dummy mutations
        var mut1 = Substitute.For<IMutationImplementation>();
        mut1.Category.Returns(MutationCategory.Arithmetic);
        mut1.Mutation.Returns(SpecificMutation.AddToSubtract);

        var mut2 = Substitute.For<IMutationImplementation>();
        mut2.Category.Returns(MutationCategory.Logical);
        mut2.Mutation.Returns(SpecificMutation.SubtractToAdd);

        _implementedMutations = new List<IMutationImplementation> { mut1, mut2 };

        // Capture EventAggregator subscription
        var refreshEvent = Substitute.For<DarwingOperationStatesChangedEvent>();
        _eventAggregator.GetEvent<DarwingOperationStatesChangedEvent>().Returns(refreshEvent);

        refreshEvent.When(x => x.Subscribe(
            Arg.Any<Action<DarwingOperation>>(),
            Arg.Any<ThreadOption>(),
            Arg.Any<bool>(),
            Arg.Any<Predicate<DarwingOperation>>()))
            .Do(info => _onRefreshAction = info.Arg<Action<DarwingOperation>>());

        _sut = new DisabledMutationTypesViewModel(_implementedMutations, _settings, _eventAggregator);
    }

    [Test]
    public void GivenMutationsExist_WhenViewModelCreated_ThenCategoriesAreGroupedCorrectly()
    {
        // Assert
        // We expect 2 categories based on our setup (Arithmetic and Logical)
        Assert.That(_sut.MutationCategories, Has.Count.EqualTo(2));
    }

    [Test]
    public void GivenSpecificMutation_WhenEnabledToggled_ThenSettingsAreUpdated()
    {
        // Arrange
        var category = _sut.MutationCategories.First();
        var mutationVm = category.Mutations.First();
        var mutationType = mutationVm.Mutation;

        // Act - Disable it
        mutationVm.Enabled = false;

        // Assert
        Assert.That(_settings.DisabledMutationTypes, Contains.Item(mutationType));

        // Act - Re-enable it
        mutationVm.Enabled = true;

        // Assert
        Assert.That(_settings.DisabledMutationTypes, Does.Not.Contain(mutationType));
    }

    [Test]
    public void GivenCategory_WhenDisableAllExecuted_ThenAllMutationsInSettingsAreDisabled()
    {
        // Arrange
        var category = _sut.MutationCategories.First();

        // Act
        category.DisableAllCommand.Execute();

        // Assert
        foreach (var mut in category.Mutations)
        {
            Assert.That(_settings.DisabledMutationTypes, Contains.Item(mut.Mutation));
        }
    }

    [Test]
    public void GivenSettingsChangedExternally_WhenRefreshEventFired_ThenViewModelSyncs()
    {
        // Arrange
        var category = _sut.MutationCategories.First();
        var mutationVm = category.Mutations.First();

        // Simulate external change (adding to disabled list)
        _settings.DisabledMutationTypes.Add(mutationVm.Mutation);

        // Act
        _onRefreshAction.Invoke(DarwingOperation.LoadSolution);

        // Assert
        Assert.That(mutationVm.Enabled, Is.False);
    }
}