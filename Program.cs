using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Xceed.Words.NET;  //libreria per usare file .docx
using Newtonsoft.Json;  //libreria per il JSON
using System.Net.Http;
using JoinValueXML;
using Spectre.Console;

namespace XmlTranslator
{
    class Program
    {
        #region Child and File names
        static string key = "<google translate api key>";

        static string mainChild = "Value";                 //name of child in xml
        static string inputFileXml = "input.xml";
        static string originalAsTxt = "original.txt";
        static string googleFormatReady = "googleFormat.docx";
        static string googleFormatAsTxt = "googleFormat.txt";
        static string translations = "translations.txt";
        static string targetLanguage;
        #endregion

        #region Main

        /// <summary>
        /// Entry point of the program. Displays a welcome banner and handles the main execution flow.
        /// Based on user input, it either translates XML content using the Google API or prepares files for manual translation.
        /// </summary>
        /// <param name="args">Command-line arguments (not used in this implementation).</param>
        static void Main(string[] args)
        {
            AnsiConsole.Write(
                new FigletText("XML translator")
                    .Centered()
                    .Color(Color.Red));

            if (!File.Exists("original.txt"))
                CopyXMLToTXT(inputFileXml, originalAsTxt);
            int choice = ExecutionMenu();
            switch (choice)
            {
                case 0:
                    {
                        SplitInTxtGoogleFormat();
                        choice = TranslationMenu();

                        switch (choice)
                        {
                            case 0:
                                foreach (string l in Enum.GetNames(typeof(Languages)))
                                {
                                    TranslatingUsingGoogleAPI(googleFormatAsTxt, translations, key, l);
                                    JoinXMLFormat(l);
                                }
                                break;

                            case 1:
                                targetLanguage = TargetLanguageMenu();
                                TranslatingUsingGoogleAPI(googleFormatAsTxt, translations, key, targetLanguage);
                                JoinXMLFormat(targetLanguage);
                                break;

                            //case 2 : scelta multipla ma non tutte le lingue (altrimenti case 0)
                            case 2:
                                
                                List<string> selectedLanguages = MultiLanguageMenu();
                                foreach (string l in selectedLanguages)
                                {
                                    TranslatingUsingGoogleAPI(googleFormatAsTxt, translations, key, l);
                                    JoinXMLFormat(l);
                                }
                                break;

                            default:
                                Console.WriteLine("incorrect choice");
                                break;
                        }
                        break;
                    }
                case 1:
                    {
                        SplitInDocXGoogleFormat();
                        break;
                    }
                default:
                    break;
            }

        }
        #endregion

        #region Methods

        /// <summary>
        /// Displays a menu to allow the user to select multiple target languages for translation.
        /// The user can toggle languages using the spacebar and confirm the selection with the enter key.
        /// </summary>
        /// <returns>A list of selected languages as their two-letter codes.</returns>
        static List<string> MultiLanguageMenu()
        {
            List<string> s = new List<string>();
            List<string> selectedLanguages = new List<string>();
            for (int i = 0; i < Enum.GetNames(typeof(Languages)).Length; i++)
                s.Add(string.Format("Translate original into {0}", Enum.GetName(typeof(Languages), i)));

            var languages = AnsiConsole.Prompt(
                new MultiSelectionPrompt<string>()
                    .Title("[bold yellow]Select two or more languages[/]")
                    .InstructionsText(
                        "[grey](Press [blue]<space>[/] to toggle a language, " +
                        "[green]<enter>[/] to accept)[/]")
                    .AddChoices(s));
            foreach (string l in Enum.GetNames(typeof(Languages)))
            {
                foreach (string lang in languages)
                {
                    if (lang.Contains(l))
                        selectedLanguages.Add(l);
                }
            }
            return selectedLanguages;
        }


