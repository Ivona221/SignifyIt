using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace SignLanguageWebCoreAuth.Models
{
    public class UploadImagesPageViewModel
    {
        public IFormFile Image { set; get; }

        public string Meaning { get; set; }
    }
}
