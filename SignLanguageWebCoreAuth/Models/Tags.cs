using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace SignLanguageWebCoreAuth.Models
{
    public class Tags
    {
        public int Id { get; set; }

        [DisplayName("Tag")]
        public string Tag { get; set; }
    }
}