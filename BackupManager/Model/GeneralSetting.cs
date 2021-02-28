using System.ComponentModel.DataAnnotations;

namespace BackupManager.Model
{
    public class GeneralSetting
    {

        [Key]
        public int Id { get; set; }

        [StringLength(200)]
        public string LocalFolder { get; set; }

        public bool AutoDelete { get; set; }

        public int? DeleteAfterDays { get; set; }

        public int DbVersion { get; set; }

        public bool ServiceInstalled { get; set; }

    }
}