        /// <summary>
        /// Displays a menu to allow the user to choose a translation option.
        /// The options include translating into all available languages, one chosen language, or a selection of multiple languages.
        /// </summary>
        /// <returns>
        /// An integer representing the user's choice:
        /// 0 - Translate into all available languages.
        /// 1 - Translate into one chosen language.
        /// 2 - Translate into a selection of multiple languages.
        /// </returns>
        static int TranslationMenu()
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold yellow]select one option[/]")
                    .AddChoices(new[] {
                        "Translate original into all available languages",
                        "Translate original into one chosen language",
                        "Translate original into all languages you select"
                    }));
            if(choice == "Translate original into all available languages")
                return 0;
            if (choice == "Translate original into one chosen language")
                return 1;
            else
                return 2;
        }


        /// <summary>
        /// Displays a menu to allow the user to select the execution mode for the program.
        /// The options include automatic translation using the Google API, creating .docx files for manual translation, or exiting the program.
        /// </summary>
        /// <returns>
        /// An integer representing the user's choice:
        /// 0 - Automatically translate using Google API.
        /// 1 - Only create .docx files for manual translation.
        /// -1 - Exit the program.
        /// </returns>
        static int ExecutionMenu()
        {

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold yellow]select the execution mode:[/]")
                    .AddChoices(new[] {
                        "Automatically translate using Google API",
                        "Only create .docx files to use in Google Translate Web Page",
                        "Exit"
                        
                    }));

            if(choice == "Automatically translate using Google API")
                return 0;
            if(choice == "Only create .docx files to use in Google Translate Web Page")
                return 1;
            else
                return -1;
        }


        /// <summary>
        /// Menu to decide the target language
        /// </summary>
        /// <returns>Target language chosen</returns>
        static string TargetLanguageMenu()
        {
            List<string> s = new List<string>();
            for (int i = 0; i < Enum.GetNames(typeof(Languages)).Length; i++)
                s.Add(string.Format("Translate original into {0}", Enum.GetName(typeof(Languages), i)));

            var twoLetters = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold yellow]select one language:[/]")
                    .MoreChoicesText("[grey](Move up and down to reveal more languages)[/]")
                    .AddChoices(s));

            foreach (string l in Enum.GetNames(typeof(Languages)))
            {
                if (twoLetters.Contains(l))
                    return l;
            }
            return null;
        }


        /// <summary>
        /// Copies the contents of an XML document in a TXT document
        /// </summary>
        /// <param name="pathXML">Path of the .xml document</param>
        /// <param name="pathTXT">Path of the .txt document and also the result of execution</param>
        static void CopyXMLToTXT(string pathXML, string pathTXT)
        {
            StreamReader input = new StreamReader(pathXML);
            string inputAsString = input.ReadToEnd();
            input.Close();
            StreamWriter original = new StreamWriter(pathTXT);
            original.Write(inputAsString);
            original.Close();
        }


        /// <summary>
        /// Creates Txt file ready to make the attributes of the children translated
        /// </summary>
        static void SplitInDocXGoogleFormat()
        {
            StreamReader original = new StreamReader(originalAsTxt);

            var googleFormatDoc = DocX.Create(googleFormatReady);

            while (!original.EndOfStream)
            {

                string line = original.ReadLine();

                if (line.Contains(string.Format("<{0}>", mainChild)))
                {
                    string formattedLine = MakeChild(line, mainChild.First().ToString());

                    googleFormatDoc.InsertParagraph(formattedLine);     //Line in Google Translate format in DOCX
                }

            }
            original.Close();
            googleFormatDoc.Save();
        }
        

        /// <summary>
        /// Creates Txt file ready to make the attributes of the children translated
        /// </summary>
        static void SplitInTxtGoogleFormat()
        {
            StreamReader original = new StreamReader(originalAsTxt);

            StreamWriter googleFormat = new StreamWriter(googleFormatAsTxt);

            while (!original.EndOfStream)
            {

                string line = original.ReadLine();

                if (line.Contains(string.Format("<{0}>", mainChild)))
                {
                    string formattedLine = MakeChild(line, mainChild.First().ToString());

                    googleFormat.WriteLine(formattedLine);  //Line in Google Translate format in TXT
                }

            }
            original.Close();
            googleFormat.Close();
        }


        /// <summary>
        /// Translation of a word using the google api
        /// </summary>
        /// <param name="apiKey">key of google api</param>
        /// <param name="twoLetterLenguage">target language</param>
        /// <param name="text">text to translate</param>
        /// <returns>Word translated</returns>
        static string TranslateViaGoogle(string apiKey, string twoLetterLenguage, string text)
        {
            HttpClient googleClient = new HttpClient();
            string apiUrl = $"https://translation.googleapis.com/language/translate/v2?key=" + apiKey;
            var request = new TranslateRequest()
            {
                q = text,
                target = twoLetterLenguage
            };

            string jsonStringaModelloRichiesta = JsonConvert.SerializeObject(request);
            StringContent content = new StringContent(jsonStringaModelloRichiesta, Encoding.UTF8, "application/json");

            var res = googleClient.PostAsync(apiUrl, content).Result;
            if (res.IsSuccessStatusCode)
            {
                var obj = JsonConvert.DeserializeObject<TranslateResponse>(res.Content.ReadAsStringAsync().Result);
                return obj.Datas.Translations[0].TranslatedText;
            }
            else
            {
                //cosa faccio in caso di errore?? nulla
                return text;
            }
        }


        /// <summary>
        /// Translates a whole file in a target language using google api translator
        /// </summary>
        /// <param name="inputPath">File with the contents to translate</param>
        /// <param name="outPath">File that will contain the output of the execution</param>
        /// <param name="apiKey">Google Translate Api key</param>
        /// <param name="targetLanguage">Two letter target language</param>
        static void TranslatingUsingGoogleAPI(string inputPath, string outPath, string apiKey, string targetLanguage)
        {
            StreamReader inputReader = new StreamReader(inputPath);
            StreamWriter outWriter = new StreamWriter(outPath);
            File.OpenRead(inputPath);
            int length = File.ReadAllLines(inputPath).Length;
            AnsiConsole.Progress()
                    .AutoRefresh(true) // Turn off auto refresh
                    .AutoClear(false)   // Do not remove the task list when done
                    .HideCompleted(false)   // Hide tasks as they are completed
                    .Columns(new ProgressColumn[]
                    {
                        new TaskDescriptionColumn(),    // Task description
                        new ProgressBarColumn(),        // Progress bar
                        new PercentageColumn(),         // Percentage
                        new SpinnerColumn(),            // Spinner
                    })
                    .Start(ctx =>
                    {
                        var task1 = ctx.AddTask($"[green]Translating {targetLanguage.ToUpper()}[/]");

                        while (!inputReader.EndOfStream)
                        {
                            string translatedLine = TranslateViaGoogle(apiKey, targetLanguage, inputReader.ReadLine());
                            outWriter.WriteLine(translatedLine);
                            task1.Increment(100.0/length);
                        }
                        task1.Value = 100;
                    });          
            inputReader.Close();
            outWriter.Close();
        }


        /// <summary>
        /// Replace the original attributes of each child with the new ones
        /// </summary>
        static void JoinXMLFormat(string lingua)
        {
            StreamReader originale = new StreamReader(originalAsTxt);                               //original file
            StreamReader traduzioni = new StreamReader(translations);                               //translations
            StreamWriter output = new StreamWriter("output\\output_" + lingua + ".txt");            //result of execution as text
            StreamWriter outputXML = new StreamWriter("output\\output_" + lingua + ".xml");         //xml result
            while (!originale.EndOfStream)  //making of xml format in result files
            {
                string line = originale.ReadLine();
                if (line.Contains(string.Format("<{0}>", mainChild)))
                {
                    line = MakeChild(traduzioni.ReadLine(), "Value");
                }
                output.WriteLine(line);
                outputXML.WriteLine(line);
            }
            outputXML.Close();
            output.Close();
            originale.Close();
            traduzioni.Close();
        }


        /// <summary>
        /// Makes an XML formatted line of a child with it's attribute
        /// </summary>
        /// <param name="attribute">Attribute of the child</param>
        /// <param name="child">Child's name</param>
        /// <returns>An XML formatted child line</returns>
        static string MakeChild(string attribute, string child)
        {
            return string.Format("<{0}>{1}</{2}>", child, TakeValue(attribute), child);
        }


        /// <summary>
        /// Takes the attribute from a child line
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns>Attribute of the child line</returns>
        static string TakeValue(string attribute)
        {
            if ((attribute.Count(x => x == '<') != 2 || attribute.Count(x => x == '>') != 2) || (!attribute.Contains("</")))
                return attribute;
            if (attribute.LastIndexOf(">") == attribute.IndexOf(">"))
                return null;
            return attribute.Substring(attribute.IndexOf(">") + 1, attribute.LastIndexOf("<") - attribute.IndexOf(">") - 1);
        }

        #endregion

        public enum Languages
        {
            en,   //english
            it,   //italian
            de,   //german
            fr,   //french
            es,   //spanish
            sv,   //swedish
            cs,   //czech
            vi,   //vietnamese
            nl,   //dutch
            hu    //hungarian
        }
    }
}