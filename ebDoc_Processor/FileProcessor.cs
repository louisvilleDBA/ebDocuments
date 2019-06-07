using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.IO;
using System.IO.Compression;

using EbDoc_DAL;
using EbDoc_DAL.Model;

namespace EbDoc_Processor
{
    public static class FileProcessor
    {
        const string HEADER = @"MODULE	PREFIX	MIDDLE	REVISION	TITLE	DATE_EFFECTIVE	" +
            "DATE_OBSOLETE	STATUS	REMARKS	SYNOPSIS	ACTIVITY CODE	ADDRESS	APPLICATION NO	" +
            "APPLICATION TYPE	APPROVAL DATE	 ASSET TYPE	EVENT	INITIATED DATE	NOTES	OWNER NAME	" +
            "PROBLEM	PROJECT NO	SCAN DATE	SERVICE REQUEST NO	SUBMITTAL DATE	TENANT NAME	UNITID	" +
            "WORK ORDER NO	HANSEN 7 WORK ORDER NO	DOCKET NO	GROUP PROJECT NO	GROUP INSPECTION NO	" +
            "INSPECTION NO	UNITID 2	REPOSITORY LOC	PATH	FILE NAME	MIME TYPE";

        const string ARCHIVE_NAME = "ebDOC_{0}.zip";
        const string METADATA_NAME = "ebDOC_{0}.txt";

        const string ARCHIVE_HEADER = "HANSEN_MODULE\tHANSEN_ID\tB1_ALT_ID\tFILE_NAME\tFILE_PATH\tFILE_SIZE";


        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static int loadMetadata(IEbDocContext context, string metadataFilename)
        {
            IEbDocContext db = context;
            /* validate file is metadata file
             * open file
             * read row into objects
             * save objects to database
             */
            log.Info($"begin processing file [{metadataFilename}]");
            if (!File.Exists(metadataFilename))
            {
                string errorMessage = $"{metadataFilename} not found!";
                log.Error(errorMessage);
                throw new FileNotFoundException(errorMessage);
            }

            try
            {
                log.Debug($"open metadata file [{metadataFilename}]");
                using (StreamReader file = new StreamReader(metadataFilename))
                {

                    int counter = -1;
                    string ln;
                    string[] headerItems = HEADER.Split("\t".ToCharArray());
                   

                    while ((ln = file.ReadLine()) != null)
                    {
                        log.Debug($"row [{counter}]:\n{ln}");

                        if (counter == -1 && !string.Equals(ln, HEADER))
                        {
                            string errorMessage = $"{metadataFilename} is NOT a metadata file";
                            log.Error(errorMessage);
                            throw new FileLoadException(errorMessage);
                        }
                        // if the first row is the HEADER then advance counter and continue
                        if (counter == -1 && string.Equals(ln, HEADER))
                        {
                            counter++;
                            continue;
                        }
                        //delimit content by tab into an array of strings
                        string[] content = ln.Split("\t".ToCharArray());
                        // if the filename is blank then continue
                        if (string.IsNullOrWhiteSpace(content.GetValue(Array.IndexOf(headerItems, "FILE NAME")).ToString()))
                            continue;

                        log.Debug($"load row [{counter}] into row and doc objects");
                        var row = new Record(
                            content.GetValue(Array.IndexOf(headerItems, "MODULE")).ToString(),
                            content.GetValue(Array.IndexOf(headerItems, "SERVICE REQUEST NO")).ToString(),
                            content.GetValue(Array.IndexOf(headerItems, "WORK ORDER NO")).ToString(),
                            content.GetValue(Array.IndexOf(headerItems, "HANSEN 7 WORK ORDER NO")).ToString(),
                            content.GetValue(Array.IndexOf(headerItems, "APPLICATION NO")).ToString()
                            );
                        var doc = GetDocument(
                                content.GetValue(Array.IndexOf(headerItems, "PATH")).ToString(),
                                content.GetValue(Array.IndexOf(headerItems, "FILE NAME")).ToString()
                                );
                        log.Debug($"get record for [{row.B1_ALT_ID}]");
                        Record record = db.Records
                                            .Include(r => r.Documents)
                                            .FirstOrDefault(r => r.B1_ALT_ID == row.B1_ALT_ID & r.Hansen_Module == row.Hansen_Module);
                        if (record is null)
                        {
                            addAccelaData(db, row);
                            row.Documents.Add(doc);
                            db.Records.Add(row);
                            log.Debug($"record [{row.B1_ALT_ID}] not found in database.\n" +
                                $"\t\tdocument [{row.Documents.Last().File_Name}] added to new record [{row.B1_ALT_ID}]");
                        }

                        // if the msd path does not exist in the list of documents then add the document
                        //else if (record.Documents.Select(d => d.MSD_path == doc.MSD_path).Count() > 0)
                        else if (!record.Documents.Any(d => d.MSD_path == doc.MSD_path))
                        {

           
                            record.Documents.Add(doc);
                            log.Debug($"[{record.B1_ALT_ID}] exists, [{doc.File_Name}] added to list of docuements");
                        }

                        if (record != null && string.IsNullOrWhiteSpace(record.Group))
                            addAccelaData(db, record);

                        log.Debug($"saving [{doc.File_Name}] to database");
                        db.SaveChanges();

                        counter++;
                        // debug line  - count off every 500 record processed
                        if (counter > 0 && counter % 500 == 0)
                            log.Info($"[{counter}] files processed ");

                    }
                    file.Close();
                    
                    log.Info($"MetaFile [{metadataFilename}] has processed [{counter}] files");

                    return counter;
                }
            }
            catch (Exception ex)
            {
                log.Error($"ERROR Found: {ex.Message}");
                throw ex;
            }

        }

