namespace ObjectDetection.API
{
    public class BoundingCoordinates
    {

        public string ImageName { get; set; }

        public string Label { get; set; }

        public double Width { get; set; }
        
        public double Height { get; set; }

        public double X { get; set; }

        public double Y { get; set; }
    }
}
