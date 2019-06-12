using EbDoc_DAL;
using EbDoc_Processor;
using System.IO;

namespace EbDoc_Metadata_Loader
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string metadataFilename;
            string repo_source;

            if (args.Length < 2)
            {
                metadataFilename = @"Y:\metadata\xaa.txt";
                repo_source = @"C:\Blah";
            }
            else
            {
                metadataFilename = args[0];
                repo_source = args[1];
            }

            getData(metadataFilename);

            //getData(metadataFilename, repo_source);


            System.Console.WriteLine("\npress any key to coninue");
            System.Console.ReadKey();
        }
        internal static void getData(string metadataFilename)
        {
            if (!File.Exists(metadataFilename))
            {
                string errorMessage = $"metadata file [{metadataFilename}] not found!";
                throw new FileNotFoundException(errorMessage);
            }

            long result = FileProcessor.loadMetadata(new EbDocContext(), metadataFilename);
            System.Console.WriteLine($"[{metadataFilename}] contains [{result}] files");
        }
        //internal static void getData(string metadataFilename, string repo_source)
        //{
        //    if (!File.Exists(metadataFilename))
        //    {
        //        string errorMessage = $"metadata file [{metadataFilename}] not found!";
        //        throw new FileNotFoundException(errorMessage);
        //    }
        //    if (!Directory.Exists(repo_source))
        //    {
        //        string errorMessage = $"source repository [{repo_source}] not found!";
        //        throw new DirectoryNotFoundException(errorMessage);
        //    }

        //    long result = FileProcessor.loadMetadata(new EbDocContext(), metadataFilename, repo_source);
        //    System.Console.WriteLine($"[{metadataFilename}] contains [{result}] files");
        //}
    }
}
