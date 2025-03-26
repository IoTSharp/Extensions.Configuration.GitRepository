using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;


namespace Extensions.Configuration.GitRepository
{
    internal sealed class JsonConfigurationFileParser
    {
        private readonly Dictionary<string, string> _data = new Dictionary<string, string>();

        private readonly Stack<string> _paths = new Stack<string>();

        private JsonConfigurationFileParser()
        {
        }

        public static IDictionary<string, string?> Parse(JsonDocument input)
        {
            return new JsonConfigurationFileParser().ParseJson(input);
        }

        private Dictionary<string, string?> ParseJson(JsonDocument jsonDocument)
        {
            JsonDocumentOptions jsonDocumentOptions = default(JsonDocumentOptions);
            jsonDocumentOptions.CommentHandling = JsonCommentHandling.Skip;
            jsonDocumentOptions.AllowTrailingCommas = true;
            JsonDocumentOptions options = jsonDocumentOptions;
            if (jsonDocument.RootElement.ValueKind != JsonValueKind.Object)
            {
                throw new FormatException($"Error invalid top level JSON Element  {jsonDocument.RootElement.ValueKind.ToString()}");
            }

            VisitObjectElement(jsonDocument.RootElement);

            return _data;
        }

        private void VisitObjectElement(JsonElement element)
        {
            bool nullIfElementIsEmpty = true;
            foreach (JsonProperty item in element.EnumerateObject())
            {
                nullIfElementIsEmpty = false;
                EnterContext(item.Name);
                VisitValue(item.Value);
                ExitContext();
            }

            SetNullIfElementIsEmpty(nullIfElementIsEmpty);
        }

        private void VisitArrayElement(JsonElement element)
        {
            int num = 0;
            foreach (JsonElement item in element.EnumerateArray())
            {
                EnterContext(num.ToString());
                VisitValue(item);
                ExitContext();
                num++;
            }

            SetNullIfElementIsEmpty(num == 0);
        }

        private void SetNullIfElementIsEmpty(bool isEmpty)
        {
            if (isEmpty && _paths.Count > 0)
            {
                _data[_paths.Peek()] = null;
            }
        }

        private void VisitValue(JsonElement value)
        {
            switch (value.ValueKind)
            {
                case JsonValueKind.Object:
                    VisitObjectElement(value);
                    break;
                case JsonValueKind.Array:
                    VisitArrayElement(value);
                    break;
                case JsonValueKind.String:
                case JsonValueKind.Number:
                case JsonValueKind.True:
                case JsonValueKind.False:
                case JsonValueKind.Null:
                    {
                        string text = _paths.Peek();
                        if (_data.ContainsKey(text))
                        {
                            throw new FormatException($"Error, Key is duplicated {text}");
                        }

                        _data[text] = value.ToString();
                        break;
                    }
                default:
                    throw new FormatException($"Error, Unsupported   {value.ValueKind}");
            }
        }

        private void EnterContext(string context)
        {
            _paths.Push((_paths.Count > 0) ? (_paths.Peek() + ConfigurationPath.KeyDelimiter + context) : context);
        }

        private void ExitContext()
        {
            _paths.Pop();
        }
    }
}
