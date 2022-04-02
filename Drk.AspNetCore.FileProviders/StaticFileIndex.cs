using System.ComponentModel.DataAnnotations;

namespace Drk.AspNetCore.FileProviders
{
    public class StaticFileIndex
    {
        [Key]
        [MaxLength(256)]
        [Required]
        public string Path { get; set; } = string.Empty;
        // -1 means Directory
        public int FileDataId { get; set; }
        public bool IsDirectory => FileDataId == Contstants.DirectoryIndex;
        public int Size { get; set; }
        public DateTime LastUpdate { get; set; }
    }

}