using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SignLanguageWebCoreAuth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SignLanguageSimplification.SimplificationAlgorithm;
using SignLanguageWebCoreAuth.SimplificationAlgorithm;
using SignLanguageWebCoreAuth.SimplificationAlgorithm.Interface;
using Google.Cloud.Vision.V1;

namespace SignLanguageWebCoreAuth.Controllers
{
    
    public class ImagesController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private IConfiguration configuration;
        private IInfinitive infinitive;
        private ITenseRecognition tenseRecognition;
        private IStopWordsRemoval stopWordsRemoval;
        private IPluralToSingular pluralToSingular;
        private ISentenceSplitting sentenceSplitting;
        private ISentenceSubsplitting sentenceSubsplitting;
        private IPhraseSynonyms phraseSynonyms;
        private IPOSTaggerSimplification posTagger;
        private readonly ILogger _logger;

        public ImagesController(IHostingEnvironment hostingEnvironment, IConfiguration _configuration, ITenseRecognition _tenseRecognition,
            IStopWordsRemoval _stopWordsRemoval, IInfinitive _infinitive, IPluralToSingular _pluralToSingular,
            ISentenceSplitting _sentenceSplitting, ISentenceSubsplitting _sentenceSubsplitting, IPhraseSynonyms _phraseSynonyms, IPOSTaggerSimplification _posTagger,
            ILogger<ImagesController> logger)
        {
            _hostingEnvironment = hostingEnvironment;
            configuration = _configuration;
            tenseRecognition = _tenseRecognition;
            stopWordsRemoval = _stopWordsRemoval;
            infinitive = _infinitive;
            pluralToSingular = _pluralToSingular;
            sentenceSplitting = _sentenceSplitting;
            sentenceSubsplitting = _sentenceSubsplitting;
            phraseSynonyms = _phraseSynonyms;
            posTagger = _posTagger;
            _logger = logger;
        }

        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Policy = "IsItAuth")]
        [HttpGet]
        public ActionResult UploadIndex()
        {
            return View("Upload");
        }

        [HttpPost]
        public ActionResult Transform([FromBody] ImagesPageViewModel model)
        {
            _logger.LogInformation("Getting item {model}", model);
            var text = model.Text;
            var simplifiedText = new Dictionary<string, string>(){{model.Text, ""}};
            if (model.Text.Trim().ToCharArray().Length > 1)
            {
                simplifiedText = Simplify(text);
            }
            
            
            _logger.LogInformation("Simplified text item {simplifiedText}", simplifiedText);
            var sentencePart = new List<string>();
            foreach(KeyValuePair<string, string> entry in simplifiedText)
            {
                var tense = "";
                var splitted = entry.Value.Split().Where(x=>x!="");
                if (entry.Value == "Минато" && splitted.Count() > 1)
                {
                    tense = "минато";
                }
                else if(entry.Value == "Идно" && splitted.Count() > 1)
                {
                    tense = "иднина";
                }
                if (tense != String.Empty)
                {
                    sentencePart.Add(entry.Key + " " + tense);
                }
                else
                {
                    sentencePart.Add(entry.Key);
                }


            }

            var finalText = string.Join(" ", sentencePart);
            text = finalText;

            var words = text.Split(" ");
            string[] files =
                Directory.GetFiles(configuration["AppSettings:ImagesPath"], "*.jpg", SearchOption.AllDirectories);
            _logger.LogInformation("Config {config}", configuration["AppSettings:ImagesPath"]);
            string webRootPath = _hostingEnvironment.WebRootPath;

            model.Images = new List<ImageMeaningModel>();
            //int i = 0;
            for (var i = 0; i < words.Count(); i++)
            {
                var word = words[i];
                if(word.Trim() == "")
                {
                    continue;
                }
                var flag = 0;
                if (words.Count() > i + 1)
                {
                    var modifiedWord1 = word.Trim() + "-" + words[i + 1];
                    var fileExists = System.IO.File.Exists(configuration["AppSettings:ImagesPath"] +
                        configuration["AppSettings:PathSeparator"] + modifiedWord1.Trim().ToLower() + ".jpg");

                    if (fileExists)
                    {
                        var file = configuration["AppSettings:ImagesPath"] +
                        configuration["AppSettings:PathSeparator"] + modifiedWord1.Trim().ToLower() + ".jpg";

                        var idx = file.LastIndexOf(configuration["AppSettings:PathSeparator"]);
                        _logger.LogInformation("Idx {idx}", idx);
                        var fileName = file.Substring(idx + 1);

                        System.IO.File.Copy(file.Substring(0, file.LastIndexOf(configuration["AppSettings:PathSeparator"]) + 1) + fileName,
                            webRootPath + "/images/" + fileName, true);
                        var imageModel = new ImageMeaningModel();
                        imageModel.Image = "/images/" + fileName;
                        imageModel.Meaning = fileName.Split(".")[0];
                        model.Images.Add(imageModel);
                        flag = 1;
                        i += 1;
                        //break;
                    }
                }
                if(flag != 1)
                {
                    if (words.Count() > i + 2)
                    {
                        var modifiedWord1 = word.Trim() + "-" + words[i + 1] + "-" + words[i + 2];
                        var fileExists = System.IO.File.Exists(configuration["AppSettings:ImagesPath"] +
                            configuration["AppSettings:PathSeparator"] + modifiedWord1.Trim().ToLower() + ".jpg");


                        if (fileExists)
                        {
                            var file = configuration["AppSettings:ImagesPath"] +
                            configuration["AppSettings:PathSeparator"] + modifiedWord1.Trim().ToLower() + ".jpg";

                            var idx = file.LastIndexOf(configuration["AppSettings:PathSeparator"]);
                            _logger.LogInformation("Idx {idx}", idx);
                            var fileName = file.Substring(idx + 1);

                            if (modifiedWord1.Trim().ToLower() == fileName.Split(".")[0])
                            {
                                System.IO.File.Copy(file.Substring(0, file.LastIndexOf(configuration["AppSettings:PathSeparator"]) + 1) + fileName,
                                    webRootPath + "/images/" + fileName, true);
                                var imageModel = new ImageMeaningModel();
                                imageModel.Image = "/images/" + fileName;
                                imageModel.Meaning = fileName.Split(".")[0];
                                model.Images.Add(imageModel);
                                flag = 1;
                                i += 2;
                                //break;
                            }
                        }

                    }
                    if(flag != 1)
                    {
                        if (System.IO.File.Exists(configuration["AppSettings:ImagesPath"] +
                        configuration["AppSettings:PathSeparator"] + word.Trim().ToLower() + ".jpg"))
                        {
                            var fileBasic = configuration["AppSettings:ImagesPath"] +
                            configuration["AppSettings:PathSeparator"] + word.Trim().ToLower() + ".jpg";

                            var idx = fileBasic.LastIndexOf(configuration["AppSettings:PathSeparator"]);
                            _logger.LogInformation("Idx {idx}", idx);
                            var fileName = fileBasic.Substring(idx + 1);

                            System.IO.File.Copy(fileBasic.Substring(0, fileBasic.LastIndexOf(configuration["AppSettings:PathSeparator"]) + 1) + fileName,
                                webRootPath + "/images/" + fileName, true);
                            var imageModel = new ImageMeaningModel();
                            imageModel.Image = "/images/" + fileName;
                            imageModel.Meaning = fileName.Split(".")[0];
                            model.Images.Add(imageModel);
                            flag = 1;
                            //break;
                        }

                        if(flag != 1)
                        {
                            var modifiedWord = word.Trim() + "ње";
                            if (word.EndsWith("и"))
                            {
                                modifiedWord = word.TrimEnd('и') + "ење";
                            }

                            var fileModExists = System.IO.File.Exists(configuration["AppSettings:ImagesPath"] +
                                configuration["AppSettings:PathSeparator"] + modifiedWord.ToLower() + ".jpg");

                            if (fileModExists)
                            {
                                var fileMod = configuration["AppSettings:ImagesPath"] +
                                configuration["AppSettings:PathSeparator"] + modifiedWord.ToLower() + ".jpg";
                                var idx = fileMod.LastIndexOf(configuration["AppSettings:PathSeparator"]);
                                _logger.LogInformation("Idx {idx}", idx);
                                var fileName = fileMod.Substring(idx + 1);

                                System.IO.File.Copy(fileMod.Substring(0, fileMod.LastIndexOf(configuration["AppSettings:PathSeparator"]) + 1) + fileName,
                                    webRootPath + "/images/" + fileName, true);
                                var imageModel = new ImageMeaningModel();
                                imageModel.Image = "/images/" + fileName;
                                imageModel.Meaning = fileName.Split(".")[0];
                                model.Images.Add(imageModel);
                                flag = 1;
                                //break;
                            }
                            if(flag != 1)
                            {
                                if (char.IsUpper(word[0]))
                                {
                                    var charArray = word.ToCharArray();
                                    foreach (var c in charArray)
                                    {
                                        System.IO.File.Copy(configuration["AppSettings:ImagesPath"] + configuration["AppSettings:PathSeparator"] + "букви" +
                                            configuration["AppSettings:PathSeparator"] + char.ToUpper(c) + ".jpg",
                                        webRootPath + "/images/" + char.ToUpper(c) + ".jpg", true);
                                        var imageModel = new ImageMeaningModel();
                                        imageModel.Image = "/images/" + char.ToUpper(c) + ".jpg";
                                        imageModel.Meaning = char.ToUpper(c).ToString();
                                        model.Images.Add(imageModel);
                                        flag = 1;
                                    }
                                    //break;
                                }
                                if (flag != 1)
                                {
                                    if (word.EndsWith("а"))
                                    {
                                        var modifiedWord1 = word.Substring(0, word.LastIndexOf("а"));
                                        modifiedWord1 += "ува";
                                        var fileModExists1 = System.IO.File.Exists(configuration["AppSettings:ImagesPath"] +
                                        configuration["AppSettings:PathSeparator"] + modifiedWord1.ToLower() + ".jpg");
                                        if (fileModExists1)
                                        {
                                            var fileMod = configuration["AppSettings:ImagesPath"] +
                                            configuration["AppSettings:PathSeparator"] + modifiedWord1.ToLower() + ".jpg";
                                            var idx = fileMod.LastIndexOf(configuration["AppSettings:PathSeparator"]);
                                            _logger.LogInformation("Idx {idx}", idx);
                                            var fileName = fileMod.Substring(idx + 1);

                                            System.IO.File.Copy(fileMod.Substring(0, fileMod.LastIndexOf(configuration["AppSettings:PathSeparator"]) + 1) + fileName,
                                                webRootPath + "/images/" + fileName, true);
                                            var imageModel = new ImageMeaningModel();
                                            imageModel.Image = "/images/" + fileName;
                                            imageModel.Meaning = fileName.Split(".")[0];
                                            model.Images.Add(imageModel);
                                            flag = 1;
                                            //break;
                                        }

                                    }

                                    if (flag != 1)
                                    {
                                        if (word.EndsWith("и"))
                                        {
                                            var modifiedWord1 = word.Substring(0, word.LastIndexOf("и"));
                                            modifiedWord1 += "ува";
                                            var fileModExists1 = System.IO.File.Exists(configuration["AppSettings:ImagesPath"] +
                                            configuration["AppSettings:PathSeparator"] + modifiedWord1.ToLower() + ".jpg");
                                            if (fileModExists1)
                                            {
                                                var fileMod = configuration["AppSettings:ImagesPath"] +
                                                configuration["AppSettings:PathSeparator"] + modifiedWord1.ToLower() + ".jpg";
                                                var idx = fileMod.LastIndexOf(configuration["AppSettings:PathSeparator"]);
                                                _logger.LogInformation("Idx {idx}", idx);
                                                var fileName = fileMod.Substring(idx + 1);

                                                System.IO.File.Copy(fileMod.Substring(0, fileMod.LastIndexOf(configuration["AppSettings:PathSeparator"]) + 1) + fileName,
                                                    webRootPath + "/images/" + fileName, true);
                                                var imageModel = new ImageMeaningModel();
                                                imageModel.Image = "/images/" + fileName;
                                                imageModel.Meaning = fileName.Split(".")[0];
                                                model.Images.Add(imageModel);
                                                flag = 1;
                                                //break;
                                            }
                                        }

                                        if (flag != 1)
                                        {
                                            if (word.EndsWith("е"))
                                            {
                                                var modifiedWord1 = word.Substring(0, word.LastIndexOf("е"));
                                                modifiedWord1 += "ува";
                                                var fileModExists1 = System.IO.File.Exists(configuration["AppSettings:ImagesPath"] +
                                                configuration["AppSettings:PathSeparator"] + modifiedWord1.ToLower() + ".jpg");
                                                if (fileModExists1)
                                                {
                                                    var fileMod = configuration["AppSettings:ImagesPath"] +
                                                    configuration["AppSettings:PathSeparator"] + modifiedWord1.ToLower() + ".jpg";
                                                    var idx = fileMod.LastIndexOf(configuration["AppSettings:PathSeparator"]);
                                                    _logger.LogInformation("Idx {idx}", idx);
                                                    var fileName = fileMod.Substring(idx + 1);

                                                    System.IO.File.Copy(fileMod.Substring(0, fileMod.LastIndexOf(configuration["AppSettings:PathSeparator"]) + 1) + fileName,
                                                        webRootPath + "/images/" + fileName, true);
                                                    var imageModel = new ImageMeaningModel();
                                                    imageModel.Image = "/images/" + fileName;
                                                    imageModel.Meaning = fileName.Split(".")[0];
                                                    model.Images.Add(imageModel);
                                                    flag = 1;
                                                    //break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            
                        }
                        
                    }
                    
                }
                

                if (flag == 0)
                {
                    var imageModel = new ImageMeaningModel();
                    imageModel.Image = "/images/noimage.jpg";
                    imageModel.Meaning = word.Trim();
                    model.Images.Add(imageModel);
                }

                //i++;
            }

            if (model.Images.Count == 0)
            {
                var imageModel = new ImageMeaningModel();
                imageModel.Image = "/images/noimage.jpg";
                imageModel.Image = "нема слика";
                model.Images = new List<ImageMeaningModel> { imageModel };
                return Json(new {images = model.Images});
            }

            return Json(new {images = model.Images});

        }

        [HttpPost]
        public async Task<ActionResult> TransformFromImage(IFormFile fileImage)
        {
            ImagesPageViewModel model = new ImagesPageViewModel();
            var filePath = configuration["AppSettings:ImagesPath"] + "/" + "temp" + ".jpg";

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await fileImage.CopyToAsync(stream);
            }

            string text = "";
            var image = Image.FromFile(filePath);
            var client = ImageAnnotatorClient.Create();
            var response = client.DetectText(image);
            foreach (var annotation in response)
            {
                if (annotation.Description != null)
                {
                    text = annotation.Description;
                    break;
                }                    
            }
            text = text.Replace("o", "о").Replace("j", "ј").Replace("k", "к").Replace("a", "а").Replace("e", "е");
            text = text.Replace("O", "О").Replace("J", "Ј").Replace("K", "К").Replace("A", "А").Replace("E", "Е");

            var simplifiedText = Simplify(text);
            var sentencePart = new List<string>();
            foreach (KeyValuePair<string, string> entry in simplifiedText)
            {
                sentencePart.Add(entry.Key);
            }

            var finalText = string.Join(" ", sentencePart);
            text = finalText;

            var words = text.Split(" ");
            string[] files =
                Directory.GetFiles(configuration["AppSettings:ImagesPath"], "*.jpg", SearchOption.AllDirectories);
            _logger.LogInformation("Config {config}", configuration["AppSettings:ImagesPath"]);
            string webRootPath = _hostingEnvironment.WebRootPath;

            model.Images = new List<ImageMeaningModel>();
            //int i = 0;
            for (var i = 0; i < words.Count(); i++)
            {
                var word = words[i];
                if (word.Trim() == "")
                {
                    continue;
                }
                var flag = 0;
                if (words.Count() > i + 1)
                {
                    var modifiedWord1 = word.Trim() + "-" + words[i + 1];
                    var fileExists = System.IO.File.Exists(configuration["AppSettings:ImagesPath"] +
                        configuration["AppSettings:PathSeparator"] + modifiedWord1.Trim().ToLower() + ".jpg");

                    if (fileExists)
                    {
                        var file = configuration["AppSettings:ImagesPath"] +
                        configuration["AppSettings:PathSeparator"] + modifiedWord1.Trim().ToLower() + ".jpg";

                        var idx = file.LastIndexOf(configuration["AppSettings:PathSeparator"]);
                        _logger.LogInformation("Idx {idx}", idx);
                        var fileName = file.Substring(idx + 1);

                        System.IO.File.Copy(file.Substring(0, file.LastIndexOf(configuration["AppSettings:PathSeparator"]) + 1) + fileName,
                            webRootPath + "/images/" + fileName, true);
                        var imageModel = new ImageMeaningModel();
                        imageModel.Image = "/images/" + fileName;
                        imageModel.Meaning = fileName.Split(".")[0];
                        model.Images.Add(imageModel);
                        flag = 1;
                        i += 1;
                        //break;
                    }
                }
                if (flag != 1)
                {
                    if (words.Count() > i + 2)
                    {
                        var modifiedWord1 = word.Trim() + "-" + words[i + 1] + "-" + words[i + 2];
                        var fileExists = System.IO.File.Exists(configuration["AppSettings:ImagesPath"] +
                            configuration["AppSettings:PathSeparator"] + modifiedWord1.Trim().ToLower() + ".jpg");


                        if (fileExists)
                        {
                            var file = configuration["AppSettings:ImagesPath"] +
                            configuration["AppSettings:PathSeparator"] + modifiedWord1.Trim().ToLower() + ".jpg";

                            var idx = file.LastIndexOf(configuration["AppSettings:PathSeparator"]);
                            _logger.LogInformation("Idx {idx}", idx);
                            var fileName = file.Substring(idx + 1);

                            if (modifiedWord1.Trim().ToLower() == fileName.Split(".")[0])
                            {
                                System.IO.File.Copy(file.Substring(0, file.LastIndexOf(configuration["AppSettings:PathSeparator"]) + 1) + fileName,
                                    webRootPath + "/images/" + fileName, true);
                                var imageModel = new ImageMeaningModel();
                                imageModel.Image = "/images/" + fileName;
                                imageModel.Meaning = fileName.Split(".")[0];
                                model.Images.Add(imageModel);
                                flag = 1;
                                i += 2;
                                //break;
                            }
                        }

                    }
                    if (flag != 1)
                    {
                        if (System.IO.File.Exists(configuration["AppSettings:ImagesPath"] +
                        configuration["AppSettings:PathSeparator"] + word.Trim().ToLower() + ".jpg"))
                        {
                            var fileBasic = configuration["AppSettings:ImagesPath"] +
                            configuration["AppSettings:PathSeparator"] + word.Trim().ToLower() + ".jpg";

                            var idx = fileBasic.LastIndexOf(configuration["AppSettings:PathSeparator"]);
                            _logger.LogInformation("Idx {idx}", idx);
                            var fileName = fileBasic.Substring(idx + 1);

                            System.IO.File.Copy(fileBasic.Substring(0, fileBasic.LastIndexOf(configuration["AppSettings:PathSeparator"]) + 1) + fileName,
                                webRootPath + "/images/" + fileName, true);
                            var imageModel = new ImageMeaningModel();
                            imageModel.Image = "/images/" + fileName;
                            imageModel.Meaning = fileName.Split(".")[0];
                            model.Images.Add(imageModel);
                            flag = 1;
                            //break;
                        }

                        if (flag != 1)
                        {
                            var modifiedWord = word.Trim() + "ње";
                            if (word.EndsWith("и"))
                            {
                                modifiedWord = word.TrimEnd('и') + "ење";
                            }

                            var fileModExists = System.IO.File.Exists(configuration["AppSettings:ImagesPath"] +
                                configuration["AppSettings:PathSeparator"] + modifiedWord.ToLower() + ".jpg");

                            if (fileModExists)
                            {
                                var fileMod = configuration["AppSettings:ImagesPath"] +
                                configuration["AppSettings:PathSeparator"] + modifiedWord.ToLower() + ".jpg";
                                var idx = fileMod.LastIndexOf(configuration["AppSettings:PathSeparator"]);
                                _logger.LogInformation("Idx {idx}", idx);
                                var fileName = fileMod.Substring(idx + 1);

                                System.IO.File.Copy(fileMod.Substring(0, fileMod.LastIndexOf(configuration["AppSettings:PathSeparator"]) + 1) + fileName,
                                    webRootPath + "/images/" + fileName, true);
                                var imageModel = new ImageMeaningModel();
                                imageModel.Image = "/images/" + fileName;
                                imageModel.Meaning = fileName.Split(".")[0];
                                model.Images.Add(imageModel);
                                flag = 1;
                                //break;
                            }
                            if (flag != 1)
                            {
                                if (char.IsUpper(word[0]))
                                {
                                    var charArray = word.ToCharArray();
                                    foreach (var c in charArray)
                                    {
                                        System.IO.File.Copy(configuration["AppSettings:ImagesPath"] + configuration["AppSettings:PathSeparator"] + "букви" +
                                            configuration["AppSettings:PathSeparator"] + char.ToUpper(c) + ".jpg",
                                        webRootPath + "/images/" + char.ToUpper(c) + ".jpg", true);
                                        var imageModel = new ImageMeaningModel();
                                        imageModel.Image = "/images/" + char.ToUpper(c) + ".jpg";
                                        imageModel.Meaning = char.ToUpper(c).ToString();
                                        model.Images.Add(imageModel);
                                        flag = 1;
                                    }
                                    //break;
                                }
                                if (flag != 1)
                                {
                                    if (word.EndsWith("a"))
                                    {
                                        var modifiedWord1 = word.Substring(0, word.LastIndexOf("а"));
                                        modifiedWord1 += "ува";
                                        var fileModExists1 = System.IO.File.Exists(configuration["AppSettings:ImagesPath"] +
                                        configuration["AppSettings:PathSeparator"] + modifiedWord1.ToLower() + ".jpg");
                                        if (fileModExists1)
                                        {
                                            var fileMod = configuration["AppSettings:ImagesPath"] +
                                            configuration["AppSettings:PathSeparator"] + modifiedWord1.ToLower() + ".jpg";
                                            var idx = fileMod.LastIndexOf(configuration["AppSettings:PathSeparator"]);
                                            _logger.LogInformation("Idx {idx}", idx);
                                            var fileName = fileMod.Substring(idx + 1);

                                            System.IO.File.Copy(fileMod.Substring(0, fileMod.LastIndexOf(configuration["AppSettings:PathSeparator"]) + 1) + fileName,
                                                webRootPath + "/images/" + fileName, true);
                                            var imageModel = new ImageMeaningModel();
                                            imageModel.Image = "/images/" + fileName;
                                            imageModel.Meaning = fileName.Split(".")[0];
                                            model.Images.Add(imageModel);
                                            flag = 1;
                                            //break;
                                        }

                                    }

                                    if(flag != 1)
                                    {
                                        if (word.EndsWith("и"))
                                        {
                                            var modifiedWord1 = word.Substring(0, word.LastIndexOf("и"));
                                            modifiedWord1 += "ува";
                                            var fileModExists1 = System.IO.File.Exists(configuration["AppSettings:ImagesPath"] +
                                            configuration["AppSettings:PathSeparator"] + modifiedWord1.ToLower() + ".jpg");
                                            if (fileModExists1)
                                            {
                                                var fileMod = configuration["AppSettings:ImagesPath"] +
                                                configuration["AppSettings:PathSeparator"] + modifiedWord1.ToLower() + ".jpg";
                                                var idx = fileMod.LastIndexOf(configuration["AppSettings:PathSeparator"]);
                                                _logger.LogInformation("Idx {idx}", idx);
                                                var fileName = fileMod.Substring(idx + 1);

                                                System.IO.File.Copy(fileMod.Substring(0, fileMod.LastIndexOf(configuration["AppSettings:PathSeparator"]) + 1) + fileName,
                                                    webRootPath + "/images/" + fileName, true);
                                                var imageModel = new ImageMeaningModel();
                                                imageModel.Image = "/images/" + fileName;
                                                imageModel.Meaning = fileName.Split(".")[0];
                                                model.Images.Add(imageModel);
                                                flag = 1;
                                                //break;
                                            }
                                        }

                                        if(flag != 1)
                                        {
                                            if (word.EndsWith("е"))
                                            {
                                                var modifiedWord1 = word.Substring(0, word.LastIndexOf("е"));
                                                modifiedWord1 += "ува";
                                                var fileModExists1 = System.IO.File.Exists(configuration["AppSettings:ImagesPath"] +
                                                configuration["AppSettings:PathSeparator"] + modifiedWord1.ToLower() + ".jpg");
                                                if (fileModExists1)
                                                {
                                                    var fileMod = configuration["AppSettings:ImagesPath"] +
                                                    configuration["AppSettings:PathSeparator"] + modifiedWord1.ToLower() + ".jpg";
                                                    var idx = fileMod.LastIndexOf(configuration["AppSettings:PathSeparator"]);
                                                    _logger.LogInformation("Idx {idx}", idx);
                                                    var fileName = fileMod.Substring(idx + 1);

                                                    System.IO.File.Copy(fileMod.Substring(0, fileMod.LastIndexOf(configuration["AppSettings:PathSeparator"]) + 1) + fileName,
                                                        webRootPath + "/images/" + fileName, true);
                                                    var imageModel = new ImageMeaningModel();
                                                    imageModel.Image = "/images/" + fileName;
                                                    imageModel.Meaning = fileName.Split(".")[0];
                                                    model.Images.Add(imageModel);
                                                    flag = 1;
                                                    //break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            
                                

                        }

                    }

                }


                if (flag == 0)
                {
                    var imageModel = new ImageMeaningModel();
                    imageModel.Image = "/images/noimage.jpg";
                    imageModel.Meaning = word.Trim();
                    model.Images.Add(imageModel);
                }

                //i++;
            }

            if (model.Images.Count == 0)
            {
                var imageModel = new ImageMeaningModel();
                imageModel.Image = "/images/noimage.jpg";
                imageModel.Image = "нема слика";
                model.Images = new List<ImageMeaningModel> { imageModel };
                return Json(new { images = model.Images });
            }

            if (System.IO.File.Exists(filePath))
            {
                // If file found, delete it    
                System.IO.File.Delete(filePath);
            }

            return Json(new { images = model.Images });

        }

        private Dictionary<string, string> Simplify(string text)
        {
            List<string> sentences = sentenceSplitting.Process(text);
            List<string> subSentences = sentenceSubsplitting.Process(sentences);
            Dictionary<string, string> taggedSents = tenseRecognition.TagSents(subSentences);          
            Dictionary<string, string> infinitive = this.infinitive.TurnVerbsToInfinitive(taggedSents);
            Dictionary<string, string> singular = pluralToSingular.ConvertToSinular(infinitive);
            Dictionary<string, string> stopwordsRemoved = stopWordsRemoval.RemoveStopWords(singular);
            Dictionary<string, string> pos = posTagger.PosTag(stopwordsRemoved);
            Dictionary<string, string> synonyms = phraseSynonyms.MapPhraseSynonyms(pos);
            //Dictionary<List<KeyValuePair<string, List<string>>>, string> synonyms = SignLanguageSimplification.SimplificationAlgorithm.Synonyms.FindSynonyms(singular);

            return synonyms;

        }

        [Authorize(Policy = "IsItAuth")]
        [HttpPost("UploadFiles")]
        public async Task<ActionResult> Upload(List<IFormFile> files, string meaning)
        {
            long size = files.Sum(f => f.Length);

            // full path to file in temp location
            string webRootPath = _hostingEnvironment.WebRootPath;
            var filePath = configuration["AppSettings:ImagePath"] + "/" + meaning + ".jpg";

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                { 
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }
            return View("Upload");
        }
    }
}
