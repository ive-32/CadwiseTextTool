using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CadwiseTextTool.TextProcessor;

public class TextRefiner : ITextRefiner
{
    public int MinWordLength { get; set; } = 5;

    public bool RemovePunctuation { get; set; } = false;

    public bool ComplexWordsAsSingle { get; set; } = true;

    public bool UseHyphens { get; set; } = true;

    public bool UseLeet { 
        get => wordCharPositive == @"\w"; 
        set => wordCharPositive = value ? @"\w" : @"\p{L}";
    } 

    private string regexPunctuation = @"\B\p{P}|\p{P}\B";
    private string regexHyphen = @"\b\w+-\s*$";
    
    private string wordCharPositive = @"\p{L}";

    private StringBuilder hyphenPart = new(20); // average max longest word for ordinary text 
    private StringBuilder wordCharExpression = new();
    private StringBuilder splitByWordsPattern = new();
    private List<Match> wordsList = new List<Match>();

    public string RefineTextBlock(string sourceText)
    {
        var result = new StringBuilder(sourceText.Length);
        var lines = sourceText.Split(Environment.NewLine);
        foreach (var line in lines)
        {
            result.AppendLine(RefineOneString(line));
        }

        return result.ToString();
    }
    
    public string RefineOneString(string sourceText)
    {
        // split by words, regex in general
        // (?'prefix'[^\w\-\']*)(?'word'[\w\-\']+)(?'suffix'[^\w\-\']*) 
        wordCharExpression.Clear().Append(ComplexWordsAsSingle ? @$"{wordCharPositive}\-\'" : @$"{wordCharPositive}");
        
        splitByWordsPattern.Clear().Append(
            @$"(?'prefix'[^{wordCharExpression}]*)(?'word'[{wordCharExpression}]+)(?'suffix'[^{wordCharExpression}]*)"); 
        
        wordsList.Clear();
        wordsList = Regex.Matches(sourceText, splitByWordsPattern.ToString()).ToList();

        if (!wordsList.Any())
            return "";

        var workString = new StringBuilder(sourceText.Length);
        
        //one word line case
        if (wordsList.Count == 1)
        {
            if (UseHyphens)
            {
                if (wordsList[^1].Groups["word"].Value[^1] == '-')
                    hyphenPart.Append(wordsList[^1].Groups["word"].Value[..^1]);
                else
                {
                    CompileWordWithHyphenPart(workString, wordsList[0], hyphenPart);
                    hyphenPart.Clear();
                }
            }
            else
                CompileOneWord(workString, wordsList[0].Groups["prefix"].Value, wordsList[0].Groups["word"].Value,
                    wordsList[0].Groups["suffix"].Value);
            
            return workString.ToString();
        }
            
        //compile first word using hyphens
        if (UseHyphens)
        {   
            // add hyphenPart before first word 
            CompileWordWithHyphenPart(workString, wordsList[0], hyphenPart);
            hyphenPart.Clear();
        }
        else
            CompileOneWord(workString, wordsList[0].Groups["prefix"].Value, wordsList[0].Groups["word"].Value, 
                wordsList[0].Groups["suffix"].Value);
            
        //compile second and ^2 words in line
        for (var i = 1; i < wordsList.Count - 1; i++)
            CompileOneWord(workString, wordsList[i].Groups["prefix"].Value, wordsList[i].Groups["word"].Value, 
                wordsList[i].Groups["suffix"].Value);
        
        //compile last word using hyphens
        //check what hyphen is present and add it to hypenPart or compile as word
        
        if (UseHyphens && wordsList[^1].Groups["word"].Value[^1] == '-')
            hyphenPart.Append(wordsList[^1].Groups["word"].Value[..^1]);
        else 
            CompileOneWord(workString, wordsList[^1].Groups["prefix"].Value, wordsList[^1].Groups["word"].Value, 
                wordsList[^1].Groups["suffix"].Value);
            
        return workString.ToString();
    }

    public void CompileOneWord(StringBuilder result, string prefix, string word, string suffix, string hyphen = "")
    {
        result.Append(RemovePunctuation ? Regex.Replace(prefix, regexPunctuation, "") : prefix);

        if (UseHyphens && !string.IsNullOrWhiteSpace(hyphen))
        {
            if (GetWordLenght(word) + GetWordLenght(hyphen) >= MinWordLength)
                result.Append(hyphen).Append(word);
        }
        else
            if (GetWordLenght(word) >= MinWordLength)
                result.Append(hyphen).Append(word);

        result.Append(RemovePunctuation ? Regex.Replace(suffix, regexPunctuation, "") : suffix);
    }
    
    private int GetWordLenght(string word)
        => ComplexWordsAsSingle ? word.Length - Regex.Matches(word, @"[\-\']").Count : word.Length;

    private void CompileWordWithHyphenPart(StringBuilder workString, Match wordGroup, StringBuilder hyphenPart)
    {
        if (Regex.IsMatch(wordGroup.Groups["prefix"].Value, @"\s*"))
        {
            CompileOneWord(workString, wordGroup.Groups["prefix"].Value, wordGroup.Groups["word"].Value,
                wordGroup.Groups["suffix"].Value, hyphenPart.ToString());
        }
        else
        {
            // if prefix of first word is not \s, compile hyphenPart as stand alone word 
            CompileOneWord(workString, wordGroup.Groups["prefix"].Value, hyphenPart.ToString(), " ");
            CompileOneWord(workString, "", wordGroup.Groups["word"].Value, wordGroup.Groups["suffix"].Value);
        }
    }
}
