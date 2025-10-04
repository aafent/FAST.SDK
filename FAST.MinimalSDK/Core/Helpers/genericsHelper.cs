using FAST.Strings;

namespace FAST.Core
{
    public static class genericsHelper
    {

        // (v) add 12/12/2019
        public static T[] initializeArray<T>(int length) where T : new()
        {
            T[] array = new T[length];
            for (int i = 0; i < length; ++i)
            {
                array[i] = new T();
            }

            return array;
        }    
        
        public static T[] initializeArrayWithDefault<T>(int length) 
        {
            T[] array = new T[length];
            for (int i = 0; i < length; ++i)
            {
                array[i] = default(T);
            }

            return array;
        }     


        // Return the largest of the values.
        private static T max<T>(params T[] values) where T : IComparable<T>
        {
            T max = values[0];
            for (int i = 1; i < values.Length; i++)
                if (values[i].CompareTo(max) > 0) max = values[i];
            return max;
        }


        public static void swap<T>(ref T input1, ref T input2)
        {
            T temp = default(T);
            temp = input2;
            input2 = input1;
            input1 = temp;
        }


        public static bool tryToGetObjectByNameFromDictionary<T>(IDictionary<string, object> TempData, string variableName,out T model)
        {
            model = default(T);
            try
            {
                if (!TempData.ContainsKey(variableName))
                {
                    return false;
                }
                model = converters.compactStringToObject<T>(TempData[variableName] as string);
                if (model == null)
                {
                    return false;
                }

                // TODO: Add insert logic here

                return true; 
            }
            catch 
            {
                return false;
            }
        }


        // (v) add 31/12/2019
        public static List<Dictionary<TKey, TValue>> toListOfDictionaries<TKey, TValue>(params Dictionary<TKey, TValue>[] dictionaries)
        {
            return dictionaries.ToList();
        }

        public static Dictionary<TKey, TValue> mergeDictionaries<TKey, TValue>(IEnumerable<Dictionary<TKey, TValue>> dictionaries)
        {
            var result = dictionaries.SelectMany(dict => dict)
                         .ToLookup(pair => pair.Key, pair => pair.Value)
                         .ToDictionary(group => group.Key, group => group.First());
            return result;
        }

        // (v) add 10/1/2020
        public static Dictionary<TKey, TValue> mergeDictionaries<TKey, TValue>(Dictionary<TKey, TValue> dic1, Dictionary<TKey, TValue> dic2=null, Dictionary<TKey, TValue> dic3=null, Dictionary<TKey, TValue> dic4=null)
        {
            var list = toListOfDictionaries<TKey, TValue>(dic1);
            if (dic2 != null) list.Add(dic2);
            if (dic3 != null) list.Add(dic3);
            if (dic4 != null) list.Add(dic4);
            return mergeDictionaries<TKey, TValue>(list);
        }


        /// <summary>
        /// Convert enumeration to dictionary 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Dictionary<int, string> EnumNamedValues<T>() where T : System.Enum
        {
            // (>) https://stackoverflow.com/questions/79126/create-generic-method-constraining-t-to-an-enum

            var result = new Dictionary<int, string>();
            var values = Enum.GetValues(typeof(T));

            foreach (int item in values)
                result.Add(item, Enum.GetName(typeof(T), item));
            return result;
        }

        /// <summary>
        /// Convert an Enum type to enumerable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> EnumToEnumerable<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

    }


    
    // (!)     05/08/2022   generics class anyType<T> moved from FAST.Core.genericsHelper to FAST.Types 



}
