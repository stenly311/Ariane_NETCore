using System.Drawing;

namespace Ariane.Types
{
    public class RPoint
    {
        public RPoint(int count, string label, Color color)
        {
            Count = count;
            Label = label;
            Color = color;
        }
        public int Count { get; set; }
        public string Label { get; set; }
        public Color Color { get; set; }
    }
}
