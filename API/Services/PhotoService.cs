using API.Helpers;
using API.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace API.Services;


//Add this service into the ApplicationServiceExtensions.cs

public class PhotoService : IPhotoService
{

    private readonly Cloudinary _cloudinary;
    public PhotoService(IOptions<CloudinarySettings> config)
    {
        var acc = new Account
        (
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret
        );

        _cloudinary = new Cloudinary(acc);
    }

    public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
    {
        var uploadResult = new ImageUploadResult();

        //To check if we have any file
        if (file.Length > 0)
        {
            //we need to access a stream of that file so we can 
            //upload that stream of stuff or the file into cloudinary
            //use of the stream to clean out the memory
            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                //specify the image size, focus on the face
                Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                //where the image going to 
                Folder = "da-net7u"
            };
            //UploadAsync is a Cludinary method to upload the image to it
            uploadResult = await _cloudinary.UploadAsync(uploadParams);
        }

        return uploadResult;
    }

    //Delete the image in the cloudinary
    public async Task<DeletionResult> DeletePhotoAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);

        return await _cloudinary.DestroyAsync(deleteParams);
    }
}