using AbpCompanyName.AbpProjectName.Web.Controllers;
using IBM.VCA.Watson.Watson.Http.Extensions;
using IBM.VCA.Watson.Watson.TextToSpeech.Model;
using System;
using System.IO;
using TemplateCoreParis.Controllers;

namespace IBM.VCA.Watson.Watson.TextToSpeech
{
    public class TextToSpeechServiceExample
    {
        private TextToSpeechService _textToSpeech = new TextToSpeechService();
        private string _voice = "es-LA_SofiaVoice";
        private string _pronunciation = "Watson";
        private string _text = "I'm sorry, Dave. I'm afraid I can't do that.";
        private string _customVoiceModelID;
        private string _customVoiceModelName = "dotnet-standard-custom-voice-model";
        private string _customVoiceModelDescription = "Custom voice model created by the .NET Standard SDK.";
        private string _customVoiceModelLanguage = "es-ES";
        private string _customVoiceModelUpdatedLanguage = "es-ES";

        static string _username = "68872ebf-c244-4f0e-b1cd-99b17b448ddb";
        static string _password = "Mw8CKC5G2EPb";

        public TextToSpeechServiceExample()
        {
            _textToSpeech.SetCredential(_username, _password);

            //GetVoices();
            //GetVoice();
            //GetPronunciation();
            Synthesize();
            //GetCustomVoiceModels();
            //SaveCustomVoiceModel();
            //UpdateCustomVoiceModel();
            //GetCustomVoiceModel();
            //SaveWords();
            //GetWords();
            //DeleteWord();
            //DeleteCustomVoiceModel();
        }

        #region Get Voices
        private void GetVoices()
        {
            Console.WriteLine("Calling GetVoices()...");

            var results = _textToSpeech.GetVoices();

            if (results != null && results.Count > 0)
            {
                Console.WriteLine("Voices found...");
                foreach (Voice voice in results)
                    Console.WriteLine(string.Format("name: {0} | language: {1} | gender: {2} | description {3}",
                        voice.Name,
                        voice.Language,
                        voice.Gender,
                        voice.Description));
            }
            else
            {
                Console.WriteLine("There are no voices.");
            }
        }
        #endregion

        #region Get Voice
        private void GetVoice()
        {
            Console.WriteLine(string.Format("Calling GetVoice({0})...", _voice));

            var results = _textToSpeech.GetVoice(_voice);

            if (results != null)
            {
                Console.WriteLine(string.Format("Voice {0} found...", _voice));
                Console.WriteLine(string.Format("name: {0} | language: {1} | gender: {2} | description {3}",
                        results.Name,
                        results.Language,
                        results.Gender,
                        results.Description));
            }
            else
            {
                Console.WriteLine("Voice not found.");
            }
        }
        #endregion

        #region Get Pronunciation
        private void GetPronunciation()
        {
            Console.WriteLine(string.Format("Calling GetPronunciation({0})...", _pronunciation));

            var results = _textToSpeech.GetPronunciation(_pronunciation);

            if (results != null)
            {
                Console.WriteLine(string.Format("Pronunciation: {0}", results.Value));
            }
            else
            {
                Console.WriteLine(string.Format("Unable to get pronunciation for {0}...", _pronunciation));
            }
        }
        #endregion

        #region Synthesize
        private void Synthesize()
        {
            Console.WriteLine(string.Format("Calling Synthesize({0})...", _text));

            var results = _textToSpeech.Synthesize(_text, Voice.EN_ALLISON, AudioType.WAV);

            if (results != null)
            {
                Console.WriteLine(string.Format("Succeeded to synthesize {0} | stream length: {1}", _text, results.Length));
            }
            else
            {
                Console.WriteLine("Failed to synthesize.");
            }
        }
        #endregion

        public static string Synthesize2(string text)
        {
            TextToSpeechService _textToSpeech = new TextToSpeechService();
            _textToSpeech.SetCredential(_username, _password);

            var sample = _textToSpeech.Synthesize(text, Voice.ES_SOFIA, AudioType.WAV);

            string file = "sample_" + Guid.NewGuid() + ".wav";

            string path = Path.Combine(HomeController._wwwRoot.WebRootPath, "audio", file);

            foreach (string f in Directory.EnumerateFiles(Path.Combine(HomeController._wwwRoot.WebRootPath, "audio"), "sample_*.wav"))
            {
                File.Delete(f);
            }


            using (var fileStream = File.Create(path))
            {
                sample.CopyTo(fileStream);
            }

            return path;
        }


        public static byte[] SynthesizeToByteArray(string text)
        {
            TextToSpeechService _textToSpeech = new TextToSpeechService();
            _textToSpeech.SetCredential(_username, _password);

            var sample = _textToSpeech.Synthesize(text, Voice.ES_SOFIA, AudioType.WAV);

            byte[] result = StreamExtension.ReadAllBytes(sample);

            return result;
        }

        #region Get Custom Voice Models
        private void GetCustomVoiceModels()
        {
            Console.WriteLine("Calling GetCustomVoiceModels()...");

            var results = _textToSpeech.GetCustomVoiceModels();

            if (results != null && results.Count > 0)
            {
                Console.WriteLine("Voice models found...");

                foreach (CustomVoiceModel voiceModel in results)
                {
                    Console.WriteLine(string.Format("Name: {0} | Id: {1} | Language: {2} | Description: {3}",
                        voiceModel.Name,
                        voiceModel.Id,
                        voiceModel.Language,
                        voiceModel.Description));
                }
            }
            else
            {
                Console.WriteLine("There are no custom voice models.");
            }
        }
        #endregion

