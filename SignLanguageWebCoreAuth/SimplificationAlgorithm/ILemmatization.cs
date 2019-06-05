using System;
using System.Collections.Generic;
using System.Text;

namespace SignLanguageWebCoreAuth.SimplificationAlgorithm
{
    public interface ILemmatization
    {
        Dictionary<string, string> FindLemma(Dictionary<string, string> subsentences);


    }
}
