using System.ComponentModel.DataAnnotations;

namespace EbDoc_DAL.Model
{
    public class ACCELA_ID
    {
        [Key]
        public string B1_ALT_ID { get; set; }
        [Required]
        public string B1_PER_GROUP { get; set; }
        [Required]
        public string B1_PER_TYPE { get; set; }
        [Required]
        public string B1_PER_SUB_TYPE { get; set; }
        [Required]
        public string B1_PER_CATEGORY { get; set; }
        public bool IS_CLOSED { get; set; }

    }
}
