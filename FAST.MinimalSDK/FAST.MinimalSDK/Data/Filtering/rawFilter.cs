using System.Collections;

namespace FAST.Data
{
    /// <summary>
    /// Example of use:
    /// </summary>
    public class rawFilter<T> : IfilterProvider<T> where T:IfilterSyntax
    {
        public T syntax { get; set; }

        public rawFilter()
        {
            this.syntax = (T)Activator.CreateInstance(typeof(T));
        }

        public filterItem to(string vname, string @operator, string value, bool isQuotable=false)
        {
            var item = new filterItem();
            item.variables.set(vname, value);
            if (isQuotable)
            {
                item.quotable.Add(vname);
            }
            item.syntax = $"{vname} {@operator} {{{vname}}}";

            return item;
        }
        public filterItem to(string vname, string @operator, object value)
        {
            var item = new filterItem();
            item.variables.setAny(vname, value);
            if (item.variables.wasQuotableType )
            {
                item.quotable.Add(vname);
            }
            item.syntax = $"{vname} {@operator} {{{vname}}}";

            return item;
        }
        public filterItem to(filterItem left, string @operator, filterItem right)
        {
            return syntax.concat(left, @operator, right );
        }

        public filterItem list(string vname, string setOperator, IEnumerable values )
        {
            int cnt = 0;
            var item = syntax.asCollection(ref cnt, values);
            item.syntax = $"{vname} {setOperator} {item.syntax}";

            return item;
        }

        /// <summary>
        /// Raw syntax to filter
        /// </summary>
        /// <param name="rawSyntax">Syntax for the filter</param>
        /// <returns></returns>
        public filterItem raw(string rawSyntax)
        {
            var item = syntax.asSubject(rawSyntax);

            return item;
        }

        /// <summary>
        /// Raw syntax to filter
        /// </summary>
        /// <param name="rawLeftSyntax">left part</param>
        /// <param name="rawOperatorSyntax">the operator</param>
        /// <param name="rawRightSyntax">right part</param>
        /// <returns></returns>
        public filterItem raw(string rawLeftSyntax, string rawOperatorSyntax, string rawRightSyntax)
        {
            var item = syntax.asSubject(rawLeftSyntax + " " + rawOperatorSyntax + " " + rawRightSyntax);

            return item;
        }


    }

}
