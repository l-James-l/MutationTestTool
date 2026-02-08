using System.Windows;

namespace GUI.Services;

/// <inheritdoc/>
public class DialogService : IDarwingDialogService
{
    /// <inheritdoc/>
    public void ErrorDialog(string title, string message)
    {
        MessageBox.Show(
            message,
            title,
            MessageBoxButton.OK,
            MessageBoxImage.Error);
    }

    /// <inheritdoc/>
    public void InfoDialog(string title, string message)
    {
        MessageBox.Show(
            message,
            title,
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }
}
