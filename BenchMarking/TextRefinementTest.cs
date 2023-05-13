using System;
using System.Text;

namespace BenchMarking;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using CadwiseTextTool.TextProcessor;

[MemoryDiagnoser]
public class TextRefinementTest
{
    [Benchmark]
    public void TestOldRefinement()
    {
        TextRefinerOld _textRefinerOld = new() 
        { 
            RemovePunctuation = true, 
            UseHyphens = true, 
            MinWordLength = 5, 
            ComplexWordsAsSingle = true, 
            UseLeet = false
        };
        var stringForTestByLine = CadwiseTextTool.Properties.Resources.ExampleText.Split(Environment.NewLine);
        
        foreach (var line in stringForTestByLine)
            _textRefinerOld.RefineOneString(line);
    }

    [Benchmark]
    public void TestNewRefinement()
    {
        TextRefiner _textRefiner = new() 
        { 
            RemovePunctuation = true, 
            UseHyphens = true, 
            MinWordLength = 5, 
            ComplexWordsAsSingle = true, 
            UseLeet = false
        };
        var stringForTestByLine = CadwiseTextTool.Properties.Resources.ExampleText.Split(Environment.NewLine);
        
        foreach (var line in stringForTestByLine)
            _textRefiner.RefineOneString(line);
    }
    
    [Benchmark]
    public void TestOneWordRefinement()
    {
        TextRefiner _textRefiner = new() 
        { 
            RemovePunctuation = true, 
            UseHyphens = true, 
            MinWordLength = 5, 
            ComplexWordsAsSingle = true, 
            UseLeet = false
        };
        var stringForTestByWords = CadwiseTextTool.Properties.Resources.ExampleText.Split(" ");
        StringBuilder stringBuilder = new(20);
        
        foreach (var line in stringForTestByWords)
            _textRefiner.CompileOneWord(stringBuilder,". ", line, ". ", "123");
    }

}