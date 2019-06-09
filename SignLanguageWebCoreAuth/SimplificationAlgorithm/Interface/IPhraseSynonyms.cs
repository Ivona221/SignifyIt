using System;
using System.Collections.Generic;
using System.Text;

namespace SignLanguageWebCoreAuth.SimplificationAlgorithm.Interface
{
    public interface IPhraseSynonyms
    {
        Dictionary<string, string> MapPhraseSynonyms(Dictionary<string, string> text);
    }
}
