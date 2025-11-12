using Core.Interfaces;
using Models;
using Models.Events;
using Serilog;

namespace CLI;

public class CLIApp
{
    private readonly IEventAggregator _eventAggregator;
    private readonly IMutationSettings _mutationSettings;
    private readonly ISolutionProvider _solutionProvider;

    public CLIApp(IEventAggregator eventAggregator, IMutationSettings mutationSettings, ISolutionProvider solutionProvider)
    {
        ArgumentNullException.ThrowIfNull(eventAggregator);
        ArgumentNullException.ThrowIfNull(mutationSettings);
        ArgumentNullException.ThrowIfNull(solutionProvider);

        _eventAggregator = eventAggregator;
        _mutationSettings = mutationSettings;
        _solutionProvider = solutionProvider;
    }

    public void Run(string[] args)
    {
        Log.Information("Running Darwing CLI application...");

        _mutationSettings.ParseCliArgs(args);

        while (!_solutionProvider.IsAvailable)
        {
            SolutionLoaderLoop();
        }
    }

    private void SolutionLoaderLoop()
    {
        if (string.IsNullOrWhiteSpace(_mutationSettings.SolutionPath))
        {
            Console.Write("\nProvide path to an '.sln' file:    ");

            string? path = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(path) && _mutationSettings.DevMode)
            {
                //We are in dev mode, so we can use a test path if one wasnt provided.
                path = @"C:\Users\THINKPAD\Documents\git\SimpleTestProject\SimpleTestProject.sln";
            }

            // If a path wasnt provided, and we are not in dev mode, the handler for the published event wont load a solution,
            // and the user will be prompted again.
            _mutationSettings.SolutionPath = path ?? "";
        }

        // We pulish the event with the sln path in the payload, rather than relying on the mutation settings directly, so that
        // in the instance a new sln path is provided later, we can handle that too.
        _eventAggregator.GetEvent<SolutionPathProvided>().Publish(new SolutionPathProvidedPayload(_mutationSettings.SolutionPath));
    }
}