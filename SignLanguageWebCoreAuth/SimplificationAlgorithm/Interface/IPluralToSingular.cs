using System;
using System.Collections.Generic;
using System.Text;

namespace SignLanguageWebCoreAuth.SimplificationAlgorithm.Interface
{
    public interface IPluralToSingular
    {
        Dictionary<string, string> ConvertToSinular(Dictionary<string, string> subsentences);
    }
}
