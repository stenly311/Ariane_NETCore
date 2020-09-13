using System;

namespace Ariane.Attributes
{
    public class Visible : Attribute
    {
        public Visible(bool isVisible)
        {
            IsVisible = isVisible;
        }
        public bool IsVisible { get; set; }
    }
}
