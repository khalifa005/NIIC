using Application.ApplicationSettings;
using Application.Interfaces;
using Application.Photos;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Infrastructure.Photo
{
    public class PhotoAccessor : IPhotoAccessor
    {
        private readonly Cloudinary _cloudinary;
        public PhotoAccessor(IOptions<CloudinarySetting> options)
        {
            var account = new Account
                (
                options.Value.Name,
                options.Value.ApiKey,
                options.Value.ApiSecret
                );

            _cloudinary = new Cloudinary(account);
        }

        public PhotoUploadResponse AddPhoto(IFormFile file)
        {
           var uploadResult = new ImageUploadResult();

            if(file.Length > 0)
            {
                //to open memory stream then disposes it
                using(var stream = file.OpenReadStream())
                {
                    var uploadPrams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        Transformation = new Transformation().Height(500).Width(500)
                        .Crop("fill").Gravity("face")
                    };

                    uploadResult = _cloudinary.Upload(uploadPrams);
                }
            }

            if(uploadResult.Error != null)
                throw new Exception(uploadResult.Error.Message);


            return new PhotoUploadResponse
            {
                PublicId = uploadResult.PublicId,
                Url = uploadResult.SecureUrl.AbsoluteUri
            };
        }

        public string DeletePhoto(string publicId)
        {
           var deletePrams = new DeletionParams(publicId);
            var result = _cloudinary.Destroy(deletePrams);

            return result.Result == "Ok" ? "ok" : null;
        }
    }
}
