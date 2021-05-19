using System;
using System.IO;
using System.Linq;

namespace TMG.TestTasks.Task3
{
    class Program
    {
        const string _engAlph = "abcdefghijklmnopqrstuvwxyz";
        const string _rusAlph = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";

        static void Main(string[] args)
        {
            if (args == null || args.Length != 2)
            {
                Console.WriteLine("Аргументы должны быть переданы в формате \"путь к файлу с строками на русском языке\", \"путь к файлу с строками на английском языке\".");
                Console.ReadKey();
                Environment.Exit(0);
            }

            var rusText = File.ReadAllLines(args[0].Remove(args[0].Length - 1));
            var enText = File.ReadAllLines(args[1])
                .Select(c => c.Split('|'));

            var rusIndexes = rusText.Select(c => new TextIndex(c, GetPetrenkoIndex(c)))
                .ToList();

            var enIndexes = enText.Where(c => c.Length == 2)
                .Select(c => new TextIndex(c[0], GetPetrenkoIndex(c[0]) + GetPetrenkoIndex(c[1]), c[1]))
                .ToList();

            var res = (from ru in rusIndexes
                       join en in enIndexes on ru.Index equals en.Index
                       select new { RuText = ru, EnText = en })
                      .ToList();

            Console.WriteLine("Обработанные русские строки:");
            foreach (var c in rusIndexes)
                Console.WriteLine(c);

            Console.WriteLine("\nОбработанные английские строки:");
            foreach (var c in enIndexes)
                Console.WriteLine(c);

            if (res.Count == 0)
            {
                Console.WriteLine("\nНе удалось найти строки с одинаковыми индексами.");
            }
            else
            {
                Console.WriteLine("\nНайдены строки с одинаковыми индексами.");

                foreach (var c in res)
                    Console.WriteLine($"Русская строка - {c.RuText}, английская строка - {c.EnText}");
            }

            Console.ReadKey();
        }

        static double GetPetrenkoIndex(string s)
        {
            if (string.IsNullOrEmpty(s) || string.IsNullOrWhiteSpace(s))
                return 0;

            var res = 0d;
            s = s.ToLowerInvariant();
            s = string.Join("", s.Split('@', ',', '.', ';', '\'', ' '));

            if (_engAlph.Contains(s[0]) || _rusAlph.Contains(s[0]))
                res += 0.5;

            for (int i = 1; i < s.Length; i++)
            {
                var c = s[i];
                if (_engAlph.Contains(c) || _rusAlph.Contains(c))
                    res += i + 0.5;
            }

            return res * s.Length;
        }

        private readonly struct TextIndex
        {
            public TextIndex(string text, double index, string comment = "")
            {
                Text = text;
                Index = index;
                Comment = comment;
            }

            public string Text { get; }
            public string Comment { get; }
            public double Index { get; }

            public override string ToString() => $"Index: {Index} | Text: {Text ?? ""} | Comment: {Comment ?? ""}";
        }
    }
}
