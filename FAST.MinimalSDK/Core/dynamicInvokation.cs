using System.Reflection;


namespace FAST.Core
{
    public abstract class dynamicInvokation
    {
        public static TRet Invoke<TRet>(object Obj, Type ObjType, string MethodName, params object[] CallerParams) where TRet : class
        {
            MemberInfo[] memberInfo = ObjType.GetMember(MethodName, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
            BindingFlags memberTypeFlag;
            int paramCount = 0;
            switch (memberInfo[0].MemberType)
            {
                default:
                    return null;
                case MemberTypes.Method:
                    MethodInfo methodInfo = ObjType.GetMethod(MethodName, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                    paramCount = methodInfo.GetParameters().Length;
                    memberTypeFlag = BindingFlags.InvokeMethod;
                    break;
                case MemberTypes.Property:
                    PropertyInfo propInfo = ObjType.GetProperty(MethodName, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                    paramCount = propInfo.GetIndexParameters().Length;
                    memberTypeFlag = BindingFlags.GetProperty;
                    break;
            }

            object[] paramArray = new object[paramCount];
            for (int i = 0; i < paramCount; ++i)
            {
                paramArray[i] = Missing.Value;
            }
            if (CallerParams != null)
                CallerParams.CopyTo(paramArray, 0);
            TRet ret = Obj.GetType().InvokeMember(MethodName, memberTypeFlag | BindingFlags.Instance | BindingFlags.Public,
                null, Obj, paramArray) as TRet;
            return ret;
        }


    }

}