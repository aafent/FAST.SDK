using System.Collections;

namespace FAST.Data
{
    public interface IfilterSyntax
    {
        filterItem concat(string @operator, filterItem operand);

        filterItem concat(filterItem left, string @operator, filterItem right);

        filterItem asParameter(int count, object value);

        filterItem asCollection(ref int countStart, IEnumerable values);

        filterItem asSubject(string subjectValue);

        string fieldName(string name);

        string operatorToString(filterSyntaxOperators @operator);

        string valueForOperators(filterSyntaxOperators @operator);

        Tuple<string, string, string> methodToOperator(filterSyntaxMethods method);

    }
}
