using CadwiseTextTool.TextProcessor;

namespace CadwiseTextToolTest;

public class CadwiseTextToolTest
{
    [Theory]
    [InlineData("что-то, когда-то"," когда-то\r\n", 6)]
    [InlineData("пе-\r\nре-\r\nнос","\r\n\r\nперенос\r\n", 6)]
    [InlineData("короткое сло-\r\nво","короткое \r\n\r\n", 6)]
    [InlineData("пе-\r\nре-\r\n  нос с отступом","\r\n\r\n  перенос  отступом\r\n", 6)]
    [InlineData("l33t 0bv10usly"," 0bv10usly\r\n", 6, true, true, true, true)]
    [InlineData("не обрабатываем пере-\r\nносы "," обрабатываем \r\n \r\n", 6, true, true, false, false)]
    [InlineData("проба, пунктуации",", пунктуации\r\n", 6, false)]
    [InlineData("удаляем слова когда-то","удаляем  -\r\n", 6, false, false)]
    [InlineData("","\r\n", 6, false, false)]
    public void CheckRefineOneStringWords(string src, string dst, int minLenght,
        bool removePunctuation = true, bool complexWordAsSingle = true, 
        bool useHyphens = true, bool useLeet = false)
    {
        var textRefiner = new TextRefiner
        {
            MinWordLength = minLenght,
            RemovePunctuation = removePunctuation,
            ComplexWordsAsSingle = complexWordAsSingle,
            UseHyphens = useHyphens,
            UseLeet = useLeet
        };

        var result = textRefiner.RefineTextBlock(src);
        Assert.Equal(result, dst);
    }
}