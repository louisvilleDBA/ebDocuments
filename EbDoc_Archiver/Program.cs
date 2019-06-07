using EbDoc_DAL;
using EbDoc_Processor;

namespace EbDoc_Archiver
{
    class Program
    {
        static void Main(string[] args)
        {
            int archive_count = 0;
            int files_to_archive = 0;

            if (int.TryParse(args[0], out archive_count)
                && int.TryParse(args[1], out files_to_archive))
            {
                archive_data(archive_count, files_to_archive);
            }
            else archive_data();
        }

        internal static void archive_data(int archive_count = 1, int files = 10)
        {
            for (int i = 0; i < archive_count; i++)
            {
                FileProcessor.createArchive2(
                        new EbDocContext(),
                        System.Configuration.ConfigurationManager.AppSettings["SourceLocation"],
                        System.Configuration.ConfigurationManager.AppSettings["TargetLocation"],
                        files);
            }

        }






    }
}
