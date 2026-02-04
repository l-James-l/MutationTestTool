using System.Windows;
using System.Windows.Controls;

namespace GUI.Views;
/// <summary>
/// Interaction logic for SolutionExplorerView.xaml
/// </summary>
public partial class SolutionExplorerView : UserControl
{
    public SolutionExplorerView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// This is used to stop the DataGrid from trying to scroll when a row is selected.
    /// </summary>
    private void DataGridRow_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
    {
        // Setting Handled to true stops the DataGrid from 
        // trying to scroll the row into a specific position.
        e.Handled = true;
    }
}
