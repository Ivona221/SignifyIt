using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace SignLanguageWebCoreAuth.SimplificationAlgorithm.Models
{
    public class PhraseSynonymModel
    {
        public ObjectId _id { get; set; }

        public string Original { get; set; }

        public string Simplified { get; set; }

        public string Synonym { get; set; }
    }
}
