using System.Diagnostics.Contracts;

namespace FAST.Core
{
    /// <summary>
    /// Provides information about the current FAST.Core version
    /// </summary>
    public static class fastInformation
    {
        /// <summary>
        /// Current major version
        /// </summary>
        public const int currentMajorVersion = 1; 

        // (v) privates
        private const int yearOfTheVersion = 2021;
        private const int monthOfTheVersion = 7;
        private const int dayOfTheVersion = 7;

        /// <summary>
        /// The version, format is: n.yyyy.mm.dd
        /// where:  n is the major version 
        ///         yyyy is the year of the minor version
        ///         mm is the month of the minor version
        ///         dd is the date of the minor version
        /// </summary>
        public static string versionString = $"{currentMajorVersion}.{yearOfTheVersion:d4}.{monthOfTheVersion:d4}.{dayOfTheVersion:d4}";

        [Obsolete("Use versionString property instead")]
        public static string vesionString => versionString;


        /// <summary>
        /// The about string of the FAST.Core libraries
        /// </summary>
        /// <returns></returns>
        public static string about()
        {
            return string.Format("FAST : Libraries and Utilities for FAST Business development. Version: {0}", versionString);
        }

        /// <summary>
        /// Build a version string
        /// </summary>
        /// <param name="majorNumber">Major Version Number</param>
        /// <param name="year">Year of the version</param>
        /// <param name="month">Month of the version</param>
        /// <param name="day">Day of the version</param>
        /// <returns>Version string</returns>
        public static string toVersion(int majorNumber, int year, int month, int day)
        {
            Contract.Ensures(majorNumber >=0 & majorNumber <= currentMajorVersion, "Invalid Major Version Number");
            Contract.Ensures(year == 0 | year  >= 2020, "Invalid Year for the Version");
            Contract.Ensures(month >= 1 & month <= 12, "Invalid Month number for the Version");
            Contract.Ensures(day >= 1 & day <= 31, "Invalid Day number for the Version");

            return $"{majorNumber}.{year:d4}.{month:d2}.{day:d2}";
        }

        /// <summary>
        /// Current version of the application
        /// </summary>
        /// <returns></returns>
        public static string appCurrentVersion()
        {
#if !NETCOREAPP
            return System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed
                    ? System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString()
                    : Assembly.GetExecutingAssembly().GetName().Version.ToString();
#else
            return versionString;
#endif
        }

    }
}