        public static int createArchive2(IEbDocContext context, string repo_source, string repo_target, int max_file_count)
        {
            IEbDocContext db = context;

            log.Info("begin createArchive process");
            int files_archived = 0;
            long size = 0;

            int max_count = max_file_count + (max_file_count * 3 / 10);
            long max_size = 2000000000; // 2GB
            List<string> metadata = new List<string>{ARCHIVE_HEADER};
            

            // setup archive variables
            string archive_no = GetNextArchiveNo(db);
            string archivePath = Path.Combine(repo_target, string.Format(ARCHIVE_NAME, archive_no));
            string metadataPath = Path.Combine(repo_target, string.Format(METADATA_NAME, archive_no));

            log.Debug($"variables set\narchive_no:\t[{archive_no}" +
                $"\narchivePath:\t[{archivePath}]" +
                $"\nmetadataPath\t[{metadataPath}]"
                );


            try
            {


                // tag files with archive no. (max files plus 30%)

                // pull records with docs that have archive no, docs that will migrate ( is not to large and has record in accela)

                // pull 500 records and documents.

                // build manifest in memory

                // archive files on mainfest

                // write manifest to file

                var docs = db.Documents
                            .Where(d => d.ArchiveNo == null && d.File_Size < 5000000 && !d.Is_Missing)
                            .Take(max_count).ToList();

                if (docs.Count() < 1)
                {
                    log.Info("no records available to process");
                    return files_archived;
                }

                log.Debug("iterate through the docuemnts");
                foreach (var doc in docs)
                {
                    log.Debug($"file is [{doc.File_Path}]");

                    // stop if we reach max file count
                    if (files_archived == max_file_count || size > max_size)
                        break;

                    // mark document with archive_no to identify data as processed.
                    doc.ArchiveNo = archive_no;

                    var data = db;

                    var rec = data.Records.SingleOrDefault(r => r.RecordId == doc.RecordId && r.Group != null);
                    if (rec is null)
                    {
                        // if record does not migrate, continue
                        log.Debug($"[{doc.File_Path}] does not have a corresponding record in accela");
                        continue;
                    }

                    log.Debug("update document data");
                    doc.Target_Repository = repo_target;
                    doc.Zip_Date = DateTime.Now;
                    doc.Zip_Path = archivePath;
                    doc.ArchiveNo = archive_no;
                    doc.Metadata_Path = metadataPath;

                    //  "HANSEN_MODULE\tHANSEN_ID\tB1_ALT_ID\tFILE_NAME\tFILE_PATH\tFILE_SIZE";
                    metadata.Add($"{rec.Hansen_Module}\t{rec.Hansen_Id}\t{rec.B1_ALT_ID}\t{doc.File_Name}\t{doc.File_Path}\t{doc.File_Size}");

                    files_archived++;
                    size += doc.File_Size;

                    // debug line  - count off every 500 record processed
                    if (files_archived % 100 == 0)
                        log.Info($"[{files_archived}] files processed ");
                }

                db.SaveChanges();

                log.Debug("create the metadata file");
                if (!Directory.Exists(repo_target))
                    Directory.CreateDirectory(repo_target);
                using (StreamWriter sw = File.CreateText(metadataPath))
                {
                    foreach(var row in metadata)
                        sw.WriteLine(row);
                }

                log.Debug("create zip file");
                zipArchive(db, archive_no, repo_source, archivePath);

                log.Info($"----- archive [{archive_no}] complete -----");
            }
            catch (Exception ex)
            {
                log.Error($"createArchive2 failure: {ex}");
                throw ex;
            }
            return files_archived;
        }

