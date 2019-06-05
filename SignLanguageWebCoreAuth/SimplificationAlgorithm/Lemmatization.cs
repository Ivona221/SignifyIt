using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MongoDB.Driver;
using SignLanguageWebCoreAuth.SimplificationAlgorithm.Models;

namespace SignLanguageWebCoreAuth.SimplificationAlgorithm
{
    class Lemmatization : ILemmatization
    {
        public Dictionary<string, string> FindLemma(Dictionary<string, string> subsentences)
        {
            HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();

            var removeChars = new[]
            {
                '.', ',', '!', '?', ')', '(', '[', ']', '{', '}', '"', '`', '+', '-', '“', '”', '‘', '“', ';', '„', ':',
                '/', '\\'
            };

            var regexForNounsFromAdj1 = @"\w*ец\b";
            var regexForNounsFromAdj2 = @"\w*ство\b";
            var regexForNounsFromAdj3 = @"\w*ина\b";
            var regexForNounsFromAdj4 = @"\w*ост\b";
            var regexForNounsFromAdj5 = @"\w*отија\b";

            var regexForNounsFromNums1 = @"\w*ица\b";
            var regexForNounsFromNums2 = @"\w*ка\b";
            var regexForNounsFromNums3 = @"\w*ина\b";

            var regexForNounsFromNouns1 = @"\w*ник\b";
            var regexForNounsFromNouns2 = @"\w*ар\b";
            var regexForNounsFromNouns3 = @"\w*ка\b";

            var regexForNounsFromVerbs1 = @"\w*ец\b";
            var regexForNounsFromVerbs2 = @"\w*ик|ник\b";
            var regexForNounsFromVerbs3 = @"\w*тел\b";
            var regexForNounsFromVerbs4 = @"\w*ње\b";
            var regexForNounsFromVerbs5 = @"\w*ба\b";
            var regexForNounsFromVerbs6 = @"\w*ач\b";
            var regexForNounsFromVerbs7 = @"\w*еж\b";
            var regexForNounsFromVerbs8 = @"\w*а\b";

            var regexForChlenuvanje = @"\w*от|ов|он|та|ва|на|то|во|но|те|ве|не\b";
            var regexForVerbsPlural = @"\w*вме|вте\b";

            var regexForPridavki = @"\w*ски|ест|ен|ји|телен|ичок|узлав\b";

            var regexForVerbSegashno1 = @"\w*ам\b";
            var regexForVerbSegashno2 = @"\w*еш\b";

            var regexForVerbSegashno3 = @"\w*еме\b";
            var regexForVerbSegashno4 = @"\w*ете\b";
            var regexForVerbSegashno5 = @"\w*ат\b";

            var regexForProfession = @"\w*ар\b";


            //string[] sentenceBuffer = new string[words.Length];

            Dictionary<string, string> lemmatized = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> entry in subsentences)
            {
                var sentencePart = entry.Key;
                var tensePart = entry.Value;

                var words = sentencePart.Split(' ');
                words = words.Where(x => x != "").ToArray();
                string[] sentenceBuffer = new string[words.Length + 1];

                for (int i = 0; i < words.Length; i++)
                {
                    var w = words[i];

                    foreach (var rc in removeChars)
                    {
                        if (w.Trim() != rc.ToString())
                        {
                            w = w.Replace(rc, ' ');
                        }
                    }

                    string current = w;

                    if (current != "" && current != null)
                    {
                        current = current.Trim();

                        string previous = PeekPrevious(words, i);
                        string next = PeekNext(words, i);
                        string nextToNext = PeekNext(words, i + 1);

                        //-ец
                        MatchCollection nounsFromAdj1 = Regex.Matches(current, regexForNounsFromAdj1);
                        //-ство
                        MatchCollection nounsFromAdj2 = Regex.Matches(current, regexForNounsFromAdj2);
                        //-ина
                        MatchCollection nounsFromAdj3 = Regex.Matches(current, regexForNounsFromAdj3);
                        //-ост
                        MatchCollection nounsFromAdj4 = Regex.Matches(current, regexForNounsFromAdj4);
                        //-отија
                        MatchCollection nounsFromAdj5 = Regex.Matches(current, regexForNounsFromAdj5);

                        MatchCollection nounsFromNums1 = Regex.Matches(current, regexForNounsFromNums1);
                        MatchCollection nounsFromNums2 = Regex.Matches(current, regexForNounsFromNums2);
                        MatchCollection nounsFromNums3 = Regex.Matches(current, regexForNounsFromNums3);

                        MatchCollection nounsFromNouns1 = Regex.Matches(current, regexForNounsFromNouns1);
                        MatchCollection nounsFromNouns2 = Regex.Matches(current, regexForNounsFromNouns2);
                        MatchCollection nounsFromNouns3 = Regex.Matches(current, regexForNounsFromNouns3);

                        MatchCollection nounsFromVerb1 = Regex.Matches(current, regexForNounsFromVerbs1);
                        MatchCollection nounsFromVerb2 = Regex.Matches(current, regexForNounsFromVerbs2);
                        MatchCollection nounsFromVerb3 = Regex.Matches(current, regexForNounsFromVerbs3);
                        MatchCollection nounsFromVerb4 = Regex.Matches(current, regexForNounsFromVerbs4);
                        MatchCollection nounsFromVerb5 = Regex.Matches(current, regexForNounsFromVerbs5);
                        MatchCollection nounsFromVerb6 = Regex.Matches(current, regexForNounsFromVerbs6);
                        MatchCollection nounsFromVerb7 = Regex.Matches(current, regexForNounsFromVerbs7);
                        MatchCollection nounsFromVerb8 = Regex.Matches(current, regexForNounsFromVerbs8);

                        

                        //                        MatchCollection pridavki = Regex.Matches(current, regexForPridavki);
                        //                        MatchCollection glagolSegashno1 = Regex.Matches(current, regexForVerbSegashno1);
                        //                        MatchCollection glagolSegashno2 = Regex.Matches(current, regexForVerbSegashno2);
                        //                        MatchCollection glagolSegashno3 = Regex.Matches(current, regexForVerbSegashno3);
                        //                        MatchCollection glagolSegashno4 = Regex.Matches(current, regexForVerbSegashno4);
                        //                        MatchCollection glagolSegashno5 = Regex.Matches(current, regexForVerbSegashno5);

                        var currentFlag = false;
                        //-ец
                        if (nounsFromAdj1.Count == 1)
                        {
                            currentFlag = true;
                            sentenceBuffer[i] = current.Substring(0, current.Length -2);
                        }
                        //-ост
                        else if (nounsFromAdj4.Count >= 1)
                        {
                            currentFlag = true;
                            sentenceBuffer[i] = current.Substring(0, current.Length - 2);
                        }
                        else if (nounsFromAdj2.Count == 1)
                        {
                            currentFlag = true;
                            if (current == "мајчинство")
                            {
                                sentenceBuffer[i] = "мајка";
                            }
                            else if (current == "арамиство")
                            {
                                sentenceBuffer[i] = "арамија";
                            }
                            else if (current == "бегство")
                            {
                                sentenceBuffer[i] = "бега";
                            }
                            else if (current == "божество")
                            {
                                sentenceBuffer[i] = "бог";
                            }
                            else if (current == "бродарство")
                            {
                                sentenceBuffer[i] = "брод";
                            }
                            else
                            {
                                sentenceBuffer[i] = current.Substring(0, current.Length - 3);
                            }
                            MatchCollection profession = Regex.Matches(sentenceBuffer[i], regexForProfession);
                            if (profession.Count == 1)
                            {
                                sentenceBuffer[i] = sentenceBuffer[i].Substring(0, sentenceBuffer[i].Length - 2);
                            }

                        }

                        if (!currentFlag)
                        {
                            sentenceBuffer[i] = w;
                        }


      
                        

                    }
                }

                var sent = string.Join(" ", sentenceBuffer);
                sent = sent.Replace("   ", " ");
                sent = sent.Replace("  ", " ");
                lemmatized.Add(sent, tensePart);

            }

