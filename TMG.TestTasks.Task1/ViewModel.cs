using System.Collections.ObjectModel;
using TMG.TestTasks.Task1.MVVM;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using TMG.TestTasks.Task1.Interfaces;

namespace TMG.TestTasks.Task1
{
    class ViewModel : INPCBase
    {
        public ObservableCollection<ProcessedText> Items { get; } = new();
        private bool _isLoading;

        private bool IsLoading
        {
            get { return _isLoading; }
            set { _isLoading = value; OnPropertyChanged(); }
        }


        public AsyncCommand ProcessCommand { get; }

        private readonly IRichTextBoxDriver _richTextBoxDriver;
        private readonly ILogger _logger;
        private static readonly HttpClient _client = new();

        public ViewModel(IRichTextBoxDriver richTextBoxDriver, ILogger logger)
        {
            _richTextBoxDriver = richTextBoxDriver;
            _logger = logger;
            ProcessCommand = new(ProcessCommandBackend, () => !IsLoading);

            _client.DefaultRequestHeaders.Add("TMG-Api-Key", "0J/RgNC40LLQtdGC0LjQutC4IQ==");
            _client.BaseAddress = new("https://tmgwebtest.azurewebsites.net/api/textstrings/");
            _client.Timeout = TimeSpan.FromSeconds(1);
        }

        private async Task ProcessCommandBackend()
        {
            IsLoading = true;
            try
            {

                var tokens = ParseIds(_richTextBoxDriver.GetText(), _logger);

                _richTextBoxDriver.ClearText();

                foreach (var token in tokens)
                    _richTextBoxDriver.AppendText($"{token.Value};", token.IsCorrect ? Color.White : Color.Red);

                if (!tokens.Any(c => c.IsCorrect))
                    return;

                Items.Clear();

                foreach (var token in tokens.Where(c => c.IsCorrect).GroupBy(c => c.Value).Select(c => c.First()))
                {
                    var (Result, Text) = await TryGetItem(token, _logger);
                    if (!Result)
                        continue;

                    Items.Add(Text);
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        private static async Task<(bool Result, ProcessedText Text)> TryGetItem(Token token, ILogger logger)
        {
            try
            {
                logger.AddLine(GetMessageForToken(token.Value, "инициализация запроса"), Color.Black);
                var request = new HttpRequestMessage(HttpMethod.Get, token.Value)
                {
                    Method = HttpMethod.Get
                };

                var stream = await _client.SendAsync(request);
                var resp = await JsonSerializer.DeserializeAsync<Responce>(await stream.Content.ReadAsStreamAsync());
                if (string.IsNullOrEmpty(resp?.text))
                    logger.AddLine(GetMessageForToken(token.Value, "запрос завершился с неизвестной ошибкой"), Color.Black);
                else
                    logger.AddLine(GetMessageForToken(token.Value, "запрос успешно завершен"), Color.Black);

                return new(true, new(token.Value, resp.text));
            }
            catch (Exception ex)
            {
                logger.AddLine(GetMessageForToken(token.Value, $"запрос завершен с ошибкой - {ex.Message}."), Color.Red);
                return new(false, default);
            }

        }

        private static IEnumerable<Token> ParseIds(string text, ILogger logger)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            var rawIds = text.Split(',', ';');
            var result = new List<Token>();
            int end = 0, start = 0;
            bool isCorrect;
            foreach (var rawId in rawIds)
            {
                if (string.IsNullOrEmpty(rawId))
                {
                    start += 1;
                    end += 1;
                    continue;
                }

                if (rawId == "\r\n")
                {
                    start += 2;
                    end += 2;
                    continue;

                }

                isCorrect = false;
                var len = rawId.Length;
                end += len + 1;
                var value = rawId.Replace(" ", "");
                isCorrect = int.TryParse(value, out int id);
                if (isCorrect)
                {
                    if (id is not (>= 1 and <= 20))
                    {
                        isCorrect = false;
                        logger.AddLine(GetMessageForToken(rawId, "меньше нуля или больше 20"), Color.Red);
                    }
                    else
                    {
                        logger.AddLine(GetMessageForToken(rawId, "обработан успешно"), Color.Black);
                    }
                }
                else
                {
                    logger.AddLine(GetMessageForToken(rawId, "не является числом"), Color.Red);
                }

                result.Add(new(start, end - 1, rawId, isCorrect));
                start += len + 1;
            }

            return result;
        }

        private static string GetMessageForToken(string token, string msg)
        {
            return string.Join("", "Идентификатор \"", token, "\": ", msg, ".");
        }

        private readonly struct Token
        {
            public Token(int start, int end, string value, bool isCorrect)
            {
                Start = start;
                End = end;
                Value = value;
                IsCorrect = isCorrect;
            }

            public int Start { get; }
            public int End { get; }
            public string Value { get; }
            public bool IsCorrect { get; }
            public int Length => End - Start;
            public override string ToString() => $"{Start}-{End} {Value} {IsCorrect}";
        }

        private class Responce
        {
            public string text { get; set; }
        }
    }

}
