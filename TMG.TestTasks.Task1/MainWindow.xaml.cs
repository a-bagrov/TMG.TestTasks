using System.Windows;
using TMG.TestTasks.Task1.Implementation;

namespace TMG.TestTasks.Task1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var rtb = new RichTextBoxDriver(RichTextBox);
            var logger = new Logger(Log);
            var vm = new ViewModel(rtb, logger);
            DataContext = vm;
        }
    }
}
