using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Update_Download_File_Project.Model
{
    public class UpdateAsTrashFileRequest
    {
        [Required]
        public int FileID { get; set; }

        //public string PublicID { get; set; }
    }

    public class UpdateAsTrashFileResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
