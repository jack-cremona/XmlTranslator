# XML Translator

XML Translator is a C# application designed to translate XML files into multiple languages using the Google Translate API. It also provides options to prepare files for manual translation using Google Translate's web interface.

## Features

- Automatically translate XML content into multiple languages using the Google Translate API.
- Prepare `.docx` or `.txt` files for manual translation.
- Support for multiple languages, including English, Italian, German, French, Spanish, and more.
- User-friendly CLI menus powered by [Spectre.Console](https://spectreconsole.net/).

## Prerequisites

- .NET Framework 4.7.2
- A valid Google Translate API key
- NuGet packages:
  - [Spectre.Console](https://www.nuget.org/packages/Spectre.Console/)
  - [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/)
  - [Xceed.Words.NET](https://www.nuget.org/packages/Xceed.Words.NET/)

## Installation

1. Clone the repository:
   
2. Open the project in Visual Studio.

3. Restore NuGet packages:
   
4. Replace `<google translate api key>` in the `Program.cs` file with your Google Translate API key.

5. Build the project.

## Usage

1. Run the application:
   
2. Follow the on-screen prompts to:
   - Automatically translate XML content.
   - Prepare `.docx` or `.txt` files for manual translation.
   - Select target languages for translation.

## Supported Languages

The application supports the following languages:
- English (`en`)
- Italian (`it`)
- German (`de`)
- French (`fr`)
- Spanish (`es`)
- Swedish (`sv`)
- Czech (`cs`)
- Vietnamese (`vi`)
- Dutch (`nl`)
- Hungarian (`hu`)

## File Structure

- `input.xml`: The input XML file to be translated.
- `original.txt`: A plain text version of the input XML.
- `googleFormat.docx`: A `.docx` file formatted for manual translation.
- `googleFormat.txt`: A `.txt` file formatted for manual translation.
- `translations.txt`: The translated content.
- `output/`: Directory containing translated XML and text files.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Acknowledgments

- [Spectre.Console](https://spectreconsole.net/) for creating beautiful CLI interfaces.
- [Newtonsoft.Json](https://www.newtonsoft.com/json) for JSON serialization.
- [Xceed.Words.NET](https://github.com/xceedsoftware/DocX) for working with `.docx` files.
