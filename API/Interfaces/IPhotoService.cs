using CloudinaryDotNet.Actions;

namespace API.Interfaces
{

    public interface IPhotoService
    {
        //To upload/delete the image to the cloudinary
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
        Task<DeletionResult> DeletePhotoAsync(string publicId);
    }

}