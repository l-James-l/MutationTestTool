namespace Core;


public interface ISolutionPathProvidedAwaiter
{
    string NotifySolutionPathProvided(string path);
}

public class SolutionPathProvidedAwaiter: ISolutionPathProvidedAwaiter
{
    private readonly IEventAggregator _eventAggregator;

    public SolutionPathProvidedAwaiter(IEventAggregator eventAggregator)
    {
        Console.WriteLine("SolutionPathProvidedAwaiter created.");
        
        _eventAggregator = eventAggregator;
    }

    public string NotifySolutionPathProvided(string path)
    {
        return $"Solution path provided: {path}";
    }
}
