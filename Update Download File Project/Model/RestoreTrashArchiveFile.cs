using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Update_Download_File_Project.Model
{
    public class RestoreTrashArchiveFileRequest
    {
        public string OperationType { get; set; }  // Trash, Archive
        public int FileID { get; set; }
    }

    public class RestoreTrashArchiveFileResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
