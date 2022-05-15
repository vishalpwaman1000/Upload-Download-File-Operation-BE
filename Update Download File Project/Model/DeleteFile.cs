using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Update_Download_File_Project.Model
{
    public class DeleteFileRequest
    {
        [Required]
        public int FileID { get; set; }

        [Required]
        public string FileName { get; set; }

        [Required]
        public string PublicID { get; set; }
    }

    public class DeleteFileResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
