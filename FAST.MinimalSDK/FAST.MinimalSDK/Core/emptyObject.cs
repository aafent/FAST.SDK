namespace FAST.Core
{

    public interface IemptyObject
    {
    }
    public class emptyObject: IemptyObject
    {
        public static IemptyObject instance()
        {
            return new emptyObject();
        }
        public static bool itIs(object obj)
        {
            return obj is IemptyObject;
        }
        public static bool isNullOrEmpty(object obj)
        {
            if (obj == null ) return true;
            if ( itIs(obj) ) return true;
            return false;
        }

        public static object? toNullIfEmpty(object obj)
        {
            if ( isNullOrEmpty(obj) ) return null;
            return obj;
        }


        public static object toEmptyIfNull(object obj)
        {
            if ( obj == null ) return new emptyObject(); 
            return obj;
        }
    }
}
