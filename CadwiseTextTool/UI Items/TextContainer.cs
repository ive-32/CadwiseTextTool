using System.Windows;
using System.Windows.Controls;

namespace CadwiseTextTool.UI_Items
{
    public class TextContainer
    {
        public TextBox TextBox { get; set; } = new TextBox();

        public TextContainer(Grid grid, int row = 0, int column = 0) 
        {
            TextBox.SetValue(Grid.RowProperty, row);
            TextBox.SetValue(Grid.ColumnProperty, column);
            TextBox.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            TextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            TextBox.AcceptsReturn = true; 
            TextBox.AcceptsTab = true;
            TextBox.TextWrapping = TextWrapping.Wrap;
            grid.Children.Add(TextBox);
        }
    }
}
