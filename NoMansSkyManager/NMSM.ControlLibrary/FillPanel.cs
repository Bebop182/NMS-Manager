using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace NMSM.ControlLibrary
{
    public class FillPanel : StackPanel
    {
        static FillPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FillPanel), new FrameworkPropertyMetadata(typeof(FillPanel)));
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            double offset = 0;
            var isVertical = Orientation == Orientation.Vertical;
            foreach (UIElement child in Children)
            {
                var childPosition = new Point()
                {
                    X = isVertical ? 0 : offset,
                    Y = isVertical ? offset : 0
                };

                var childSize = new Size()
                {
                    Height = isVertical ? child.DesiredSize.Height : arrangeSize.Height,
                    Width = isVertical ? arrangeSize.Width : child.DesiredSize.Width
                };
                child.Arrange(new Rect(childPosition, childSize));
                offset += isVertical ? childSize.Height : childSize.Width;
            }
            return arrangeSize;
        }
    }
}
