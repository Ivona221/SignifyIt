using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SignLanguageWebCore;
using SignLanguageWebCoreAuth.Data;
using SignLanguageWebCoreAuth.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Aerospike.Client;
using Microsoft.Extensions.Configuration;
using SignLanguageSimplification.SimplificationAlgorithm;
using Synonyms = SignLanguageWebCoreAuth.Models.Synonyms;
using MongoDB.Driver;
using SignLanguageWebCoreAuth.SimplificationAlgorithm;
using SignLanguageWebCoreAuth.SimplificationAlgorithm.Models;

namespace SignLanguageWebCoreAuth.Controllers
{
    [Authorize(Policy = "IsItAuth")]
    public class SynonymsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        IConfiguration configuration;
        private ITenseRecognition tenseRecognition;
        private IStopWordsRemoval stopWordsRemoval;
        private IInfinitive infinitive;
        private IPluralToSingular pluralToSingular;

        public SynonymsController(IConfiguration _configuration, ITenseRecognition _tenseRecognition,
            IStopWordsRemoval _stopWordsRemoval, IInfinitive _infinitive, IPluralToSingular _pluralToSingular)
        {
            configuration = _configuration;
            tenseRecognition = _tenseRecognition;
            stopWordsRemoval = _stopWordsRemoval;
            infinitive = _infinitive;
            pluralToSingular = _pluralToSingular;
        }

        // GET: Synonyms
        public ActionResult Index()
        {
            var model = new SynonymsPageViewModel();

            var tags = db.Tags.ToList();
            model.Tags = tags;

            var synonyms = db.Synonyms.ToList();
            model.Synonyms = synonyms;

            return View(model);
        }

        // GET: Synonyms/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // POST: Synonyms/Create
        [HttpPost]
        public JsonResult Create([FromBody]SynonymsPageViewModel synonymsModel)
        {
            //var synonymsModel = JsonConvert.DeserializeObject<SynonymsPageViewModel>(synonymsString);

            Synonyms synonym = new Synonyms();
            var simplified = Simplify(synonymsModel.Phrase);
            synonym.Simplified = simplified;
            synonym.Phrase = synonymsModel.Phrase;
            synonym.RelatedWord = synonymsModel.RelatedWord;
            synonym.Tag = synonymsModel.Tag;

            var synonymsDb = db.Synonyms.Where(x => x.Phrase == synonymsModel.Phrase && x.RelatedWord == synonymsModel.RelatedWord).FirstOrDefault();

            if(synonymsDb != null)
            {
                return Json(new { MessageТype = MessageType.Info, Message = "Овој збор веќе постои" });
            }

            try
            {
               
                // insert
                using (var db = new ApplicationDbContext())
                {
                    var synonyms = db.Set<Synonyms>();
                    synonyms.Add(synonym);

                    db.SaveChanges();
                }
                AerospikeClient client = new AerospikeClient(configuration["AppSettings:AerospikeClient"], 3000);
                //var _client = new MongoClient();
                //var _database = _client.GetDatabase("SignLanguage");

//                var _client = new MongoClient();
//                var _database = _client.GetDatabase("SignLanguage");
//                var _collection = _database.GetCollection<PhraseSynonymModel>("PhraseSynonyms");

                PhraseSynonymModel model = new PhraseSynonymModel();
                model.Simplified = simplified;
                model.Original = synonymsModel.Phrase;
                model.Synonym = synonymsModel.RelatedWord;

                WritePolicy policyWrite = new WritePolicy();
                policyWrite.SetTimeout(50);  // 50 millisecond timeout.
                Key keyWrite = new Key("sign-language", "Infinitive", simplified);
                Bin simpf = new Bin("Simplified", simplified);
                Bin original = new Bin("Original", synonymsModel.Phrase);
                Bin syn = new Bin("Infinitive", synonymsModel.RelatedWord);
                client.Put(policyWrite, keyWrite, simpf, original, syn);
                //_collection.InsertOne(inf);

                //_collection.InsertOne(model);

                return Json(new { MessageТype = MessageType.Success, Message = "Успешно зачувување" });
            }
            catch
            {
                return Json(new { MessageТype = MessageType.Error, Message = "Грешка" });
            }
        }

        private string Simplify(string word)
        {
            Dictionary<string, string> taggedSents = tenseRecognition.TagSents(new List<string>{word});
            Dictionary<string, string> stopwordsRemoved = stopWordsRemoval.RemoveStopWords(taggedSents);
            Dictionary<string, string> infinitive = this.infinitive.TurnVerbsToInfinitive(stopwordsRemoved);
            Dictionary<string, string> singular = pluralToSingular.ConvertToSinular(infinitive);

            return singular.First().Key;
        }

        // GET: Synonyms/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Synonyms/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Synonyms/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Synonyms/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        private bool ValidateModel(SynonymsPageViewModel synonymsModel)
        {
            if (string.IsNullOrEmpty(synonymsModel.Phrase) ||
                string.IsNullOrEmpty(synonymsModel.RelatedWord) ||
                string.IsNullOrEmpty(synonymsModel.Tag))
            {
                return false;
            }

            return true;

        }
    }
}
