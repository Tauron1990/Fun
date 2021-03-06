﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DynamicData.Annotations;
using ImageViewerV3.Ecs.Components;
using Sprache;

namespace ImageViewerV3.Ui.Services
{
    public sealed class SearchQuery
    {
        // exclude tags -test
        // rating:5
        // user:test
        // height:>=5 width:>=5
        // mpixels:b10
        // date:>=2019-01-30

        public static readonly SearchQuery Empty = new SearchQuery();

        public string Term { get; private set; } = string.Empty;

        private bool IsInvalid { get; set; }

        private bool Favorite { get; set; }

        private string? SearchName { get; set; }

        private List<string> ExcludedTags { get; } = new List<string>();

        private List<string> Tags { get; } = new List<string>();

        private ExpressionValue<int> Rating { get; set; }

        private ExpressionValue<string> User { get; set; }

        private ExpressionValue<int> Height { get; set; }

        private ExpressionValue<int> Width { get; set; }

        private ExpressionValue<int> Mpixels { get; set; }

        private ExpressionValue<DateTime> Date { get; set; }

        public static SearchQuery ParseTerm(string term)
        {
            try
            {
                var output = Parser.ParseQuery(new Input(term.Trim() + " "));

                if (!output.WasSuccessful) return new SearchQuery { Term = term, IsInvalid = true };
                
                output.Value.Term = term;
                return output.Value;

            }
            catch
            {
                return new SearchQuery { Term = term, IsInvalid = true };
            }
        }

