namespace FAST.Core
{
    public static class string_Extensions
    {
        //NETFRAMEWORK, NET48, NET472, NET471, NET47, NET462, NET461, NET46, NET452, NET451, NET45, NET40, NET35, NET20
        //NETSTANDARD, NETSTANDARD2_1, NETSTANDARD2_0, NETSTANDARD1_6, NETSTANDARD1_5, NETSTANDARD1_4, NETSTANDARD1_3, NETSTANDARD1_2, NETSTANDARD1_1, NETSTANDARD1_0
        //NET5_0, NETCOREAPP, NETCOREAPP3_1, NETCOREAPP3_0, NETCOREAPP2_2, NETCOREAPP2_1, NETCOREAPP2_0, NETCOREAPP1_1, NETCOREAPP1_0
        /* (v) in .NET Framework but not in .Net Core and .NET Standard, you have to add these lines to the project xml file (project name) 
             <PropertyGroup>
		            <DefineConstants Condition=" '$(TargetFrameworkVersion)' == 'v4.7.2' ">NET472</DefineConstants>
              </PropertyGroup>
         */


#if NET472 
        public static string[] Split(this String value, string delemiter)
        {
             return value.Split(new string[] { delemiter }, StringSplitOptions.None);
        }
#endif
    }

}
