using System;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using TMG.TestTasks.Task1.Interfaces;

namespace TMG.TestTasks.Task1.Implementation
{
    class RichTextBoxDriver : IRichTextBoxDriver
    {
        private readonly RichTextBox _rtb;
        private readonly Brush _whiteBrush = Brushes.White;
        private readonly Brush _redBrush = Brushes.Red;

        public RichTextBoxDriver(RichTextBox rtb)
        {
            _rtb = rtb;
            _rtb.PreviewKeyDown += Rtb_KeyDown;
            _rtb.Document.MinPageWidth = 1000000;
        }

        public string GetText() =>
            new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentEnd).Text;

        public void AppendText(string s, Color textColor)
        {
            var range = new TextRange(_rtb.Document.ContentEnd, _rtb.Document.ContentEnd);
            range.Text = s;
            range.ApplyPropertyValue(TextElement.BackgroundProperty, TextColorToBrush(textColor));
        }

        public void ClearText()
        {
            _rtb.Document.Blocks.Clear();
        }

        private void Rtb_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    e.Handled = true;
                    return;
            }
        }

        private Brush TextColorToBrush(Color textColor)
        {
            return textColor switch
            {
                Color.Red => _redBrush,
                Color.White => _whiteBrush,
                _ => throw new ArgumentException("Unknown text color.", nameof(textColor)),
            };
        }
    }
}
