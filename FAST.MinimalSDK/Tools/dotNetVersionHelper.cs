using System.Runtime.InteropServices;

namespace FAST.Tools
{
    /// <summary>
    /// Helper to identify the .NET version at runtime 
    /// </summary>
    public static class dotNetVersionHelper
    {

        /// <summary>
        /// True if is Dot net Core runtime
        /// </summary>
        /// <returns>Boolean</returns>
        public static bool isDotNetCore()
        {
            return RuntimeInformation.FrameworkDescription.StartsWith(".NET Core") ||
                   !RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework");
        }


        /// <summary>
        /// Return the exact .NET version
        /// </summary>
        /// <returns></returns>
        public static string getDotNetVersion()
        {
            if (RuntimeInformation.FrameworkDescription.StartsWith(".NET Core"))
            {
                // .NET Core and .NET (5+)
                return RuntimeInformation.FrameworkDescription.Trim();
            }
#if (!NETCOREAPP)
            else if (RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework"))
            {
                // .NET Framework (4.x, 3.x, etc)
                Version version = Environment.Version;
                if (version.Major == 4)
                {
#if WINDOWS
                    // Determine the exact 4.x version.
                    using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full"))
                    {
                        if (key != null)
                        {
                            var release = (int?)key.GetValue("Release");
                            if (release != null)
                            {
                                if (release >= 533519) return ".NET 4.8.1";
                                if (release >= 528040) return ".NET 4.8";
                                if (release >= 461808) return ".NET 4.7.2";
                                if (release >= 461308) return ".NET 4.7.1";
                                if (release >= 460798) return ".NET 4.7";
                                if (release >= 394802) return ".NET 4.6.2";
                                if (release >= 394254) return ".NET 4.6.1";
                                if (release >= 393295) return ".NET 4.6";
                                if (release >= 378675) return ".NET 4.5.2";
                                if (release >= 378389) return ".NET 4.5.1";
                                if (release >= 378389) return ".NET 4.5";
                                // Handle older 4.x versions or unknown releases.
                                return ".NET 4.x";
                            }
                        }
                    }
#endif // WINDOWS
                    return ".NET 4.x"; //Fallback if registry key is not found.
                }
                else if (version.Major == 3)
                {
                    return ".NET 3.x"; // Potentially refine further if needed, but 3.x is less common now.
                }
                else if (version.Major == 2)
                {
                    return ".NET 2.x";
                }
                else if (version.Major == 1)
                {
                    return ".NET 1.x";
                }

            else
            {
                    return ".NET Framework (Unknown Version)";
                }
            }
#endif // (!NETCOREAPP)
            else
            {
                return "Unknown .NET Runtime";
            }

        }

}

}
