using CLI;
using Core.IndustrialEstate;
using Core.Interfaces;
using Models;
using Models.Events;
using NSubstitute;

namespace CLITests;

public class CLIAppTests
{
    private CLIApp _app; //SUT

    private IEventAggregator _eventAggregator;
    private IMutationSettings _mutationSettings;
    private ISolutionProvider _solutionProvider;
    private ICancelationTokenFactory _cancelationTokenFactory;

    private TextReader _originalIn;
    private SolutionPathProvidedEvent _solutionPathProvided;

    [SetUp]
    public void SetUp()
    {
        _originalIn = Console.In;
        _eventAggregator = Substitute.For<IEventAggregator>();
        _mutationSettings = Substitute.For<IMutationSettings>();
        _solutionProvider = Substitute.For<ISolutionProvider>();
        _cancelationTokenFactory = Substitute.For<ICancelationTokenFactory>();

        _solutionPathProvided = Substitute.For<SolutionPathProvidedEvent>();

        _solutionProvider.IsAvailable.Returns(false);

        _eventAggregator.GetEvent<SolutionPathProvidedEvent>().Returns(_solutionPathProvided);

        _app = new CLIApp(_eventAggregator, _mutationSettings, _solutionProvider, _cancelationTokenFactory);

         //Limit testing to a single run through.
        _solutionPathProvided.When(x => x.Publish(Arg.Any<SolutionPathProvidedPayload>()))
            .Do(_ => _solutionProvider.IsAvailable.Returns(true));
    }

    [TearDown]
    public void TearDown()
    {
        Console.SetIn(_originalIn);
    }

    [Test]
    public void GivenUserProvidesSolutionPath_WhenRun_ThenPublishesThatPath()
    {
        // Arrange
        const string providedPath = "C:\\temp\\MySolution.sln";
        Console.SetIn(new StringReader(providedPath + Environment.NewLine));

        // Act
        _app.Run(Array.Empty<string>());

        // Assert
        _solutionPathProvided.Received(1).Publish(Arg.Is<SolutionPathProvidedPayload>(x => x.SolutionPath == providedPath));
    }

    [Test]
    public void GivenNoInputAndDevArg_WhenRun_ThenPublishesDevPath()
    {
        // Arrange
        // Simulate user pressing enter (empty input)
        Console.SetIn(new StringReader(Environment.NewLine));

        var args = new[] { "--dev" };

        // Act
        _app.Run(args);

        // Assert
        _solutionPathProvided.Received(1)
            .Publish(Arg.Is<SolutionPathProvidedPayload>(x => x.SolutionPath == @"C:\Users\THINKPAD\Documents\git\SimpleTestProject\SimpleTestProject.sln"));
    }

    [Test]
    public void GivenNoInputAndNoDevArg_WhenRun_ThenPublishesEmptyPath()
    {
        // Arrange
        Console.SetIn(new StringReader(Environment.NewLine));

        // Act
        _app.Run(Array.Empty<string>());

        // Assert
        _solutionPathProvided.Received(1).Publish(Arg.Is<SolutionPathProvidedPayload>(x => x.SolutionPath == ""));
    }

    [Test]
    public void GivenSlnPathInArgs_WhenRun_ThenPublishesThatPath()
    {
        // Arrange
        const string argPath = "C:\\temp\\ArgSolution.sln";
        var args = new[] { "--sln", argPath };
        
        // Act
        _app.Run(args);
        
        // Assert
        _solutionPathProvided.Received(1).Publish(Arg.Is<SolutionPathProvidedPayload>(x => x.SolutionPath == argPath));
    }
}
