using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
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

    public MainWindow()
    {
        InitializeComponent();

        ComplexWordCheck.DataContext = _textRefiner;
        UseHyphensCheck.DataContext = _textRefiner;
        UseLeetCheck.DataContext = _textRefiner;
        RemovePunctuationCheck.DataContext = _textRefiner;

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

    private void ValidationLengthTextBox(object sender, TextCompositionEventArgs e)
    {
        var regexOnlyDigits = new Regex("[^0-9]+");
        e.Handled = regexOnlyDigits.IsMatch(e.Text);

        if (e.Handled)
            return;

        if (!int.TryParse(e.Text, out var minWordLenght))
            minWordLenght = 1;

        _textRefiner.MinWordLength = minWordLenght;

        PreviewRefinement(sender, e);
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

    private void FileRefinement(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
        };

        if (openFileDialog.ShowDialog() != true) 
            return;
            
        var fileRefiner = new FileRefiner(_textRefiner, openFileDialog.FileName)
        {
            FileEncoding = Encoding.GetEncoding(
                CodePagesSelector.SelectedValue.ToString() ?? "windows-1251")
        };

        fileRefiner.RefineOneFile();

        
        using StreamReader sourceFileStream = new(openFileDialog.FileName, fileRefiner.FileEncoding);
        
        string? sourceLine;

        var i = 50;

        var text = new StringBuilder(120 * 50); // let say string lenght 120 character and 50 lines total
        while ((sourceLine = sourceFileStream.ReadLine()) != null && i-- > 0)
            text.AppendLine(sourceLine);

        text.AppendLine("...First 50 lines of text");
        _sourceTextContainer.TextBox.Clear();
        _sourceTextContainer.TextBox.AppendText(text.ToString());

        PreviewRefinement(this, null);
    }
}
