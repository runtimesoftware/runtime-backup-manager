using System.ComponentModel.DataAnnotations;

namespace BackupManager.Model
{
    public class EmailSetting
    {

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string DisplayName { get; set; }

        [Required]
        [StringLength(100)]
        public string EmailAddress { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        [Required]
        [StringLength(100)]
        public string Password { get; set; }

        [Required]
        [StringLength(100)]
        public string SmtpServer { get; set; }

        public int SmtpPort { get; set; }

        public bool SmtpSsl { get; set; }

        public string RecipientEmail { get; set; }

        public bool IsValid { get; set; }

        public bool LocalSuccess { get; set; }

        public bool LocalFailure { get; set; }

        public bool RemoteSuccess { get; set; }

        public bool RemoteFailure { get; set; }

    }
}
