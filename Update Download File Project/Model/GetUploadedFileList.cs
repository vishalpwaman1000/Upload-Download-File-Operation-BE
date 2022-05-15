using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Update_Download_File_Project.Model
{
    public class GetUploadedFileListRequest
    {
        [Required]
        public string OperationType { get; set; } // Read, Trash, Archive

        [Required]
        public int NumberOfRecordPerPage { get; set; }

        [Required]
        public int PageNumber { get; set; }
    }

    public class GetUploadedFileListResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int CurrentPage { get; set; }
        public double TotalRecords { get; set; }
        public int TotalPage { get; set; }
        public List<GetUploadedFileList> data { get; set; }

    }

    public class GetUploadedFileList
    {
        public int FileID { get; set; }
        public string ResourcePublicID { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string FileTypeUrl { get; set; }
        public string InsertionDate { get; set; }
        public string FileStatus { get; set; }
    }
}
