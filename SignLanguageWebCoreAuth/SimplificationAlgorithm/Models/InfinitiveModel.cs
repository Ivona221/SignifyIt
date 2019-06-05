using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignLanguageWebCoreAuth.SimplificationAlgorithm.Models
{
    public class InfinitiveModel
    {
        public ObjectId Id { get; set; }

        public string Verb { get; set; }

        public string Infinitive { get; set; }

    }
}
