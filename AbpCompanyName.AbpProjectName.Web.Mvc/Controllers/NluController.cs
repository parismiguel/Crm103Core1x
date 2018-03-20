using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IBM.WatsonDeveloperCloud.NaturalLanguageUnderstanding.v1;
using IBM.WatsonDeveloperCloud.NaturalLanguageUnderstanding.v1.Model;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;

namespace TemplateCoreParis.Controllers
{
    public class NluController : Controller
    {
        #region NLU parameters
        private static NaturalLanguageUnderstandingService _naturalLanguageUnderstandingService;

        static string userNLU = "41e3eac1-88e0-4ed9-aca2-b7c2d74bdcc1";
        static string pswNLU = "442L2dJkoqAO";

        string modelNLU = "";

        private string _nluText;
        private string _nluModel;
        #endregion

        public NluController()
        {
            _naturalLanguageUnderstandingService = new NaturalLanguageUnderstandingService(userNLU, pswNLU, NaturalLanguageUnderstandingService.NATURAL_LANGUAGE_UNDERSTANDING_VERSION_DATE_2017_02_27);
        }


        #region Constructor
        public AnalysisResults NaturalLanguageUnderstandingExample(string username, string password)
        {
            _naturalLanguageUnderstandingService = new NaturalLanguageUnderstandingService(username, password, NaturalLanguageUnderstandingService.NATURAL_LANGUAGE_UNDERSTANDING_VERSION_DATE_2017_02_27);

            AnalysisResults model = new AnalysisResults();

            model = Analyze(_nluModel, _nluText);
            return model;

        }
        #endregion

        #region Analyze
        public static AnalysisResults Analyze(string queryModelID, string _nluText)
        {
            _naturalLanguageUnderstandingService = new NaturalLanguageUnderstandingService(userNLU, pswNLU, NaturalLanguageUnderstandingService.NATURAL_LANGUAGE_UNDERSTANDING_VERSION_DATE_2017_02_27);

            List<string> model = new List<string>();

            List<string> _emotions = new List<string>()
            {
                "anger",
                "disgust",
                "fear",
                "joy",
                "sadness"
            };

            List<string> _targets = new List<string>()
            {
                "comida",
                "restaurante",
                "plato"
            };

            Parameters parameters = new Parameters()
            {
                Clean = true,
                FallbackToRaw = true,
                ReturnAnalyzedText = true,
                Features = new Features()
                {
                    Relations = new RelationsOptions(),
                    Sentiment = new SentimentOptions()
                    {
                        Document = true
                        //Targets = _targets
                    },
                    Emotion = new EmotionOptions()
                    {
                        Document = true
                        //Targets = _emotions
                    },
                    Keywords = new KeywordsOptions()
                    {
                        Limit = 50,
                        Sentiment = true,
                        Emotion = true
                    },
                    Entities = new EntitiesOptions()
                    {
                        Limit = 50,
                        Emotion = true,
                        Sentiment = true
                    },
                    Categories = new CategoriesOptions(),
                    Concepts = new ConceptsOptions()
                    {
                        Limit = 8
                    },
                    SemanticRoles = new SemanticRolesOptions()
                    {
                        Limit = 50,
                        Entities = true,
                        Keywords = true
                    }
                }
            };

            if (!string.IsNullOrEmpty(queryModelID))
            {
                parameters.Features.Relations.Model = queryModelID;
                parameters.Features.Entities.Model = queryModelID;
            }

            if (_nluText.StartsWith("http"))
            {
                parameters.Url = _nluText;
                parameters.Features.Metadata = new MetadataOptions();
            }
            else
            {
                parameters.Text = _nluText;
            }

            Console.WriteLine(string.Format("\nAnalizando()..."));

            AnalysisResults result = new AnalysisResults();

            try
            {
                result = _naturalLanguageUnderstandingService.Analyze(parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                result.AnalyzedText = ex.Message;
            }

            return result;
        }
        #endregion



    }
}