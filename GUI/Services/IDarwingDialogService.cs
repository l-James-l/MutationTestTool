namespace GUI.Services;

/// <summary>
/// Interface for the dialog service, which is used to show error and information dialogs to the user.
/// Simplifies showing dialogs from the view models, and means they dont show up in the unit tests.
/// </summary>
public interface IDarwingDialogService
{
    public void ErrorDialog(string title, string message);

    public void InfoDialog(string title, string message);
}
