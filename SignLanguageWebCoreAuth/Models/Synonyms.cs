using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignLanguageWebCoreAuth.Models
{
    public class Synonyms
    {
        public int Id { get; set; }

        public string Phrase { get; set; }

        public string Simplified { get; set; }

        public string RelatedWord { get; set; }

        public string Tag { get; set; }
    }
}