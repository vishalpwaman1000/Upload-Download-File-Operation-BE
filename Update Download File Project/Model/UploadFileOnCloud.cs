using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Update_Download_File_Project.Model
{
    public class UploadFileOnCloudRequest
    {
        [Required]
        public IFormFile File { get; set; }

        /*[Required]
        public string PublicId { get; set; }*/
    }

    public class UploadFileOnCloudResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
