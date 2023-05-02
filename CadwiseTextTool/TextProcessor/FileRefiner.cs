using System.Text;
using System.IO;
using System.Collections.Generic;

namespace CadwiseTextTool.TextProcessor;

public class FileRefiner
{
    public string SourceFileName { get; set; } = "";

    public string TargetFileName { get; set; } = "";

    public Encoding FileEncoding { get; set; } = Encoding.UTF8;

    ITextRefiner TextRefiner { get; set; }

    public FileRefiner(ITextRefiner textRefiner, string sourceFileName)
    {
        TextRefiner = textRefiner;
        SourceFileName = sourceFileName;
    }

    public void RefineOneFile() 
    {
        if (TextRefiner is null)
            return;

        if (string.IsNullOrEmpty(SourceFileName) || !File.Exists(SourceFileName))
            return;

        if (string.IsNullOrEmpty(TargetFileName))
            TargetFileName = Path.ChangeExtension(SourceFileName, " (Refinement)" + Path.GetExtension(SourceFileName));

        using StreamReader sourceFileStream = new(SourceFileName, FileEncoding);
        using StreamWriter targetFileStream = new(TargetFileName, false, FileEncoding);

        string? sourceLine;

        while ((sourceLine = sourceFileStream.ReadLine()) != null)
        {
            var refinedLine = TextRefiner.RefineOneString(sourceLine);
            SaveRefinedLine(targetFileStream, refinedLine);
        }
    }

    private void SaveRefinedLine(StreamWriter streamWriter, string line)
    {
        streamWriter.WriteLine(line);
    }
}
