using System.Linq;
using System.Text.RegularExpressions;

namespace BenchMarking;

public class TextRefinerOld 
{
    public int MinWordLength { get; set; } = 5;

    public bool RemovePunctuation { get; set; } = false;

    public bool ComplexWordsAsSingle 
    { 
        get => complexWordPart != "";
        set => complexWordPart = value ? @"|\b(?:-|')\b" : ""; 
    } 

    public bool UseHyphens { get; set; } = true;

    public bool UseLeet { 
        get => wordCharPositive == @"\w"; 
        set
        {
            wordCharPositive = value ? @"\w" : @"\p{L}";
            wordCharNegative = value ? @"\W" : @"\P{L}";
        }
    } 

    private string regexWordsMoreThan = "";
    private string regexPunctuation = @"\B\p{P}|\p{P}\B";
    private string regexHyphen = @"\b\w+-\s*$";
    
    private string wordCharPositive = @"\p{L}";
    private string wordCharNegative = @"\P{L}";
    private string complexWordPart = @"|\b(?:-|')\b";


    private string hyphenPart = "";

    public string RefineOneString(string sourceText)
    {
        // Regex in general "((\w|\b(-|')\b){5,})?(\W*)" 
        regexWordsMoreThan = $@"(?:({wordCharPositive}{complexWordPart}){{{MinWordLength},}})?" +
                             $@"(?:{wordCharNegative}*)";

        // Compile hyphens, link previous wordpart if exist to current line
        if (UseHyphens)
        {
            if (!string.IsNullOrEmpty(hyphenPart))
            {
                if (Regex.IsMatch(sourceText, @"^\s*\w"))
                {
                    var prefix = Regex.Match(sourceText, @"^\s*\b").Value ?? "";
                    sourceText = Regex.Replace(sourceText, @"^\s*\b", prefix + hyphenPart);
                }
                else
                    sourceText = hyphenPart + " " + sourceText;
            }

            var hyphen = Regex.Match(sourceText, regexHyphen);

            if (hyphen.Success)
            {
                hyphenPart = hyphen?.Value ?? "";
                hyphenPart = Regex.Replace(hyphenPart, @"-\s*$", "");
                sourceText = Regex.Replace(sourceText, regexHyphen, "");
            }
            else
                hyphenPart = "";
        }

        // Select long words and non letter symbols
        var tempResult = string.Join("", Regex.Matches(sourceText, regexWordsMoreThan).Select(s => s.Value));

        //Check complex words for real letters count
        if (ComplexWordsAsSingle)
        {
            var words = Regex
                .Matches(tempResult, $@"({wordCharPositive}{complexWordPart})+")
                .Select(x => x.Value).ToList();

            foreach (var word in words)
            {
                var cleanWord = Regex.Replace(word, wordCharNegative, "");
                if (cleanWord.Length < MinWordLength)
                {
                    tempResult = tempResult.Replace(word, "");
                }
            }
        }

        // Replace punctuation
        if (RemovePunctuation)
            tempResult = Regex.Replace(tempResult, regexPunctuation, "");

        return tempResult;
    }
}
