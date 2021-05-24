using AutoMapper;
using BlazorUI.Models;
using ML5;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazorUI.Repository
{
    public class ImageHttpRepository : IImageHttpRepository
    {
        private readonly IMapper _mapper;
        private readonly HttpClient _client;

        public ImageHttpRepository(IMapper mapper,
            HttpClient client)
        {
            _mapper = mapper;
            _client = client;
        }

        public async Task<string> DrawBoundingBox(string imageName, ObjectResult[] objectResult)
        {
            var content = _mapper.Map<BoxInfo[]>(objectResult);
            var response = await _client.PostAsJsonAsync($"api/DrawBoundingBox?ImageName={imageName}", content);

            return _client.BaseAddress.OriginalString + await response.Content.ReadAsStringAsync();
        }

        public async Task<string> UploadObjectImage(MultipartFormDataContent content)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/Upload")
            {
                Content = content
            };

            var postResult = await _client.SendAsync(request);
            var postContent = await postResult.Content.ReadAsStringAsync();

            if (!postResult.IsSuccessStatusCode)
            {
                throw new ApplicationException(postContent);
            }

            var imgUrl = Path.Combine(_client.BaseAddress.OriginalString, postContent);
            return imgUrl;
        }
    }
}
