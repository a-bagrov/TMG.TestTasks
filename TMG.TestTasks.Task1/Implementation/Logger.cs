using System;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using TMG.TestTasks.Task1.Interfaces;

namespace TMG.TestTasks.Task1.Implementation
{
    class Logger : ILogger
    {
        private readonly RichTextBox _rtb;
        private readonly Brush _blackBrush = Brushes.Black;
        private readonly Brush _redBrush = Brushes.Red;

        public Logger(RichTextBox rtb)
        {
            _rtb = rtb;
            _rtb.Document.MinPageWidth = 100000;
        }

        public void AddLine(string line, Color textColor)
        {
            line = line.Replace("\r\n", "");
            _rtb.Document.Blocks.Add(CreatePara(line, TextColorToBrush(textColor)));
            _rtb.ScrollToEnd();
        }

        private Paragraph CreatePara(string s, Brush brush)
        {
            var para = new Paragraph();
            para.Inlines.Add(new Bold(new Run(GetTime())));
            para.Inlines.Add(new Run(s) { Foreground = brush });

            return para;
        }

        private string GetTime()
        {
            return DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss ");
        }

        private Brush TextColorToBrush(Color textColor)
        {
            return textColor switch
            {
                Color.Black => _blackBrush,
                Color.Red => _redBrush,
                _ => throw new ArgumentException("Unknown text color.", nameof(textColor)),
            };
        }

    }
}
