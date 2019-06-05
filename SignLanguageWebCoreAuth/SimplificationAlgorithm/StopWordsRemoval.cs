using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SignLanguageWebCoreAuth.SimplificationAlgorithm;

namespace SignLanguageSimplification.SimplificationAlgorithm
{
    class StopWordsRemoval : IStopWordsRemoval
    {
        public Dictionary<string, string> RemoveStopWords(Dictionary<string, string> subsentences)
        {
            //string sentences = System.IO.File.ReadAllText(@"D:\C#Projects\SignLanguageSimplification\taggedSimplifiedTense.txt");

            var removeChars = new[] { '.', ',', '!', '?', ')', '(', '[', ']', '{', '}', '"', '`', '+', '-', '“', '”', '‘', '“', ';', '„', ':', '/', '\\', '"' };

            var predlozi = new[]
            {
                "без", "во", "в", "врз", "до", "за",
                "зад", "заради", "искрај", "крај", "кај", "каде",
                "како", "кон", "крај", "меѓу", "место", "на", "над",
                "накај", "накрај", "наместо", "наспроти", "насред",
                "низ", "од", "одавде", "оданде", "отаде", "околу",
                "освен", "откај", "по", "под", "покрај", "помеѓу",
                "поради", "посред", "потем", "пред", "през",
                "преку", "при", "против", "среде", "сред",
                "според", "спроти", "спротив", "спрема", "со", "сосе", "у"
            };

            var svrznici = new[]
            {
                "и", "а" , "но", "ама", "или", "да",
                "за да" , "макар што", "поради тоа што",
                "и", "ни", "ниту", "па", "та", "не само што" , "туку и",
                "а", "но", "ама", "туку", "ами", "меѓутоа",
                "само", "само што", "освен што", "единствено",
                "кога", "штом", "штотуку", "тукушто", "откако", "откога", "пред да", "дури", "додека",
                "затоа што", "зашто", "бидејќи", "дека", "оти",
                "така што", "толку што", "такви што", "така што",
                "да", "за да",
                "ако", "да", "без да", "ли",
                "иако", "макар што", "и покрај тоа што", "и да",
                "така како што", "како да", "како божем",
                "што", "кој што", "којшто", "чиј", "чијшто", "каков што", "колкав што",
                "дека", "оти", "како", "што", "да", "дали", "кој", "чиј", "кога"
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
                    if (!predlozi.Contains(w.ToLower()) && !chestici.Contains(w.ToLower()) && !zamenki.Contains(w.ToLower()) && 
                        !svrznici.Contains(w.ToLower()) && !verbs.Contains(w.ToLower()))
                    {
                        sentenceBuffer[i] = w + " ";
                    }

                }
                //sentenceBuffer[words.Length] = "---->" + tensePart + '\n';
                var sent = string.Join(" ", sentenceBuffer);
                sent = sent.Replace("   ", " ");
                sent = sent.Replace("  ", " ");
                stopWordsRemoved.Add(sent, tensePart);
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
