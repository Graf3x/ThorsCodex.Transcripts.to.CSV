# Thor's Codex
Thor's Codex is a project that aims to index all of Pirate Software's available VODs from YouTube and the content of the Discord channel. The project provides a search engine for the words said during these VODs, making it easier to find specific moments and information. In the future, we plan to run a local RAG and LLM to summarize large parts of the VODs. This page hosts the HTML and JavaScript to interact with the application.

## Project Overview

- **Project Name:** Thor's Codex
- **Description:** A static HTML/JS page to host an index of Pirate Software's VODs and Discord content, providing a searchable interface.
- **Future Plans:** Implement a local RAG and LLM to summarize VOD content.

## ThorsCodex.Transcripts.to.CSV

Quick tool to grab the transcripts from YouTube and dump them to CSV.


### Installation

1. Clone the repository.
2. Install the required NuGet packages:
	dotnet add package McMaster.Extensions.CommandLineUtils dotnet add package YoutubeExplode dotnet add package CsvHelper
3. Build the project:
	dotnet build
### Usage

This tool fetches videos from a specified YouTube channel, processes the closed captions, and saves them as CSV files. It uses command line arguments to specify the channel URL, output directory, length cutoff for videos, and YouTube handle name.

### Command Line Arguments

- `-u` or `--url`: The URL of the YouTube channel (default: `https://www.youtube.com/@PirateSoftware`)
- `-o` or `--output`: The output directory for transcripts (default: `S:\\Transcripts`)
- `-l` or `--length`: The length cutoff for videos in `hh:mm:ss` format (default: `01:03:00`)
- `-h` or `--handle`: The YouTube handle name (default: `PirateSoftware`)

### Example
dotnet run --url "https://www.youtube.com/@PirateSoftware" --output "S:\Transcripts" --length "01:03:00" --handle "PirateSoftware"

### Contributing

Feel free to submit issues or pull requests if you have any improvements or bug fixes.

3. Build the project: