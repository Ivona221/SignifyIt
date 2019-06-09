using System;
using System.Collections.Generic;
using System.Text;

namespace SignLanguageWebCoreAuth.SimplificationAlgorithm.Interface
{
    public interface ITenseRecognition
    {
        Dictionary<string, string> TagSents(List<string> subsentences);
    }
}
