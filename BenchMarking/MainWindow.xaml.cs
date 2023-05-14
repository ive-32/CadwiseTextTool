using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace BenchMarking
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void BenchMark(object sender, RoutedEventArgs e)
        {
            var progress = new ProgressBar();
            progress.IsIndeterminate = true;
            progress.SetValue(Grid.RowProperty, 0);
            progress.Height = 20;
            progress.Margin = new Thickness(20, 0, 20, 0);
            MainGrid.Children.Add(progress);
            StartTest.Visibility = Visibility.Hidden;
            

            var res = await Task.Run(() => BenchmarkRunner.Run<TextRefinementTest>()); 
            
            MainGrid.Children.Remove(progress);
            StartTest.Visibility = Visibility.Visible;
            Results.Navigate(res.ResultsDirectoryPath + "/BenchMarking.TextRefinementTest-report.html");
        }

        
    }
}