namespace FAST.Core
{

    /// <summary>
    /// An empty object to represent null or empty values in a type-safe way.
    /// </summary>
    public interface IemptyObject
    {
    }


    /// <summary>
    /// An empty object to represent null or empty values in a type-safe way.
    /// </summary>
    public class emptyObject: IemptyObject
    {
        /// <summary>
        /// Creates a new instance of an empty object.
        /// </summary>
        /// <returns></returns>
        public static IemptyObject instance()
        {
            return new emptyObject();
        }

        /// <summary>
        /// Return if an object is an empty object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool itIs(object obj)
        {
            return obj is IemptyObject;
        }

        /// <summary>
        /// Return if an object is null or an empty object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool isNullOrEmpty(object obj)
        {
            if (obj == null ) return true;
            if ( itIs(obj) ) return true;
            return false;
        }


        /// <summary>
        /// Return null if the object is null or an empty object, otherwise return the object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object? toNullIfEmpty(object obj)
        {
            if ( isNullOrEmpty(obj) ) return null;
            return obj;
        }

        /// <summary>
        /// Return an empty object if the object is null, otherwise return the object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object toEmptyIfNull(object obj)
        {
            if ( obj == null ) return new emptyObject(); 
            return obj;
        }
    }
}