        public static int createArchive(IEbDocContext context, string repo_source, string repo_target, int max_file_count)
        {
            IEbDocContext db = context;

            log.Info("begin createArchive process");
            int files_archived = 0;

            try
            {
                var docs = db.Documents
                            .Where(d => d.ArchiveNo == null && d.File_Size < 5000000 && !d.Is_Missing)
                            .Take(max_file_count).ToList();

                if (docs.Count() < 1)
                {
                    log.Info("no records available to process");
                    return files_archived;
                }

                string archive_no = GetNextArchiveNo(db);
                string archivePath = Path.Combine(repo_target, string.Format(ARCHIVE_NAME, archive_no));
                string metadataPath = Path.Combine(repo_target, string.Format(METADATA_NAME, archive_no));

                log.Debug($"variables set\narchive_no:\t[{archive_no}" +
                    $"\narchivePath:\t[{archivePath}]" +
                    $"\nmetadataPath\t[{metadataPath}]"
                    );
                if (!Directory.Exists(repo_target))
                    Directory.CreateDirectory(repo_target);

                log.Debug("create the metadata file");
                using (StreamWriter sw = File.CreateText(metadataPath))
                {
                    sw.WriteLine(ARCHIVE_HEADER);
                }

                log.Debug("iterate through the docuemnts");
                foreach (var doc in docs)
                {
                    log.Debug($"file is [{doc.File_Path}]");

                    var data = db;

                    var rec = data.Records.SingleOrDefault(r => r.RecordId == doc.RecordId && r.Group != null);
                    if (rec is null)
                    {
                        // mark document with archive_no to identify data as processed.
                        log.Debug($"[{doc.File_Path}] does not have a corresponding record in accela");
                        doc.ArchiveNo = archive_no;
                        db.SaveChanges();
                        continue;
                    }
                    
                    zipArchive(repo_source, doc, archivePath);

                    log.Debug($"add [{doc.File_Path} to metadata file [{metadataPath}]");
                    using (StreamWriter sw = File.AppendText(metadataPath))
                    {
                        //  "HANSEN_MODULE\tHANSEN_ID\tB1_ALT_ID\tFILE_NAME\tFILE_PATH\tFILE_SIZE";
                        string metadata = $"{rec.Hansen_Module}\t{rec.Hansen_Id}\t{rec.B1_ALT_ID}\t{doc.File_Name}\t{doc.File_Path}\t{doc.File_Size}";
                        sw.WriteLine(metadata);
                    }

                    log.Debug("update document data");
                    doc.Target_Repository = repo_target;
                    doc.Zip_Date = DateTime.Now;
                    doc.Zip_Path = archivePath;
                    doc.ArchiveNo = archive_no;
                    doc.Metadata_Path = metadataPath;

                    db.SaveChanges();
                    files_archived++;

                    // debug line  - count off every 500 record processed
                    if (files_archived % 100 == 0)
                        log.Info($"[{files_archived}] files processed ");

                }
                log.Info($"[{archivePath}] contains [{files_archived}] files of [{max_file_count}] requested.");

            }
            catch (Exception ex)
            {
                log.Error($"Epic failure: {ex}");
                throw ex;
            }
            return files_archived;
        }

        public static void archive7zip(string sourceName, string archiveName)
        {

            log.Debug("begin 7-zip archive");
            string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

            // 1
            // Initialize process information.
            //
            var startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.FileName = $@"{programFiles}\7-Zip\7z.exe";


            if (!File.Exists(startInfo.FileName))
            {
                string message = "unable to locate x64 7z.exe";
                log.Error(message);
                throw new FileNotFoundException(message);
            }
                

            // 2
            // Use 7-zip
            // specify a=archive and -tgzip=gzip
            // and then target file in quotes followed by source file in quotes
            //
            startInfo.Arguments = $"a -tzip \"{archiveName}\" \"{sourceName}\"  -mx=9";
            log.Debug($"args: [{startInfo.Arguments}]");

            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;


            // 3.
            // Start process and wait for it to exit
            //
            var process = System.Diagnostics.Process.Start(startInfo);
            process.WaitForExit();

        }

