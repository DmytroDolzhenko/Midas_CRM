using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Application.Common.Interfaces
{
    public interface IFileService
    {
        Task<string> UploadImageAsycn(IFormFile file, string folderName);
    }
}