            return lemmatized;

            //using (System.IO.StreamWriter file =
            //new System.IO.StreamWriter(@"D:\C#Projects\SignLanguageSimplification\textWithInfVerbs3.txt", true))
            //{
            //    foreach (var word in sentenceBuffer)
            //    {
            //       file.Write(word+" ");  
            //    }

            //}
        }

        private string PeekNext(string[] content, int index)
        {
            var nextIndex = index + 1;
            if (content.Length < 0) return null;
            if (nextIndex >= content.Length) return null;

            return content[nextIndex];
        }

        public string PeekPrevious(string[] content, int index)
        {
            var prevIndex = index - 1;
            if (content.Length < 0) return null;
            if (prevIndex < 0) return null;

            return content[prevIndex];
        }

        public bool IsUpper(char c)
        {
            var capitalLetters = new[]
            {
                'А', 'Б', 'В', 'Г', 'Д', 'Ѓ', 'Е', 'Ж', 'З', 'Ѕ', 'И', 'Ј', 'К',
                'Л', 'Љ', 'М', 'Н', 'Њ', 'О', 'П', 'Р', 'С', 'Т', 'Ќ', 'У', 'Ф', 'Х', 'Ц', 'Ч', 'Џ', 'Ш'
            };
            if (capitalLetters.Contains(c))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private string WriteToDB(string word)
        {
            //HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();
            //var _client = new MongoClient();
            //var _database = _client.GetDatabase("SignLanguage");
            //var _collection = _database.GetCollection<InfinitiveModel>("Infinitive");
            //if (_collection.Find(x => x.Verb == word.Trim()).Count() > 0)
            //{
            //    return _collection.Find(x => x.Verb == word.Trim()).FirstOrDefault().Infinitive;
            //}

            //HtmlAgilityPack.HtmlDocument doc = web.Load("http://www.makedonski.info/search/" + word);
            //var h2 = doc.DocumentNode.SelectNodes("//h2[@class='lexem']");
            //if (h2 != null)
            //{
            //    var spanText = h2.Descendants("span").First()?.InnerText;
            //    if (spanText != null)
            //    {
            //        InfinitiveModel inf = new InfinitiveModel()
            //        {
            //            Verb = word,
            //            Infinitive = spanText
            //        };

            //        _collection.InsertOne(inf);
            //        return spanText + " ";
            //    }

            //}
            //else
            //{
            //    var div = doc.DocumentNode.SelectNodes("//div[@id='word_not_found']");
            //    var div2 = div.Descendants("div").Skip(1).Take(1);
            //    var aWords = div.Descendants("a");

            //    foreach (var aWord in aWords)
            //    {
            //        var innerText = aWord.InnerText;
            //        if (word.Contains(innerText))
            //        {
            //            InfinitiveModel inf = new InfinitiveModel()
            //            {
            //                Verb = word,
            //                Infinitive = innerText
            //            };
            //            _collection.InsertOne(inf);
            //            return innerText + " ";
            //        }
            //    }
            //    //Regex regex = new Regex("(show/*)", RegexOptions.IgnoreCase);
            //    //var synsA = doc.DocumentNode.SelectNodes(".//a");

            //    //var regexMatch = synsA.Where(a => regex.IsMatch(a.Attributes["href"].Value)).ToList<HtmlNode>();

            //    //for (var i = 1; i < regexMatch.Count; i++)
            //    //{
            //    //    var a = regexMatch[i];
            //    //    var correctWord = a.InnerText;
            //    //    if (word.Contains(correctWord) || 
            //    //        word.Contains(correctWord.Substring(0, correctWord.Length-1)) ||
            //    //        correctWord.Contains(correctWord.Substring(0, correctWord.Length - 1)))
            //    //    {
            //    //        InfinitiveModel inf = new InfinitiveModel()
            //    //        {
            //    //            Verb = word,
            //    //            Infinitive = correctWord
            //    //        };

            //    //        _collection.InsertOne(inf);
            //    //        return correctWord + " ";
            //    //    }
            //    //}
            //}

            return null;
        }
    }
}
