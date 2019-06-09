using System;
using System.Collections.Generic;
using System.Text;

namespace SignLanguageWebCoreAuth.SimplificationAlgorithm.Interface
{
    public interface ILemmatization
    {
        Dictionary<string, string> FindLemma(Dictionary<string, string> subsentences);


    }
}
