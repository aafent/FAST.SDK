using FAST.Core;

namespace FAST.Types
{

    // (v) 18/12/2019   add 
    //     05/08/2022   replaced by the new code and move from FAST.Core.genericsHelper to FAST.Types 
    public class anyType<T> : IniceToInstantiateOnConstructor
    {
        public T V { get; set; }

        public anyType()
        {}

        public anyType(T value):this()
        {
            this.V=value;
        }

        public static implicit operator anyType<T>(T value)
        {
            anyType<T> objectInstance = new() { V = value };
            return objectInstance;
        }

        public static implicit operator T(anyType<T> value)
        {
            if (value == null) return default(T);
            return value.V;
        }

        public override string ToString() { return V.ToString(); }
    }

    #region (o) old code for anyType<T>
    /*
    public class anyType<T>
    {
        private T _value;

        public T value
        {
            get
            {
                // insert desired logic here
                return _value;
            }
            set
            {
                // insert desired logic here
                _value = value;
            }
        }

        public static implicit operator T(anyType<T> valueToGet )
        {
            return valueToGet.value;
        }

        public static implicit operator anyType<T>(T valueToSet )
        {
            return new anyType<T> { value = valueToSet };
        }
    }
    */
    #endregion (o) old code for anyType<T>

}
