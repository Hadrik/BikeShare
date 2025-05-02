using System.Windows;
using BikeShare.Desktop.ViewModels;

namespace BikeShare.Desktop;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindowView : Window
{
    public MainWindowView()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}