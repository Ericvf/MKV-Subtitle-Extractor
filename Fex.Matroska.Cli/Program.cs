using System;
using System.IO;
using System.Linq;

namespace Fex.Matroska.Cli
{
    class Program
    {

        static void Main(string[] args)
        {
            // Use the helper class to access the arguments
            var arguments = new Arguments(args);

            // Read the arguments
            string fullName = arguments["filename"] ?? arguments["f"];
            string trackNo = arguments["trackNo"] ?? arguments["t"] ?? "All";
            
            // Output init
            Console.WriteLine("Using file: {0}", fullName);
            Console.WriteLine("Exporting subtitle track(s): {0}", trackNo);

            // Find the file info to determine the path and filename
            string filePath = new FileInfo(fullName).Directory.FullName;
            string fileName = Path.GetFileName(fullName);

            // Create a new Matroska helper and find the subtitle tracks
            Matroska mkv = new Matroska();
            var tracks = mkv.GetMatroskaSubtitleTracks(fullName);

            // Output track info
            Console.WriteLine("Tracks found in file: {0}", tracks.Count);

            // Check for specific track or all tracks
            if (trackNo.Equals("All"))
            {
                foreach (var track in tracks)
                    ExtractSubtitleAndSave(fullName, filePath, fileName, mkv, track);
            }
            else
            {
                // Find the correct track
                int trackNoInt;
                var parsedInt = int.TryParse(trackNo, out trackNoInt);

                if (!parsedInt)
                {
                    Console.WriteLine("Invalid track number: {0}", trackNo);
                }
                else
                {
                    var track = tracks.SingleOrDefault(t => t.TrackNumber.Equals(int.Parse(trackNo)));
                    if (track == null)
                    {
                        Console.WriteLine("Track {0} not found", trackNo);
                    }
                    else
                    {
                        ExtractSubtitleAndSave(fullName, filePath, fileName, mkv, track);
                    }
                }
            }

            // Console.ReadLine();
        }

        private static void ExtractSubtitleAndSave(string fullName, string filePath, string fileName, Matroska mkv, MatroskaSubtitleInfo track)
        {
            // Output track status
            Console.WriteLine("Extracting subtitle track: {0}", track.TrackNumber);
            Console.WriteLine(" - Language: {0}", track.Language);
            Console.WriteLine(" - Name: {0}", track.Name);

            bool isSubtitleValid = false;
            var subtitle = mkv.GetMatroskaSubtitle(fullName, track.TrackNumber, out isSubtitleValid);
            if (isSubtitleValid)
            {
                // Create filename
                var subtitleFileName = Path.Combine(filePath, fileName + "." + track.Language.ToString() + ".srt");
                var srtText = subtitle.ToSrtFormat();

                // Write the file
                File.WriteAllText(subtitleFileName, srtText);

                // Output
                Console.WriteLine("Finished extracting, written to file:");
                Console.WriteLine(subtitleFileName);
            }
            else
            {
                Console.WriteLine("Failed extracting");
            }

            Console.WriteLine();
        }
    }
}
