using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Update_Download_File_Project.DataAccessLayer;
using Update_Download_File_Project.Model;

namespace Update_Download_File_Project.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class UpdateDownloadFileController : ControllerBase
    {

        public readonly IUpdateDownloadFileRL _updateDownloadFileRL;

        public UpdateDownloadFileController(IUpdateDownloadFileRL updateDownloadFileRL)
        {
            _updateDownloadFileRL = updateDownloadFileRL;
        }

        [HttpPost]
        public async Task<ActionResult> SignUp(SignUpRequest request)
        {
            SignUpResponse response = new SignUpResponse();
            try
            {
                response = await _updateDownloadFileRL.SignUp(request);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult> SignIn(SignInRequest request)
        {
            SignInResponse response = new SignInResponse();
            try
            {
                response = await _updateDownloadFileRL.SignIn(request);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> UploadFileOnCloud([FromForm] UploadFileOnCloudRequest request)
        {
            UploadFileOnCloudResponse response = new UploadFileOnCloudResponse();
            try
            {

                response = await _updateDownloadFileRL.UploadFileOnCloud(request);

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exception Occurs : " + ex.Message;
            }

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> GetUploadedFileList(GetUploadedFileListRequest request)
        {
            GetUploadedFileListResponse response = new GetUploadedFileListResponse();
            try
            {

                response = await _updateDownloadFileRL.GetUploadedFileList(request);

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exception Occurs : " + ex.Message;
            }

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAsArchiveTrashFile(UpdateAsArchiveTrashFileRequest request)
        {
            UpdateAsArchiveTrashFileResponse response = new UpdateAsArchiveTrashFileResponse();
            try
            {

                response = await _updateDownloadFileRL.UpdateAsArchiveTrashFile(request);

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exception Occurs : " + ex.Message;
            }

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFile(DeleteFileRequest request)
        {
            DeleteFileResponse response = new DeleteFileResponse();
            try
            {

                response = await _updateDownloadFileRL.DeleteFile(request);

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exception Occurs : " + ex.Message;
            }

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> RestoreTrashArchiveFile(RestoreTrashArchiveFileRequest request)
        {
            RestoreTrashArchiveFileResponse response = new RestoreTrashArchiveFileResponse();
            try
            {

                response = await _updateDownloadFileRL.RestoreTrashArchiveFile(request);

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exception Occurs : " + ex.Message;
            }

            return Ok(response);
        }
    }
}
