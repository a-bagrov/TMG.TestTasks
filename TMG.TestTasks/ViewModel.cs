using TMG.TestTasks.MVVM;
using System.Collections.ObjectModel;
namespace TMG.Task1
{
    class ViewModel : INPCBase
    {
        ObservableCollection<ProcessedText> Items { get; } = new();

    }
}
