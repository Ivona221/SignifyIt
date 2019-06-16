﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SignLanguageWebCoreAuth.SimplificationAlgorithm.Interface
{
    public interface IPOSTagger
    {
        Dictionary<string, string> PosTag(Dictionary<string, string> text);
    }
}
