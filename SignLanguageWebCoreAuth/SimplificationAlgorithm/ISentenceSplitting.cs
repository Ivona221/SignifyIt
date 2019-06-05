﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SignLanguageWebCoreAuth.SimplificationAlgorithm
{
    public interface ISentenceSplitting
    {
        List<string> Process(string text);
    }
}
