using HtmlAgilityPack;
using MongoDB.Driver;
using SignLanguageWebCoreAuth;
using SignLanguageWebCoreAuth.SimplificationAlgorithm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Aerospike.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SignLanguageWebCoreAuth.SimplificationAlgorithm;
using SignLanguageWebCoreAuth.SimplificationAlgorithm.Interface;

namespace SignLanguageSimplification.SimplificationAlgorithm.Implementation
{
    class Infinitive : IInfinitive
    {
        private readonly IConfiguration configuration;
        public Infinitive(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        public Dictionary<string,string> TurnVerbsToInfinitive(Dictionary<string,string> subsentences)
        {
            HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();
            
            var removeChars = new[] { '.', ',', '!', '?', ')', '(', '[', ']', '{', '}', '"', '`', '+', '-', '“', '”', '‘', '“', ';', '„', ':', '/', '\\'};

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
            var regexForVerbLForm4 = @"\w*ло\b";

            /*Сегашно време*/
            //Прво лице еднина пр. гледам
            var regexVerbPresentTense1 = @"\w*м\b";
            //Второ лице еднина пр. гледаш
            var regexVerbPresentTense2 = @"\w*ш\b";
            //За трето лице еднина не проверуваме бидејќи тоа е основата за глаголите
            //Во таа форма треба да ги добиеме

            //Прво лице множина пр. гледаме
            var regexVerbPresentTense3 = @"\w*ме\b";
            //Второ лице множина пр. гледате
            var regexVerbPresentTense4 = @"\w*те\b";
            //Трето лице множина пр. гледаат
            var regexVerbPresentTense5 = @"\w*ат\b";


            /*Минато определено несвршено*/
            //Прво лице еднина пр. викав
            var regexVerbPastTenseUnfinished1 = @"\w*в\b";
            //Второ лице еднина пр. викаше
            var regexVerbPastTenseUnfinished2 = @"\w*ше\b";
            //Трето лице еднина пр. викаше
            var regexVerbPastTenseUnfinished3 = @"\w*ше\b";

            //Прво лице еднина пр. викавме
            var regexVerbPastTenseUnfinished4 = @"\w*вме\b";
            //Прво лице еднина пр. викавте
            var regexVerbPastTenseUnfinished5 = @"\w*вте\b";
            //Прво лице еднина пр. викаа
            var regexVerbPastTenseUnfinished6 = @"\w*а\b";


            /*Минато определено свршено*/
            //Прво лице еднина пр. прочитав
            var regexVerbPastTenseFinished1 = @"\w*в\b";
            //За второ и трето лице еднина глаголот не добива наставка

            //Прво лице еднина пр. прочитавме
            var regexVerbPastTenseFinished2 = @"\w*вме\b";
            //Прво лице еднина пр. прочитавте
            var regexVerbPastTenseFinished3 = @"\w*вте\b";
            //Прво лице еднина пр. прочитаа
            var regexVerbPastTenseFinished4 = @"\w*а\b";

            /*Минато неопределено*/
            //Се образува со глаголот сум + глаголска л-форма


            /*Предминато*/
            //Предминато се образува со глаголот сум во минато време + глаголска л-форма
            var regexForPrePast1 = @"\w*л\b";
            var regexForPrePast2 = @"\w*ла\b";
            var regexForPrePast3 = @"\w*ла\b";

            var regexForPrePast4 = @"\w*ле\b";
            var regexForPrePast5 = @"\w*ле\b";
            var regexForPrePast6 = @"\w*ле\b";

            /*Идно*/
            //Се образува со честичката ќе

            /*Минато-идно*/
            //Се образува со ќе + минато определено несвршено

            /*Идно прекажано*/
            //Се образува со ќе + минато неопределено


            //string[] sentenceBuffer = new string[words.Length];

            Dictionary<string, string> infinitive = new Dictionary<string, string>();
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


                        //Сегашно време на глаголите
                        MatchCollection verbsPresent1 = Regex.Matches(current, regexVerbPresentTense1);
                        MatchCollection verbsPresent2 = Regex.Matches(current, regexVerbPresentTense2);
                        MatchCollection verbsPresent3 = Regex.Matches(current, regexVerbPresentTense3);
                        MatchCollection verbsPresent4 = Regex.Matches(current, regexVerbPresentTense4);
                        MatchCollection verbsPresent5 = Regex.Matches(current, regexVerbPresentTense5);

                        //Минато време на глаголите
                        //Тешко да се пропознае самостојно бидејќи завршува на в
                        MatchCollection verbsPast1 = Regex.Matches(current, regexVerbPastTenseUnfinished1);
                        //Второ и трето лице се исти
                        MatchCollection verbsPast2 = Regex.Matches(current, regexVerbPastTenseUnfinished2);
                        MatchCollection verbsPast3 = Regex.Matches(current, regexVerbPastTenseUnfinished3);
                        MatchCollection verbsPast4 = Regex.Matches(current, regexVerbPastTenseUnfinished4);
                        MatchCollection verbsPast5 = Regex.Matches(current, regexVerbPastTenseUnfinished5);
                        //Тешко да се препознае бидејќи завршува на а
                        MatchCollection verbsPast6 = Regex.Matches(current, regexVerbPastTenseUnfinished6);

                        MatchCollection verbLForm1 = Regex.Matches(current, regexForVerbLForm1);
                        MatchCollection verbLForm2 = Regex.Matches(current, regexForVerbLForm2);
                        MatchCollection verbLForm3 = Regex.Matches(current, regexForVerbLForm3);
                        MatchCollection verbLForm4 = Regex.Matches(current, regexForVerbLForm4);


                        var currentFlag = false;
                        if (char.IsUpper(current[0]))
                        {
                            sentenceBuffer[i] = current;
                        }
                        if (verbsPresent1.Count == 1 || verbsPresent2.Count == 1 || verbsPresent3.Count == 1 || verbsPresent4.Count == 1
                           || verbsPresent5.Count == 1)
                        {
                            var word = WriteToDB(current);
                            if (word != null)
                            {
                                currentFlag = true;
                                sentenceBuffer[i] = word;
                            }
                        }
                        else if (verbsPast1.Count >= 1 || verbsPast2.Count >= 1 || verbsPast3.Count >= 1 || verbsPast4.Count >= 1 || verbsPast5.Count >= 1 || verbsPast6.Count >= 1)
                        {
                            var word = WriteToDB(current);
                            if (word != null)
                            {
                                currentFlag = true;
                                sentenceBuffer[i] = word;
                            }
                        }
                        else if (verbLForm1.Count >= 1 || verbLForm2.Count >= 1 ||
                            verbLForm3.Count >= 1 || verbLForm4.Count >=1
                            )
                        {
                            var word = WriteToDB(current);
                            if (word != null)
                            {
                                currentFlag = true;
                                sentenceBuffer[i] = word;
                            }
                        }
                        else if (previous != null && previous != "" && previous.Trim() != "")
                        {
                            if (zamenkiGlagol.Contains(previous))
                            {
                                if (verbsPresent1.Count == 1 || verbsPresent2.Count == 1 || verbsPresent3.Count == 1 || verbsPresent4.Count == 1
                                    || verbsPresent5.Count == 1)
                                {
                                    var word = WriteToDB(current);
                                    if (word != null)
                                    {
                                        currentFlag = true;
                                        sentenceBuffer[i] = word;
                                    }
                                }
                                else if (verbsPast1.Count >= 1 || verbsPast2.Count >= 1 || verbsPast4.Count >= 1 || verbsPast5.Count >= 1 || verbsPast5.Count >= 1)
                                {
                                    var word = WriteToDB(current);
                                    if (word != null)
                                    {
                                        currentFlag = true;
                                        sentenceBuffer[i] = word;
                                    }
                                }
                            }
                        }


                        if (!currentFlag)
                        {
                            sentenceBuffer[i] = w;
                        }


                        if (next != null && next != "" && next.Trim() != "")
                        {
                            MatchCollection matches1 = Regex.Matches(next, regexForVerbLForm1);
                            MatchCollection matches2 = Regex.Matches(next, regexForVerbLForm2);
                            MatchCollection matches3 = Regex.Matches(next, regexForVerbLForm3);

                            var nextFlag = false;
                            if (current == "би" && next != null && (matches1.Count == 1 || matches2.Count == 1 || matches3.Count == 1))
                            {
                                var word = WriteToDB(next);
                                if (word != null)
                                {
                                    nextFlag = true;
                                    sentenceBuffer[i + 1] = word;
                                }
                            }

                            //else if (zamenkiGlagol.Contains(current))
                            //{
                            //    if (!svrznici.Contains(next) && !predlozi.Contains(next) &&
                            //        !prilozi.Contains(next) && !chestici.Contains(next) &&
                            //        !zamenki.Contains(next) && !modalniZborovi.Contains(next))
                            //    {
                            //        var word = WriteToDB(next);
                            //        if (word != null)
                            //        {
                            //            nextFlag = true;
                            //            sentenceBuffer[i + 1] = word;
                            //        }
                            //    }
                            //}

                            else if (((current == "сум" || current == "си") && (matches1.Count == 1 || matches2.Count == 1))
                                || (current == "сме" || current == "сте") && matches3.Count == 1)
                            {
                                var word = WriteToDB(next);
                                if (word != null)
                                {
                                    nextFlag = true;
                                    sentenceBuffer[i + 1] = word;
                                }
                            }
                            else if (((current == "бев" || current == "беше") && (matches1.Count == 1 || matches2.Count == 1))
                                || ((current == "бевме" || current == "бевте" || current == "беа") && matches3.Count == 1))
                            {
                                var word = WriteToDB(next);
                                if (word != null)
                                {
                                    nextFlag = true;
                                    sentenceBuffer[i + 1] = word;
                                }
                            }

                            else if (current == "го" || current == "ја" || current == "ме" || current == "те" || current == "ве" || current == "ги" || current == "не")
                            {
                                var word = WriteToDB(next);
                                if (word != null)
                                {
                                    nextFlag = true;
                                    sentenceBuffer[i + 1] = word;
                                }
                            }
                            else if (current == "ќе")
                            {
                                if (!svrznici.Contains(next) && !predlozi.Contains(next) &&
                                    !prilozi.Contains(next) && !chestici.Contains(next) &&
                                    !zamenki.Contains(next) && !modalniZborovi.Contains(next))
                                {
                                    var word = WriteToDB(next);
                                    if (word != null)
                                    {
                                        nextFlag = true;
                                        sentenceBuffer[i + 1] = word;
                                    }
                                }
                            }

                            else if (current == "се")
                            {
                                {
                                    var word = WriteToDB(next);
                                    if (word != null)
                                    {
                                        nextFlag = true;
                                        sentenceBuffer[i + 1] = word;
                                    }
                                }
                            }

                            else if (current == "ја")
                            {
                                {
                                    var word = WriteToDB(next);
                                    if (word != null)
                                    {
                                        nextFlag = true;
                                        sentenceBuffer[i + 1] = word;
                                    }
                                }
                            }

                            if (nextFlag)
                            {
                                i++;
                            }
                        }

                    }
                }

