using System.Linq.Expressions;

namespace FAST.Types
{

    /// <summary>
    /// A helper collection for type casting and convertions
    /// </summary>
    public static class typeConverter
    {
        /// <summary>
        /// Convert input object to specific TResult type by using compiling on the fly
        /// lamdba expression. The technic can convert (cast) classes with 
        /// implicit or explicit operators from the assignment. 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static TResult convertObjectUsingLambda<TResult>(object obj)
        {
            if (obj == null) return default(TResult);
            var p = Expression.Parameter(typeof(object));
            var c1 = Expression.Convert(p, obj.GetType());
            var c2 = Expression.Convert(c1, typeof(TResult));
            var e = (Func<object, TResult>)Expression.Lambda(c2, p).Compile();
            return e(obj);
        }

        /// <summary>
        /// Converts anything to anything (when is possible) by combining all the casting and convertion technics
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static TResult convertObject<TResult>(object obj)
        {
            if ( obj == null ) return default(TResult);
            if (typeof(TResult).IsClass)
            {
                return convertObjectUsingLambda<TResult>(obj);
            }    
            else
            {
                return (TResult)Convert.ChangeType(obj, typeof(TResult));
            }
        }



    }
}
