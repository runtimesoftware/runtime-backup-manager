using System;
using System.ComponentModel.DataAnnotations;

namespace BackupManager.Model
{
    public class FolderBackup
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FolderName { get; set; }

        public DateTime BackupTime { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool IsActive { get; set; }

        public DateTime? LastBackupTime { get; set; }

        public string LastBackupResult { get; set; }


    }
}
