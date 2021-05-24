using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;

namespace ObjectDetection.API.Controllers
{
    [ApiController]
    [Route("api/[action]")]
    public class ImageController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                Name = "It works"
            });
        }

        [HttpPost]
        public IActionResult Upload()
        {
            try
            {
                var file = Request.Form.Files[0];
                var folderName = Path.Combine("StaticFiles", "Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (file.Length <= 0)
                {
                    return BadRequest();
                }

                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName?.Trim('"');
                var fullPath = Path.Combine(pathToSave, fileName ?? throw new InvalidOperationException());
                var dbPath = Path.Combine(folderName, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                return Ok(dbPath);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpPost]
        public IActionResult DrawBoundingBox([FromQuery] string imageName, BoundingCoordinates[] coordinates)
        {
            var outputPath = Draw(imageName, coordinates);
            return Ok(outputPath);
        }

        private static string Draw(string imageName, IReadOnlyList<BoundingCoordinates> coordinates)
        {
            var folderName = Path.Combine("StaticFiles", "Images");
            var path = Path.Combine(Directory.GetCurrentDirectory(), folderName, imageName);

            var image = Image.FromFile(path);

            var originalImageHeight = image.Height;
            var originalImageWidth = image.Width;

            for (var i = 0; i < coordinates.Count; i++)
            {
                var colorIndex = i < Colors.Length ? i : i % 20; 
                var x = (uint)Math.Max(coordinates[i].X, 0);
                var y = (uint)Math.Max(coordinates[i].Y, 0);
                var width = (uint)Math.Min(originalImageWidth - x, coordinates[i].Width);
                var height = (uint)Math.Min(originalImageHeight - y, coordinates[i].Height);

                var text = $"{coordinates[i].Label}";

                using var thumbnailGraphic = Graphics.FromImage(image);

                thumbnailGraphic.CompositingQuality = CompositingQuality.HighQuality;
                thumbnailGraphic.SmoothingMode = SmoothingMode.HighQuality;
                thumbnailGraphic.InterpolationMode = InterpolationMode.HighQualityBicubic;

                // Define Text Options
                var drawFont = new Font("Arial", 12, FontStyle.Bold);
                var size = thumbnailGraphic.MeasureString(text, drawFont);
                var fontBrush = new SolidBrush(Color.Black);
                var atPoint = new Point((int)x, (int)y - (int)size.Height - 1);

                // Define BoundingBox options
                var pen = new Pen(Colors[colorIndex], 3.2f);
                var colorBrush = new SolidBrush(Colors[colorIndex]);

                thumbnailGraphic.FillRectangle(colorBrush, (int)x, (int)(y - size.Height - 1), (int)size.Width, (int)size.Height);

                thumbnailGraphic.DrawString(text, drawFont, fontBrush, atPoint);

                // Draw bounding box on image
                thumbnailGraphic.DrawRectangle(pen, x, y, width, height);
            }

            var outputPath = Path.Combine(Directory.GetCurrentDirectory(), folderName, "OutPut", imageName);

            image.Save(outputPath);
            image.Dispose();

            return Path.Combine(folderName, "OutPut", imageName);
        }

        private static readonly Color[] Colors =
        {
            Color.Red,
            Color.Khaki,
            Color.Fuchsia,
            Color.Silver,
            Color.RoyalBlue,
            Color.Green,
            Color.DarkOrange,
            Color.Purple,
            Color.Gold,
            Color.Aquamarine,
            Color.Lime,
            Color.AliceBlue,
            Color.Sienna,
            Color.Orchid,
            Color.Tan,
            Color.LightPink,
            Color.Yellow,
            Color.HotPink,
            Color.OliveDrab,
            Color.SandyBrown,
            Color.DarkTurquoise
        };
    }
}
