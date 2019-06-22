using Aerospike.Client;
using SignLanguageWebCoreAuth.SimplificationAlgorithm.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SignLanguageWebCoreAuth.SimplificationAlgorithm.Implementation
{
    public class POSTaggerSimplification : IPOSTaggerSimplification
    {
        private IConfiguration configuration;

        public POSTaggerSimplification(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        public Dictionary<string, string> PosTag(Dictionary<string, string> text)
        {
            var removeChars = new[] { '.', ',', '!', '?', ')', '(', '[', ']', '{', '}', '"', '`', '+', '-', '“', '”', '‘', '“', ';', '„', ':', '/', '\\' };


            Dictionary<string, string> pos = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> entry in text)
            {

                var sentencePart = entry.Key;
                var tensePart = entry.Value;

                var words = sentencePart.Split(' ');
                words = words.Where(x => x != "").ToArray();

                string[] sentenceBuffer = new string[words.Length + 1];

                for (int i = 0; i < words.Length; i++)
                {
                    var currentFlag = false;
                    var w = words[i];
                    foreach (var rc in removeChars)
                    {
                        if (w.Trim() != rc.ToString())
                        {
                            w = w.Replace(rc, ' ');
                        }
                    }

                    var modified = WriteToDB(w);
                    if (modified != null)
                    {
                        currentFlag = true;
                        sentenceBuffer[i] = modified;
                    }
                    if (!currentFlag)
                    {
                        sentenceBuffer[i] = w;
                    }
                }
                var sent = string.Join(" ", sentenceBuffer);
                sent = sent.Replace("   ", " ");
                sent = sent.Replace("  ", " ");
                pos.Add(sent, tensePart);
            }

            return pos;

        }

        public string WriteToDB(string word)
        {

            AerospikeClient client = new AerospikeClient(configuration["AppSettings:AerospikeClient"], 3000);
            word = word.Trim().ToLower();

            var posKey = new Key("sign-language", "POS", word);
            var posRecord = client.Get(null, posKey);
            if (posRecord != null)
            {
                return posRecord.GetValue("Word").ToString();
            }
            else
            {
                //if (word.EndsWith("а"))
                //{
                //    var modifiedWord = word.Substring(0, word.LastIndexOf("а"));
                //    modifiedWord += 'и';
                //    var posKeyMod = new Key("sign-language", "POS", modifiedWord);
                //    var posRecordMod = client.Get(null, posKeyMod);
                //    if (posRecordMod != null)
                //    {
                //        return posRecordMod.GetValue("Word").ToString();
                //    }
                //    else
                //    {
                //        var modifiedWord1 = word.Substring(0, word.LastIndexOf("а"));
                //        modifiedWord1 += 'о';
                //        var posKeyMod1 = new Key("sign-language", "POS", modifiedWord1);
                //        var posRecordMod1 = client.Get(null, posKeyMod1);
                //        if (posRecordMod1 != null)
                //        {
                //            return posRecordMod1.GetValue("Word").ToString();
                //        }
                //    }
                //}
                //if (word.EndsWith("и"))
                //{
                //    var modifiedWord = word.Substring(0, word.LastIndexOf("и"));
                //    modifiedWord += 'а';
                //    var posKeyMod = new Key("sign-language", "POS", modifiedWord);
                //    var posRecordMod = client.Get(null, posKeyMod);
                //    if (posRecordMod != null)
                //    {
                //        return posRecordMod.GetValue("Word").ToString();
                //    }
                //    else
                //    {
                //        var modifiedWord1 = word.Substring(0, word.LastIndexOf("и"));

                //        var posKeyMod1 = new Key("sign-language", "POS", modifiedWord1);
                //        var posRecordMod1 = client.Get(null, posKeyMod1);
                //        if (posRecordMod1 != null)
                //        {
                //            return posRecordMod1.GetValue("Word").ToString();
                //        }
                //        else
                //        {
                //            var modifiedWord2 = word.Substring(0, word.LastIndexOf("и"));
                //            modifiedWord2 += "ја";
                //            var posKeyMod2 = new Key("sign-language", "POS", modifiedWord2);
                //            var posRecordMod2 = client.Get(null, posKeyMod2);
                //            if (posRecordMod2 != null)
                //            {
                //                return posRecordMod2.GetValue("Word").ToString();
                //            }
                //            else
                //            {
                //                var modifiedWord3 = word.Substring(0, word.LastIndexOf("и"));
                //                modifiedWord3 += "ј";
                //                var posKeyMod3 = new Key("sign-language", "POS", modifiedWord3);
                //                var posRecordMod3 = client.Get(null, posKeyMod3);
                //                if (posRecordMod3 != null)
                //                {
                //                    return posRecordMod3.GetValue("Word").ToString();
                //                }
                //            }
                //        }
                //    }
                //}
                if (word.EndsWith("еви"))
                {
                    var modifiedWord = word.Substring(0, word.LastIndexOf("еви"));
                    modifiedWord += 'ј';
                    var posKeyMod = new Key("sign-language", "POS", modifiedWord);
                    var posRecordMod = client.Get(null, posKeyMod);
                    if (posRecordMod != null)
                    {
                        return posRecordMod.GetValue("Word").ToString();
                    }
                }
                if (word.EndsWith("ови"))
                {
                    var modifiedWord = word.Substring(0, word.LastIndexOf("ови"));
                    var posKeyMod = new Key("sign-language", "POS", modifiedWord);
                    var posRecordMod = client.Get(null, posKeyMod);
                    if (posRecordMod != null)
                    {
                        return posRecordMod.GetValue("Word").ToString();
                    }
                }
                if (word.EndsWith("иња"))
                {
                    var modifiedWord = word.Substring(0, word.LastIndexOf("иња"));
                    modifiedWord += 'е';
                    var posKeyMod = new Key("sign-language", "POS", modifiedWord);
                    var posRecordMod = client.Get(null, posKeyMod);
                    if (posRecordMod != null)
                    {
                        return posRecordMod.GetValue("Word").ToString();
                    }
                }
                if (word.EndsWith("ни"))
                {
                    var modifiedWord = word.Substring(0, word.LastIndexOf("ни"));
                    modifiedWord += "ен";
                    var posKeyMod = new Key("sign-language", "POS", modifiedWord);
                    var posRecordMod = client.Get(null, posKeyMod);
                    if (posRecordMod != null)
                    {
                        return posRecordMod.GetValue("Word").ToString();
                    }
                }
                if (word.EndsWith("јни"))
                {
                    var modifiedWord = word.Substring(0, word.LastIndexOf("јни"));
                    modifiedWord += "ен";
                    var posKeyMod = new Key("sign-language", "POS", modifiedWord);
                    var posRecordMod = client.Get(null, posKeyMod);
                    if (posRecordMod != null)
                    {
                        return posRecordMod.GetValue("Word").ToString();
                    }
                }
                if (word.EndsWith("ско"))
                {
                    var modifiedWord = word.Substring(0, word.LastIndexOf("ско"));
                    modifiedWord += "ски";
                    var posKeyMod = new Key("sign-language", "POS", modifiedWord);
                    var posRecordMod = client.Get(null, posKeyMod);
                    if (posRecordMod != null)
                    {
                        return posRecordMod.GetValue("Word").ToString();
                    }
                }
                string[] endings = { "та", "ва", "на", "от", "ов", "он", "то", "во", "но", "те", "ве", "не" };
                if (endings.Any(x => word.EndsWith(x)))
                {
                    var modifiedWord = word.Substring(0, word.Length - 2);
                    var posKeyMod = new Key("sign-language", "POS", modifiedWord);
                    var posRecordMod = client.Get(null, posKeyMod);
                    if (posRecordMod != null)
                    {
                        return posRecordMod.GetValue("Word").ToString();
                    }
                    else if (modifiedWord.EndsWith("ње"))
                    {
                        return "Именка";
                    }
                    else if (modifiedWord.EndsWith("на"))
                    {
                        var modifiedWord1 = word.Substring(0, word.LastIndexOf("на"));
                        modifiedWord1 += "ен";
                        var posKeyMod1 = new Key("sign-language", "POS", modifiedWord1);
                        var posRecordMod1 = client.Get(null, posKeyMod1);
                        if (posRecordMod1 != null)
                        {
                            return posRecordMod1.GetValue("Word").ToString();
                        }
                    }
                }


            }

            return null;
        }
    }
}
