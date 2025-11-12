namespace Core.IndustrialEstate;

public class CancelationTokenFactory : ICancelationTokenFactory
{
    public CancellationTokenSource Generate() => new CancellationTokenSource();
}

public interface ICancelationTokenFactory
{
    /// <summary>
    /// DI factory for cancelation tokens.
    /// </summary>
    /// <returns></returns>
    CancellationTokenSource Generate();
}