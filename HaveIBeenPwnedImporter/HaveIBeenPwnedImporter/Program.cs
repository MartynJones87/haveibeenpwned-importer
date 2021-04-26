using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace HaveIBeenPwnedImporter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            const Int32 BufferSize = 128;
            using var fileStream = File.OpenRead("C:\\HaveIBeenPwned\\Source\\Top1M.txt");
            using var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize);

            string line;
            int count = 0;
            while ((line = streamReader.ReadLine()) != null && count < 1000)
            {
                ProcessLine(line);
                count++;
            }
        }

        public static void ProcessLine(string line)
        {
            const string splitFolderPath = "C:\\HaveIBeenPwned\\Split\\";

            var hash = line.AsSpan().Slice(0, 40);
            var occurrences = line.AsSpan().Slice(41);

            var partitionKey = line.AsSpan().Slice(0, 5);

            var currentFolderPath = splitFolderPath;
            for (var i = 0; i < 5; i++)
            {
                currentFolderPath = Path.Join(currentFolderPath, partitionKey.Slice(i, 1), "\\");
                if (!Directory.Exists(currentFolderPath))
                {
                    Directory.CreateDirectory(currentFolderPath);
                }
            }

            var fileName = $"{partitionKey.ToString()}.txt";
            var filePath = Path.Join(currentFolderPath, fileName);
            if (!File.Exists(filePath))
            {
                using StreamWriter streamWriter = File.CreateText(filePath);
                streamWriter.WriteLine(line);
                streamWriter.Close();
            }
            else
            {
                using StreamWriter streamWriter = new StreamWriter(filePath, append: true);
                streamWriter.WriteLine(line);
                streamWriter.Close();
            }

            // Console.WriteLine(string.Format("{0}:{1}", hash.ToString(), occurrences.ToString()));
        }
    }
}
