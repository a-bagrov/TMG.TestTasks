using System;
using System.Linq;

namespace TMG.Task1
{
    class ProcessedText
    {
        public ProcessedText(string text)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
            LinesCount = text.Count("\n".Contains);
            VowelsCount = text.ToLowerInvariant().Count("aeiou".Contains);
        }

        public string Text { get; }
        public int LinesCount { get; }
        public int VowelsCount { get; }
    }
}
