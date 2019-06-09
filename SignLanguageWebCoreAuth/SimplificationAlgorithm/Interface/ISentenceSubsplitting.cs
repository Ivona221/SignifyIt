using System;
using System.Collections.Generic;
using System.Text;

namespace SignLanguageWebCoreAuth.SimplificationAlgorithm.Interface
{
    public interface ISentenceSubsplitting
    {
        List<string> Process(List<string> sentences);
    }
}
