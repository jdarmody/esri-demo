using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

#nullable disable

namespace EsriDemo.Core.Converters
{
    public class HtmlConverter : IValueConverter
    {
        private Color LinkColor { set; get; }

        Color GetParameter(object parameter)
        {
            if (parameter is Color)
                return (Color)parameter;

            return Colors.Blue;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            LinkColor = GetParameter(parameter);

            Label label = null;  // label is used to set the FontSize for Bold and Italic
            if (parameter is Label)
            {
                label = (Label)parameter;
            }

            var formatted = new FormattedString();

            foreach (var item in ProcessString((string)value))
            {
                if (item.Type == "b")
                {
                    formatted.Spans.Add(CreateBoldSpan(item, label));
                }
                else
                if (item.Type == "i")
                {
                    formatted.Spans.Add(CreateItalicSpan(item, label));
                }
                else
                {
                    formatted.Spans.Add(CreateAnchorSpan(item));
                }
            }

            return formatted;
        }

        private Span CreateAnchorSpan(StringSection section)
        {
            var span = new Span()
            {
                Text = section.Text
            };

            if (!string.IsNullOrEmpty(section.Link))
            {
                span.GestureRecognizers.Add(new TapGestureRecognizer()
                {
                    Command = _navigationCommand,
                    CommandParameter = section.Link
                });
                span.TextColor = LinkColor;
                // Underline coming soon from https://github.com/xamarin/Xamarin.Forms/pull/2221
                // Currently available in Nightly builds if you wanted to try, it does work :)
                // As of 2018-07-22. But not avail in 3.2.0-pre1.
                span.TextDecorations = TextDecorations.Underline;
            }

            return span;
        }

        private Span CreateBoldSpan(StringSection section, Label label)
        {
            var span = new Span()
            {
                Text = section.Text,
                FontAttributes = FontAttributes.Bold
            };
            if (label != null)
            {
                span.FontSize = label.FontSize;
            }

            return span;
        }

        private Span CreateItalicSpan(StringSection section, Label label)
        {
            var span = new Span()
            {
                Text = section.Text,
                FontAttributes = FontAttributes.Italic
            };
            if (label != null)
            {
                span.FontSize = label.FontSize;
            }

            return span;
        }

        public IList<StringSection> ProcessString(string rawText)
        {
            rawText = System.Net.WebUtility.HtmlDecode(rawText);
            rawText = rawText.Replace("<br>", "\n");
            rawText = rawText.Replace("<br/>", "\n");
            rawText = rawText.Replace("<br />", "\n");
            rawText = rawText.Replace("<p>", "\n");
            rawText = rawText.Replace("</p>", "\n");
            const string spanPattern = @"(<[abi].*?>.*?</[abi]>)";

            MatchCollection collection = Regex.Matches(rawText, spanPattern, RegexOptions.Singleline);

            var sections = new List<StringSection>();

            var lastIndex = 0;

            foreach (Match item in collection)
            {
                var foundText = item.Value;
                sections.Add(new StringSection() { Text = rawText.Substring(lastIndex, item.Index - lastIndex) });
                lastIndex = item.Index + item.Length;

                var html = new StringSection()
                {
                    Link = Regex.Match(item.Value, "(?<=href=\\\")[\\S]+(?=\\\")").Value,
                    Text = Regex.Replace(item.Value, "<.*?>", string.Empty),
                    Type = item.Value.Substring(1, 1)
                };

                sections.Add(html);
            }

            sections.Add(new StringSection() { Text = rawText.Substring(lastIndex) });

            return sections;
        }

        public class StringSection
        {
            public string Text { get; set; }
            public string Link { get; set; }
            public string Type { get; set; }
        }

        private ICommand _navigationCommand = new AsyncRelayCommand<string>(async (url) =>
        {
            Uri uriResult;
            Uri.TryCreate(url, UriKind.Absolute, out uriResult);

            if (uriResult == null)
                return;

            await Launcher.OpenAsync(uriResult);
        });

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}