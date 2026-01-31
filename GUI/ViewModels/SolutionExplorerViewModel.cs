using GUI.Services;
using GUI.ViewModels.SolutionExplorerElements;
using System.Collections.ObjectModel;
using System.IO;

namespace GUI.ViewModels;

public interface ISolutionExplorerViewModel
{

}

public class SolutionExplorerViewModel : ViewModelBase, ISolutionExplorerViewModel
{
    public SolutionExplorerViewModel(FileExplorerViewModel fileExplorerViewModel)
    {
        FileExplorerViewModel = fileExplorerViewModel;

        fileExplorerViewModel.SelectedFileChangedCallBack += OnSelectedFileChanged;
    }

    public ObservableCollection<LineDetails> FileDetails { get; } = [];

    private void OnSelectedFileChanged(string newFilePath)
    {
        FileDetails.Clear();
        if (File.Exists(newFilePath))
        {
            IEnumerable<string> lines = File.ReadLines(newFilePath);
            List<LineDetails> lineDetails = [.. lines.Select((line, index) => new LineDetails
            {
                SourceCode = line,
                LineNumber = index + 1
            })];

            FileDetails.AddRange(lineDetails);
        }
    }

    public FileExplorerViewModel FileExplorerViewModel { get; }
}

public class LineDetails
{
    public string SourceCode { get; set; }

    public int LineNumber { get; set; }
}