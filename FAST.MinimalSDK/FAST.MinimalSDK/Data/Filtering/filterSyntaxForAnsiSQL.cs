using FAST.Core;
using FAST.Strings;
using System.Collections;
using System.Text;

namespace FAST.Data
{
    public class filterSyntaxForAnsiSQL : IfilterSyntax
    {
        private static int variableCount = 0;

        public filterSyntaxForAnsiSQL()
        {
            variableCount = 0;
        }
        
	    public filterItem concat(string @operator, filterItem operand)
	{
            return new filterItem()
            {
                    variables = operand.variables,
                    syntax= $"({@operator} {operand.syntax})",
                    quotable=operand.quotable
                    
	    };
	}

	    public filterItem concat(filterItem left, string @operator, filterItem right)
	{
            var merged = new variablesContainer(left.variables);
            right.variables.copyVariablesTo(merged);
            var quotable = left.quotable.Union(right.quotable).ToList<String>();

            return new filterItem()
            {
                variables = merged,
                syntax = $"({left.syntax} {@operator} {right.syntax})",
                quotable=quotable
	    };
	}

	    public filterItem asParameter(int count, object value)
	{
            var variables = new variablesContainer();
            var quotable = new List<string>();
            string vname = $"V{variableCount}";
            variableCount++;
            variables.setAny(vname, value);
            if (variables.wasQuotableType) quotable.Add(vname);

            return new filterItem()
            {
                variables = variables,
                syntax = $"{{{vname}}}",
                quotable = quotable
	    };
	}

        public filterItem asCollection(ref int countStart, IEnumerable values)
	{
            var variables = new variablesContainer();
            var quotable = new List<string>();
            var syntax = new StringBuilder("(");

            bool isQuotable = false;
            bool isTheFirst = false;
            string valuesList = "";
	    foreach (var value in values)
	    {
                if (isTheFirst)
                {
                    isTheFirst = false;
                    variableCount++;
                    string vname = $"V{variableCount}_first";
                    string vnameFirst = $"V{variableCount}_first";
                    variables.setAny( vnameFirst, value);
                    isQuotable = variables.wasQuotableType;
                    if (isQuotable) quotable.Add(vname);
                }

                if (isQuotable)
                {
                    valuesList += stringValue.singleQuote + value + stringValue.singleQuote+stringValue.comma;
                }
                else
                {
                    valuesList += value+stringValue.comma;
                }
		countStart++;
	    }

            if (isTheFirst) valuesList = "null,";

            syntax.Append(valuesList);

            syntax[syntax.Length-1] = ')';

            return new filterItem()
            {
                variables = variables,
                quotable = quotable,
                syntax = syntax.ToString()
	    };
	}

	    public filterItem asSubject(string subjectValue )
	{
            return new filterItem()
            {
                variables = new variablesContainer(),
                quotable = new List<string>(),
                syntax = subjectValue
	    };
	}


        public string fieldName(string name)
        {
            return name;
        }

        public string operatorToString(filterSyntaxOperators @operator)
        {
            switch (@operator)
            {
                case filterSyntaxOperators.Add:
                    return "+";
                case filterSyntaxOperators.And:
                    return "&";
                case filterSyntaxOperators.AndAlso:
                    return "AND";
                case filterSyntaxOperators.Divide:
                    return "/";
                case filterSyntaxOperators.Equal:
                    return "=";
                case filterSyntaxOperators.ExclusiveOr:
                    return "^";
                case filterSyntaxOperators.GreaterThan:
                    return ">";
                case filterSyntaxOperators.GreaterThanOrEqual:
                    return ">=";
                case filterSyntaxOperators.LessThan:
                    return "<";
                case filterSyntaxOperators.LessThanOrEqual:
                    return "<=";
                case filterSyntaxOperators.Modulo:
                    return "%";
                case filterSyntaxOperators.Multiply:
                    return "*";
                case filterSyntaxOperators.Negate:
                    return "-";
                case filterSyntaxOperators.Not:
                    return "NOT";
                case filterSyntaxOperators.NotEqual:
                    return "<>";
                case filterSyntaxOperators.Or:
                    return "|";
                case filterSyntaxOperators.OrElse:
                    return "OR";
                case filterSyntaxOperators.Subtract:
                    return "-";
                case filterSyntaxOperators.IsTrue:
                    return "=";
            }
            throw new Exception($"Unsupported operator: {@operator}");
        }

        public string valueForOperators(filterSyntaxOperators @operator)
        {
            switch (@operator)
            {
                case filterSyntaxOperators.IsTrue:
                        return "1";
            }
            throw new Exception($"valueForOperators() Unsupported operator: {@operator}");
        }

        /// <summary>
        /// return the operator, the prefix and the suffix
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public Tuple<string,string,string> methodToOperator(filterSyntaxMethods method)
        {
            switch (method)
            {
                case filterSyntaxMethods.Contains:
                        return new Tuple<string,string,string>("LIKE","%","%");
                case filterSyntaxMethods.EndsWith:
                        return new Tuple<string,string,string>("LIKE","%","");
                case filterSyntaxMethods.StartsWith:
                        return new Tuple<string,string,string>("LIKE","","%");
                case filterSyntaxMethods.existsInList:
                        return new Tuple<string,string,string>("IN","","");

            }
            throw new Exception($"methodToOperator() Unsupported method: {method}");
        }

        public static filterItem and(filterItem a1, filterItem a2, params filterItem[] arg)
        {
            filterItem result;
            var SF = new rawFilter<filterSyntaxForAnsiSQL>();
            result = SF.to(a1, "AND", a2);
            for (int inx = 0; inx < arg.Length; inx++)
            {
                result = SF.to(result, "AND", arg[inx]);
            }

            return result;
        }

    }
}
