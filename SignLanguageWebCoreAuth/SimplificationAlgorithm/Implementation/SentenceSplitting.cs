using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SignLanguageWebCoreAuth.SimplificationAlgorithm.Interface;

namespace SignLanguageSimplification.SimplificationAlgorithm.Implementation
{
    class SentenceSplitting : ISentenceSplitting
    {
        public List<string> Process(string text)
        {
            //string contentEx = System.IO.File.ReadAllText(@"D:\C#Projects\SignLanguageSimplification\textForSimplification1.txt");
                        var contentEx = text;
            contentEx = contentEx.Trim();

            // interpunction symbols definition
            var eofSymbols = new[] { '.', '?', '!' };
            var newlineSymbol = new[] { '\r', '\n' };
            var quotes = new[] { '\'', '"', '`', '“', '”', '‘', '“' };
            var specialCharacters = new[] { '+', '-', '*', '&', '^', '%', '$', '#', '@' };
            var openingBrackets = new[] { '(', '{', '[' };
            var closingBrackets = new[] { ')', '}', ']' };
            var additinalEof = new List<char>();
            //after these we have a capital letter and that does not represent a new sentence
            var titles = new[]{ "проф.", "в.", "в.д.", "г.", "г.г.",
                                "год.","ж.р.", "л.", "м.р.",
                                "н.е.", "о.г.", "о.м.", "с.",
                                "с.р.", "т.", "т.е.", "т.н.",
                                "m.", "f.", "n.", "l.c.", "o.c.", "a.a", "p.s",
                                "арх.","бел.","забел.","гимн.","ден.",
                                "ген.","год.","дем.","едн.","ул.",
                                "инж.","Ing.","комп.","синг.",
                                "студ.","ак.","бот.","вок.","грам.","дат.",
                                "им.", "мат.","ном.","прил.","прид.","сврз.",
                                "физ.","хем.", "пр.н.",
                                "бр.", "гр.", "др.", "и др.", "и сл.",
                                "кн.", "мн.", "на пр.", "од н.е.",
                                "пр. н.е.", "по хр.", "пр. хр.", "св.",
                                "сп.", "ср. р.", "стр", "с.", "ст. ст.", "чл.", "ст.",
                                "аугм.", "геогр.", "истор.",
                                "литер.", "мит.", "неменл.",
                                "поет.", "зоол.", "стсл.",
                                "пф.", "импф." , "д-р.",  "итн."};
            var months = new[] { "Јануари", "Фебруари", "Март", "Април", "Maj", "Јуни",
                                 "Јули", "Август", "Септември", "Октомври", "Ноември", "Декември"};
            var englishAbbr = new[]
            {
                "feat.", "vs."
            };

            var flag = 0;
            foreach(var symbmols in eofSymbols)
            {
                if (contentEx.Contains(symbmols))
                {
                    flag = 1;
                    break;
                }
            }
            if(flag == 0)
            {
                List<string> sentences = new List<string>();
                var sents = contentEx.Split('\n'); 
                foreach(var sent in sents)
                {
                    sentences.Add(sent);
                }
                return sentences;
            }

            Stack<char> sentenceBuffer = new Stack<char>();
            var lines = new List<string>();

            // process the content
            for (int i = 0; i < contentEx.Length; i++)
            {
                // get the char values
                var current = contentEx[i];
                var previous = TakePrevoius(contentEx, i);
                var prePrevious = TakePrevoius(contentEx, i - 1);
                var next = PeekNext(contentEx, i);
                var nextToNext = PeekNext(contentEx, i + 1);


                // heuristics to check if a new line should be closed
                var isEof = false;
                // check if the current index is a sentence breaker
                var isCurrentEofSymbol = eofSymbols.Contains(current);
                var isNextEofSymbol = next.HasValue && eofSymbols.Contains(next.Value);
                var isCurrentNewLineSymbol = newlineSymbol.Contains(current);


                if (isCurrentEofSymbol)
                {
                    isEof = true;
                    if (current == '.' && (next.HasValue && char.IsLower(next.Value)) && nextToNext == '.')
                    {
                        isEof = false;
                    }
                    if (current == '.' && (next == ' ' && (nextToNext.HasValue && char.IsLower(nextToNext.Value))
                        || (nextToNext.HasValue && char.IsLower(nextToNext.Value))))
                    {
                        isEof = false;
                    }
                    if (current == '.' && (previous.HasValue && char.IsUpper(previous.Value)) && (next.HasValue && char.IsUpper(next.Value)) && nextToNext == '.')
                    {
                        isEof = false;
                    }
                    if (next == '\"' && nextToNext.HasValue && !eofSymbols.Contains(nextToNext.Value))
                    {
                        isEof = false;
                    }
                    if (current == '.' && (previous.HasValue && char.IsUpper(previous.Value)) && (next.HasValue && char.IsUpper(next.Value)) && nextToNext == '.')
                    {
                        isEof = false;
                    }
                    if (current == '.' && (previous.HasValue && char.IsUpper(previous.Value) && prePrevious == '.') && ((next.HasValue && char.IsUpper(next.Value)) ||
                        next == ' ' && nextToNext.HasValue && char.IsUpper(nextToNext.Value)))
                    {
                        isEof = false;
                    }
                    if (current == '.' && (previous.HasValue && char.IsUpper(previous.Value)) && prePrevious == ' ' &&
                        (next == ' ' || (next.HasValue && char.IsUpper(next.Value))))
                    {
                        isEof = false;
                    }
                    if (current == '.' && (next.HasValue && openingBrackets.Contains(next.Value)) ||
                        (next == ' ' && nextToNext.HasValue && openingBrackets.Contains(nextToNext.Value)))
                    {
                        isEof = false;
                    }
                    if (current == '!' && next == ',' ||
                        current == '!' && next == ' ' && nextToNext == ',')
                    {
                        isEof = false;
                    }
                    if (current == '?' && next == ',' ||
                        current == '?' && next == ' ' && nextToNext == ',')
                    {
                        isEof = false;
                    }
                    if ((eofSymbols.Contains(current) && (next == ' ' && nextToNext.HasValue && quotes.Contains(nextToNext.Value)))
                        || (eofSymbols.Contains(current) && next.HasValue && quotes.Contains(next.Value)))
                    {
                        isEof = false;
                    }
                    if ((eofSymbols.Contains(current) && (next == ' ' && (nextToNext == '–' || nextToNext == '-'))) || (eofSymbols.Contains(current) && (next == '-' || next == '–')))
                    {
                        isEof = false;
                    }
                    if ((current == '.' && next.HasValue && char.IsDigit(next.Value) && previous.HasValue && char.IsDigit(previous.Value)) ||
                        (current == '.' && next == ' ' && nextToNext.HasValue && char.IsDigit(nextToNext.Value) && previous == ' ' && prePrevious.HasValue && char.IsDigit(prePrevious.Value)))
                    {
                        isEof = false;
                    }
                    if ((eofSymbols.Contains(current) &&
                        (next == ' ' && nextToNext.HasValue && specialCharacters.Contains(nextToNext.Value)) ||
                        (next.HasValue && specialCharacters.Contains(next.Value))))
                    {
                        isEof = false;
                    }
                    if ((eofSymbols.Contains(current) &&
                        (next == ' ' && nextToNext == ',') ||
                        (next == ',')))
                    {
                        isEof = false;
                    }
                    if ((eofSymbols.Contains(current) &&
                        (next == ' ' && nextToNext.HasValue && closingBrackets.Contains(nextToNext.Value)) ||
                        (next.HasValue && closingBrackets.Contains(next.Value))))
                    {
                        isEof = false;
                    }
                    if ((eofSymbols.Contains(current) &&
                        (next == ' ' && nextToNext == ':') ||
                        (next == ':')))
                    {
                        isEof = false;
                    }
                    if ((eofSymbols.Contains(current) &&
                        (next == ' ' && nextToNext == '.') ||
                        (next == '.' && nextToNext == '.')))
                    {
                        isEof = false;
                    }

                }
                if ((closingBrackets.Contains(current) && next == ' ' && nextToNext.HasValue && char.IsUpper(nextToNext.Value)) ||
                            (closingBrackets.Contains(current) && next.HasValue && char.IsUpper(next.Value)))
                {
                    isEof = true;
                }
                if ((quotes.Contains(current) && (previous == ' ' && prePrevious.HasValue && eofSymbols.Contains(prePrevious.Value))) ||
                    (quotes.Contains(current) && previous.HasValue && eofSymbols.Contains(previous.Value)))
                {
                    isEof = true;
                }
                if ((current == '/') && nextToNext.HasValue && char.IsLetterOrDigit(next.Value))
                {
                    isEof = true;
                }
                if ((quotes.Contains(current) && (next == ' ' && nextToNext == ',')) ||
                    (quotes.Contains(current) && next == ','))
                {
                    isEof = false;
                }



                if (isNextEofSymbol) isEof = false;
                if (isCurrentNewLineSymbol) isEof = true;


                if (!isCurrentNewLineSymbol)
                {
                    sentenceBuffer.Push(current);
                }

                if (current == '.')
                {

                    var line = sentenceBuffer.ToList();
                    line.Reverse();
                    var lineEx = string.Join(string.Empty, line);
                    var splited = lineEx.Split(' ');
                    var element = splited[splited.Length - 1].ToLower();
                    var result = FindAbbreviationRecursively(titles, lineEx, element, 2);
                    if (result == false)
                    {
                        isEof = false;
                    }
                    // This is not for sure. Some of those may be on the end of a sentence but it is very rare for them to be abreviated 
                    //if (titles.Contains(element))
                    //{
                    //    isEof = false;
                    //}
                }

                // prepare the sentence
                if (isEof)
                {
                    var line = sentenceBuffer.ToList();
                    line.Reverse();
                    var lineEx = string.Join(string.Empty, line);
                    //lineEx = lineEx.Trim();
                    var splited = lineEx.Split(' ');
                    if (!string.IsNullOrEmpty(lineEx))
                    {
                        if (splited.Count() < 3)
                        {
                            if(lines.Count == 0)
                            {
                                lines.Add(lineEx);
                            }
                            else
                            {
                                lines[lines.Count() - 1] += lineEx;
                            }
                            
                        }
                        else if (months.Contains(splited[0]))
                        {
                            var previousWords = lines[lines.Count() - 1].Split(' ');
                            var lastWord = previousWords[previousWords.Count() - 1];
                            var lastChar = lastWord.ToCharArray().ElementAt(lastWord.Length - 1);
                            if (char.IsDigit(lastChar))
                            {
                                if (lines.Count == 0)
                                {
                                    lines.Add(lineEx);
                                }
                                else
                                {
                                    lines[lines.Count() - 1] += lineEx;
                                }
                                
                            }
                        }
                        else
                        {
                            lineEx = lineEx.Replace("   ", " ");
                            lineEx = lineEx.Replace("  ", " ");
                            lines.Add(lineEx);
                        }

                    }
                    sentenceBuffer.Clear();
                }
            }
            return lines;
            //foreach (var line in lines)
            //{
            //    using (System.IO.StreamWriter file =
            //    new System.IO.StreamWriter(@"D:\C#Projects\SignLanguageSimplification\sentenceSplittedText.txt", true))
            //    {
            //        file.WriteLine(line);
            //    }
            //}
        }

        public char? PeekNext(string content, int index)
        {
            var nextIndex = index + 1;
            if (string.IsNullOrEmpty(content)) return null;
            if (nextIndex >= content.Length) return null;

            return content[nextIndex];
        }

        public char? TakePrevoius(string content, int index)
        {
            var prevIndex = index - 1;
            if (string.IsNullOrEmpty(content)) return null;
            if (prevIndex < 0) return null;

            return content[prevIndex];
        }

        public bool FindAbbreviationRecursively(string[] titles, string line, string element, int n)
        {
            if (n >= 4)
            {
                return true;
            }
            // This is not for sure. Some of those may be on the end of a sentence but it is very rare for them to be abreviated 
            if (titles.Contains(element.Trim(new[] { '.', ',', '\'', '!', '?', '\r', '\n', '"', '`', '“' })))
            {
                return false;
            }
            else
            {
                var splited = line.Split(' ');
                var index = n <= splited.Length ? splited.Length - n : 0;
                var el = splited[index].ToLower() + " " + element;
                return FindAbbreviationRecursively(titles, line, el, n + 1);
            }
        }
    }
}
