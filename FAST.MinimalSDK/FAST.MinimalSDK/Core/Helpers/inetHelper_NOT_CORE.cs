namespace FAST.Core
{
    /// (!) NOT CORE METHODS
    public static partial class inetHelper
    {
#if !NETCOREAPP


        public static string imageInFileToBase64(string filePath, bool htmlSRCPrefix = false)
        {
            using (System.Drawing.Image image = System.Drawing.Image.FromFile(filePath))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();

                    // Convert byte[] to Base64 String
                    string base64String = Convert.ToBase64String(imageBytes);

                    if (htmlSRCPrefix)
                    {
                        return "data:image/png;base64," + base64String;
                    }
                    else
                    {
                        return base64String;
                    }
                }
            }
        }




#endif
    }
}
