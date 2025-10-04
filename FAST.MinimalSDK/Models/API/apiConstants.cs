
namespace FAST.Services.API
{
    public static class apiConstants
    {
        public const string routeRootPrefix = "/api";
        public const string routeVersion = "1001";
        public const string routeForController = $"{routeRootPrefix}-v{routeVersion}/[controller]";
        public const string routeForInvokation = $"{routeRootPrefix}-v{routeVersion}";
    }
}
