using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BlazorUI.Repository;

namespace BlazorUI.Pages
{
    public partial class ImageUpload
    {
        [Parameter]
        public string ImgUrl { get; set; }

        [Parameter]
        public EventCallback<string> OnChange { get; set; }

        [Inject]
        public IImageHttpRepository Repository { get; set; }

        private async Task HandleSelected(InputFileChangeEventArgs e)
        {
            var imageFiles = e.GetMultipleFiles();

            foreach (var imageFile in imageFiles)
            {
                var resizedFile = await imageFile.RequestImageFileAsync("image/png", 300, 500);
                
                await using var ms = resizedFile.OpenReadStream(resizedFile.Size);

                var content = new MultipartFormDataContent();
                content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
                content.Add(new StreamContent(ms, Convert.ToInt32(resizedFile.Size)), "image", imageFile.Name);

                ImgUrl = await Repository.UploadObjectImage(content);

                await OnChange.InvokeAsync(ImgUrl);
            }
        }
    }
}
