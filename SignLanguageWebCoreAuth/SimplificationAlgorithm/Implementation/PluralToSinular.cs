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
using SignLanguageWebCoreAuth.SimplificationAlgorithm;
using SignLanguageWebCoreAuth.SimplificationAlgorithm.Interface;

namespace SignLanguageSimplification.SimplificationAlgorithm.Implementation
{
    class PluralToSinular : IPluralToSingular
    {
        private IConfiguration configuration;

        public PluralToSinular(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public Dictionary<string,string> ConvertToSinular(Dictionary<string, string> subsentences)
        {
            HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();

            var removeChars = new[] { '.', ',', '!', '?', ')', '(', '[', ']', '{', '}', '"', '`', '+', '-', '“', '”', '‘', '“', ';', '„', ':', '/', '\\' };

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

            var regexForVerbLForm1 = @"\w*л\b";
            var regexForVerbLForm2 = @"\w*ла\b";
            var regexForVerbLForm3 = @"\w*ле\b";

            var regexForVerbNoun1 = @"\w*ние\b";
            var regexForVerbNoun2 = @"\w*ње\b";

            var regexForCollectiveNouns1 = @"\w*иште\b";
            var regexForCollectiveNouns2 = @"\w*ишта\b";

            var regexForPlural1 = @"\w*ови\b";
            var regexForPlural2 = @"\w*еви\b";
            var regexForPlural3 = @"\w*вци\b";
            var regexForPlural4 = @"\w*иња\b";
            var regexForPlural5 = @"\w*и\b";

            var regexForChlenuvanje = @"\w*от|ов|он|та|ва|на|то|во|но|те|ве|не\b";

            var regexForVerbSegashno1 = @"\w*ам\b";
            var regexForVerbSegashno2 = @"\w*еш\b";

            var regexForVerbSegashno3 = @"\w*еме\b";
            var regexForVerbSegashno4 = @"\w*ете\b";
            var regexForVerbSegashno5 = @"\w*ат\b";

            var regexForVerbsPlural = @"\w*вме|вте\b";

            Dictionary<string, string> pluralSing = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> entry in subsentences)
            {
                //var sentensePlusTense = subSent.Split(new string[] { "---->" }, StringSplitOptions.None);

                var sentencePart = entry.Key;
                var tensePart = entry.Value;

                //if (sentencePart.Trim() == "")
                //{
                //    return;
                //}
                //var tensePart = "";
                //if(sentensePlusTense.Length > 1)
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
                        }
                    }

                    string current = w;
                    if (current != "" && current != null)
                    {
                        current = current.Trim();
                        string previous = PeekPrevious(sentenceBuffer, i);
                        string next = PeekNext(sentenceBuffer, i);
                        string nextToNext = PeekNext(sentenceBuffer, i + 1);


                        MatchCollection glagolskiImenki1 = Regex.Matches(current, regexForVerbNoun1);
                        MatchCollection glagolskiImenki2 = Regex.Matches(current, regexForVerbNoun2);
                        MatchCollection collectiveNouns1 = Regex.Matches(current, regexForCollectiveNouns1);
                        MatchCollection collectiveNouns2 = Regex.Matches(current, regexForCollectiveNouns2);
                        MatchCollection chlenvanje = Regex.Matches(current, regexForChlenuvanje);
                        MatchCollection plural1 = Regex.Matches(current, regexForPlural1);
                        MatchCollection plural2 = Regex.Matches(current, regexForPlural2);
                        MatchCollection plural3 = Regex.Matches(current, regexForPlural3);
                        MatchCollection plural4 = Regex.Matches(current, regexForPlural4);
                        MatchCollection plural5 = Regex.Matches(current, regexForPlural5);
                        MatchCollection glagolSegashno1 = Regex.Matches(current, regexForVerbSegashno1);
                        MatchCollection glagolSegashno2 = Regex.Matches(current, regexForVerbSegashno2);
                        MatchCollection glagolSegashno3 = Regex.Matches(current, regexForVerbSegashno3);
                        MatchCollection glagolSegashno4 = Regex.Matches(current, regexForVerbSegashno4);
                        MatchCollection glagolSegashno5 = Regex.Matches(current, regexForVerbSegashno5);
                        MatchCollection glagoli = Regex.Matches(current, regexForVerbsPlural);

                        var currentFlag = false;
                        if (char.IsUpper(current[0]))
                        {
                            sentenceBuffer[i] = current;
                        }
                        else if (predlozi.Contains(current) || prilozi.Contains(current) || zamenki.Contains(current) ||
                            chestici.Contains(current) ||
                            modalniZborovi.Contains(current) || svrznici.Contains(current))
                        {
                            sentenceBuffer[i] = current;
                        }
                        else if (collectiveNouns1.Count == 1 || collectiveNouns2.Count == 1)
                        {
                            var word = WriteToDB(current);
                            if (word != null)
                            {
                                currentFlag = true;
                                sentenceBuffer[i] = word + " ";
                            }

                        }

                        else if (chlenvanje.Count >= 1)
                        {
                            var word = WriteToDB(current);
                            if (word != null)
                            {
                                currentFlag = true;
                                sentenceBuffer[i] = word + " ";
                            }

                        }

                        else if (plural1.Count >= 1 || plural2.Count >= 1 || plural3.Count >= 1 || plural4.Count >= 1)
                        {
                            var word = WriteToDB(current);
                            if (word != null)
                            {
                                currentFlag = true;
                                sentenceBuffer[i] = word + " ";
                            }

                        }
                        else if ((glagolSegashno1.Count < 1 && glagolSegashno2.Count < 1 && glagolSegashno3.Count < 1 && glagolSegashno3.Count < 1
                            && glagolSegashno4.Count < 1 && glagolSegashno5.Count < 1 && glagoli.Count < 1) && plural5.Count > 1)
                        {
                            var word = WriteToDB(next);
                            if (word != null)
                            {
                                currentFlag = true;
                                sentenceBuffer[i + 1] = word + " ";
                            }

                        }

                        if (!currentFlag)
                        {
                            sentenceBuffer[i] = w;
                        }

                        var nextFlag = false;
                        if (next != null)
                        {
                            MatchCollection matches1 = Regex.Matches(next, regexForVerbLForm1);
                            MatchCollection matches2 = Regex.Matches(next, regexForVerbLForm2);
                            MatchCollection matches3 = Regex.Matches(next, regexForVerbLForm3);

                            if (predloziImenki.Contains(current) && !predlozi.Contains(next)
                                && !prilozi.Contains(next) && chestici.Contains(next)
                                && !zamenki.Contains(next) && !svrznici.Contains(next) && !modalniZborovi.Contains(next))
                            {
                                if (char.IsUpper(next[0]))
                                {
                                    sentenceBuffer[i] = next;
                                    //continue;
                                }
                                else
                                {
                                    var word = WriteToDB(next);
                                    if (word != null)
                                    {
                                        currentFlag = true;
                                        sentenceBuffer[i + 1] = word + " ";
                                    }
                                }                               
                            }
                        }

                        if (nextFlag)
                        {
                            i++;
                        }
                    }

                }

