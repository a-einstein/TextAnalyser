using System.Windows;
using TextAnalyser.ViewModels;

namespace TextAnalyser.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
