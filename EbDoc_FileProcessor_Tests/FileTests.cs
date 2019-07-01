using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EbDoc_DAL.Model;
using System.Linq;
using System.Data.Entity;
using EbDoc_Processor;

namespace EbDoc_FileProcessor_Tests
{
    [TestClass]
    public class FileTests
    {
        [TestMethod]
        public void File_Exist()
        {
            string local_folder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string fileName = @"Test_Docs\TextFile1.txt";
            var testfile = 
                System.IO.Path.Combine(local_folder, fileName);
            Assert.IsTrue(System.IO.File.Exists(testfile));
        }

        [TestMethod]
        public void mockDBContext_test()
        {
            var db = new TestDBContext();
            db.Records.Add(new Record("TRADE_APP", "test", "test", "test", "test"));

            //var test = db.Records.FirstOrDefault(x=>x.B1_ALT_ID=="asd");

            var test = db.Records.FirstOrDefault();

            var testa = db.Records.Include(r => r.Documents).FirstOrDefault();

            Assert.IsNotNull(test);
            Assert.IsNotNull(testa);

        }
        
        #region load metadata tests
        [TestMethod]
        public void loadMetadata_test()
        {
            //ARRANGE
            var metadataFilePath = @"Test_Docs\Trade_License_Sample.txt";
            var mockDB_Context = new TestDBContext();
            var source_repo = new string[] { };

            //ACTION
            int result = FileProcessor.loadMetadata(mockDB_Context, metadataFilePath, source_repo);

            var docs = mockDB_Context.Records.FirstOrDefault(r => r.B1_ALT_ID == "13GTD1001-TRD").Documents;
            Document doc = docs.FirstOrDefault(d => d.File_Name == @"13GTD1001 DRIVER RENEW PERMIT PIC 2018.PDF");

            //ASSERT
            Assert.AreEqual(2, mockDB_Context.Records.Count());
            Assert.AreEqual(10, result);
            Assert.AreEqual(8, docs.Count());
            Assert.IsFalse(doc.Is_Large);
            Assert.AreEqual(342888, doc.File_Size);
        }
        [TestMethod]
        public void loadMetadata_test_no_APNO()
        {
            //ARRANGE
            string metadataFilePath = @"Test_Docs\Trade_License_Sample_no_APNO.txt";
            var mockDB_Context = new TestDBContext();
            var source_repo = new string[] { };

            int expected_file_count = 10;
            int expected_record_count = 3;

            //ACTION
            int actual_file_count = FileProcessor.loadMetadata(mockDB_Context, metadataFilePath, source_repo);
            int actual_record_count = mockDB_Context.Records.Count();
            //ASSERT
            Assert.AreEqual(expected_file_count, actual_file_count, $"Expected [{expected_file_count}] files to be loaded");
            Assert.AreEqual(expected_record_count, actual_record_count, $"expected [{expected_record_count}] records to be loaded");
        }

        #endregion load metadata tests

        #region archive tests
        /* create a metadata file with four
         * FilePath - includes file name
         * FileName 
         * Module - CDR_BUILDING, CDR_BUSINESSLICENSE, CDR_CODEENFORCEMENT, CDR_PLANNING, CDR_PROJECT, CDR_TRADELICENSE, CDR_USE, CRM, WORKMANAGMENT
         * Hansen ID
         * B1_ALT_ID
         */

