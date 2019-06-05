using HtmlAgilityPack;
using MongoDB.Driver;
using SignLanguageWebCoreAuth.SimplificationAlgorithm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Aerospike.Client;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using SignLanguageWebCoreAuth.SimplificationAlgorithm;

namespace SignLanguageSimplification.SimplificationAlgorithm
{
    class Synonyms : ISynonyms
    {
        private readonly IConfiguration configuration;

        public Synonyms(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public Dictionary<List<KeyValuePair<string, List<string>>>, string> FindSynonyms(Dictionary<string, string> subsentences)
        {
            HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();

            //var _client = new MongoClient();
            //var _database = _client.GetDatabase("SignLanguage");

            //string sentences = System.IO.File.ReadAllText(@"D:\C#Projects\SignLanguageSimplification\pluralNouns3.txt");
            //var words = sentences.Split(' ');

            var removeChars = new[] { '.', ',', '!', '?', ')', '(', '[', ']', '{', '}', '"', '`', '+', '-', '“', '”', '‘', '“', ';', '„', ':', '/', '\\' };

            //var subsentences = sentences.Split('\n');

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

            var predloziImenki = new[]
           {
                "без", "во", "в", "врз", "до", "за",
                "зад", "заради", "искрај", "крај", "кај", "каде",
                "кон", "крај", "меѓу", "место", "на", "над",
                "накај", "накрај", "наместо", "наспроти", "насред",
                "низ", "од",
                "освен", "откај", "по", "под", "покрај", "помеѓу",
                "поради", "посред", "потем", "пред", "през",
                "преку", "при", "против", "среде", "сред",
                "според", "спроти", "спротив", "спрема", "со", "сосе", "у"
            };

            var prilozi = new[]
            {
                "кога", "вечер", "утре", "лани", "денес", "доцна",
                "рано", "тогаш", "некогаш", "никогаш", "сега", "одамна",
                "некни", "после", "потоа", "зимоска", "зимава", "понекогаш",
                "оттогаш", "бргу", "дење", "ноќе",
                "каде", "близу", "далеку", "овде", "таму", "онде",
                "горе", "долу", "натаму", "наваму", "напред",
                "назад", "лево", "десно", "налево", "тука",
                "некаде", "никаде", "дома", "озгора", "оздола",
                "како", "добро", "лошо", "силно", "слабо", "така", "вака",
                "инаку", "онака", "брзо", "полека", "машки", "пријателски",
                "тешко", "смешно", "тажно", "некако", "секако",
                "јасно", "чисто", "високо", "ниско",
                "колку", "малку", "многу", "толку", "сосем", "доста",
                "неколку", "николку", "онолку", "двојно", "тројно", "веќе", "премногу", "подоцна"
            };

            var chestici = new[]
            {
                "де", "бе", "ма", "барем", "пак", "меѓутоа", "просто",
                "да",
                "не", "ни", "ниту",
                "зар", "ли", "дали",
                "само", "единствено",
                "точно", "токму", "скоро", "речиси", "рамно",
                "би", "да", "нека", "ќе",
                "исто така", "уште", "притоа",
                "по" , "нај",
                "имено", "токму", "баш",
                "било", "годе",
                "ете", "еве", "ене",
                "го", "ме", "ги", "те", "ве", "ја", "не",
                "им"
            };

            var zamenki = new[]
            {
                "јас", "ти", "тој", "таа", "тоа", "ние", "вие", "тие",
                "мене ме", "тебе те", "него го", "неа ја", "нас нè", "вас ве", "нив ги",
                "мене ми", "тебе ти", "нему му", "нејзе ù", "нам ни", "вам ви", "ним им",
                "себе се", "себе си",
                "кој", "која", "кое", "кои", "што", "чија", "чие", "чиј",
                "чии", "сечиј", "нечиј", "ничиј", "некој", "секој", "никој",
                "оваа", "овој", "ова", "овие", "оној", "онаа", "она", "оние"
            };

            var zamenkiGlagol = new[]
            {
                "јас", "ти", "тој", "таа", "тоа", "ние", "вие", "тие",
                "мене ме", "тебе те", "него го", "неа ја", "нас нè", "вас ве", "нив ги",
                "мене ми", "тебе ти", "нему му", "нејзе ù", "нам ни", "вам ви", "ним им",
                "себе се", "себе си"
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

            var modalniZborovi = new[]
            {
                "се разбира", "значи", "нормално", "природно", "главно", "сигурно",
                "навистина", "секако", "можеби", "веројатно", "очигледно", "бездруго",
                "за жал", "за чудо", "божем", "за среќа", "за несреќа",
                "то ест", "на пример", "впрочем", "најпосле", "без сомнение", "по секоја цена",
                 "на секој начин", "односно"
            };

            Dictionary<List<KeyValuePair<string, List<string>>>, string> synonymsDict = new Dictionary<List<KeyValuePair<string, List<string>>>, string>();
            foreach (KeyValuePair<string, string> entry in subsentences)
            {
                
                var sentencePart = entry.Key;
                var tensePart = entry.Value;

                var words = sentencePart.Split(' ');
                words = words.Where(x => x != "").ToArray();
                string[] sentenceBuffer = new string[words.Length + 1];
                List<KeyValuePair<string, List<string>>> wordsDict = new List<KeyValuePair<string, List<string>>>();
                //Dictionary<string, List<string>> a = new Dictionary<string, List<string>>();

                //Lookup<string, List<string>> wordsDict = (Lookup<string, List<string>>)a.ToLookup(p => p.Key, p => p.Value);
                //Lookup<string, List<string>> wordsDict = new Lookup<string, List<string>>;
                for (int i = 0; i < words.Length; i++)
                {
                    
                    List<string> syns = new List<string>();
                    var w = words[i];


                    string current = w;
                    if (current != "" && current != null)
                    {
                        current = current.Trim();
                        string previous = PeekPrevious(sentenceBuffer, i);
                        string next = PeekNext(sentenceBuffer, i);
                        string nextToNext = PeekNext(sentenceBuffer, i + 1);

                        var currentFlag = false;
                        if (!predlozi.Contains(current) && !predloziImenki.Contains(current) && !prilozi.Contains(current)
                            && !chestici.Contains(current) && !zamenki.Contains(current) && !svrznici.Contains(current)
                            && !modalniZborovi.Contains(current) && !zamenkiGlagol.Contains(current))
                        {
                            var synonyms = WriteToDB(current);
                            if (synonyms != null && synonyms.Count > 0)
                            {
                                //sentenceBuffer[i] = current + " ( ";
                                currentFlag = true;
                                foreach (var synonym in synonyms)
                                {
                                    syns.Add(synonym);
                                    //sentenceBuffer[i] += synonym + " , ";
                                }
                                //sentenceBuffer[i].TrimEnd();
                                //sentenceBuffer[i].TrimEnd(',');
                                //sentenceBuffer[i] += " )";
                            }
                            else
                            {
                                syns = null;
                                //sentenceBuffer[i] = current;
                            }

                        }

                        if (!currentFlag)
                        {
                            syns = null;
                            //sentenceBuffer[i] = w;
                        }
                    }
                    if (current.Trim() == "")
                        continue;
                    wordsDict.Add(new KeyValuePair<string, List<string>>(w, syns));
                }
                //wordsDict = wordsDict.ToLookup(kvp => kvp.Key, kvp => kvp.Value);
                synonymsDict.Add(wordsDict, tensePart);
            }


            return synonymsDict;
            
            //using (System.IO.StreamWriter file =
            //new System.IO.StreamWriter(c@"D:\C#Projects\SignLanguageSimplification\synonyms3.txt", true))
            //{
            //    foreach (var word in sentenceBuffer)
            //    {
            //        file.Write(word + " ");
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
        private List<string> WriteToDB(string word)
        {
            HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();

//            var _client = new MongoClient();
//            var _database = _client.GetDatabase("SignLanguage");
//            var _collection = _database.GetCollection<SynonymsModel>("Synonyms");

            AerospikeClient client = new AerospikeClient(configuration["AppSettings:AerospikeClient"], 3000);
            Policy policy = new Policy();
            List<string> synonyms = new List<string>();
            Key key = new Key("sign-language", "Synonyms", word);
            Record record = client.Get(policy, key);
            ScanPolicy policyScan = new ScanPolicy();
            if (record != null)
            {
                foreach (KeyValuePair<string, object> entry in record.bins)
                {
                    var syns = "";
                    if (entry.Key == "Synonyms")
                    {
                        syns = entry.Value.ToString();
                    }

                    synonyms = syns.Split(",").ToList();
                    return synonyms;
                }

            }
            //client.ScanAll(policyScan, "sign-language", "Synonyms", ScanCallback);


            //var node = new Uri("http://192.168.0.36:9200");

            //var settings = new ConnectionSettings(
            //    node
            //).DefaultIndex("synonyms");

            //var client = new ElasticClient(settings);

            List<string> links = new List<string>();
            List<string> articleTexts = new List<string>();

            
            //var searchResponse = client.Search<SynonymsModel>(s => s
            //   .From(0)
            //   .Size(10)
            //   .Query(q => q
            //        .Match(m => m
            //           .Field(f => f.Word)
            //           .Query(word)
            //        )
            //   )
            //);
            //var synonymES = searchResponse.Documents.ToList();
            //if (synonymES.Count > 0)
            //{

            //    foreach (var synonym in synonymES)
            //    {
            //        synonyms.Add(synonym.Synonym);
            //    }
            //    return synonyms;
            //}
            List<string> synonymsList = new List<string>();
//            if (_collection.Find(x => x.Word == word.Trim()).Count() > 0)
//            {
//                foreach(var syn in _collection.Find(x => x.Word == word.Trim()).ToList())
//                {
//                    synonymsList.Add(syn.Synonym);
//                }
//                return synonymsList;
//            }
            
            HtmlAgilityPack.HtmlDocument doc = web.Load("http://www.makedonski.info/synonyms/show/"+word);
            Regex regex = new Regex("(synonyms/show)", RegexOptions.IgnoreCase);
            var synsA = doc.DocumentNode.SelectNodes(".//a");
            if(synsA != null)
            {
                var regexMatch = synsA.Where(a => regex.IsMatch(a.Attributes["href"].Value)).ToList<HtmlNode>();
                var synsString = "";
                for (var i = 1; i < regexMatch.Count; i++)
                {
                    var a = regexMatch[i];
                    var syn = a.InnerText.Split('(')[0];
                    SynonymsModel plural = new SynonymsModel()
                    {
                        Word = word,
                        Synonym = syn
                    };

                    //var indexResponse = client.IndexDocument(plural);
                    //_collection.InsertOne(plural);
                    synsString += syn;
                    if (i != regexMatch.Count - 1)
                    {
                        synsString += ",";
                    }
                    synonyms.Add(syn);
                }
                WritePolicy policyWrite = new WritePolicy();
                policy.SetTimeout(50);  // 50 millisecond timeout.
                Key keyWrite = new Key("sign-language", "Synonyms", word);
                Bin binVerb = new Bin("Word", word);
                Bin binInf = new Bin("Synonym", synsString);
                client.Put(policyWrite, keyWrite, binVerb, binInf);
                return synonyms;
            }
            return null;
        }

      
    }

}

