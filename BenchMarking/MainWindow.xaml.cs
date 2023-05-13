using System.Windows;
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

        private void BenchMark(object sender, RoutedEventArgs e)
        {
            var res = BenchmarkRunner.Run<TextRefinementTest>();
            
            Results.Navigate(res.ResultsDirectoryPath + "/BenchMarking.TextRefinementTest-report.html");
        }

    }
}