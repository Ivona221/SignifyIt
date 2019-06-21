using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SignLanguageWebCoreAuth.SimplificationAlgorithm;
using SignLanguageWebCoreAuth.SimplificationAlgorithm.Interface;

namespace SignLanguageSimplification.SimplificationAlgorithm.Implementation
{
    class SentenceSubSplitting : ISentenceSubsplitting
    {
        public List<string> Process(List<string> sentences)
        {
            //string content = System.IO.File.ReadAllText(@"D:\C#Projects\SignLanguageSimplification\sentenceSplittedText.txt");

            //var splited = content.Split(',');
            //var content = sentences;
            

            List<string> subsentences = new List<string>();
            foreach(var sentence in sentences)
            {
                var splited = sentence.Split(',');

                var sentenceArray = new List<string>();
                var j = 0;
                splited = splited.Where(x => x.Trim() != "").ToArray();
                for (int i = 0; i < splited.Length; i++)
                {

                    var words = splited[i].Split(' ');

                    words = words.Where(x => x != "").ToArray();
                    if (words.Length == 0)
                        continue;
                    if (words.Length >= 2)
                    {
                        sentenceArray.Add(splited[i]);
                        j++;
                    }
                    else
                    {
                        if (sentenceArray.Any())
                        {
                            sentenceArray[j - 1] = sentenceArray[j - 1] + ", " + words[0];
                        }
                        else
                        {
                            sentenceArray.Add(words[0]);
                        }
                    }
                    
                }
                subsentences.AddRange(sentenceArray);

            }
            

            return subsentences;

            //foreach(var sentence in sentenceArray)
            //{
            //    using (System.IO.StreamWriter file =
            //    new System.IO.StreamWriter(@"D:\C#Projects\SignLanguageSimplification\sentenceSubSplit.txt", true))
            //    {
            //        file.WriteLine(sentence);
            //    }
            //}

        }
    }
}
