using System.Text;
using System.IO;
using System;
using System.Threading.Tasks;

namespace CadwiseTextTool.TextProcessor;

public class FileRefiner
{
    private int blockSize = 1024;

    public string SourceFileName { get; set; } = "";

    public string TargetFileName { get; set; } = "";

    public Encoding FileEncoding { get; set; } = Encoding.UTF8;

    ITextRefiner TextRefiner { get; set; }

    public FileRefiner(ITextRefiner textRefiner, string sourceFileName)
    {
        TextRefiner = new TextRefiner
        {
            MinWordLength = textRefiner.MinWordLength,
            RemovePunctuation = textRefiner.RemovePunctuation,
            ComplexWordsAsSingle = textRefiner.ComplexWordsAsSingle,
            UseHyphens = textRefiner.UseHyphens,
            UseLeet = textRefiner.UseLeet
        };
        SourceFileName = sourceFileName;
    }

    public async Task<bool> RefineOneFile() 
    {
        if (TextRefiner is null)
            return false;

        if (string.IsNullOrEmpty(SourceFileName) || !File.Exists(SourceFileName))
            return false;

        if (string.IsNullOrEmpty(TargetFileName))
            TargetFileName = Path.ChangeExtension(SourceFileName, " (Refinement)" + Path.GetExtension(SourceFileName));

        using StreamReader sourceFileStream = new(SourceFileName, FileEncoding);
        await using StreamWriter targetFileStream = new(TargetFileName, false, FileEncoding);

        var tail = new StringBuilder(blockSize);
        
        var buffer = new char[blockSize];
        var sourceText = new StringBuilder(blockSize);
        
        await Task.Delay(5000);
        
        while (await sourceFileStream.ReadAsync(buffer, tail.Length, blockSize - tail.Length) > 0)
        {
            sourceText.Clear().Append(buffer);
            var splitBySpace = sourceText.ToString().LastIndexOf(' ');
            tail.Clear();
            if (splitBySpace > 0)
            {
                tail.Append(sourceText.ToString()[(splitBySpace + 1) .. blockSize]);
                sourceText.Length = blockSize - tail.Length;
            }

            var refinedLine = TextRefiner.RefineTextBlock(sourceText.ToString());
            await targetFileStream.WriteAsync(refinedLine);
            
            buffer.AsSpan().Fill((char)0);
            tail.CopyTo(0, buffer, tail.Length);
        }
        
        if (tail.Length >0)
            await targetFileStream.WriteAsync(TextRefiner.RefineTextBlock(tail.ToString()));

        return true;
    }
}
