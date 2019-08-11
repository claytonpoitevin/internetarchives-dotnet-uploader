using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace proj
{
    class Program
    {
        private const string METADATA_FILE_NAME = "metadata.json";
        static void Main(string[] args)
        {
            var path = args[0];
            Console.WriteLine($"Uploading each folder from {path}");
            Console.WriteLine($"Every folder that has a '{METADATA_FILE_NAME}' file will be a new IA item.");
            Console.WriteLine($"Every file that is not '{METADATA_FILE_NAME}' will be uploaded.");

            UploadFolder(path);

        }

        static void UploadFolder(string path)
        {
            var dirs = Directory.EnumerateDirectories(path);
            foreach (var folder in dirs)
            {
                UploadFolder(folder);
                var metadata = ReadMetadata(folder);
                if (metadata == null) continue;
                Console.WriteLine($"Preparing upload for item with identifier '{metadata.identifier}'.");

                Directory.SetCurrentDirectory(folder);

                var commandline = GenerateUploadCommandLine(metadata, folder);
                UploadItem(commandline);
                AppendSubjectMetadata(metadata);

            }
        }

        static Metadata ReadMetadata(string currentFolder)
        {
            var metadataFile = $"{currentFolder}/{METADATA_FILE_NAME}";
            if (!File.Exists(metadataFile)) return null;
            var metadataJson = File.ReadAllText(metadataFile);
            return JsonConvert.DeserializeObject<Metadata>(metadataJson);

        }

        static string GenerateUploadCommandLine(Metadata metadata, string currentFolder)
        {
            var filesUpload = Directory.GetFiles(currentFolder).Select(x => Path.GetFileName(x)).Where(x => x != METADATA_FILE_NAME);
            var commandline = $"upload {metadata.identifier} {string.Join(' ', filesUpload)} ";
            foreach (var prop in metadata.GetType().GetProperties())
            {
                if (prop.Name == "identifier") continue;
                if (prop.Name == "subject") continue;
                var v = prop.GetValue(metadata);
                commandline += $"--metadata=\"{prop.Name}:{v}\" ";

            }
            commandline += "--metadata=\"language:Portuguese\" ";
            return commandline;
        }

        static void UploadItem(string commandline)
        {
            var psi = new ProcessStartInfo("ia", commandline);
            var p = Process.Start(psi);
            p.WaitForExit();
        }

        static void AppendSubjectMetadata(Metadata metadata)
        {
            foreach (var subj in metadata.subject.Split(';'))
            {
                var commandline = $"metadata {metadata.identifier} --append-list=\"subject:{subj}\"";

                var process = Process.Start(new ProcessStartInfo("ia", commandline));
                process.WaitForExit();
            }

        }
    }

    class Metadata
    {
        public string identifier { get; set; }
        public string title { get; set; }
        public string creator { get; set; }
        public string mediatype { get; set; }
        public string collection { get; set; }
        public string description { get; set; }
        public string date { get; set; }
        public string subject { get; set; }
    }
}
