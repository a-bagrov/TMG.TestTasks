using System;
using System.Linq;

namespace TMG.TestTasks.Task1
{
    class ProcessedText
    {
        public ProcessedText(string id, string text)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
            Id = id;
            WordsCount = text.Count(c => c == ' ');
            VowelsCount = text.ToLowerInvariant().Count("aeiouауоыиэяюёеäöü".Contains);
        }

        public string Id { get; }
        public string Text { get; }
        public int WordsCount { get; }
        public int VowelsCount { get; }
    }
}
