namespace GUI.Services;

/// <summary>
/// Service that allows the prism GUI to show a console window for the user to view logs and other information during the mutation testing process.
/// Closing the console will be disabled to prevent the user from accidentally closing the console and crashing the entire application. 
/// Instead, the user can toggle the console visibility via a button in the GUI.
/// </summary>
public interface IConsoleService
{
    void ToggleConsoleVisable();
}