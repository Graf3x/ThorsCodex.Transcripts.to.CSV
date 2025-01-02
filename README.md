## ThorsCodex.Transcripts.to.CSV

Quick tool to grab transcripts from YouTube and dump them to CSV.

### Installation

1. Clone the repository.
2. Install the required NuGet packages:
```sh
	dotnet add package McMaster.Extensions.CommandLineUtils
	dotnet add package YoutubeExplode
	dotnet add package CsvHelper
```
3. Build the project:
```sh
	dotnet build
```
### Usage

This tool fetches videos from a specified YouTube channel, processes the closed captions, and saves them as CSV files. It uses command line arguments to specify the channel URL, output directory, length cutoff for videos, and YouTube handle name.

### Command Line Arguments

- `-u` or `--url`: The URL of the YouTube channel (default: `https://www.youtube.com/@PirateSoftware`)
- `-o` or `--output`: The output directory for transcripts (default: `S:\\Transcripts`)
- `-l` or `--length`: The length cutoff for videos in `hh:mm:ss` format (default: `01:03:00`)
- `-h` or `--handle`: The YouTube handle name (default: `PirateSoftware`)

### Example
```sh
dotnet run --url "https://www.youtube.com/@PirateSoftware" --output "S:\Transcripts" --length "01:03:00" --handle "PirateSoftware"
```

### Contributing

Feel free to submit issues or pull requests if you have any improvements or bug fixes.
