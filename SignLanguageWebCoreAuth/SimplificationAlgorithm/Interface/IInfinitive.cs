using System;
using System.Collections.Generic;
using System.Text;

namespace SignLanguageWebCoreAuth.SimplificationAlgorithm.Interface
{
    public interface IInfinitive
    {
        Dictionary<string, string> TurnVerbsToInfinitive(Dictionary<string, string> subsentences);


    }
}
