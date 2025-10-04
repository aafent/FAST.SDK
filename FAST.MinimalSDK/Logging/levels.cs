namespace FAST.Logging
{
    //
    // Summary:
    //     Defines logging severity levels.
    //     similar with Microsoft.Extensions.Logging -> LogLevel enumaration
    public enum levels:sbyte
    {
        //
        // Summary:
        //     Logs that contain the most detailed messages. These messages may contain sensitive
        //     application data. These messages are disabled by default and should never be
        //     enabled in a production environment.
        trace = 0,
        //
        // Summary:
        //     Logs that are used for interactive investigation during development. These logs
        //     should primarily contain information useful for debugging and have no long-term
        //     value.
        debug = 1,
        //
        // Summary:
        //     Logs that track the general flow of the application. These logs should have long-term
        //     value.
        info = 2,
        //
        // Summary:
        //     Logs that highlight an abnormal or unexpected event in the application flow,
        //     but do not otherwise cause the application execution to stop.
        warning = 3,
        //
        // Summary:
        //     Logs that highlight when the current flow of execution is stopped due to a failure.
        //     These should indicate a failure in the current activity, not an application-wide
        //     failure.
        error = 4,
        //
        // Summary:
        //     Logs that describe an unrecoverable application or system crash, or a catastrophic
        //     failure that requires immediate attention.
        fatal = 5,
        //
        // Summary:
        //     Not used for writing log messages. Specifies that a logging category should not
        //     write any messages.
        none = 6
    }
}
