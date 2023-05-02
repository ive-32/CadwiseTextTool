﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CadwiseTextTool.TextProcessor;
using CadwiseTextTool.UI_Items;
using Microsoft.Win32;

namespace CadwiseTextTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TextContainer SourceTextContainer;
        private TextContainer TargetTextContainer;
        private ITextRefiner textRefiner = new TextRefiner();

        public MainWindow()
        {
            InitializeComponent();

            ComplexWordCheck.DataContext = textRefiner;
            UseHyphensCheck.DataContext = textRefiner;
            UseLeetCheck.DataContext = textRefiner;

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

            SourceTextContainer = new TextContainer(SourceTextGrid);
            TargetTextContainer = new TextContainer(TargetTextGrid);

            SourceTextContainer.TextBox.Text = Properties.Resources.ExampleText;
            SourceTextContainer.TextBox.TextChanged += PreviewRefinement;
            
            PreviewRefinement(this, null);
        }

        private void ValidationLengthTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regexOnlyDigits = new Regex("[^0-9]+");
            e.Handled = regexOnlyDigits.IsMatch(e.Text);

            if (e.Handled)
                return;

            int minWordLenght;
            if (!int.TryParse(e.Text, out minWordLenght))
                minWordLenght = 1;

            textRefiner.MinWordLength = minWordLenght;

            PreviewRefinement(sender, e);
        }

        private void PreviewRefinement(object sender, RoutedEventArgs e)
        {

            TargetTextContainer.TextBox.Clear();
            
            var lines = SourceTextContainer.TextBox.Text.Split(Environment.NewLine);

            var result = new StringBuilder(SourceTextContainer.TextBox.Text.Length);

            foreach (var line in lines)
                result.AppendLine(textRefiner.RefineOneString(line));
            
            TargetTextContainer.TextBox.AppendText(result.ToString());
        }

        private void FileRefinement(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() != true) 
                return;
                
            var fileRefiner = new FileRefiner(textRefiner, openFileDialog.FileName);

            fileRefiner.FileEncoding = Encoding.GetEncoding(
                CodePagesSelector.SelectedValue.ToString() ?? "windows-1251");

            fileRefiner.RefineOneFile();

            
            using StreamReader sourceFileStream = new(openFileDialog.FileName, fileRefiner.FileEncoding);
            
            string? sourceLine;

            var i = 50;

            var text = new StringBuilder(120 * 50); // let say string lenght 120 character and 50 lines total
            while ((sourceLine = sourceFileStream.ReadLine()) != null && i-- > 0)
            {
                text.AppendLine(sourceLine);
            }

            text.AppendLine("...First 50 lines of text");
            SourceTextContainer.TextBox.Clear();
            SourceTextContainer.TextBox.AppendText(text.ToString());

            PreviewRefinement(this, null);
        }
    }
}