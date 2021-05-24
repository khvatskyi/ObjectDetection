using ML5;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazorUI.Repository
{
    public interface IImageHttpRepository
    {
        Task<string> UploadObjectImage(MultipartFormDataContent content);

        Task<string> DrawBoundingBox(string ImagePath, ObjectResult[] objectResult);
    }
}
