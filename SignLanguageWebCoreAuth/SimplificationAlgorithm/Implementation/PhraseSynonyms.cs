using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aerospike.Client;
using Microsoft.AspNetCore.Rewrite.Internal.UrlMatches;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using SignLanguageWebCoreAuth.SimplificationAlgorithm.Interface;
using SignLanguageWebCoreAuth.SimplificationAlgorithm.Models;

namespace SignLanguageWebCoreAuth.SimplificationAlgorithm.Implementation
{
    public class PhraseSynonyms : IPhraseSynonyms
    {
        private IConfiguration configuration;

        public PhraseSynonyms(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public Dictionary<string, string> MapPhraseSynonyms(Dictionary<string,string> text)
        {
            //var _client = new MongoClient();
            //var _database = _client.GetDatabase("SignLanguage");

            var removeChars = new[] { '.', ',', '!', '?', ')', '(', '[', ']', '{', '}', '"', '`', '+', '-', '“', '”', '‘', '“', ';', '„', ':', '/', '\\', '"' };

            Dictionary<string, string> synonymsFound = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> entry in text)
            {
                //var sentensePlusTense = subSent.Split(new string[] { "---->"}, StringSplitOptions.None);
                var sentencePart = entry.Key;
                var tensePart = entry.Value;
                //if (sentensePlusTense.Length > 1)
                //{
                //    tensePart = sentensePlusTense[1];
                //}

                //string[] sentenceBuffer = new string[words.Length + 1];
                var words = sentencePart.Split(' ');
                var synonym = "";
                words = words.Where(x => x != "").ToArray();
                for (int i = 0; i < words.Length; i++)
                {                   
                    var w = words[i];
                    synonym = CheckInDb(w);
                    if (synonym != null)
                    {
                        sentencePart = sentencePart.Replace(w, synonym);
                        var synSplit = synonym.Split(" ");
                        
                         i += synSplit.Length;
                        
                        continue;
                    }

                    if (words.Length < i + 1 + 1)
                    {
                        continue;
                    }
                    w += " " + words[i + 1];
                    synonym = CheckInDb(w);
                    if (synonym != null)
                    {
                        sentencePart = sentencePart.Replace(w, synonym);
                        var synSplit = synonym.Split(" ");
                        //if (synSplit.Length > 2)
                        //{
                            i += synSplit.Length;
                        //}
                        //else
                        //{
                        //    i += 2;
                        //}

                        continue;
                    }
                    if (words.Length < i + 2 + 1)
                    {
                        continue;
                    }
                    w += " " + words[i + 2];
                    synonym = CheckInDb(w);
                    if (synonym != null)
                    {
                        sentencePart = sentencePart.Replace(w, synonym);
                        var synSplit = synonym.Split(" ");
                        //if (synSplit.Length > 3)
                        //{
                            i += synSplit.Length;
                        //}
                        //else
                        //{
                        //    i += 3;
                        //}
                        
                        continue;
                    }
                    if (words.Length < i + 3 + 1)
                    {
                        continue;
                    }
                    w += " " + words[i + 3];
                    synonym = CheckInDb(w);
                    if (synonym != null)
                    {
                        sentencePart = sentencePart.Replace(w, synonym);
                        var synSplit = synonym.Split(" ");
                        //if (synSplit.Length > 4)
                        //{
                            i += synSplit.Length;
                        //}
                        //else
                        //{
                        //    i += 4;
                        //}
                        continue;
                    }
                }
                //sentenceBuffer[words.Length] = "---->" + tensePart + '\n';
                
                var sent = sentencePart.Replace("   ", " ");
                sent = sent.Replace("  ", " ");
                synonymsFound.Add(sent, tensePart);
                //using (System.IO.StreamWriter file =
                //new System.IO.StreamWriter(@"D:\C#Projects\SignLanguageSimplification\extractedPredlozi3.txt", true))
                //{
                //    foreach (var word in sentenceBuffer)
                //    {
                //        file.Write(word + " ");
                //    }

                //}
            }

            return synonymsFound;
        }

        private string CheckInDb(string phrase)
        {
//            var _client = new MongoClient();
//            var _database = _client.GetDatabase("SignLanguage");
//            var _collection = _database.GetCollection<PhraseSynonymModel>("PhraseSynonyms");
            AerospikeClient client = new AerospikeClient(configuration["AppSettings:AerospikeClient"], 3000);
            Policy policy = new Policy();
            Key key = new Key("sign-language", "PhraseSynonyms", phrase);
            Record record = client.Get(policy, key);
            if (record != null)
            {
                foreach (KeyValuePair<string, object> entry in record.bins)
                {
                    if (entry.Key == "Synonym")
                    {
                        return entry.Value.ToString();
                    }
                }

            }

//            if (_collection.CountDocuments(x=>x.Simplified == phrase.Trim())>0)
//            {
//                return _collection.Find(x => x.Simplified == phrase.Trim()).FirstOrDefault().Synonym.Trim();
//            }

            return null;
            
        }
    }
}
