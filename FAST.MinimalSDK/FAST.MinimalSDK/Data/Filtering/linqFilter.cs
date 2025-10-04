using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace FAST.Data
{
    /// <summary>
    /// Example of use:
    ///     var filter = new linqFilter<filterSyntaxForAnsiSQL>();
    ///     var x1 =filter.to<sysobjects>(x => x.type == "P" && x.type != "S");
    ///     x1 = filter.to<sysobjects>(x1, "AND", x => x.name.Contains("XXX"));
    /// </summary>
    public class linqFilter<T> : IfilterProvider<T> where T:IfilterSyntax
    {
        public T syntax { get; set; }

        public linqFilter()
        {
            this.syntax = (T)Activator.CreateInstance(typeof(T));
        }



        public filterItem to<F>(Expression<Func<F, bool>> expression)
        {
            var loop = 1;
            return recurse(ref loop, expression.Body, isUnary: true);
        }
        public filterItem to<F>(filterItem filters, string @operator, Expression<Func<F, bool>> expression)
        {
            return syntax.concat(filters, @operator, to<F>(expression) );
        }
        public filterItem to<F>(filterItem filters, filterSyntaxOperators @operator, Expression<Func<F, bool>> expression)
        {
            return syntax.concat(filters, syntax.operatorToString(@operator), to<F>(expression) );
        }

        private filterItem recurse(ref int loop, Expression expression, bool isUnary = false, string prefix = null, string postfix = null)
        {
            if (expression is UnaryExpression)
            {
                var unary = (UnaryExpression)expression;
                // (!v) convert unary.NodeType to filterSyntaxOperators
                var oper=(filterSyntaxOperators)Enum.Parse(typeof(filterSyntaxOperators),unary.NodeType.ToString());
                return syntax.concat(syntax.operatorToString(oper), recurse(ref loop, unary.Operand, true));
            }
            if (expression is BinaryExpression)
            {
                var body = (BinaryExpression)expression;
                // (!v) convert body.NodeType to filterSyntaxOperators
                var oper=(filterSyntaxOperators)Enum.Parse(typeof(filterSyntaxOperators),body.NodeType.ToString());
                return syntax.concat(recurse(ref loop, body.Left), syntax.operatorToString(oper), recurse(ref loop, body.Right));
            }
            if (expression is ConstantExpression)
            {
                var constant = (ConstantExpression)expression;
                var value = constant.Value;
                if (value is int)
                {
                    return syntax.asSubject(value.ToString());
                }
                if (value is string)
                {
                    value = prefix + (string)value + postfix;
                }
                if (value is bool && isUnary)
                {
                    return syntax.concat(syntax.asParameter(loop++, value), syntax.operatorToString(filterSyntaxOperators.IsTrue), syntax.asSubject( syntax.valueForOperators(filterSyntaxOperators.IsTrue)) );
                }
                return syntax.asParameter(loop++, value);
            }
            if (expression is MemberExpression)
            {
                var member = (MemberExpression)expression;

                if (member.Member is PropertyInfo)
                {
                    var property = (PropertyInfo)member.Member;

                    var colName = property.Name; // SOS (!!!) Modify this to adapt to the name of the SQL table or SP. Now the property name is used.
                                                 //var colName = _tableDef.GetColumnNameFor(property.Name);

                    if (isUnary && member.Type == typeof(bool))
                    {
                        return syntax.concat(recurse(ref loop, expression), syntax.operatorToString(filterSyntaxOperators.Equal), syntax.asParameter(loop++, true));
                    }
                    return syntax.asSubject(syntax.fieldName(colName) );
                }
                if (member.Member is FieldInfo)
                {
                    var value = getValue(member);
                    if (value is string)
                    {
                        value = prefix + (string)value + postfix;
                    }
                    return syntax.asParameter(loop++, value);
                }
                throw new Exception($"Expression does not refer to a property or field: {expression}");
            }
            if (expression is MethodCallExpression)
            {
                var methodCall = (MethodCallExpression)expression;
                // LIKE queries:
                if (methodCall.Method == typeof(string).GetMethod("Contains", new[] { typeof(string) }))
                {
                    var oper = syntax.methodToOperator(filterSyntaxMethods.Contains);
                    return syntax.concat(recurse(ref loop, methodCall.Object), oper.Item1, recurse(ref loop, methodCall.Arguments[0], prefix: oper.Item2, postfix: oper.Item3));
                }
                if (methodCall.Method == typeof(string).GetMethod("StartsWith", new[] { typeof(string) }))
                {
                    var oper = syntax.methodToOperator(filterSyntaxMethods.StartsWith);
                    return syntax.concat(recurse(ref loop, methodCall.Object), oper.Item1, recurse(ref loop, methodCall.Arguments[0], prefix: oper.Item2, postfix: oper.Item3));
                }
                if (methodCall.Method == typeof(string).GetMethod("EndsWith", new[] { typeof(string) }))
                {
                    var oper = syntax.methodToOperator(filterSyntaxMethods.EndsWith);
                    return syntax.concat(recurse(ref loop, methodCall.Object), oper.Item1, recurse(ref loop, methodCall.Arguments[0], prefix: oper.Item2, postfix: oper.Item3));
                }
                // IN queries:
                if (methodCall.Method.Name == "Contains")
                {
                    Expression collection;
                    Expression property;
                    if (methodCall.Method.IsDefined(typeof(ExtensionAttribute)) && methodCall.Arguments.Count == 2)
                    {
                        collection = methodCall.Arguments[0];
                        property = methodCall.Arguments[1];
                    }
                    else if (!methodCall.Method.IsDefined(typeof(ExtensionAttribute)) && methodCall.Arguments.Count == 1)
                    {
                        collection = methodCall.Object;
                        property = methodCall.Arguments[0];
                    }
                    else
                    {
                        throw new Exception("Unsupported method call: " + methodCall.Method.Name);
                    }
                    var values = (IEnumerable)getValue(collection);
                    var oper = syntax.methodToOperator(filterSyntaxMethods.existsInList);
                    return syntax.concat(recurse(ref loop, property), oper.Item1, syntax.asCollection(ref loop, values));
                }
                throw new Exception("Unsupported method call: " + methodCall.Method.Name);
            }
            throw new Exception("Unsupported expression: " + expression.GetType().Name);
        }

        private static object getValue(Expression member)
        {
            // source: http://stackoverflow.com/a/2616980/291955
            var objectMember = Expression.Convert(member, typeof(object));
            var getterLambda = Expression.Lambda<Func<object>>(objectMember);
            var getter = getterLambda.Compile();
            return getter();
        }

    }

}