        private static void zipArchive(string repo_source, Document doc, string archivePath)
        {
            using (ZipArchive archive = ZipFile.Open(archivePath, ZipArchiveMode.Update))
            {
                //archive.CreateEntryFromFile(Path.Combine(repo_source, doc.MSD_path), doc.File_Path);
                archive.CreateEntryFromFile(Path.Combine(repo_source, doc.MSD_path), doc.File_Path.Replace(@"\", @"/"));
            }

            log.Debug($"added entry [{doc.File_Path}] into archive file [{archivePath}]");
        }

        private static void zipArchive(IEbDocContext context, string archive_no, string repo_source, string archivePath)
        {
            IEbDocContext db = context;
            try
            {
                var docs = db.Documents.Where(d => d.ArchiveNo == archive_no & d.Zip_Date != null);
                int total = docs.Count();
                log.Info($"[{total}] documents are ready to be archived.");


                using (ZipArchive archive = ZipFile.Open(archivePath, ZipArchiveMode.Update))
                {
                    int i = 0;
                    long size = 0;
                    foreach(Document doc in docs)
                    {
                        archive.CreateEntryFromFile(Path.Combine(repo_source, doc.MSD_path), doc.File_Path.Replace(@"\", @"/"));
                        i++;
                        size += doc.File_Size;
                        

                        if (i % 100 == 0)
                            log.Info($"[{i}] documents of [{total}] have been archived to [{archivePath}]");
                    }
                    log.Info($"[{i}] documents have been archived to [{archivePath}]");
                }
            }
            catch (Exception ex)
            {
                log.Error($"zipArchive failure: {ex}");
                throw ex;
            }
        }

        private static void addAccelaData(IEbDocContext context,Record row)
        {
            IEbDocContext db = context;

            log.Debug($"begin load record [{row.B1_ALT_ID}] with accela data");

            var accela_id = db.ACCELA_IDs.FirstOrDefault(a => a.B1_ALT_ID == row.B1_ALT_ID);
            if (accela_id != null)
            {
                row.Group = accela_id.B1_PER_GROUP;
                row.Type = accela_id.B1_PER_TYPE;
                row.Subtype = accela_id.B1_PER_SUB_TYPE;
                row.Category = accela_id.B1_PER_CATEGORY;
                row.Is_Closed = accela_id.IS_CLOSED;
            }
        }

        private static Document GetDocument(string MSD_Path, string File_Name)
        {
            string repo_source = System.Configuration.ConfigurationManager.AppSettings["SourceLocation"];

            if (string.IsNullOrWhiteSpace(MSD_Path))
                throw new FileLoadException("MSD_Path missing");
            if (string.IsNullOrWhiteSpace(File_Name))
                throw new FileLoadException("Document File_Name missing");
            if (string.IsNullOrWhiteSpace(repo_source))
                throw new FileLoadException("Source Location not configured.");

            var invalidChars = Path.GetInvalidFileNameChars();
            var valid_file_name = new string(File_Name.Where(f => !invalidChars.Contains(f)).ToArray());

            Document doc = new Document(repo_source, MSD_Path, valid_file_name);

            if (File.Exists(Path.Combine(repo_source, MSD_Path)))
            {
                doc.Is_Missing = false;
                doc.File_Size = new FileInfo(Path.Combine(repo_source, MSD_Path)).Length;
            }

            // print out dcoument object
            log.Debug("\n\tDocument Details:" +
                $"\n\t File_Repository: [{doc.Source_Repository}]" +
                $"\n\t MSD_path: [{doc.MSD_path}]" +
                $"\n\t File_Name: [{doc.File_Name}]" +
                $"\n\t File_Size: [{doc.File_Size}]" +

                $"\n\t File_Path: [{doc.File_Path}]" +
                $"\n\t Zip_Path: [{doc.Zip_Path}]" +
                $"\n\t Zip_Date: [{doc.Zip_Date.ToString()}]" +
                $"\n\t Metadata_Path: [{doc.Metadata_Path}]"
                );

            return doc;
        }

        private static string GetNextArchiveNo(IEbDocContext context)
        {
            // assume documents database will be populaed and can be maintained on the context of EF6
            IEbDocContext db = context;

            string nextArchiveNo = string.Empty;
            int increment_val = 1;

            log.Debug("begin - GetNextArchiveNo");
            try
            {
                var ArchiveNos =
                    db.Documents.Where(d=>d.ArchiveNo!= null).Select(d => d.ArchiveNo).Distinct().ToList();

                if (ArchiveNos.FirstOrDefault() is null)
                {
                    log.Debug("this is the first archive file.");
                    return "00000";
                }

                log.Debug("increment the archive number");
                increment_val += ArchiveNos.Select(int.Parse).Max();

                nextArchiveNo = increment_val.ToString("00000");

                log.Debug($"ArchiveNo is set - next archiveNO = [{nextArchiveNo}] ");
                return nextArchiveNo;
            }
            catch (Exception ex)
            {
                log.Error($"unable to process GetNextArchiveNo, Error: [{ex}]");
                throw ex;
            }
        }

    }


}
