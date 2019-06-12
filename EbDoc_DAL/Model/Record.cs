using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EbDoc_DAL.Model
{
    public class Record
    {
        [Key]
        public long RecordId { get; set; }
        [Required]
        public string Hansen_Module { get; set; }
        [Required]
        public string Hansen_Id { get; set; }
        public string B1_ALT_ID { get; set; }
        public string Group { get; set; }
        public string Type { get; set; }
        public string Subtype { get; set; }
        public string Category { get; set; }
        public bool Is_Closed { get; set; }

        public virtual List<Document> Documents { get; set; }

        public Record() { } //added for Entity Framework to function.
        public Record(string module, string service_request, string work_order, string work_order_h7, string application_no)
        {
            long testid;
            this.Hansen_Module = module;
            this.Documents = new List<Document>();

            if (module == "SRV_REQ" && long.TryParse(service_request, out testid))
            {
                this.Hansen_Id = testid.ToString();
                this.B1_ALT_ID = $"{testid.ToString()}-SRV";
            }
            if (module == "WKR_ORDR" && !string.IsNullOrEmpty(work_order))
                if (long.TryParse(work_order, out testid))
                {
                    this.Hansen_Id = testid.ToString();
                    this.B1_ALT_ID = $"{testid.ToString()}-WO";
                }
            if (module == "WKR_ORDR" && !string.IsNullOrEmpty(work_order_h7))
                if (long.TryParse(work_order_h7, out testid))
                {
                    this.Hansen_Id = testid.ToString();
                    this.B1_ALT_ID = $"{testid.ToString()}-WO";
                }
            if (module == "CASE_APP" && !string.IsNullOrEmpty(application_no))
            {
                this.Hansen_Id = application_no;
                if(long.TryParse(application_no,out testid))
                    this.B1_ALT_ID = $"{testid.ToString()}-ENF";
                else
                    this.B1_ALT_ID = application_no;
            }
            if (module == "BLD-APP" && !string.IsNullOrEmpty(application_no))
            {
                this.Hansen_Id = application_no;
                if (long.TryParse(application_no, out testid))
                    this.B1_ALT_ID = $"{testid.ToString()}-BLD";
                else
                    this.B1_ALT_ID = application_no;
            }
            if (module == "PP_APP" && !string.IsNullOrEmpty(application_no))
            {
                this.Hansen_Id = application_no;
                if (long.TryParse(application_no, out testid))
                    this.B1_ALT_ID = $"{testid.ToString()}-PLN";
                else
                    this.B1_ALT_ID = application_no;
            }
            if (module == "USE_APP" && !string.IsNullOrEmpty(application_no))
            {
                this.Hansen_Id = application_no;
                if (long.TryParse(application_no, out testid))
                    this.B1_ALT_ID = $"{testid.ToString()}-USE";
                else
                    this.B1_ALT_ID = application_no;
            }
            if (module == "LIC_APP" && !string.IsNullOrEmpty(application_no))
            {
                this.Hansen_Id = application_no;
                this.B1_ALT_ID = application_no + "-BUS";
            }
            if (module == "TRADE_APP" && !string.IsNullOrEmpty(application_no))
            {
                this.Hansen_Id = application_no;
                this.B1_ALT_ID = application_no + "-TRD";
            }

            if (string.IsNullOrEmpty(this.B1_ALT_ID))
            {
                this.Hansen_Id = "UNKNOWN";
                this.B1_ALT_ID = "UNKNOWN";
            }
        }
    }
}
