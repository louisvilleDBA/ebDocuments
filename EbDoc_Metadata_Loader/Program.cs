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

            if (args.Length == 0)
            {
                metadataFilename = @"Y:\metadata\xaa.txt";
            }
            else
            {
                metadataFilename = args[0];
            }

            getData(metadataFilename);

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
            //string[] source_repo = new string[] 
            //{
            //    ConfigurationManager.AppSettings["SourceLocation"],
            //    ConfigurationManager.AppSettings["AltSourceLocation"]
            //};

            string source_repo = ConfigurationManager.AppSettings["SourceLocation"];

            long result = FileProcessor.loadMetadata(new EbDocContext(), metadataFilename, source_repo);
            System.Console.WriteLine($"[{metadataFilename}] contains [{result}] files");
        }


    }
}
