using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mozilla.UniversalCharacterDetection.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length >= 1)
            {
                if (File.Exists(args[0]))
                {
                    Console.WriteLine(args[0]);
                    ProcessFile(args[0]);
                }
                else if (Directory.Exists(args[0]))
                { 
                    foreach (var file in Directory.GetFiles(args[0], "*", SearchOption.AllDirectories))
                    {
                        Console.WriteLine(file);
                        ProcessFile(file);
                    }
                }
                else
                {
                    Console.WriteLine("The path does not exist.");
                }
            }
            else
            {
                Console.WriteLine("A path to the file or directory is required.");
            }

            Console.ReadLine();
        }

        static void ProcessFile(String filePath)
        {
            var fileStream = new FileStream(filePath, FileMode.Open);

            if (fileStream.Length > 0)
            {
                fileStream.Seek(0, SeekOrigin.Begin);
                var pageBytes = new Byte[fileStream.Length];
                fileStream.Read(pageBytes, 0, pageBytes.Length);

                fileStream.Seek(0, SeekOrigin.Begin);
                var detectionLength = 0;
                var detectionBuffer = new Byte[4096];
                var universalDetector = new UniversalDetector(null);

                while ((detectionLength = fileStream.Read(detectionBuffer, 0, detectionBuffer.Length)) > 0 && !universalDetector.IsDone())
                {
                    universalDetector.HandleData(detectionBuffer, 0, detectionBuffer.Length);
                }

                universalDetector.DataEnd();

                if (universalDetector.GetDetectedCharset() != null)
                {
                    Console.WriteLine("Charset: " + universalDetector.GetDetectedCharset() + ". Encoding: " + System.Text.Encoding.GetEncoding(universalDetector.GetDetectedCharset()).EncodingName);
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("Charset: " + "ASCII" + ". Encoding: " + System.Text.Encoding.GetEncoding("ASCII"));
                    Console.WriteLine();
                }
            }

            fileStream.Dispose();
        }
    }
}