        [TestMethod]
        public void createArchive_test()
        {
            //ARRANGE
            string local_folder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string metadataFilePath = @"Test_Docs\Trade_License_Sample.txt";
 
            var mockDB_Context = new TestDBContext();
            var source_repos = new string[] {
                local_folder,
                "placeholder"
            };

            string expected_module = "TRADE_APP";  // "CDR_TRADELICENSE";
            string expected_file_name = "13GTD1000 NEW TEMP 2013.PDF";
            //ACTION - 1 setupdata
            int result = FileProcessor.loadMetadata(mockDB_Context, metadataFilePath, source_repos);

            string result_module = mockDB_Context.Records.First().Hansen_Module;
            string result_file_name = mockDB_Context.Records.Single(r => r.B1_ALT_ID == "13GTD1000-TRD").Documents.Single(d => d.MSD_path == @"00\00\20\1078703").File_Name;
            
            //ASSERT
            Assert.AreEqual(expected_module, result_module);
            Assert.AreEqual(expected_file_name, result_file_name);

            //ARRANGE - 2 - load documents into documents database.
            foreach(var rec in mockDB_Context.Records)
            {
                foreach(var doc in rec.Documents)
                {
                    mockDB_Context.Documents.Add(doc);
                }
            }

            int expected_files_archived = 10; // set in config
            string expected_dir = @"c:\TEST_TARGET";
            // clean up system
            foreach (var filePath in System.IO.Directory.GetFiles(expected_dir))
                System.IO.File.Delete(filePath);


            //action - 2 - run archive
            int result_files_archived = FileProcessor.createArchive(mockDB_Context, expected_dir, 2);

            //ASSERT
            Assert.AreEqual(expected_files_archived, result_files_archived);
            Assert.IsTrue(System.IO.Directory.Exists(expected_dir));
            Assert.AreEqual(2, System.IO.Directory.GetFiles(expected_dir).Count());

            // clean up system
            foreach (var filePath in System.IO.Directory.GetFiles(expected_dir))
                System.IO.File.Delete(filePath);
            Assert.AreEqual(0, System.IO.Directory.GetFiles(expected_dir).Count());



        }


        [TestMethod]
        public void create_archive_test_nothing_archived()
        {
            //ARRANGE
            //load context with metadata info
            string local_folder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //string metadataFilePath = @"Test_Docs\Trade_License_Sample.txt";
            string target_repo = @"c:\TEST_TARGET";

            string testFolder = @"Test_Docs\";
            string testDoc = testFolder + "TextFile1.txt";

            Record rec1 = new Record("TRADE_APP", null, null, null, "00TEST1234");
            rec1.Documents.Add(new Document { Source_Repository = local_folder, MSD_path = testDoc, File_Name = "my_test_file0.txt", File_Path= testFolder + "my_test_file0.txt", ArchiveNo = "00001" });
            rec1.Documents.Add(new Document { Source_Repository = local_folder, MSD_path = testDoc, File_Name = "my_test_file1.txt", File_Path = testFolder + "my_test_file1.txt", ArchiveNo = "00001" });
            rec1.Documents.Add(new Document { Source_Repository = local_folder, MSD_path = testDoc, File_Name = "my_test_file2.txt", File_Path = testFolder + "my_test_file2.txt", ArchiveNo = "00002" });
            rec1.Documents.Add(new Document { Source_Repository = local_folder, MSD_path = testDoc, File_Name = "my_test_file3.txt", File_Path = testFolder + "my_test_file3.txt" });
            rec1.Documents.Add(new Document { Source_Repository = local_folder, MSD_path = testDoc, File_Name = "my_test_file4.txt", File_Path = testFolder + "my_test_file4.txt" });

            Record rec2 = new Record("TRADE_APP", null, null, null, "200TEST1234");
            rec2.Documents.Add(new Document { Source_Repository = local_folder, MSD_path = testDoc, File_Name = "my_test_file20.txt", File_Path = testFolder + "my_test_file20.txt" });
            rec2.Documents.Add(new Document { Source_Repository = local_folder, MSD_path = testDoc, File_Name = "my_test_file21.txt", File_Path = testFolder + "my_test_file21.txt" });
            rec2.Documents.Add(new Document { Source_Repository = local_folder, MSD_path = testDoc, File_Name = "my_test_file22.txt", File_Path = testFolder + "my_test_file22.txt" });
            rec2.Documents.Add(new Document { Source_Repository = local_folder, MSD_path = testDoc, File_Name = "my_test_file23.txt", File_Path = testFolder + "my_test_file23.txt" });
            rec2.Documents.Add(new Document { Source_Repository = local_folder, MSD_path = testDoc, File_Name = "my_test_file24.txt", File_Path = testFolder + "my_test_file24.txt" });

            var mockDB_Context = new TestDBContext();
            mockDB_Context.Records.Add(rec1);
            mockDB_Context.Records.Add(rec2);

            //ACTION
            int result = FileProcessor.createArchive(mockDB_Context, target_repo,2);

            //ASSERT
            Assert.AreEqual(0, result, $"Expected 2 files to be achived");
        }

