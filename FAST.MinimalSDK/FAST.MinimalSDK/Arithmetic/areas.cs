namespace FAST.Arithmetic
{
    public static class areas  
  {

    /// <summary>  
    /// Area of Circle is πr2.  
    /// value of pi is 3.14159 and r is the radius of the circle.  
    /// </summary>  
    /// <returns></returns>  
    public static double areaOfCircle(double radius)  
    {  
        return Math.PI * (radius * radius);  
    }  
  
    /// <summary>  
    /// Area of Square is side2.  
    /// Side * Side  
    /// </summary>  
    /// <returns></returns>  
    public static double areaOfSquare(double side)  
    {  
      return side * side;  
    }  
  
    /// <summary>  
    /// Area of Rectangle is L*W.  
    /// L is the length of one side and W is the width of one side  
    /// </summary>  
    /// <returns></returns>  
    public static double areaOfRectangle(double length, double width)  
    {  
      return length * width;  
    }  
  
    /// <summary>  
    /// Area of Traingle is b*h/2.  
    /// b is base and h is height of traingle  
    /// </summary>  
    /// <returns></returns>  
    public static double areaOfTraingle(double baseOfTraingle, double heightOfTraingle)  
    {  
      return (baseOfTraingle * heightOfTraingle)/2;  
    }  

    /// <summary>
    /// Calculate the area based on geo-coordinates 
    /// example of use:
    /// double area = GetArea(new GeoCoordinate[]{new GeoCoordinate(40.76822,-73.981567),new GeoCoordinate(40.76439,-73.97308),
    /// new GeoCoordinate(40.796787,-73.949398),new GeoCoordinate(40.800654,-73.958248),new GeoCoordinate(40.76822,-73.981567)});
    ///			Console.WriteLine(area/1000000 + " km^2");
    /// </summary>
    /// <param name="coords"></param>
    /// <returns></returns>
    public static double areaByCoordinates(geoCoordinate[] coords)
    {
	var area = 0.0;
	var len = coords.Length;
	if (len > 2)
        {
	    for (var i = 0; i < len - 1; i++)
            {
		geoCoordinate p1 = coords[i];
		geoCoordinate p2 = coords[i + 1];
		area += arithmeticHelper.rad(p2.longitude - p1.longitude) * (2 + Math.Sin(arithmeticHelper.rad(p1.latitude)) + Math.Sin(arithmeticHelper.rad(p2.latitude)));
	    }
	    area = area * 6378137.0 * 6378137.0 / 2.0;
	}
	return Math.Abs(area);
    }

  }  
}
