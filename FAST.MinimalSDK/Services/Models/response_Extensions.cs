namespace FAST.Services.Models
{
    /// <summary>
    /// Extension methods for response models
    /// </summary>
    public static class response_Extensions
    {

        /// <summary>
        /// Return the undelaying collection
        /// </summary>
        /// <typeparam name="collectionType"></typeparam>
        /// <param name="response"></param>
        /// <returns></returns>
        public static IEnumerable<collectionType> toCollection<collectionType>(this collectionResponseModel<collectionType> response) where collectionType : class, new()
        {
            return response.data;
        }


    }
}
