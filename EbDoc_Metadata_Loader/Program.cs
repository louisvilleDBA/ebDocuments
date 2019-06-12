using EbDoc_DAL;
using EbDoc_Processor;
using System.Configuration;
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
            string[] source_repo = new string[] 
            {
                ConfigurationManager.AppSettings["SourceLocation"],
                ConfigurationManager.AppSettings["AltSourceLocation"]
            };

            long result = FileProcessor.loadMetadata(new EbDocContext(), metadataFilename, source_repo);
            System.Console.WriteLine($"[{metadataFilename}] contains [{result}] files");
        }


    }
}
