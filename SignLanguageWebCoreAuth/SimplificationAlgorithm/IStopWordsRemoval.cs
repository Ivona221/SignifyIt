using System;
using System.Collections.Generic;
using System.Text;

namespace SignLanguageWebCoreAuth.SimplificationAlgorithm
{
    public interface IStopWordsRemoval
    {
        Dictionary<string, string> RemoveStopWords(Dictionary<string, string> subsentences);
    }
}
