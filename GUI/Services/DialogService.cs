using System.Windows;

namespace GUI.Services;

public class DialogService : IDarwingDialogService
{
    public void ErrorDialog(string title, string message)
    {
        MessageBox.Show(
            message,
            title,
            MessageBoxButton.OK,
            MessageBoxImage.Error);
    }

    public void InfoDialog(string title, string message)
    {
        MessageBox.Show(
            message,
            title,
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }
}
