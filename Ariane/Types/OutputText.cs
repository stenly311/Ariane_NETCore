using Ariane.Common.Types;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Ariane.Types
{
    public class OutputText : ListViewItem
    {
        Regex _errorFailRegex = new Regex(@"\b(?:fail|exception|error)\b", RegexOptions.IgnoreCase);
        Regex _successRegex = new Regex(@"\b(?:success)\b", RegexOptions.IgnoreCase);

        public OutputText(string text)
        {   
            Init(text);
        }

        private void Init(string text)
        {
            Content = text;
            if (text != null)
            {
                TextType = _successRegex.IsMatch(text)
                    ? TextTypeEnum.Success : _errorFailRegex.IsMatch(text)
                    ? TextTypeEnum.Error : TextTypeEnum.Text;
            }
            else
            {
                TextType = TextTypeEnum.Text;
            }

            switch (TextType)
            {
                case TextTypeEnum.Error:
                    Foreground = Brushes.Yellow;
                    FontWeight = FontWeights.Bold;
                    FontSize = FontSize + 2;
                    break;
                case TextTypeEnum.Success:
                    Foreground = Brushes.Green;
                    FontWeight = FontWeights.Bold;
                    FontSize = FontSize + 2;
                    break;
                default:
                    Foreground = Brushes.White;
                    FontWeight = FontWeights.Normal;
                    break;
            }
        }

        public string Text
        {
            get
            {
                return Content.ToString();
            }
        }

        public TextTypeEnum TextType { get; private set; }
    }
}
