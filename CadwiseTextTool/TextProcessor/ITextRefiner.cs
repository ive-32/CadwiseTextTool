namespace CadwiseTextTool.TextProcessor;

public interface ITextRefiner
{
    bool ComplexWordsAsSingle { get; set; }

    int MinWordLength { get; set; }

    public bool UseHyphens { get; set; }

    public bool UseLeet { get; set; } 

    public string RefineOneString(string sourceText);
}