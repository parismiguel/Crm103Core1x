using Newtonsoft.Json;

namespace IBM.VCA.Watson.Watson.SpeechToText.Model
{
    public class SupportedFeatures
    {
        [JsonProperty("custom_language_model")]
        public bool CustomLanguageModel { get; set; }

        [JsonProperty("speaker_labels")]
        public bool SpeakerLabels { get; set; }
    }
}