        #region Save Custom Voice Model
        private void SaveCustomVoiceModel()
        {
            Console.WriteLine("Calling SaveCustomVoiceModel()...");
            CustomVoiceModel voiceModel = new CustomVoiceModel()
            {
                Name = _customVoiceModelName,
                Description = _customVoiceModelDescription,
                Language = _customVoiceModelLanguage
            };

            var results = _textToSpeech.SaveCustomVoiceModel(voiceModel);

            if (results != null)
            {
                Console.WriteLine("Custom voice model created...");

                _customVoiceModelID = results.Id;

                Console.WriteLine(string.Format("Name: {0} | Id: {1} | Language: {2} | Description: {3}",
                        results.Name,
                        results.Id,
                        results.Language,
                        results.Description));
            }
            else
            {
                Console.WriteLine("Failed to create custom voice model.");
            }
        }
        #endregion

        #region Update Custom Voice Model
        private void UpdateCustomVoiceModel()
        {
            if (string.IsNullOrEmpty(_customVoiceModelID))
                throw new ArgumentNullException("customVoiceModelID");

            Console.WriteLine(string.Format("Calling UpdateCustomVoiceModel({0})...", _customVoiceModelID));
            CustomVoiceModel voiceModel = new CustomVoiceModel()
            {
                Name = _customVoiceModelName,
                Description = _customVoiceModelDescription,
                Language = _customVoiceModelUpdatedLanguage,
                Id = _customVoiceModelID
            };

            var results = _textToSpeech.SaveCustomVoiceModel(voiceModel);

            if (results != null)
            {
                Console.WriteLine("Custom voice model updated...");

                Console.WriteLine(string.Format("Name: {0} | Id: {1} | Language: {2} | Description: {3}",
                        results.Name,
                        results.Id,
                        results.Language,
                        results.Description));
            }
            else
            {
                Console.WriteLine("Failed to update custom voice model.");
            }
        }
        #endregion

        #region Get Custom Voice Model
        private void GetCustomVoiceModel()
        {
            if (string.IsNullOrEmpty(_customVoiceModelID))
                throw new ArgumentNullException("customVoiceModelID");

            Console.WriteLine(string.Format("Calling GetCustomVoiceModel({0})...", _customVoiceModelID));

            var results = _textToSpeech.GetCustomVoiceModel(_customVoiceModelID);

            if (results != null)
            {
                Console.WriteLine(string.Format("Name: {0} | Id: {1} | Language: {2} | Description: {3}",
                        results.Name,
                        results.Id,
                        results.Language,
                        results.Description));
            }
            else
            {
                Console.WriteLine(string.Format("Failed to find custom voice model {0} ...", _customVoiceModelID));
            }
        }
        #endregion

        #region Delete Custom Voice Model
        private void DeleteCustomVoiceModel()
        {
            if (string.IsNullOrEmpty(_customVoiceModelID))
                throw new ArgumentNullException("customVoiceModelID");

            Console.WriteLine(string.Format("Calling DeleteCustomVoiceModel({0})...", _customVoiceModelID));
            _textToSpeech.DeleteCustomVoiceModel(_customVoiceModelID);
            Console.WriteLine(string.Format("Custom voice model {0} deleted.", _customVoiceModelID));
        }
        #endregion

        #region Get Words
        private void GetWords()
        {
            if (string.IsNullOrEmpty(_customVoiceModelID))
                throw new ArgumentNullException("customVoiceModelID");

            Console.WriteLine(string.Format("Calling GetWords({0})...", _customVoiceModelID));

            var results = _textToSpeech.GetWords(_customVoiceModelID);

            if (results != null && results.Count > 0)
            {
                Console.WriteLine("Custom words found.");
                foreach (CustomWordTranslation word in results)
                {
                    Console.WriteLine(string.Format("word: {0} | translation: {1}", word.Word, word.Translation));
                }
            }
            else
            {
                Console.WriteLine(string.Format("There are no words for custom voice model {0}...", _customVoiceModelID));
            }
        }
        #endregion

        #region Save Words
        private void SaveWords()
        {
            Console.WriteLine("Calling SaveWords()...");

            CustomWordTranslation ibm = new CustomWordTranslation()
            {
                Word = "IBM",
                Translation = "eye bee M"
            };

            CustomWordTranslation iPhone = new CustomWordTranslation()
            {
                Word = "iPhone",
                Translation = "i Phone"
            };

            CustomWordTranslation jpl = new CustomWordTranslation()
            {
                Word = "jpl",
                Translation = "J P L"
            };

            _textToSpeech.SaveWords(_customVoiceModelID, ibm, iPhone, jpl);

            Console.WriteLine("Words saved.");
        }
        #endregion

        #region Delete Word
        private void DeleteWord()
        {
            Console.WriteLine(string.Format("Calling DeleteWords({0})...", "jpl"));

            _textToSpeech.DeleteWord(_customVoiceModelID, "jpl");

            Console.WriteLine("Word deleted.");
        }
        #endregion
    }
}
