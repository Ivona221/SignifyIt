using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SignLanguageWebCoreAuth.SimplificationAlgorithm;
using SignLanguageWebCoreAuth.SimplificationAlgorithm.Interface;

namespace SignLanguageSimplification.SimplificationAlgorithm.Implementation
{
    class StopWordsRemoval : IStopWordsRemoval
    {
        public Dictionary<string, string> RemoveStopWords(Dictionary<string, string> subsentences)
        {
            //string sentences = System.IO.File.ReadAllText(@"D:\C#Projects\SignLanguageSimplification\taggedSimplifiedTense.txt");

            var removeChars = new[] { '.', ',', '!', '?', ')', '(', '[', ']', '{', '}', '"', '`', '+', '-', '“', '”', '‘', '“', ';', '„', ':', '/', '\\', '"' };

            var predlozi = new[]
            {
                "врз","за",
                "зад", "заради", "кај",
                "како", "кон", "крај", "меѓу", "место", "на", "над",
                "накај", "накрај", "наместо", "наспроти", "насред",
                "низ", "одавде", "оданде", "отаде", "околу",
                "освен", "откај", "по", "под", "покрај", "помеѓу",
                "поради", "посред", "потем", "пред", "през",
                "преку", "при", "против", "среде", "сред",
                "според", "со", "сосе", "у"
            };

            var svrznici = new[]
            {
                "и", "а" , "но", "ама","да",
                "за да" , "макар што", "поради тоа што",
                "и", "ни", "ниту", "па", "та", "не само што" , "туку и",
                "а", "но", "ама", "туку", "ами", "меѓутоа",
                "само", "само што", "освен што", "единствено"
                , "штом", "штотуку", "тукушто", "откако", "откога", "пред да", "дури", "додека",
                "затоа што", "бидејќи", "дека", "оти",
                "така што", "толку што", "такви што", "така што",
                "да", "за да",
                "да", "ли",
                "иако", "макар што", "и покрај тоа што", "и да",
                "така како што", "како да", "како божем",
                "што", "чиј", "чијшто", "каков што", "колкав што",
                "дека", "оти", "како", "што", "да", "дали", "чиј"
            };

            var zamenki = new[]
            {
                "ѝ"
            };

            var chestici = new[]
            {
                "да", "ќе"
            };

            var verbs = new[]
            {
                "се"
            };

            //var subsentences = sentences.Split('\n');
            Dictionary<string, string> stopWordsRemoved = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> entry in subsentences)
            {
                //var sentensePlusTense = subSent.Split(new string[] { "---->"}, StringSplitOptions.None);
                var sentencePart = entry.Key;
                var tensePart = entry.Value;
                //if (sentensePlusTense.Length > 1)
                //{
                //    tensePart = sentensePlusTense[1];
                //}
                if(sentencePart.Contains("него го"))
                {
                    sentencePart = sentencePart.Replace("него го", "тој");
                }
                if (sentencePart.Contains("нему му"))
                {
                    sentencePart = sentencePart.Replace("нему му", "тој");
                }
                if (sentencePart.Contains("неа ја"))
                {
                    sentencePart = sentencePart.Replace("неа ја", "таа");
                }
                if (sentencePart.Contains("нејзе и"))
                {
                    sentencePart = sentencePart.Replace("нејзе и", "таа");
                }
                if (sentencePart.Contains("вас ве"))
                {
                    sentencePart = sentencePart.Replace("вас ве", "вие");
                }
                if (sentencePart.Contains("вам ви"))
                {
                    sentencePart = sentencePart.Replace("вам ви", "вие");
                }
                if (sentencePart.Contains("нас ни"))
                {
                    sentencePart = sentencePart.Replace("нас ни", "нас");
                }
                if (sentencePart.Contains("нив им"))
                {
                    sentencePart = sentencePart.Replace("нив им", "тие");
                }
                if (sentencePart.Contains("ним им"))
                {
                    sentencePart = sentencePart.Replace("ним им", "тие");
                }
                if (sentencePart.Contains("нас не"))
                {
                    sentencePart = sentencePart.Replace("нас не", "ние");
                }
                var words = sentencePart.Split(' ');
                string[] sentenceBuffer = new string[words.Length+1];
                words = words.Where(x => x != "").ToArray();
                for (int i = 0; i < words.Length; i++)
                {
                    var w = words[i];
                    foreach (var rc in removeChars)
                    {
                        if (w.Trim() != rc.ToString())
                        {
                            w = w.Replace(rc, ' ');
                            w = w.Trim();
                        }
                    }                   
                    if(w == "спротив" || w == "спроти" || w == "наспроти")
                    {                                               
                      sentenceBuffer[i] = "спротивно" + " ";                       
                    }
                    else if (w == "среде" || w == "насреде" || w == "насред" || w == "сред")
                    {
                        sentenceBuffer[i] = "средина" + " ";
                    }
                    else if (w == "покрај")
                    {
                        sentenceBuffer[i] = "до" + " ";
                    }
                    else if (w == "но" || w == "меѓутоа")
                    {
                        sentenceBuffer[i] = "до" + " ";
                    }
                    else if (w == "којшто" || w == "коишто")
                    {
                        sentenceBuffer[i] = "кој" + " ";
                    }
                    else if (!predlozi.Contains(w.ToLower()) && !chestici.Contains(w.ToLower()) && !zamenki.Contains(w.ToLower()) &&
                        !svrznici.Contains(w.ToLower()) && !verbs.Contains(w.ToLower()))
                    {
                        sentenceBuffer[i] = w + " ";
                    }

                }
                //sentenceBuffer[words.Length] = "---->" + tensePart + '\n';
                var sent = string.Join(" ", sentenceBuffer);
                sent = sent.Replace("   ", " ");
                sent = sent.Replace("  ", " ");
                if (!stopWordsRemoved.ContainsKey(sent))
                {
                    stopWordsRemoved.Add(sent, tensePart);
                }
                
                //using (System.IO.StreamWriter file =
                //new System.IO.StreamWriter(@"D:\C#Projects\SignLanguageSimplification\extractedPredlozi3.txt", true))
                //{
                //    foreach (var word in sentenceBuffer)
                //    {
                //        file.Write(word + " ");
                //    }

                //}
            }

            return stopWordsRemoved;


        }
    }
}
