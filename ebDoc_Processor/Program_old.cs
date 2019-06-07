using System.IO;
using EbDoc_DAL;

namespace EbDoc_Processor
{
    class Program
    {
        static void Main(string[] args)
        {
            //string metadataFilename = args[0];
            //if (!File.Exists(metadataFilename))
            //{
            //    string errorMessage = $"{metadataFilename} not found!";
            //    throw new FileNotFoundException(errorMessage);
            //}

            //long result = FileProcessor.loadMetadata(new EbDocContext(), metadataFilename);

            //System.Console.WriteLine($"[{metadataFilename}] contains [{result}] files");
            //System.Console.WriteLine("\npress any key to coninue");
            //System.Console.ReadKey();

            //-------------------------------------------------------------
            // NOTES
            //-------------------------------------------------------------
            //string use_docs = @"Y:\metadata\USE_APPLICATIONS.txt"; // 3182
            //FileProcessor.loadMetadata(new EbDocContext(), use_docs);

            //string trade_license_docs = @"Y:\metadata\TRADE_LICENSE_APPLICATIONS.txt"; // 22145
            //string planning_docs = @"Y:\metadata\PROJECT_PLANNING_APPS.txt"; // 72598
            //string business_license_docs = @"Y:\metadata\LICENSE_APPLICATIONS.txt"; // 76831 //[Y:\metadata\LICENSE_APPLICATIONS.txt] has processed [57402] files
            //string workorder_docs = @"Y:\metadata\WORK_ORDER_ATTACHMENTS.txt"; // 289495
            //string building_docs = @"Y:\metadata\BUILDING_APPLICATIONS.txt"; // 330023
            //string service_request_docs = @"Y:\metadata\SERVICE_REQUEST_ATTACHMENTS.txt";
            //string enforcement_docs = @"Y:\metadata\CASE_APPLICATIONS.txt";


            archive_data(1,1000);
        }

        internal static void archive_data(int archive_count = 1, int files = 50)
        {
            for(int i=0; i<archive_count; i++)
            {
                FileProcessor.createArchive(
                        new EbDocContext(),
                        System.Configuration.ConfigurationManager.AppSettings["SourceLocation"],
                        System.Configuration.ConfigurationManager.AppSettings["TargetLocation"],
                        files);
            }

        }






    }
}
