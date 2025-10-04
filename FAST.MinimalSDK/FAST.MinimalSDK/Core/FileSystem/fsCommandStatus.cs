namespace FAST.Core.FileSystem
{
    public enum fsCommandStatus : sbyte
    {
        success = 0,

        notFound = -2,
        alreadyExists = -3,
        rootFileAccessProhibited = -4,
        emptyArgument = -5,
        destinationDoesNotExists = -10,
        expectedFile = -11,
        expectedFolder = -12
    }
}
