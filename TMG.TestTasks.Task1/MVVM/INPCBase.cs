using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TMG.TestTasks.Task1.MVVM
{
    internal class INPCBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string caller = null) => 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
    }
}
