using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SignLanguageWebCoreAuth.Models
{
    public class SynonymsPageViewModel
    {
        [DisplayName("Tags")]
        [Required(ErrorMessage = "Полето не смее да биде празно")]
        public List<Tags> Tags { get; set; }
     
        [DisplayName("Tag")]
        [Required(ErrorMessage = "Полето не смее да биде празно")]
        public string Tag { get; set; }
            
        [DisplayName("Phrase")]
        [Required(ErrorMessage = "Полето не смее да биде празно")]
        public string Phrase { get; set; }

        [DisplayName("Related Phrase")]
        [Required(ErrorMessage = "Полето не смее да биде празно")]
        public string RelatedWord { get; set; }

        public List<Synonyms> Synonyms { get; set; }
        
    }
}