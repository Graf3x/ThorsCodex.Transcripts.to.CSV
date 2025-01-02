using System.Globalization;
using YoutubeExplode.Common;
using CsvHelper;
using McMaster.Extensions.CommandLineUtils;

class Program
{
    [Option(Description = "The URL of the YouTube channel", ShortName = "u", LongName = "url")]
    public string ChannelUrl { get; set; } = "https://www.youtube.com/@PirateSoftware";

    [Option(Description = "The output directory for transcripts", ShortName = "o", LongName = "output")]
    public string OutputDirectory { get; set; } = "S:\\Transcripts";

    [Option(Description = "The length cutoff for videos (in hh:mm:ss format)", ShortName = "l", LongName = "length")]
    public TimeSpan LengthCutOff { get; set; } = new TimeSpan(1, 3, 0);

    [Option(Description = "The YouTube handle name", ShortName = "h", LongName = "handle")]
    public string YTHandleName { get; set; } = "PirateSoftware";

    static async Task Main(string[] args)
    {
        var app = new CommandLineApplication<Program>();
        app.Conventions.UseDefaultConventions();
        await app.ExecuteAsync(args);
    }

    private async Task OnExecuteAsync()
    {
        var youtube = new YoutubeExplode.YoutubeClient();

        Console.WriteLine($"Fetching videos from channel: {ChannelUrl}");

        Directory.CreateDirectory(OutputDirectory);

        try
        {
            var channel = await youtube.Channels.GetByHandleAsync(YTHandleName);
            Console.WriteLine($"Found channel: {channel.Title}");

            var videos = await youtube.Channels.GetUploadsAsync(channel.Id).CollectAsync();

            // Filter for livestreams
            foreach (var video in videos)
            {
                // Get detailed video info to check if it's a pants, shorts, or vod
                var videoDetails = await youtube.Videos.GetAsync(video.Id);

                // Check if this was a vod, most pants are less than an hour and a half so thats the catch-all
                if (!videoDetails.Description.Contains("Streamed", StringComparison.OrdinalIgnoreCase) &&
                    !videoDetails.Description.Contains("Live stream", StringComparison.OrdinalIgnoreCase) &&
                    !(videoDetails.Duration > LengthCutOff))
                {
                    Console.WriteLine($"Skipping non-livestream: {video.Title}, {videoDetails.Description}");
                    continue;
                }

                Console.WriteLine($"Processing livestream: {video.Title}");
                try
                {
                    // Get closed captions tracks
                    var trackManifest = await youtube.Videos.ClosedCaptions.GetManifestAsync(video.Id);
                    var track = trackManifest.GetByLanguage("en");

                    if (track != null)
                    {
                        // Download the actual captions
                        YoutubeExplode.Videos.ClosedCaptions.ClosedCaptionTrack closedCaptions = await youtube.Videos.ClosedCaptions.GetAsync(track);
                        var transcriptEntries = new List<TranscriptEntry>();

                        foreach (var caption in closedCaptions.Captions)
                        {
                            int timestampSeconds = (int)caption.Offset.TotalSeconds;
                            transcriptEntries.Add(new TranscriptEntry
                            {
                                VideoId = video.Id,
                                VideoTitle = video.Title,
                                VideoUrl = $"https://www.youtube.com/watch?v={video.Id}",
                                VideoUrlWithTimestamp = $"https://www.youtube.com/watch?v={video.Id}&t={timestampSeconds}s",
                                TimestampSeconds = timestampSeconds,
                                StreamDate = videoDetails.UploadDate.ToString("yyyy-MM-dd"),
                                Timestamp = caption.Offset.ToString(),
                                Duration = caption.Duration.ToString(),
                                Text = caption.Text
                            });
                        }

                        // Save to CSV
                        var filename = Path.Combine(OutputDirectory,
                            $"{videoDetails.UploadDate.ToString("yyyy-MM-dd")}_{SanitizeFileName(video.Title)}_{video.Id}.csv");

                        using (var writer = new StreamWriter(filename))
                        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                        {
                            csv.WriteRecords(transcriptEntries);
                        }

                        Console.WriteLine($"Saved transcript to: {filename}");
                    }
                    else
                    {
                        Console.WriteLine($"No English captions found for: {video.Title}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing video {video.Title}: {ex.Message}");
                }

                // Dont get flagged by the API
                await Task.Delay(3000);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }

    private static string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        return string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
    }
}
