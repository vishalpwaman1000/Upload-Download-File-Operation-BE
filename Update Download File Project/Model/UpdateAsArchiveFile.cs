using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Update_Download_File_Project.Model
{
    public class UpdateAsArchiveFileRequest
    {
        public int FileID { get; set; }
    }

    public class UpdateAsArchiveFileResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
