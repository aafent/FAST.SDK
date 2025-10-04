namespace FAST.Arithmetic
{
    public class geoCoordinate
    {
        public double longitude { get; set; }
        public double latitude { get; set; }
        public geoCoordinate(double latitude, double longitude)
        {
            this.latitude = latitude;
            this.longitude = longitude;
        }
    }
}
