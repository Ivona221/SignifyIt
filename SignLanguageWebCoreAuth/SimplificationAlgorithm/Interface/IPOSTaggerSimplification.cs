﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignLanguageWebCoreAuth.SimplificationAlgorithm.Interface
{
    public interface IPOSTaggerSimplification
    {
        Dictionary<string, string> PosTag(Dictionary<string, string> text);
    }
}
