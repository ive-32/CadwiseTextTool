using System.Windows.Controls;

namespace CadwiseTextTool.UI_Items
{
    public class TextContainer
    {
        public TextBox TextBox { get; set; } = new TextBox();

        private Grid grid;

        public TextContainer(Grid _grid, int _row = 0, int _column = 0) 
        {
            grid = _grid;
            TextBox.SetValue(Grid.RowProperty, _row);
            TextBox.SetValue(Grid.ColumnProperty, _column);
            TextBox.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            TextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            TextBox.AcceptsReturn = true; 
            TextBox.AcceptsTab = true;
            grid.Children.Add(TextBox);
        }

    }
}
