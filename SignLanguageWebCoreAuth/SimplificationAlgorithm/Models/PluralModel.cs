using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignLanguageWebCoreAuth.SimplificationAlgorithm.Models
{
    public class PluralModel
    {
        public ObjectId _id { get; set; }

        public string Plural { get; set; }

        public string Singular { get; set; }
    }
}
