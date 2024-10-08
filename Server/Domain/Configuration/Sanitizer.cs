using Domain.Enums;
using Domain.Exceptions;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html;
using System.Text;
using Domain.Extensions;
using Ganss.XSS;

namespace Domain.Configuration
{
    public interface ISanitizer
    {
        object SanitizeAndRemoveHtml(object input);

        object SanitizeAndThrowExceptionIfHtml(object input);
    }

    /// <summary>
    /// Use this sanitizer if you don't want to clean anything (output == input).
    /// </summary>
    public class SanitizerNothing : ISanitizer
    {
        public object SanitizeAndRemoveHtml(object input)
        {
            return input;
        }

        public object SanitizeAndThrowExceptionIfHtml(object input)
        {
            return input;
        }
    }



    /// <summary>
    /// Standard sanitizer for my.quitt. Currently removes all HTML tags from input, but may be extended further.
    /// </summary>
    public class Sanitizer : ISanitizer
    {
        // https://stackoverflow.com/questions/9942751/asp-net-html-sanitization-library-adding-line-feed-after-256-characters/9973439
        static readonly string newlinemarker = Guid.NewGuid().ToString();

        private static readonly HtmlSanitizer sanitizer = new HtmlSanitizer(
            allowedTags: new List<string> { "b", "br" },
            allowedSchemes: new List<string> { "http", "https" },
            allowedAttributes: new List<string>(),
            uriAttributes: new List<string>(),
            allowedCssProperties: new List<string>()
        );

        /// <summary>
        /// Sanitizes the input by removing any HTML character (currently only strings, other objects are returned without modifications).
        /// </summary>
        public object SanitizeAndRemoveHtml(object input)
        {
            return VerifyAndCleanInput(input).inputCleaned;
        }

        /// <summary>
        /// Throws an exception if the input contains HTML
        /// </summary>
        public object SanitizeAndThrowExceptionIfHtml(object input)
        {
            var (inputIsClean, inputCleaned) = VerifyAndCleanInput(input);

            if (inputIsClean)
            {
                return inputCleaned;
            }
            else
            {
                throw new BaseException(ErrorCodeEnum.InputPotentiallyDangerous, ErrorCodeEnum.InputPotentiallyDangerous.GetDescription());
            }
        }

        private static (bool inputIsClean, object inputCleaned) VerifyAndCleanInput(object input)
        {
            if (input == null)
            {
                return (inputIsClean: true, inputCleaned: null);
            }

            if (input is string)
            {
                /*
                 * <br/> tags are changed slightly, so we have to standardize them first.
                 */
                var inputString = input
                    .ToString()
                    .Replace("<br/>", "<br>")
                    .Replace("<br />", "<br>")
                    .Trim(' ');

                /*
                 * Newlines are converted from \n\r to \n\n by the sanitizer, so we replaced them with a GUID before and then back after the sanitization
                 */
                string inputNewlineReplaced = inputString.Replace(Environment.NewLine, newlinemarker);

                var sanitizedInputWithNewlinesReplaced = sanitizer.Sanitize(inputNewlineReplaced, string.Empty, NoEntityMarkupFormatter.Instance).Trim();

                // put the newline back
                var sanitizedInput = sanitizedInputWithNewlinesReplaced.Replace(newlinemarker, Environment.NewLine);

                // the second condition is necessary, otherwise text containing a single '<' plus text (e.g. '<Simon') will be blocked
                if (sanitizedInput != inputString && inputString.Contains(">", StringComparison.InvariantCultureIgnoreCase))
                {
                    return (inputIsClean: false, inputCleaned: sanitizedInput);
                }
            }

            return (inputIsClean: true, inputCleaned: input is string ? input.ToString().Trim(' ') : input);
        }

        /// <summary>
        /// Required to avoid turning & into &amp; which breaks URLs
        /// https://stackoverflow.com/questions/56016509/allow-amparsand-in-model-while-sanitize-using-htmlsanitizer-c-sharp
        /// </summary>
        private class NoEntityMarkupFormatter : IMarkupFormatter
        {
            internal static readonly NoEntityMarkupFormatter Instance = new NoEntityMarkupFormatter();
            private static readonly IMarkupFormatter defaultFormatter = HtmlMarkupFormatter.Instance;

            public string Text(string text)
            {
                return text;
            }

            public string Comment(IComment comment)
            {
                return defaultFormatter.Comment(comment);
            }

            public string Processing(IProcessingInstruction processing)
            {
                return defaultFormatter.Processing(processing);
            }

            public string Doctype(IDocumentType doctype)
            {
                return defaultFormatter.Doctype(doctype);
            }

            public string OpenTag(IElement element, bool selfClosing)
            {
                var sb = new StringBuilder();
                sb.Append('<');

                if (!string.IsNullOrEmpty(element.Prefix))
                {
                    sb.Append(element.Prefix).Append(':');
                }

                sb.Append(element.LocalName);

                foreach (var attribute in element.Attributes)
                {
                    sb.Append(" ").Append(Instance.Attribute(attribute));
                }

                sb.Append('>');
                return sb.ToString();
            }

            public string CloseTag(IElement element, bool selfClosing)
            {
                return defaultFormatter.CloseTag(element, selfClosing);
            }

            public string Attribute(IAttr attr)
            {
                var namespaceUri = attr.NamespaceUri;
                var localName = attr.LocalName;
                var value = attr.Value;
                var sb = new StringBuilder();

                if (string.IsNullOrEmpty(namespaceUri))
                {
                    sb.Append(localName);
                }
                else if (Is(namespaceUri, NamespaceNames.XmlUri))
                {
                    sb.Append(NamespaceNames.XmlPrefix).Append(':').Append(localName);
                }
                else if (Is(namespaceUri, NamespaceNames.XLinkUri))
                {
                    sb.Append(NamespaceNames.XLinkPrefix).Append(':').Append(localName);
                }
                else if (Is(namespaceUri, NamespaceNames.XmlNsUri))
                {
                    sb.Append(XmlNamespaceLocalName(localName));
                }
                else
                {
                    sb.Append(attr.Name);
                }

                sb.Append('=').Append('"').Append(value).Append('"');

                return sb.ToString();
            }

            private static bool Is(string a, string b)
            {
                return string.Equals(a, b, StringComparison.Ordinal);
            }

            private static string XmlNamespaceLocalName(string name)
            {
                return Is(name, NamespaceNames.XmlNsPrefix) ? name : string.Concat(NamespaceNames.XmlNsPrefix, ":");
            }

            public string Text(ICharacterData text)
            {
                return text.TextContent;
            }

            public string LiteralText(ICharacterData text)
            {
                return text.TextContent;
            }
        }
    }
}
