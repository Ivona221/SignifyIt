using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SignLanguageWebCoreAuth.Models
{
    public class ImagesPageViewModel
    {
        [Required]
        public string Text { get; set; }

        public List<ImageMeaningModel> Images { get; set; }

        public ImagesPageViewModel()
        {
            Images = new List<ImageMeaningModel>();
        }
    }
}
