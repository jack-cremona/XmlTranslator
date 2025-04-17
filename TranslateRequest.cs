using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace JoinValueXML
{
    public class TranslateRequest
    {
        [JsonProperty("q")]
        public string q { get; set; }

        [JsonProperty("target")]
        public string target { get; set; }
    }
    public class TranslateResponse
    {

        [JsonProperty("data")]
        public Data Datas { get; set; }

    }
    public class Translation
    {
        [JsonProperty("translatedText")]
        public string TranslatedText { get; set; }

        [JsonProperty("detectedSourceLanguage")]
        public string DetectedSourceLanguage { get; set; }
    }

    public class Data
    {
        [JsonProperty("translations")]
        public List<Translation> Translations { get; set; }
    }
}
