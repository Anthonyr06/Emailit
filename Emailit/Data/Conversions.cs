using AngleSharp;
using Ganss.XSS;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Emailit.Data
{
    public class Conversions
    {
        /// <summary>
        /// With this method all unnecessary spaces are removed.
        /// </summary>
        /// <param name="text">string type parameter</param>
        /// <returns></returns>
        public static string ExtraSpaceRemover(string text)
        {
            return Regex.Replace(text.Trim(), @"\s+", " ");
        }

        public static string ToSentenceCase(string text)
        {
            string s = text.ToLower();

            bool IsNewSentense = true;
            var result = new StringBuilder(s.Length);
            for (int i = 0; i < s.Length; i++)
            {
                if (IsNewSentense && char.IsLetter(s[i]))
                {
                    result.Append(char.ToUpper(s[i]));
                    IsNewSentense = false;
                }
                else
                    result.Append(s[i]);

                if (s[i] == '!' || s[i] == '?' || s[i] == '.')
                {
                    IsNewSentense = true;
                }
            }

            return result.ToString();
        }

        public class StringSetenceCaseFormatter : ValueConverter<string, string>
        {
            public StringSetenceCaseFormatter(ConverterMappingHints mappingHints = null)
                    : base(InputFormat, OutputFormat, mappingHints)
            { }

            private static readonly Expression<Func<string, string>> InputFormat = x => ToStorage(x);

            private static string ToStorage(string text) => ExtraSpaceRemover(text.ToLower());

            private static readonly Expression<Func<string, string>> OutputFormat = x => ToSentenceCase(x);

        }

        public class StringTitleCaseFormatter : ValueConverter<string, string>
        {
            public StringTitleCaseFormatter(ConverterMappingHints mappingHints = null)
                    : base(InputFormat, OutputFormat, mappingHints)
            { }

            private static readonly Expression<Func<string, string>> InputFormat = x => ToStorage(x);

            private static string ToStorage(string text) => ExtraSpaceRemover(text.ToLower());

            private static readonly Expression<Func<string, string>> OutputFormat = x => FromStorage(x);

            private static string FromStorage(string text)
            {
                TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

                return ti.ToTitleCase(text.ToLower());
            }
        }

        public class StringTitleCaseFormatterEmail : ValueConverter<string, string>
        {
            public StringTitleCaseFormatterEmail(ConverterMappingHints mappingHints = null)
                    : base(InputFormat, OutputFormat, mappingHints)
            { }

            private static readonly Expression<Func<string, string>> InputFormat = x => ToStorage(x);

            private static string ToStorage(string text) => ExtraSpaceRemover(text.ToLower());

            private static readonly Expression<Func<string, string>> OutputFormat = x => FromStorage(x);

            private static string FromStorage(string text)
            {
                return text.First().ToString().ToUpper() + text.Substring(1).ToLower();
            }
        }

        public class IdCardFormatter : ValueConverter<string, string>
        {
            public IdCardFormatter(ConverterMappingHints mappingHints = null)
                    : base(InputFormat, OutputFormat, mappingHints)
            { }

            private static readonly Expression<Func<string, string>> InputFormat = x => ToStorage(x);

            private static string ToStorage(string IdCard) => IdCard.Replace("-", "");

            private static readonly Expression<Func<string, string>> OutputFormat = x => FromStorage(x);

            private static string FromStorage(string IdCard)
            {
                return IdCard.Insert(3, "-").Insert(11, "-");
            }
        }
        public class HtmlToString : ValueConverter<string, string>
        {
            public HtmlToString(ConverterMappingHints mappingHints = null)
                 : base(InputFormat, OutputFormat, mappingHints)
            { }

            private static readonly Expression<Func<string, string>> InputFormat = x => ToStorage(x);

            private static string ToStorage(string html)
            {
                Ganss.XSS.HtmlSanitizer sanitizer = new Ganss.XSS.HtmlSanitizer();
                string HtmlSanitized = sanitizer.Sanitize(html);

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(HttpUtility.HtmlDecode(HtmlSanitized));

                var sb = new StringBuilder();

                foreach (var node in doc.DocumentNode.DescendantsAndSelf())
                {
                    if (!node.HasChildNodes)
                    {
                        string text = node.InnerText;
                        if (!string.IsNullOrEmpty(text))
                            sb.AppendLine(text.Trim());
                    }
                }

                return ExtraSpaceRemover(sb.ToString().ToLower());
            }

            private static readonly Expression<Func<string, string>> OutputFormat = x => ToSentenceCase(x);

        }


        public class HtmlSanitizer : ValueConverter<string, string>
        {
            public HtmlSanitizer(ConverterMappingHints mappingHints = null)
                 : base(InputFormat, OutputFormat, mappingHints)
            { }

            private static readonly Expression<Func<string, string>> InputFormat = x => ToStorage(x);

            private static string ToStorage(string html)
            {

                Ganss.XSS.HtmlSanitizer sanitizer = new Ganss.XSS.HtmlSanitizer();

                string sanitized = sanitizer.Sanitize(html);

                return sanitized;
            }

            private static readonly Expression<Func<string, string>> OutputFormat = x => x;

        }

    }

}
