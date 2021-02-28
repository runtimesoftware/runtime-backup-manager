using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackupManager.Model
{

    [Table("AWSS3Settings")]
    public class AWSS3Setting
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string BucketName { get; set; }

        [Required]
        [StringLength(100)]
        public string AccessKeyId { get; set; }

        [Required]
        [StringLength(100)]
        public string AccessSecretKey { get; set; }

        [Required]
        [StringLength(20)]
        public string AWSRegion { get; set; }

        public DateTime BackupTime { get; set; }

        public bool DeleteAfterBackup { get; set; }

        public bool IsActive { get; set; }

        public bool IsValid { get; set; }

    }

}