                var sent = string.Join(" ", sentenceBuffer);
                sent = sent.Replace("   ", " ");
                sent = sent.Replace("  ", " ");
                if (!infinitive.ContainsKey(sent))
                {
                    infinitive.Add(sent, tensePart);
                }    
            }

            return infinitive;

            //using (System.IO.StreamWriter file =
            //new System.IO.StreamWriter(@"D:\C#Projects\SignLanguageSimplification\textWithInfVerbs3.txt", true))
            //{
            //    foreach (var word in sentenceBuffer)
            //    {
            //       file.Write(word+" ");  
            //    }

            //}
        }

        private static string PeekNext(string[] content, int index)
        {
            var nextIndex = index + 1;
            if (content.Length < 0) return null;
            if (nextIndex >= content.Length) return null;

            return content[nextIndex];
        }

        public static string PeekPrevious(string[] content, int index)
        {
            var prevIndex = index - 1;
            if (content.Length < 0) return null;
            if (prevIndex < 0) return null;

            return content[prevIndex];
        }

        public static bool IsUpper(char c)
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
            AerospikeClient client = new AerospikeClient(configuration["AppSettings:AerospikeClient"], 3000);
            //var _client = new MongoClient();
            //var _database = _client.GetDatabase("SignLanguage");
            Policy policy = new Policy();
            var posKey = new Key("sign-language", "POS", word);
            var posRecord = client.Get(null, posKey);
            if (posRecord != null)
            {
                return posRecord.GetValue("Word").ToString();
            }

            if (word.EndsWith("ам"))
            {
                var modifiedWord = word.Substring(0, word.LastIndexOf("ам"));
                modifiedWord += 'и';
                var posKeyMod = new Key("sign-language", "POS", modifiedWord);
                var posRecordMod = client.Get(null, posKeyMod);
                if (posRecordMod != null)
                {
                    return posRecordMod.GetValue("Word").ToString();
                }
                else
                {
                    var modifiedWord1 = word.Substring(0, word.LastIndexOf("м"));
                    var posKeyMod1 = new Key("sign-language", "POS", modifiedWord1);
                    var posRecordMod1 = client.Get(null, posKeyMod1);
                    if (posRecordMod1 != null)
                    {
                        return posRecordMod1.GetValue("Word").ToString();
                    }
                    else
                    {
                        var modifiedWord2 = word.Substring(0, word.LastIndexOf("ам"));
                        modifiedWord += 'е';
                        var posKeyMod2 = new Key("sign-language", "POS", modifiedWord2);
                        var posRecordMod2 = client.Get(null, posKeyMod2);
                        if (posRecordMod2 != null)
                        {
                            return posRecordMod2.GetValue("Word").ToString();
                        }
                    }
                }
            }
            if (word.EndsWith("ш"))
            {
                var modifiedWord = word.Substring(0, word.LastIndexOf("ш"));
                var posKeyMod = new Key("sign-language", "POS", modifiedWord);
                var posRecordMod = client.Get(null, posKeyMod);
                if (posRecordMod != null)
                {
                    return posRecordMod.GetValue("Word").ToString();
                }
            }
            if (word.EndsWith("ме"))
            {
                var modifiedWord = word.Substring(0, word.LastIndexOf("ме"));
                var posKeyMod = new Key("sign-language", "POS", modifiedWord);
                var posRecordMod = client.Get(null, posKeyMod);
                if (posRecordMod != null)
                {
                    return posRecordMod.GetValue("Word").ToString();
                }
            }
            if (word.EndsWith("те"))
            {
                var modifiedWord = word.Substring(0, word.LastIndexOf("те"));
                var posKeyMod = new Key("sign-language", "POS", modifiedWord);
                var posRecordMod = client.Get(null, posKeyMod);
                if (posRecordMod != null)
                {
                    return posRecordMod.GetValue("Word").ToString();
                }
            }
            if (word.EndsWith("ат"))
            {
                var modifiedWord = word.Substring(0, word.LastIndexOf("ат"));
                modifiedWord += "и";
                var posKeyMod = new Key("sign-language", "POS", modifiedWord);
                var posRecordMod = client.Get(null, posKeyMod);
                if (posRecordMod != null)
                {
                    return posRecordMod.GetValue("Word").ToString();
                }
                else
                {
                    var modifiedWord1 = word.Substring(0, word.LastIndexOf("ат"));
                    modifiedWord1 += "е";
                    var posKeyMod1 = new Key("sign-language", "POS", modifiedWord1);
                    var posRecordMod1 = client.Get(null, posKeyMod1);
                    if (posRecordMod1 != null)
                    {
                        return posRecordMod1.GetValue("Word").ToString();
                    }
                    else
                    {
                        var modifiedWord2 = word.Substring(0, word.LastIndexOf("ат"));
                        var posKeyMod2 = new Key("sign-language", "POS", modifiedWord2);
                        var posRecordMod2 = client.Get(null, posKeyMod2);
                        if (posRecordMod2 != null)
                        {
                            return posRecordMod2.GetValue("Word").ToString();
                        }
                    }
                }
            }
            if (word.EndsWith("ав"))
            {
                var modifiedWord = word.Substring(0, word.LastIndexOf("в"));
                var posKeyMod = new Key("sign-language", "POS", modifiedWord);
                var posRecordMod = client.Get(null, posKeyMod);
                if (posRecordMod != null)
                {
                    return posRecordMod.GetValue("Word").ToString();
                }
            }
            if (word.EndsWith("ев"))
            {
                var modifiedWord = word.Substring(0, word.LastIndexOf("в"));
                var posKeyMod = new Key("sign-language", "POS", modifiedWord);
                var posRecordMod = client.Get(null, posKeyMod);
                if (posRecordMod != null)
                {
                    return posRecordMod.GetValue("Word").ToString();
                }
                else
                {
                    var modifiedWord1 = word.Substring(0, word.LastIndexOf("ев"));
                    modifiedWord1 += "и";
                    var posKeyMod1 = new Key("sign-language", "POS", modifiedWord1);
                    var posRecordMod1 = client.Get(null, posKeyMod1);
                    if (posRecordMod1 != null)
                    {
                        return posRecordMod1.GetValue("Word").ToString();
                    }
                }
            }
            if (word.EndsWith("ше"))
            {
                var modifiedWord = word.Substring(0, word.LastIndexOf("ше"));
                var posKeyMod = new Key("sign-language", "POS", modifiedWord);
                var posRecordMod = client.Get(null, posKeyMod);
                if (posRecordMod != null)
                {
                    return posRecordMod.GetValue("Word").ToString();
                }
                else if(word.EndsWith("еше"))
                {
                    var modifiedWord1 = word.Substring(0, word.LastIndexOf("еше"));
                    modifiedWord1 += "и";
                    var posKeyMod1 = new Key("sign-language", "POS", modifiedWord1);
                    var posRecordMod1 = client.Get(null, posKeyMod1);
                    if (posRecordMod1 != null)
                    {
                        return posRecordMod1.GetValue("Word").ToString();
                    }
                }
            }
            if (word.EndsWith("вме"))
            {
                var modifiedWord = word.Substring(0, word.LastIndexOf("вме"));
                var posKeyMod = new Key("sign-language", "POS", modifiedWord);
                var posRecordMod = client.Get(null, posKeyMod);
                if (posRecordMod != null)
                {
                    return posRecordMod.GetValue("Word").ToString();
                }
                else if(word.EndsWith("евме"))
                {
                    var modifiedWord1 = word.Substring(0, word.LastIndexOf("евме"));
                    modifiedWord1 += "и";
                    var posKeyMod1 = new Key("sign-language", "POS", modifiedWord1);
                    var posRecordMod1 = client.Get(null, posKeyMod1);
                    if (posRecordMod1 != null)
                    {
                        return posRecordMod1.GetValue("Word").ToString();
                    }
                }
            }
            if (word.EndsWith("вте"))
            {
                var modifiedWord = word.Substring(0, word.LastIndexOf("вте"));
                var posKeyMod = new Key("sign-language", "POS", modifiedWord);
                var posRecordMod = client.Get(null, posKeyMod);
                if (posRecordMod != null)
                {
                    return posRecordMod.GetValue("Word").ToString();
                }
                else if (word.EndsWith("евте"))
                {
                    var modifiedWord1 = word.Substring(0, word.LastIndexOf("евме"));
                    modifiedWord1 += "и";
                    var posKeyMod1 = new Key("sign-language", "POS", modifiedWord1);
                    var posRecordMod1 = client.Get(null, posKeyMod1);
                    if (posRecordMod1 != null)
                    {
                        return posRecordMod1.GetValue("Word").ToString();
                    }
                }
            }
            if (word.EndsWith("а"))
            {
                var modifiedWord = word.Substring(0, word.LastIndexOf("а"));
                var posKeyMod = new Key("sign-language", "POS", modifiedWord);
                var posRecordMod = client.Get(null, posKeyMod);
                if (posRecordMod != null)
                {
                    return posRecordMod.GetValue("Word").ToString();
                }
                else if (word.EndsWith("еа"))
                {
                    var modifiedWord1 = word.Substring(0, word.LastIndexOf("еа"));
                    modifiedWord1 += "и";
                    var posKeyMod1 = new Key("sign-language", "POS", modifiedWord1);
                    var posRecordMod1 = client.Get(null, posKeyMod1);
                    if (posRecordMod1 != null)
                    {
                        return posRecordMod1.GetValue("Word").ToString();
                    }
                }
            }
            if (word.EndsWith("л"))
            {
                var modifiedWord = word.Substring(0, word.LastIndexOf("л"));
                var posKeyMod = new Key("sign-language", "POS", modifiedWord);
                var posRecordMod = client.Get(null, posKeyMod);
                if (posRecordMod != null)
                {
                    return posRecordMod.GetValue("Word").ToString();
                }
                else if (word.EndsWith("ел"))
                {
                    var modifiedWord1 = word.Substring(0, word.LastIndexOf("ел"));
                    modifiedWord1 += "и";
                    var posKeyMod1 = new Key("sign-language", "POS", modifiedWord1);
                    var posRecordMod1 = client.Get(null, posKeyMod1);
                    if (posRecordMod1 != null)
                    {
                        return posRecordMod1.GetValue("Word").ToString();
                    }
                }
            }
            if (word.EndsWith("ла"))
            {
                var modifiedWord = word.Substring(0, word.LastIndexOf("ла"));
                var posKeyMod = new Key("sign-language", "POS", modifiedWord);
                var posRecordMod = client.Get(null, posKeyMod);
                if (posRecordMod != null)
                {
                    return posRecordMod.GetValue("Word").ToString();
                }
                else if (word.EndsWith("ела"))
                {
                    var modifiedWord1 = word.Substring(0, word.LastIndexOf("ела"));
                    modifiedWord1 += "и";
                    var posKeyMod1 = new Key("sign-language", "POS", modifiedWord1);
                    var posRecordMod1 = client.Get(null, posKeyMod1);
                    if (posRecordMod1 != null)
                    {
                        return posRecordMod1.GetValue("Word").ToString();
                    }
                }
            }
            if (word.EndsWith("ло"))
            {
                var modifiedWord = word.Substring(0, word.LastIndexOf("ло"));
                var posKeyMod = new Key("sign-language", "POS", modifiedWord);
                var posRecordMod = client.Get(null, posKeyMod);
                if (posRecordMod != null)
                {
                    return posRecordMod.GetValue("Word").ToString();
                }
                else if (word.EndsWith("ело"))
                {
                    var modifiedWord1 = word.Substring(0, word.LastIndexOf("ело"));
                    modifiedWord1 += "и";
                    var posKeyMod1 = new Key("sign-language", "POS", modifiedWord1);
                    var posRecordMod1 = client.Get(null, posKeyMod1);
                    if (posRecordMod1 != null)
                    {
                        return posRecordMod1.GetValue("Word").ToString();
                    }
                }
            }
            if (word.EndsWith("ле"))
            {
                var modifiedWord = word.Substring(0, word.LastIndexOf("ле"));
                var posKeyMod = new Key("sign-language", "POS", modifiedWord);
                var posRecordMod = client.Get(null, posKeyMod);
                if (posRecordMod != null)
                {
                    return posRecordMod.GetValue("Word").ToString();
                }
                else if (word.EndsWith("еле"))
                {
                    var modifiedWord1 = word.Substring(0, word.LastIndexOf("еле"));
                    modifiedWord1 += "и";
                    var posKeyMod1 = new Key("sign-language", "POS", modifiedWord1);
                    var posRecordMod1 = client.Get(null, posKeyMod1);
                    if (posRecordMod1 != null)
                    {
                        return posRecordMod1.GetValue("Word").ToString();
                    }
                }
            }

            //Record record = client.Get(policy, key);
            //if (record != null)
            //{
            //    foreach (KeyValuePair<string, object> entry in record.bins)
            //    {
            //        if (entry.Key == "Infinitive")
            //        {
            //            return entry.Value.ToString();
            //        }
            //    }

            //}
            //else
            //{
            //    HtmlAgilityPack.HtmlDocument doc = web.Load("http://www.makedonski.info/search/" + word);
            //    var h2 = doc.DocumentNode.SelectNodes("//h2[@class='lexem']");

            //    if (h2 != null)
            //    {
            //        var spanText = h2.Descendants("span").First()?.InnerText;
            //        if (spanText != null)
            //        {
            //            WritePolicy policyWrite = new WritePolicy();
            //            policyWrite.SetTimeout(50);  // 50 millisecond timeout.
            //            InfinitiveModel inf = new InfinitiveModel()
            //            {
            //                Verb = word,
            //                Infinitive = spanText
            //            };
            //            Key keyWrite = new Key("sign-language", "Infinitive", word);
            //            Bin binVerb = new Bin("Verb", inf.Verb);
            //            Bin binInf = new Bin("Infinitive", inf.Infinitive);
            //            client.Put(policyWrite, keyWrite, binVerb, binInf);
            //            //_collection.InsertOne(inf);
            //            return spanText + " ";
            //        }

            //    }
            //    else
            //    {
            //        var div = doc.DocumentNode.SelectNodes("//div[@id='word_not_found']");
            //        var div2 = div.Descendants("div").Skip(1).Take(1);
            //        var aWords = div.Descendants("a");

            //        foreach (var aWord in aWords)
            //        {
            //            var innerText = aWord.InnerText;
            //            if (word.Contains(innerText))
            //            {
            //                InfinitiveModel inf = new InfinitiveModel()
            //                {
            //                    Verb = word,
            //                    Infinitive = innerText
            //                };
            //                WritePolicy policyWrite = new WritePolicy();
            //                policyWrite.SetTimeout(50);  // 50 millisecond timeout.
            //                Key keyWrite = new Key("sign-language", "Infinitive", word);
            //                Bin binVerb = new Bin("Verb", inf.Verb);
            //                Bin binInf = new Bin("Infinitive", inf.Infinitive);
            //                client.Put(policyWrite, keyWrite, binVerb, binInf);
            //                //_collection.InsertOne(inf);
            //                return innerText + " ";
            //            }
            //        }
            //    }
            //}
            //            Statement statement = new Statement();
            //            statement.
            //            var collection = client.Query(policy, "SELECT * FROM sign-language.Infinitive");

            //var _collection = _database.GetCollection<InfinitiveModel>("Infinitive");
            //            if (_collection.Find(x => x.Verb == word.Trim()).Count() > 0)
            //            {
            //                return _collection.Find(x => x.Verb == word.Trim()).FirstOrDefault().Infinitive;
            //            }



            return null;
        }
    }
    
}
