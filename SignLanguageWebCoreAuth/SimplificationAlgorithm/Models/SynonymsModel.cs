using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignLanguageWebCoreAuth.SimplificationAlgorithm.Models
{
    class SynonymsModel
    {
        public ObjectId _id { get; set; }

        public string Word { get; set; }

        public string Synonym { get; set; }
    }
}