        [TestMethod]
        public void create_archive_test_s_archived()
        {
            //ARRANGE
            string local_folder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            
            string target_repo = @"c:\TEST_TARGET";

            string testFolder = @"Test_Docs\";
            string testDoc = testFolder + "TextFile1.txt";

            Record rec1 = new Record("TRADE_APP", null, null, null, "00TEST1234");
            rec1.Documents.Add(new Document { Source_Repository = local_folder, MSD_path = testDoc, File_Name = "my_test_file0.txt", File_Path = testFolder + "my_test_file0.txt", ArchiveNo = "00001" });
            rec1.Documents.Add(new Document { Source_Repository = local_folder, MSD_path = testDoc, File_Name = "my_test_file1.txt", File_Path = testFolder + "my_test_file1.txt", ArchiveNo = "00001" });
            rec1.Documents.Add(new Document { Source_Repository = local_folder, MSD_path = testDoc, File_Name = "my_test_file2.txt", File_Path = testFolder + "my_test_file2.txt", ArchiveNo = "00002" });
            rec1.Documents.Add(new Document { Source_Repository = local_folder, MSD_path = testDoc, File_Name = "my_test_file3.txt", File_Path = testFolder + "my_test_file3.txt" });
            rec1.Documents.Add(new Document { Source_Repository = local_folder, MSD_path = testDoc, File_Name = "my_test_file4.txt", File_Path = testFolder + "my_test_file4.txt" });

            Record rec2 = new Record("TRADE_APP", null, null, null, "200TEST1234");
            rec2.Documents.Add(new Document { Source_Repository = local_folder, MSD_path = testDoc, File_Name = "my_test_file20.txt", File_Path = testFolder + "my_test_file20.txt" });
            rec2.Documents.Add(new Document { Source_Repository = local_folder, MSD_path = testDoc, File_Name = "my_test_file21.txt", File_Path = testFolder + "my_test_file21.txt" });
            rec2.Documents.Add(new Document { Source_Repository = local_folder, MSD_path = testDoc, File_Name = "my_test_file22.txt", File_Path = testFolder + "my_test_file22.txt" });
            rec2.Documents.Add(new Document { Source_Repository = local_folder, MSD_path = testDoc, File_Name = "my_test_file23.txt", File_Path = testFolder + "my_test_file23.txt" });
            rec2.Documents.Add(new Document { Source_Repository = local_folder, MSD_path = testDoc, File_Name = "my_test_file24.txt", File_Path = testFolder + "my_test_file24.txt" });
            rec2.Group = "TEST_GROUP";

            var mockDB_Context = new TestDBContext();
            mockDB_Context.Records.Add(rec1);
            mockDB_Context.Records.Add(rec2);

            int expected_doc_count = rec2.Documents.Count(d => d.ArchiveNo == null);

            string expected_dir = @"c:\TEST_TARGET";
            // clean up system
            foreach (var filePath in System.IO.Directory.GetFiles(expected_dir))
                System.IO.File.Delete(filePath);

            //ACTION
            int result = FileProcessor.createArchive(mockDB_Context, target_repo, 10);

            // clean up system
            foreach (var filePath in System.IO.Directory.GetFiles(expected_dir))
                System.IO.File.Delete(filePath);

            //ASSERT
            Assert.AreEqual(expected_doc_count, result, $"Expected [{expected_doc_count}] files to be achived");
        }

        #endregion archive tests
    }
}