        public bool FilterAction(ImageComponent component)
        {
            if (string.IsNullOrWhiteSpace(Term) || IsInvalid)
                return false;

            if (Favorite)
            {
                if (!component.IsFavorite.Value)
                    return false;
            }

            if (ExcludedTags.Count != 0)
            {
                if (ExcludedTags.Any(t => component.MetaData.Tags.Contains(t)))
                    return false;
            }

            if (Tags.Count != 0)
            {
                if (!Tags.Any(t => component.MetaData.Tags.Contains(t)))
                    return false;
            }

            if (!string.IsNullOrWhiteSpace(SearchName))
            {
                if (!component.Name.Contains(SearchName))
                    return false;
            }

            if (!string.IsNullOrWhiteSpace(User.Value))
            {
                if (!component.MetaData.User.Contains(User.Value))
                    return false;
            }

            if (CompareInt(component.MetaData.Rating, Rating) && CompareInt(component.MetaData.Height, Height) &&
                CompareInt(component.MetaData.Width, Width) && CompareInt(component.MetaData.Mpixels, Mpixels))
            {
                return Date.Expression switch
                {
                    Expression.None => true,
                    Expression.Larger => (component.MetaData.CreationTime > Date.Value),
                    Expression.Smaller => (component.MetaData.CreationTime < Date.Value),
                    Expression.Equal => (component.MetaData.CreationTime == Date.Value),
                    Expression.Between => (component.MetaData.CreationTime >= Date.Value && component.MetaData.CreationTime <= Date.Next),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            return false;
        }

        private bool CompareInt(int meta, ExpressionValue<int> target)
        {
            var first = target.Value;
            var next = target.Value;

            return target.Expression switch
            {
                Expression.None => true,
                Expression.Larger => first > meta,
                Expression.Smaller => first < meta,
                Expression.Equal => first == meta,
                Expression.Between => meta >= first && meta <= next,
                _ => throw new ArgumentOutOfRangeException(nameof(target.Expression), target.Expression, "No Valid Expressen Value.")
            };
        }

        public struct ExpressionValue<TValue>
        {
            public TValue Value { get; }

            [AllowNull] public TValue Next { get; }

            public Expression Expression { get; }

            public ExpressionValue(TValue value, Expression expression, Func<TValue> next)
            {
                Value = value;
                Expression = expression;
                Next = Expression == Expression.Between ? next() : default;
            }
        }

        public enum Expression
        {
            None = 0,
            Larger,
            Smaller,
            Equal,
            Between
        }

        private static class Parser
        {
            private struct CommandEntry
            {
                public string Name { get; }

                public string First { get; }

                public string Next { get; }

                public Expression Expression { get; }

                public CommandEntry(string name, string first, string next, Expression expression)
                {
                    Name = name;
                    First = first;
                    Next = next;
                    Expression = expression;
                }
            }


            // exclude tags -test
            // rating:5
            // user:test
            // height:>=5 width:>=5
            // mpixels:b10
            // date:>=2019-01-30

            private static readonly Parser<string> CommandParse = //from leading in Parse.WhiteSpace.Many()
                TrimInput(
                from first in Parse.Letter.Once().Text()
                from rest in Parse.LetterOrDigit.Many().Text()
                from end in Parse.Char(':')
                from trailing in Parse.WhiteSpace.Many()
                select first + rest);

            private static readonly Parser<Expression> ExpressionParse =
                Parse.String("<=").Select(_ => Expression.Smaller)
                    .Or(Parse.String(">=").Select(_ => Expression.Larger))
                    .Or(Parse.String("==").Select(_ => Expression.Equal))
                    .Or(Parse.String("b").Select(_ => Expression.Between));

            private static readonly Parser<string> SegmntParse =
                from first in Parse.LetterOrDigit.Once().Text()
                from rest in Parse.CharExcept("; :,<>=").Many().Text()
                from end in Parse.Char(' ').Or(Parse.Char(';'))
                select first + rest; //Parse.CharExcept(new[] { ';', ':', ' ', '<', '>', '=' }).Until(Parse.Char(' ').Or(Parse.Char(';'))).Text();

            private static readonly Parser<CommandEntry> CommandEntryParse =
                CommandParse
                    .Then(c => ExpressionParse.Optional().Select(e => new {Command = c, Expression = e.GetOrElse(Expression.None)}))
                    .Then(e => SegmntParse.Select(t => new {e.Command, e.Expression, First = t}))
                    .Then(e => SegmntParse.Optional().Select(o => new {e.Command, e.Expression, e.First, Next = o.GetOrElse(string.Empty)}))
                    .Select(r => new CommandEntry(r.Command, r.First, r.Next, r.Expression));

            private static readonly Parser<(bool exclude, string tag)> TagParse =
                Parse.WhiteSpace.Many().Then(_ =>
                    Parse.Char('-').Until(Parse.Char(' ')).Text()
                        .Or(Parse.AnyChar.Until(Parse.Char(' ')).Text())
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .Select(s => (s.StartsWith('-'), s)));

            private static readonly Parser<string> FileNameParse =
                TrimInput(
                from first in Parse.Char('<')
                from rest in Parse.CharExcept('>').Many().Text()
                from end in Parse.Char('>')
                select rest);
            //Parse.Char('<').Until(Parse.Char('>')).Text();

            private static Parser<string> TrimInput(Parser<IEnumerable<char>> input)
            {
                return Parse.WhiteSpace.Many()
                    .Then(_ => input.Text());
            }

            public static readonly Parser<SearchQuery> ParseQuery = ParseQueryMethod;

            private static IResult<SearchQuery> ParseQueryMethod(IInput input)
            {
                var query = new SearchQuery();
                bool sucess;
                var remainer = input;

                do
                {
                    var result = CommandEntryParse(remainer);
                    sucess = result.WasSuccessful;

                    if (!sucess) continue;
                    Apply(query, result.Value);
                    remainer = result.Remainder;

                } while (sucess);
                

                var file = FileNameParse(remainer);
                if (file.WasSuccessful)
                {
                    remainer = file.Remainder;
                    query.SearchName = file.Value;
                }

                do
                {
                    var result = TagParse(remainer);
                    sucess = result.WasSuccessful;

                    if (!sucess) continue;
                    Apply(query, result.Value);
                    remainer = result.Remainder;
                } while (sucess);

                return Result.Success(query, remainer);
            }

            // exclude tags -test
            // rating:5
            // user:test
            // height:>=5 width:>=5
            // mpixels:b10
            // date:>=2019-01-30

            private static void Apply(SearchQuery query, CommandEntry entry)
            {
                switch (entry.Name)
                {
                    case "rating":
                        query.Rating = new ExpressionValue<int>(int.Parse(entry.First), entry.Expression, () => int.Parse(entry.Next));
                        break;
                    case "user":
                        query.User = new ExpressionValue<string>(entry.First, Expression.None, () => string.Empty);
                        break;
                    case "height":
                        query.Height = new ExpressionValue<int>(int.Parse(entry.First), entry.Expression, () => int.Parse(entry.Next));
                        break;
                    case "width":
                        query.Width = new ExpressionValue<int>(int.Parse(entry.First), entry.Expression, () => int.Parse(entry.Next));
                        break;
                    case "mpixels":
                        query.Mpixels = new ExpressionValue<int>(int.Parse(entry.First), entry.Expression, () => int.Parse(entry.Next));
                        break;
                    case "date":
                        query.Date = new ExpressionValue<DateTime>(DateTime.Parse(entry.First), entry.Expression, () => DateTime.Parse(entry.Next));
                        break;
                    case "favorite":
                        query.Favorite = bool.Parse(entry.First);
                        break;
                    default:
                        throw new InvalidOperationException("Unkowen Command");
                }
            }

            private static void Apply(SearchQuery query, (bool exclude, string tag) entry)
            {
                var cleanTag = entry.tag.Replace('_', ' ').Trim();
                if (entry.exclude)
                    query.ExcludedTags.Add(cleanTag.Remove(0, 1));
                else
                    query.Tags.Add(cleanTag);
            }
        }
    }
}