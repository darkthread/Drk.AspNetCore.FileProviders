using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drk.AspNetCore.FileProviders
{
    public class StaticFileData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FileDataId { get; set; }
        [MaxLength(1024)]
        public string Path { get; set; } = string.Empty;
        public byte[] Content { get; set; } = new byte[] { };
        [MaxLength(64)]
        public string UserId { get; set; } = string.Empty;
        [MaxLength(32)]
        public string ClientIp { get; set; } = string.Empty;
        [MaxLength(1)]
        public string Status { get; set; } = "A";
        public DateTime UpdateTime { get; set; }
        public string Remark { get; set; } = string.Empty;
    }

}