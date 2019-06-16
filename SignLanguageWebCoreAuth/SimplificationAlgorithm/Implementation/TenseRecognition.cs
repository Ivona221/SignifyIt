using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SignLanguageWebCoreAuth.SimplificationAlgorithm.Interface;

namespace SignLanguageSimplification.SimplificationAlgorithm.Implementation
{
    class TenseRecognition : ITenseRecognition
    {
        public Dictionary<string, string> TagSents(List<string> subsentences)
        {
            //string sentences = System.IO.File.ReadAllText(@"D:\C#Projects\SignLanguageSimplification\sentenceSubSplit.txt");
            //var sentences = subSentences;
            //var wordsForFile = sentences.Split(' ');

            //var subsentences = sentences.Split('\n');

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
                "ете", "еве", "ене"
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
            var regexForNumber = @"\w*мина\b";

            var regexForCollectiveNouns1 = @"\w*иште\b";
            var regexForCollectiveNouns2 = @"\w*ишта\b";

            var regexForChlenuvanje = @"\w*от|ов|он|та|ва|на|то|во|но|те|ве|не\b";
            var regexForVerbsPlural = @"\w*вме|вте\b";

            var regexForPridavki = @"\w*ски|ест|ен|ји|телен|ичок|узлав\b";

            var regexForMinatoOpredelenoNesvrsheno1 = @"\w*в\b";
            var regexForMinatoOpredelenoNesvrsheno2 = @"\w*ше\b";
            var regexForMinatoOpredelenoNesvrsheno3 = @"\w*ше\b";

            var regexForMinatoOpredelenoNesvrsheno4 = @"\w*вме\b";
            var regexForMinatoOpredelenoNesvrsheno5 = @"\w*вте\b";
            var regexForMinatoOpredelenoNesvrsheno6 = @"\w*а\b"; 

            //minato neopredeleno se obrazuva so sum + glagolska l forma
            //predminato neopredeleno se obrazuva so bev + glagolska l forma
            //idno so ќе
            //minato - idno ќе минато определено несвршено
            //идно прекажано ќе + минато неопределено

            var regexForVerbSegashno1 = @"\w*ам\b";
            var regexForVerbSegashno2 = @"\w*еш\b";

            var regexForVerbSegashno3 = @"\w*еме\b";
            var regexForVerbSegashno4 = @"\w*ете\b";
            var regexForVerbSegashno5 = @"\w*ат\b";

            var sentTense = new Dictionary<string, string>();
            foreach (var ss in subsentences)
            {
                var flag = 0;
                var subsentence = ss.Trim();

                var words = subsentence.Split(' ');
                words = words.Where(x => x != "").ToArray();
                string[] sentenceBuffer = new string[100];

                for (int i = 0; i < words.Length; i++)
                {
                    var w = words[i];
                    foreach (var rc in removeChars)
                    {
                        w = w.Replace(rc, ' ');
                    }

                    sentenceBuffer[i] = w + " ";
                }
                string sent = "";
                foreach (string word in sentenceBuffer)
                {
                    sent += word;
                }
                Console.WriteLine(sent);

                for (int i = 0; i < sentenceBuffer.Length; i++)
                {

                    string current = sentenceBuffer[i];
                    if (current != "" && current != null)
                    {
                        current = current.Trim();

                        string prePrevious = PeekPrevious(sentenceBuffer, i - 1);
                        string previous = PeekPrevious(sentenceBuffer, i);
                        string next = PeekNext(sentenceBuffer, i);
                        string nextToNext = PeekNext(sentenceBuffer, i + 1);

                        //current
                        
                        
                        MatchCollection glagolskiImenki1 = Regex.Matches(current, regexForVerbNoun1);
                        MatchCollection glagolskiImenki2 = Regex.Matches(current, regexForVerbNoun2);
                        MatchCollection number = Regex.Matches(current, regexForNumber);
                        MatchCollection collectiveNouns1 = Regex.Matches(current, regexForCollectiveNouns1);
                        MatchCollection collectiveNouns2 = Regex.Matches(current, regexForCollectiveNouns2);
                        MatchCollection chlenvanje = Regex.Matches(current, regexForChlenuvanje);
                        MatchCollection glagoli = Regex.Matches(current, regexForVerbsPlural);
                        MatchCollection pridavki = Regex.Matches(current, regexForPridavki);
                        MatchCollection glagolSegashno1 = Regex.Matches(current, regexForVerbSegashno1);
                        MatchCollection glagolSegashno2 = Regex.Matches(current, regexForVerbSegashno2);
                        MatchCollection glagolSegashno3 = Regex.Matches(current, regexForVerbSegashno3);
                        MatchCollection glagolSegashno4 = Regex.Matches(current, regexForVerbSegashno4);
                        MatchCollection glagolSegashno5 = Regex.Matches(current, regexForVerbSegashno5);
                        MatchCollection matches1 = Regex.Matches(current, regexForVerbLForm1);
                        MatchCollection matches2 = Regex.Matches(current, regexForVerbLForm2);
                        MatchCollection matches3 = Regex.Matches(current, regexForVerbLForm3);

                        //TODO: Fix this
                        if ((previous == "го" || previous == "ја" || previous == "ме" || previous == "те" || previous == "ве" || previous == "ги" || previous == "не") &&
                            (matches1.Count == 1 || matches2.Count == 1 || matches3.Count == 1))
                        {
                            flag = 1;
                            //sent = sent.Replace(previous, "");
                            sentTense[sent] = "Минато";
                            break;
                        }
                        if (glagoli.Count >= 1)
                        {
                            //sent.Replace(current, current.Substring(0, current.Length - 3));
                            sentTense[sent] = "Минато";
                            flag = 1;
                            break;
                        }

                        if (matches1.Count == 1)
                        {
                            //sent = sent.Replace(current, current.Substring(0, current.Length - 1));
                            sentTense[sent] = "Минато";
                            flag = 1;
                            break;
                        }
                        if (matches2.Count == 1 || matches3.Count == 1)
                        {
                            //sent = sent.Replace(current, current.Substring(0, current.Length - 2));
                            sentTense[sent] = "Минато";
                            flag = 1;
                            break;
                        }

                        if (glagolSegashno1.Count == 1 || glagolSegashno2.Count == 1 || glagolSegashno3.Count == 1 || glagolSegashno3.Count == 1
                            || glagolSegashno4.Count == 1 || glagolSegashno5.Count == 1)
                        {
                            sentTense[sent] = "Сегашно";
                            flag = 1;
                            break;
                        }
                        

                        var nextWord = new Dictionary<string, string>();

                        if (next != null)
                        {
                            nextWord.Add(next, "");
                            MatchCollection matches11 = Regex.Matches(next, regexForVerbLForm1);
                            MatchCollection matches22 = Regex.Matches(next, regexForVerbLForm2);
                            MatchCollection matches33 = Regex.Matches(next, regexForVerbLForm3);


                            if ((current == "сум" || current == "си") && matches11.Count == 1)
                            {
                                sent = sent.Replace(current, "");
                                //sent = sent.Replace(next, next.Substring(0, next.Length - 1));
                                if (prePrevious == "ќе")
                                {
                                    sent = sent.Replace(prePrevious, "");
                                    flag = 1;
                                    sentTense[sent] = "Минато";
                                    break;
                                }
                                else
                                {
                                    flag = 1;
                                    sentTense[sent] = "Минато";
                                    break;
                                }
                            }
                            if ((current == "сум" || current == "си") && matches22.Count == 1)
                            {
                                sent = sent.Replace(current, "");
                                //sent = sent.Replace(next, next.Substring(0, next.Length - 2));
                                if (prePrevious == "ќе")
                                {
                                    sent = sent.Replace(prePrevious, "");
                                    flag = 1;
                                    sentTense[sent] = "Минато";
                                    break;
                                }
                                else
                                {
                                    flag = 1;
                                    sentTense[sent] = "Минато";
                                    break;
                                }
                            }

                            if ((current == "сме" || current == "сте") && matches33.Count == 1)
                            {
                                sent = sent.Replace(current, "");
                                //sent = sent.Replace(next, next.Substring(0, next.Length - 2));
                                if (prePrevious == "ќе")
                                {
                                    sent = sent.Replace(prePrevious, "");
                                    flag = 1;
                                    sentTense[sent] = "Минато";
                                    break;
                                }
                                else
                                {
                                    flag = 1;
                                    sentTense[sent] = "Минато";
                                    break;
                                }
                            }
                            if (((current == "бев" || current == "беше") && (matches11.Count == 1)))
                            {
                                sent = sent.Replace(current, "");
                                //sent = sent.Replace(next, next.Substring(0, next.Length - 1));
                                flag = 1;
                                sentTense[sent] = "Минато";
                                break;
                            }
                            if (((current == "бев" || current == "беше") && matches22.Count == 1)
                                || ((current == "бевме" || current == "бевте" || current == "беа") && matches33.Count == 1))
                            {
                                sent = sent.Replace(current, "");
                                //sent = sent.Replace(next, next.Substring(0, next.Length - 2));
                                flag = 1;
                                sentTense[sent] = "Минато";
                                break;
                            }
                            if (((current == "бил" || current == "била"))
                                || ((current == "биле" || current == "било" || current == "беа")))
                            {
                                sent = sent.Replace(current, "");
                                flag = 1;
                                sentTense[sent] = "Минато";
                                break;
                            }

                            if (current == "ќе")
                            {
                                sent = sent.Replace(current, "");
                                flag = 1;
                                sentTense[sent] = "Идно";
                                break;
                            }
                            MatchCollection glagol1 = Regex.Matches(next, regexForMinatoOpredelenoNesvrsheno1);
                            MatchCollection glagol2 = Regex.Matches(next, regexForMinatoOpredelenoNesvrsheno2);
                            MatchCollection glagol3 = Regex.Matches(next, regexForMinatoOpredelenoNesvrsheno3);
                            MatchCollection glagol4 = Regex.Matches(next, regexForMinatoOpredelenoNesvrsheno4);
                            MatchCollection glagol5 = Regex.Matches(next, regexForMinatoOpredelenoNesvrsheno5);
                            MatchCollection glagol6 = Regex.Matches(next, regexForMinatoOpredelenoNesvrsheno6);
                            if (current == "се")
                            {
                                sent = sent.Replace(current, "");
                                {
                                    if (glagol1.Count == 1 || glagol6.Count == 1)
                                    {
                                        //sent = sent.Replace(next, next.Substring(0, next.Length - 1));
                                        flag = 1;
                                        sentTense[sent] = "Минато";
                                        break;
                                    }
                                    if (glagol2.Count == 1 || glagol3.Count == 1)
                                    {
                                        //sent = sent.Replace(next, next.Substring(0, next.Length - 2));
                                        flag = 1;
                                        sentTense[sent] = "Минато";
                                        break;
                                    }
                                    if (glagol4.Count == 1 || glagol5.Count == 1)
                                    {
                                        //sent = sent.Replace(next, next.Substring(0, next.Length - 3));
                                        flag = 1;
                                        sentTense[sent] = "Минато";
                                        break;
                                    }
                                    else
                                    {
                                        flag = 1;
                                        sentTense[sent] = "Сегашно";
                                        break;
                                    }

                                    //nextWord[next] = TypesOfWords.Глагол.ToString("g");
                                }
                            }

                            if (current == "ја")
                            {
                                sent = sent.Replace(current, "");
                                if (glagol1.Count == 1 || glagol6.Count == 1)
                                {
                                    //sent = sent.Replace(next, next.Substring(0, next.Length - 1));
                                    flag = 1;
                                    sentTense[sent] = "Минато";
                                    break;
                                }
                                if (glagol2.Count == 1 || glagol3.Count == 1)
                                {
                                    //sent = sent.Replace(next, next.Substring(0, next.Length - 2));
                                    flag = 1;
                                    sentTense[sent] = "Минато";
                                    break;
                                }
                                if (glagol4.Count == 1 || glagol5.Count == 1)
                                {
                                    //sent = sent.Replace(next, next.Substring(0, next.Length - 3));
                                    flag = 1;
                                    sentTense[sent] = "Минато";
                                    break;
                                }
                                else
                                {
                                    flag = 1;
                                    sentTense[sent] = "Сегашно";
                                    break;
                                }
                            }
                        }
                    }

                }
                if (flag == 0)
                {
                    sentTense[sent] = "Сегашно";
                }

            }

            return sentTense;

            //using (System.IO.StreamWriter file =
            //new System.IO.StreamWriter(@"D:\C#Projects\SignLanguageSimplification\taggedSimplifiedTense.txt", true))
            //{
            //    foreach (KeyValuePair<string, string> entry in sentTense)
            //    {
            //        // do something with entry.Value or entry.Key
            //        file.WriteLine(entry.Key + " ----> " + entry.Value);
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
    }
}
