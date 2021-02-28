using System;
using System.ComponentModel.DataAnnotations;

namespace BackupManager.Model
{
    public class ActivityLog
    {

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string LogType { get; set; }

        [Required]
        public string Description { get; set; }

        public DateTime LogTime { get; set; }

    }
}