                var sent = string.Join(" ", sentenceBuffer);
                sent = sent.Replace("   ", " ");
                sent = sent.Replace("  ", " ");
                if (!pluralSing.ContainsKey(sent))
                {
                    pluralSing.Add(sent, tensePart);
                }
                
                //sentenceBuffer[words.Length] = "---->" + tensePart + '\n';
                //using (System.IO.StreamWriter file =
                //new System.IO.StreamWriter(@"D:\C#Projects\SignLanguageSimplification\pluralNouns3.txt", true))
                //{
                //    foreach (var word in sentenceBuffer)
                //    {
                //        file.Write(word + " ");
                //    }

                //}

            }

            return pluralSing;
            
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
            HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();
            //            var _client = new MongoClient();
            //            var _database = _client.GetDatabase("SignLanguage");
            //            var _collection = _database.GetCollection<PluralModel>("Plural");
            //            if (_collection.Find(x => x.Plural == word.Trim()).Count() > 0)
            //            {
            //                return _collection.Find(x => x.Plural == word).FirstOrDefault().Singular.Trim();
            //            }
            AerospikeClient client = new AerospikeClient(configuration["AppSettings:AerospikeClient"], 3000);
            //var _client = new MongoClient();
            //var _database = _client.GetDatabase("SignLanguage");
            Policy policy = new Policy();
            Key key = new Key("sign-language", "Plural", word);
            Record record = client.Get(policy, key);
            if (record != null)
            {
                foreach (KeyValuePair<string, object> entry in record.bins)
                {
                    if (entry.Key == "Singular")
                    {
                        return entry.Value.ToString();
                    }
                }

            }
            HtmlAgilityPack.HtmlDocument doc = web.Load("http://www.makedonski.info/search/" + word);
            var h2 = doc.DocumentNode.SelectNodes("//h2[@class='lexem']");
            if (h2 != null)
            {
                var spanText = h2.Descendants("span").First()?.InnerText;
                if (spanText != null)
                {
                    PluralModel plural = new PluralModel()
                    {
                        Plural = word,
                        Singular = spanText
                    };
                    WritePolicy policyWrite = new WritePolicy();
                    policy.SetTimeout(50);  // 50 millisecond timeout.
                    Key keyWrite = new Key("sign-language", "Plural", word);
                    Bin binVerb = new Bin("Plural", plural.Plural);
                    Bin binInf = new Bin("Singular", plural.Singular);
                    client.Put(policyWrite, keyWrite, binVerb, binInf);
                    //_collection.InsertOne(plural);
                    return spanText + " ";
                }

            }

            return null;
        }
    }
    
}


