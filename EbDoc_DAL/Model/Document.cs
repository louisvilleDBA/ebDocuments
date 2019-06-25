using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace EbDoc_DAL.Model
{
    public class Document
    {
        [Key]
        public long DocumentId { get; set; }

        public string Source_Repository { get; set; } // this should be set in configuration
        public string Target_Repository { get; set; } // this should be set in configuration
        [Required]
        public string MSD_path { get; set; }
        [Required]
        public string File_Name { get; set; }
        public bool Is_Missing { get; set; }
        public bool Has_Record { get; set; } 
        public long File_Size { get; set; }
        public bool Is_Large
        { // this is an internal function
            get
            {
                return this.File_Size > 5000000;
            }
        }

        public string ArchiveNo { get; set; }
        public bool IsArchived { get { return !string.IsNullOrWhiteSpace(this.ArchiveNo); } }
        public string File_Path { get; set; }
        public string Zip_Path { get; set; }
        public DateTime? Zip_Date { get; set; }


        public string Metadata_Path { get; set; }

        //document belongs to a record
        public long RecordId { get; set; }


        public Document() { }
        public Document(string repo_source,string msd_path,string filename)
        {
            Source_Repository = repo_source;
            MSD_path = msd_path;
            File_Name = filename;
            //File_Path = Path.Combine(Path.GetDirectoryName(msd_path), filename);
            File_Path = $"{msd_path}_{filename}";
            Is_Missing = true;
        }
    }
}
