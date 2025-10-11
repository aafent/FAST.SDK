namespace FAST.API
{
    /// <summary>
    /// HTTP Verbs Supported by FAST http verbs
    /// for more info: https://www.c-sharpcorner.com/UploadFile/47fc0a/working-with-http-verbs/
    /// </summary>
    public enum httpVerbs : sbyte 
    {
        /// <summary>
        /// HTTP GET method requests a representation of the specified resource. Requests using GET should only retrieve data.
        /// </summary>
        get,
        /// <summary>
        /// The HTTP PUT method replaces all current representations of the target resource with the request payload.
        /// </summary>
        put,
        /// <summary>
        /// The HTTP POST method is used to submit an entity to the specified resource, often causing a change in state or side effects on the server.
        /// </summary>
        post,
        /// <summary>
        /// The HTTP DELETE method deletes the specified resource.
        /// </summary>
        delete,
        /// <summary>
        /// The HTTP PATCH method is used to apply partial modifications to a resource.
        /// </summary>
        patch
    }


}
