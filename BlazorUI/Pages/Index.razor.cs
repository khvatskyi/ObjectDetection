using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ML5;
using System;
using BlazorUI.Repository;

namespace BlazorUI.Pages
{
    public partial class Index
    {
        private string _labels;

        private string _imageUrl;

        private string ResultUrl { get; set; }

        private ObjectDetector ObjectDetector { get; set; }

        private ElementReference Refer { get; set; }

        [Inject]
        public IJSInProcessRuntime Runtime { get; set; }

        [Inject]
        public IImageHttpRepository Repository { get; set; }

        private void Recognize()
        {
            ObjectDetector = new ObjectDetector(Runtime, ObjectDetectorModel.COCOSSD);
            ObjectDetector.OnModelLoad += Load;
        }

        private void Load()
        {
            Console.WriteLine("Loaded Successfully");
            ObjectDetector.OnDetection += Det;
            ObjectDetector.Detect(Refer);
        }

        private async void Det(string err, ObjectResult[] result)
        {
            ResultUrl = await Repository.DrawBoundingBox(_imageUrl.Split('\\')[^1], result);
            _labels = string.Empty;

            foreach(var item in result)
            {
                _labels += item.label + ", ";
            }

            _labels = _labels.TrimEnd(new char[] { ',', ' ' });

            StateHasChanged();
            Console.WriteLine(result[0].label);
        }

        private void AssignImageUrl(string imgUrl)
        {
            _imageUrl = imgUrl;
            _labels = ResultUrl = null;
            StateHasChanged();
        }
    }
}
