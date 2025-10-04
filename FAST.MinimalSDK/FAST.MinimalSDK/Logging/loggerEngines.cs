namespace FAST.Logging
{
    /// <summary>
    /// Available logging engines
    /// </summary>
    public enum loggerEngines:sbyte
    {
        /// <summary>
        /// Do nothing engine, do not log
        /// </summary>
        doNotLog=0, 

        /// <summary>
        /// Log on console ouput
        /// </summary>
        console=1,

        /// <summary>
        /// Log to diagnostics service
        /// System.Diagnostics.Debug.WriteLine() will be used
        /// </summary>
        diagnostics = 5,

        /// <summary>
        /// FAST Services, Telemetry Controller
        /// </summary>
        fastTelemetry = 6,

        /// <summary>
        /// Runtime slot 1
        /// </summary>
        otherSlot1 =2, 

        /// <summary>
        /// Runtime slot 2
        /// </summary>
        otherSlot2=3, 

        /// <summary>
        /// Runtime slot 3
        /// </summary>
        otherSlot3=4


        // next will be: 7
        //todo: ELMAH, windows event
    }
}
