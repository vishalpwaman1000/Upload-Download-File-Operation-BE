using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Update_Download_File_Project.Model;

namespace Update_Download_File_Project.DataAccessLayer
{
    public interface IUpdateDownloadFileRL
    {
        public Task<SignUpResponse> SignUp(SignUpRequest request);
        public Task<SignInResponse> SignIn(SignInRequest request);
        public Task<UploadFileOnCloudResponse> UploadFileOnCloud(UploadFileOnCloudRequest request);
        public Task<GetUploadedFileListResponse> GetUploadedFileList(GetUploadedFileListRequest request);
        public Task<UpdateAsArchiveFileResponse> UpdateAsArchiveFile(UpdateAsArchiveFileRequest request);
        public Task<GetUploadedFileListResponse> GetArchiveFileList(GetUploadedFileListRequest request);
        public Task<UpdateAsTrashFileResponse> UpdateAsTrashFile(UpdateAsTrashFileRequest request);
        public Task<GetUploadedFileListResponse> GetTrashFileList(GetUploadedFileListRequest request);
        public Task<DeleteFileResponse> DeleteFile(DeleteFileRequest request);
        public Task<RestoreTrashArchiveFileResponse> RestoreTrashArchiveFile(RestoreTrashArchiveFileRequest request);
    }
}
