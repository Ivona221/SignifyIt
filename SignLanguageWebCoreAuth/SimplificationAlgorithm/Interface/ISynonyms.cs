using System;
using System.Collections.Generic;
using System.Text;

namespace SignLanguageWebCoreAuth.SimplificationAlgorithm.Interface
{
    public interface ISynonyms
    {
        Dictionary<List<KeyValuePair<string, List<string>>>, string> FindSynonyms(
            Dictionary<string, string> subsentences);
    }
}
