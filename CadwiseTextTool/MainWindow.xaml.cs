using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using CadwiseTextTool.TextProcessor;
using CadwiseTextTool.UI_Items;
using Microsoft.Win32;

namespace CadwiseTextTool;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private TextContainer _sourceTextContainer;
    private TextContainer _targetTextContainer;
    private ITextRefiner _textRefiner = new TextRefiner();

    public string MinWordLenght
    {
        get => _textRefiner.MinWordLength.ToString();
        set
        {
            if (Regex.IsMatch(value, "[^0-9]+"))
                return;
            
            int.TryParse(value, out var minWordLenght);
            _textRefiner.MinWordLength = minWordLenght;
            PreviewRefinement(this, null);
        }
    }

    public MainWindow()
    {
        InitializeComponent();

        ComplexWordCheck.DataContext = _textRefiner;
        UseHyphensCheck.DataContext = _textRefiner;
        UseLeetCheck.DataContext = _textRefiner;
        RemovePunctuationCheck.DataContext = _textRefiner;
        MinLengthTextBox.DataContext = this;

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var codepages = Encoding.GetEncodings().ToList().OrderBy(x => (int)x.CodePage);
        
        foreach(var codepage in codepages)
            CodePagesSelector.Items.Add(
                new 
                {
                    Key = codepage.Name, 
                    Value = $"{codepage.CodePage} - {codepage.DisplayName}"
                }
            );

        CodePagesSelector.DisplayMemberPath = "Value";
        CodePagesSelector.SelectedValuePath = "Key";

        CodePagesSelector.SelectedValue = "windows-1251";

        _sourceTextContainer = new TextContainer(SourceTextGrid);
        _targetTextContainer = new TextContainer(TargetTextGrid);

        _sourceTextContainer.TextBox.Text = Properties.Resources.ExampleText;
        _sourceTextContainer.TextBox.TextChanged += PreviewRefinement;
        
        PreviewRefinement(this, null);
    }

    private void PreviewRefinement(object sender, RoutedEventArgs e)
    {
        _targetTextContainer.TextBox.Clear();
        
        var lines = _sourceTextContainer.TextBox.Text.Split(Environment.NewLine);

        var result = new StringBuilder(_sourceTextContainer.TextBox.Text.Length);

        foreach (var line in lines)
            result.AppendLine(_textRefiner.RefineOneString(line));
        
        _targetTextContainer.TextBox.AppendText(result.ToString());
    }

    private async void FileRefinement(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
        };

        if (openFileDialog.ShowDialog() != true) 
            return;

        UIPanel.IsEnabled = false;
        StartTextRefinementButton.IsEnabled = false;
        ProgressBar.Visibility = Visibility.Visible;
        ProgressBar.IsIndeterminate = true;
        
        var fileRefiner = new FileRefiner(_textRefiner, openFileDialog.FileName)
        {
            FileEncoding = Encoding.GetEncoding(
                CodePagesSelector.SelectedValue.ToString() ?? "windows-1251")
        };
        
        try
        {
            _ = await fileRefiner.RefineOneFile();

            using StreamReader sourceFileStream = new(openFileDialog.FileName, fileRefiner.FileEncoding);

            var text = new StringBuilder(1024); 
            var buffer = new char[1024];
            var size = sourceFileStream.ReadBlock(buffer);
                text.Append(buffer);

            if (size >= 1024)
            {
                text.AppendLine("");
                text.AppendLine("...First 1024 bytes of text");
            }

            _sourceTextContainer.TextBox.Clear();
            _sourceTextContainer.TextBox.AppendText(text.ToString());

            PreviewRefinement(this, null);
        }
        finally
        {
            ProgressBar.Visibility = Visibility.Hidden;
            ProgressBar.IsIndeterminate = false;
            StartTextRefinementButton.IsEnabled = true;
            UIPanel.IsEnabled = true;
        }
    }
}
