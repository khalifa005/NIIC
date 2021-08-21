using Application.Photos;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IPhotoAccessor
    {
        PhotoUploadResponse AddPhoto(IFormFile file);
        string DeletePhoto(string publicId);
    }
}
