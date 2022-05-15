using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Update_Download_File_Project.Model;

namespace Update_Download_File_Project.DataAccessLayer
{
    public class UpdateDownloadFileRL : IUpdateDownloadFileRL
    {

        public readonly IConfiguration _configuration;
        public readonly SqlConnection _sqlConnection;

        public UpdateDownloadFileRL(IConfiguration configuration)
        {
            _configuration = configuration;
            _sqlConnection = new SqlConnection(_configuration["ConnectionStrings:SqlServerConnection"]);
        }

        public async Task<SignInResponse> SignIn(SignInRequest request)
        {
            SignInResponse response = new SignInResponse();
            response.IsSuccess = true;
            response.Message = "Successful";
            try
            {

                if (_sqlConnection.State != ConnectionState.Open)
                {
                    await _sqlConnection.OpenAsync();
                }

                /*string SqlQuery = @"SELECT * 
                                    FROM Userdetail 
                                    WHERE UserName=@UserName AND PassWord=@PassWord AND Role=@Role;";*/

                string SqlQuery = _configuration["SignIn"];

                using (SqlCommand sqlCommand = new SqlCommand(SqlQuery, _sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    sqlCommand.CommandTimeout = 180;
                    sqlCommand.Parameters.AddWithValue("@UserName", request.UserName);
                    sqlCommand.Parameters.AddWithValue("@PassWord", request.Password);
                    sqlCommand.Parameters.AddWithValue("@Role", request.Role);
                    using (DbDataReader dataReader = await sqlCommand.ExecuteReaderAsync())
                    {
                        if (dataReader.HasRows)
                        {
                            response.Message = "Login Successfully";
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.Message = "Login Unsuccessfully";
                            return response;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            finally
            {

            }

            return response;
        }

        public async Task<SignUpResponse> SignUp(SignUpRequest request)
        {
            SignUpResponse response = new SignUpResponse();
            response.IsSuccess = true;
            response.Message = "Successful";
            try
            {
                if (_sqlConnection.State != ConnectionState.Open)
                {
                    await _sqlConnection.OpenAsync();
                }

                if (!request.Password.Equals(request.ConfigPassword))
                {
                    response.IsSuccess = false;
                    response.Message = "Password & Confirm Password not Match";
                    return response;
                }

                string SqlQuery = _configuration["SignUp"];

                using (SqlCommand sqlCommand = new SqlCommand(SqlQuery, _sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    sqlCommand.CommandTimeout = 180;
                    sqlCommand.Parameters.AddWithValue("@UserName", request.UserName);
                    sqlCommand.Parameters.AddWithValue("@PassWord", request.Password);
                    sqlCommand.Parameters.AddWithValue("@Role", request.Role);
                    int Status = await sqlCommand.ExecuteNonQueryAsync();
                    if (Status <= 0)
                    {
                        response.IsSuccess = false;
                        response.Message = "Something Went Wrong";
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            finally
            {

            }

            return response;
        }

        public async Task<GetUploadedFileListResponse> GetUploadedFileList(GetUploadedFileListRequest request)
        {
            GetUploadedFileListResponse response = new GetUploadedFileListResponse();
            response.IsSuccess = true;
            response.Message = "Successful";

            try
            {

                if (_sqlConnection.State != ConnectionState.Open)
                {
                    await _sqlConnection.OpenAsync();
                }

                int Offset = (request.PageNumber - 1) * request.NumberOfRecordPerPage;
                string SqlQuery = string.Empty;
                if (request.OperationType.ToLowerInvariant() == "active")
                {
                    SqlQuery = @"
                                    SELECT FileID,
                                           ResourcePublicID,
	                                       FileName,
	                                       InsertionDate,
	                                       FileUrl,
	                                       (select FileUrl from dbo.master_url where UrlID=FileTypeID) as FileTypeUrl,
                                           (select COUNT(*) from UploadDownloadFile WHERE IsActive=1 AND IsArchive=0) As TotalRecord
                                    FROM UploadDownloadFile 
                                    WHERE IsActive=1 AND IsArchive=0
                                    order by InsertionDate desc
                                    OFFSET @Offset ROWS FETCH NEXT @NumberOfRecordPerPage ROWS ONLY;
                                    ";

                }else if (request.OperationType.ToLowerInvariant() == "trash")
                {

                    SqlQuery = @"
                                    SELECT FileID,
                                           ResourcePublicID,
	                                       FileName,
	                                       InsertionDate,
	                                       FileUrl,
	                                       (select FileUrl from dbo.master_url where UrlID=FileTypeID) as FileTypeUrl,
                                           (select COUNT(*) from UploadDownloadFile WHERE IsActive=0) As TotalRecord
                                    FROM UploadDownloadFile 
                                    WHERE IsActive=0
                                    order by InsertionDate desc
                                    OFFSET @Offset ROWS FETCH NEXT @NumberOfRecordPerPage ROWS ONLY;
                                    ";

                }
                else if (request.OperationType.ToLowerInvariant() == "archive")
                {
                    SqlQuery = @"
                                    SELECT FileID,	
                                           ResourcePublicID,
	                                       FileName,
	                                       InsertionDate,
	                                       FileUrl,
	                                       (select FileUrl from dbo.master_url where UrlID=FileTypeID) as FileTypeUrl,
                                           (select COUNT(*) from UploadDownloadFile WHERE IsActive=1 AND IsArchive=1) As TotalRecord
                                    FROM UploadDownloadFile 
                                    WHERE IsActive=1 AND IsArchive=1
                                    order by InsertionDate desc
                                    OFFSET @Offset ROWS FETCH NEXT @NumberOfRecordPerPage ROWS ONLY;
                                    ";

                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid Operation Type";
                    return response;
                }

                using (SqlCommand sqlCommand = new SqlCommand(SqlQuery, _sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.Text;
                    sqlCommand.CommandTimeout = 180;
                    sqlCommand.Parameters.AddWithValue("@NumberOfRecordPerPage", request.NumberOfRecordPerPage);
                    sqlCommand.Parameters.AddWithValue("@Offset", Offset);

                    using (DbDataReader dataReader = await sqlCommand.ExecuteReaderAsync())
                    {
                        if (dataReader.HasRows)
                        {
                            response.data = new List<GetUploadedFileList>();
                            int Count = 0;

                            while (await dataReader.ReadAsync())
                            {
                                response.data.Add(new GetUploadedFileList()
                                {
                                    FileID = dataReader["FileID"] != DBNull.Value ? Convert.ToInt32(dataReader["FileID"]) : -1,
                                    ResourcePublicID = dataReader["ResourcePublicID"] != DBNull.Value ? dataReader["ResourcePublicID"].ToString() : string.Empty,
                                    FileName = dataReader["FileName"] != DBNull.Value ? dataReader["FileName"].ToString() : string.Empty,
                                    FileUrl = dataReader["FileUrl"] != DBNull.Value ? dataReader["FileUrl"].ToString() : string.Empty,
                                    FileTypeUrl = dataReader["FileTypeUrl"] != DBNull.Value ? dataReader["FileTypeUrl"].ToString() : string.Empty,
                                    InsertionDate = dataReader["InsertionDate"] != DBNull.Value ? Convert.ToDateTime(dataReader["InsertionDate"]).ToString("dddd, dd MMMM yyyy h:mm tt") : string.Empty,
                                    FileStatus = request.OperationType
                                });

                                if (Count == 0)
                                {
                                    Count++;
                                    response.TotalRecords = dataReader["TotalRecord"] != DBNull.Value ? Convert.ToInt32(dataReader["TotalRecord"]) : -1;
                                    response.TotalPage = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(response.TotalRecords / request.NumberOfRecordPerPage)));
                                    response.CurrentPage = request.PageNumber;
                                }
                            }

                            
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exception Occurs : " + ex.Message;
            }
            finally
            {
                await _sqlConnection.CloseAsync();
                await _sqlConnection.DisposeAsync();
            }

            return response;
        }

        public async Task<UploadFileOnCloudResponse> UploadFileOnCloud(UploadFileOnCloudRequest request)
        {
            UploadFileOnCloudResponse response = new UploadFileOnCloudResponse();
            response.IsSuccess = true;
            response.Message = "Successfully File Upload";

            string ImageExtensionList = ".webp, .svg, .png, .pjpeg, .jfif, .jpeg, .jpg, .gif, .avif, .apng";
            string VideoExtensionList = ".mp4, .mov, .wmv, .avi, .avchd, .flv, .f4v, .swf, .mkv, .webm , .html5, .mpeg-2, mpeg-4, .mts, .h264";
            string ProjectUsableExtensionList = ".txt, .pdf, .xls, .csv, .xlsx";
            string FileExtension = string.Empty, Url=string.Empty, PublicID=string.Empty;
            try
            {

                string[] FileName = request.File.FileName.ToString().Split(".");

                Account account = new Account(
                                _configuration["CloudinarySettings:CloudName"],
                                _configuration["CloudinarySettings:ApiKey"],
                                _configuration["CloudinarySettings:ApiSecret"]);

                var path = request.File.OpenReadStream();

                Cloudinary cloudinary = new Cloudinary(account);


                FileExtension = "." + FileName[1].ToString();

                if (ImageExtensionList.Contains(FileExtension))
                {
                    FileExtension = "img";
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(request.File.FileName, path),
                        //Folder=""
                    };
                    var uploadResult = await cloudinary.UploadAsync(uploadParams);
                    Url = uploadResult.Url.ToString();
                    PublicID = uploadResult.PublicId.ToString();
                }
                else if (VideoExtensionList.Contains(FileExtension))
                {
                    FileExtension = "video";
                    var uploadParams = new VideoUploadParams()
                    {
                        File = new FileDescription(request.File.FileName, path),
                        //Folder=""
                    };
                    var uploadResult = await cloudinary.UploadAsync(uploadParams);
                    Url = uploadResult.Url.ToString();
                    PublicID = uploadResult.PublicId.ToString();
                }
                else if (!ProjectUsableExtensionList.Contains(FileExtension))
                {
                    FileExtension = "raw";
                    var uploadParams = new RawUploadParams()
                    {
                        File = new FileDescription(request.File.FileName, path),
                        //Folder=""
                    };
                    var uploadResult = await cloudinary.UploadAsync(uploadParams);
                    Url = uploadResult.Url.ToString();
                    PublicID = uploadResult.PublicId.ToString();
                }
                else
                {
                    FileExtension = FileName[1].ToString();
                    var uploadParams = new RawUploadParams()
                    {
                        File = new FileDescription(request.File.FileName, path),
                        //Folder=""
                    };
                    var uploadResult = await cloudinary.UploadAsync(uploadParams);
                    Url = uploadResult.Url.ToString();
                    PublicID = uploadResult.PublicId.ToString();
                }

                if (_sqlConnection.State != ConnectionState.Open)
                {
                    await _sqlConnection.OpenAsync();
                }

                string SqlQuery = @"
                                    INSERT INTO UploadDownloadFile 
                                    ( 
                                      FileName, 
                                      FileTypeID, 
                                      FileUrl,
                                      ResourcePublicID
                                    )  
                                    VALUES 
                                    ( 
                                      @FileName, 
                                      (select UrlID from master_url where FileType=@FileType), 
                                      @FileUrl ,
                                      @ResourcePublicID
                                    );
                                   ";

                using (SqlCommand sqlCommand = new SqlCommand(SqlQuery, _sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.Text;
                    sqlCommand.CommandTimeout = 180;
                    //@FileName, @FileUrl
                    sqlCommand.Parameters.AddWithValue("@FileName", request.File.FileName);
                    sqlCommand.Parameters.AddWithValue("@FileType", FileExtension);
                    sqlCommand.Parameters.AddWithValue("@FileUrl", Url);
                    sqlCommand.Parameters.AddWithValue("@ResourcePublicID", PublicID);
                    int Status = await sqlCommand.ExecuteNonQueryAsync();
                    if (Status <= 0)
                    {
                        response.IsSuccess = false;
                        response.Message = "Something Went Wrong";
                    }
                }

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exception Occur : Message : " + ex.Message;
            }
            finally
            {
                await _sqlConnection.CloseAsync();
                await _sqlConnection.DisposeAsync();
            }

            return response;
        }

        public async Task<UpdateAsArchiveTrashFileResponse> UpdateAsArchiveTrashFile(UpdateAsArchiveTrashFileRequest request)
        {
            UpdateAsArchiveTrashFileResponse response = new UpdateAsArchiveTrashFileResponse();
            response.IsSuccess = true;
            response.Message = "Successful";
            string SqlQuery = string.Empty;

            try
            {

                if (_sqlConnection.State != ConnectionState.Open)
                {
                    await _sqlConnection.OpenAsync();
                }

                if (request.OperationType.ToLowerInvariant().Equals("archive"))
                {
                     SqlQuery = @"
                                    UPDATE UploadDownloadFile
                                    SET IsArchive=1
                                    WHERE FileID=@FileID;
                                    ";
                }else if (request.OperationType.ToLowerInvariant().Equals("trash"))
                {
                     SqlQuery = @"
                                    UPDATE UploadDownloadFile
                                    SET IsActive=0
                                    WHERE FileID=@FileID;
                                    ";
                }

                using (SqlCommand sqlCommand = new SqlCommand(SqlQuery, _sqlConnection))
                {

                    sqlCommand.CommandType = CommandType.Text;
                    sqlCommand.CommandTimeout = 180;
                    sqlCommand.Parameters.AddWithValue("@FileID", request.FileID);
                    int Status = await sqlCommand.ExecuteNonQueryAsync();
                    if (Status <= 0)
                    {
                        response.IsSuccess = false;
                        response.Message = "Something Went Wrong";
                    }

                }

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exception Occurs : " + ex.Message;
            }
            finally
            {
                await _sqlConnection.CloseAsync();
                await _sqlConnection.DisposeAsync();
            }

            return response;
        }

        public async Task<DeleteFileResponse> DeleteFile(DeleteFileRequest request)
        {
            DeleteFileResponse response = new DeleteFileResponse();
            response.IsSuccess = true;
            response.Message = "Delete File Successful";

            string ImageExtensionList = ".webp, .svg, .png, .pjpeg, .jfif, .jpeg, .jpg, .gif, .avif, .apng";
            string VideoExtensionList = ".mp4, .mov, .wmv, .avi, .avchd, .flv, .f4v, .swf, .mkv, .webm , .html5, .mpeg-2, mpeg-4, .mts, .h264";
            string ProjectUsableExtensionList = ".txt, .pdf, .xls, .csv, .xlsx";
            string FileExtension = string.Empty, Result = string.Empty;

            try
            {
                Account account = new Account(
                                _configuration["CloudinarySettings:CloudName"],
                                _configuration["CloudinarySettings:ApiKey"],
                                _configuration["CloudinarySettings:ApiSecret"]);

                
                Cloudinary cloudinary = new Cloudinary(account);

                string[] FileName = request.FileName.Split(".");
                FileExtension = "." + FileName[1].ToString();

                if (ImageExtensionList.Contains(FileExtension))
                {
                    var deletionParams = new DeletionParams(request.PublicID)
                    {
                        ResourceType = ResourceType.Image
                    };

                    var deletionResult = cloudinary.Destroy(deletionParams);
                    Result = deletionResult.Result.ToString();
                }
                else if (VideoExtensionList.Contains(FileExtension))
                {
                    var deletionParams = new DeletionParams(request.PublicID)
                    {
                        ResourceType = ResourceType.Video
                    };

                    var deletionResult = cloudinary.Destroy(deletionParams);
                    Result = deletionResult.Result.ToString();
                }
                else if (!ProjectUsableExtensionList.Contains(FileExtension))
                {
                    var deletionParams = new DeletionParams(request.PublicID)
                    {
                        ResourceType = ResourceType.Raw
                    };

                    var deletionResult = cloudinary.Destroy(deletionParams);
                    Result = deletionResult.Result.ToString();
                }
                else
                {
                    var deletionParams = new DeletionParams(request.PublicID)
                    {
                        ResourceType = ResourceType.Raw
                    };

                    var deletionResult = cloudinary.Destroy(deletionParams);
                    Result = deletionResult.Result.ToString();
                }

                

                if (Result.ToLower() != "ok")
                {
                    response.IsSuccess = false;
                    response.Message = "Something Went To Wrong In Cloudinary Destroy Method";
                }

                if (_sqlConnection.State != ConnectionState.Open)
                {
                    await _sqlConnection.OpenAsync();
                }

                string SqlQuery = @"
                                    DELETE
                                    from UploadDownloadFile
                                    WHERE FileID=@FileID;
                                    ";

                using (SqlCommand sqlCommand = new SqlCommand(SqlQuery, _sqlConnection))
                {

                    sqlCommand.CommandType = CommandType.Text;
                    sqlCommand.CommandTimeout = 180;
                    sqlCommand.Parameters.AddWithValue("@FileID", request.FileID);
                    int Status = await sqlCommand.ExecuteNonQueryAsync();
                    if (Status <= 0)
                    {
                        response.IsSuccess = false;
                        response.Message = "Something Went Wrong";
                    }

                }

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exception Occurs : " + ex.Message;
            }
            finally
            {
                await _sqlConnection.CloseAsync();
                await _sqlConnection.DisposeAsync();
            }

            return response;
        }

        public async Task<RestoreTrashArchiveFileResponse> RestoreTrashArchiveFile(RestoreTrashArchiveFileRequest request)
        {
            RestoreTrashArchiveFileResponse response = new RestoreTrashArchiveFileResponse();
            response.IsSuccess = true;
            response.Message = "Successful";

            try
            {

                if (_sqlConnection.State != ConnectionState.Open)
                {
                    await _sqlConnection.OpenAsync();
                }

                string SqlQuery = string.Empty;

                if (request.OperationType.ToLowerInvariant() == "archive")
                {

                     SqlQuery = @"
                                    UPDATE UploadDownloadFile
                                    SET IsArchive=0
                                    WHERE FileID=@FileID;
                                   ";

                }
                else if (request.OperationType.ToLowerInvariant() == "trash")
                {
                     SqlQuery = @"
                                    UPDATE UploadDownloadFile
                                    SET IsActive=1
                                    WHERE FileID=@FileID;
                                   ";
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid Operation Type";
                    return response;
                }



                using (SqlCommand sqlCommand = new SqlCommand(SqlQuery, _sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.Text;
                    sqlCommand.CommandTimeout = 180;
                    sqlCommand.Parameters.AddWithValue("@FileID", request.FileID);
                    int Status = await sqlCommand.ExecuteNonQueryAsync();
                    if (Status <= 0)
                    {
                        response.IsSuccess = false;
                        response.Message = "Something Went Wrong";
                    }
                }

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exception Occur : Message : " + ex.Message;
            }
            finally
            {
                await _sqlConnection.CloseAsync();
                await _sqlConnection.DisposeAsync();
            }

            return response;
        }
    }
}
