using Core.Startup;
using Core;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Main entry point for the CLI application.
/// </summary>
public class Program
{
    public static void Main(string[] args)
    {
        //Create a new container to pass to the DependencyRegistrar

        new DependencyRegistrar(new ServiceCollection());        
    }
}