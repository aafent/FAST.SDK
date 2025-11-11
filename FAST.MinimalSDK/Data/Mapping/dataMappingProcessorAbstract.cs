using FAST.Core;

namespace FAST.Data
{
    /// <summary>
    /// Data mapping options, 
    /// Use to pass parameters and hold the passed expression
    /// </summary>
    public class dataMappingOptions : IdataMappingAddOnsCollection
    {
        /// <summary>
        /// if True, the argument definitionDirection will be overwritten by the expression 
        /// default is false
        /// </summary>
        public bool overrideDirectionFromExpression { get; set; } = false;

        /// <summary>
        /// Force parsing before each mapping
        /// </summary>
        public bool forceParsing { get; set; } = true;

        /// <summary>
        /// Add-Ons methods
        /// </summary>
        public Dictionary<string, Func<string, object, object> > addOns { get; private set;} = null;

        /// <summary>
        /// Reset the parsing metadata
        /// </summary>
        public void reset() => this.internalData = null;

        /// <summary>
        /// Add an identifier/mapping-command to the add-ons
        /// </summary>
        /// <param name="name">The name identifier</param>
        /// <param name="addOnMethod">The delegate, string is the arguments to the methods, object is the input value and returns an object with the output value</param>
        public void add(string name, Func<string,object,object> addOnMethod)
        {
            if (addOns==null) addOns=new();
            this.addOns.Add(name,addOnMethod);
        }

        #region (+) internal data
        internal internalParssingData internalData = null;
        internal class internalParssingData
        {
            public bool returnValue = true;
            public bool returnNull = false;
            public string inExpr = null;
            public string outExpr = null;
        }
        #endregion (+) internal data

    }

    
    /// <summary>
    /// The data mapping abstract processor
    /// </summary>
    public abstract class dataMappingProcessorAbstract
    {
        /* 
        Rules Syntax:
        ------------------------------------------------------------------------------------------------
        First Character:
        ................
        * if 1st Character is - the rule is commented out.
        * if 1st Character is < the rule expression is for input and ends with end of expression or < or with <>
        * if 1st Character is < and ends with <> the characters after the <> is the expression for output and ends at the end of the expression
        * if 1st Character is > the rule expression is for output and ends to the end of the expression
        * any other character, defines both expression

        Mapping rules separated by ; if only one mapping rule exists the semicolon is optional
        the mapping rules are running from left to right
        ex: rule1;rule2;rule3

        Rules:
        ......
        * IDENTIFIER or IDENTIFIER.NAME
        * COMMAND (build-in command OR addOns command)
        *  =value  (to assign the value )
        
        example of identifier: G30 or G30.cps2
        example of assignment: =YES


        Commands:
        .........
        * empty   (to assign enter empty space)
        * null    (to assign null value)
        

        All together: 
            <G30<>empty;null;G12.cps      (for incoming map with G30 for outgoing assign empty string value, then assign null value, then map with cps field of G12)
            -This is a commented-out line
            G13                           (map both directions with G13)
            G13.xyz                       (map both directions with G13 field xyz)
            <G13.xyz<>G13.abc             (map incoming with G13 field xyz and outgoing with abc field of G13)
            <G75.des<>G75.des.cod         (map incoming with G75 fields des and outgoing with G75 field des with key the field cod
        */


        /// <summary>
        /// Parse the expression, 
        /// the passed metadata are stored in the options class
        /// </summary>
        /// <param name="options">The mapping options</param>
        /// <param name="definitionDirection">The definition of the directions of the mapping expression</param>
        /// <param name="mappingExpression">The definition of the mapping expression</param>
        public void parse( dataMappingOptions options,
                           dataMappingDirection definitionDirection, // incoming,outgoing,both
                           string mappingExpression
                          ) 
        {

            // (v) init

            options.internalData = new();
            // (from object init)> options.internalData.returnValue = true;
            // (from object init)> options.internalData.returnNull = false;

            //
            // (v) parse the rules expression
            //

            if (string.IsNullOrWhiteSpace(mappingExpression)) return; // nothing more to do, return the input value

            mappingExpression = mappingExpression.Trim();

            if (mappingExpression.StartsWith('-')) return; // nothing more to do, the expression is commented-out, so return the input value

            #region (+) split the expression to input, output and both
            //
            string inExpr = null;
            string outExpr = null;
            if (options.overrideDirectionFromExpression)
            {
                int p1;
                if (mappingExpression.StartsWith('>')) // output only expression
                {
                    outExpr = mappingExpression.Substring(1).Trim();
                }
                else if (mappingExpression.StartsWith('<')) // input expression (without output or with output)
                {
                    p1 = mappingExpression.IndexOf('<', 1); // the next < after the first one.
                    if (p1 < 0) // no < found
                        inExpr = mappingExpression.Substring(1);
                    else
                    {
                        inExpr = mappingExpression.Substring(1, p1 - 1);
                        mappingExpression = mappingExpression.Substring(p1 + 1);
                        if (!string.IsNullOrEmpty(mappingExpression))
                        {
                            if (mappingExpression.StartsWith('>')) outExpr = mappingExpression.Substring(1);
                            else
                            {
                                if (mappingExpression.StartsWith('<'))
                                    throw new Exception("The incoming expression must be declared first, then the outgoing");
                                throw new Exception($"Unexpected character found after second > {mappingExpression}");
                            }
                        }

                    }
                }
                else // (v) both
                {
                    inExpr = mappingExpression;
                    outExpr = mappingExpression;
                }

                options.internalData.inExpr = inExpr;
                options.internalData.outExpr = outExpr;
            }
            else // (v) if it is not permmited by the options to override the definition directions from the syntax
            {
                switch (definitionDirection)
                {
                    case dataMappingDirection.outgoing:
                        options.internalData.outExpr = mappingExpression;
                        break;
                    case dataMappingDirection.incoming:
                        options.internalData.inExpr = mappingExpression;
                        break;
                    case dataMappingDirection.both:
                        options.internalData.inExpr = mappingExpression;
                        options.internalData.outExpr = mappingExpression;
                        break;
                    case dataMappingDirection.none:
                        options.internalData.returnNull = true;
                        return;
                }
            }
            //
            #endregion (+) split the expression to input, output and both

            
            options.internalData.returnValue=false; // (<) now-on run the expression to return the value
            return;
        }


        #region (+) Main mapping methods    

        /// <summary>
        /// Map input data using the expression
        /// </summary>
        /// <typeparam name="T">The type of the value to be mapped</typeparam>
        /// <param name="currentDirection">The current directions, can be incoming or outgoing</param>
        /// <param name="options">The mapping options</param>
        /// <param name="forceParse">Force parsing before execution</param>
        /// <param name="definitionDirection">The definition of the directions of the mapping expression</param>
        /// <param name="mappingExpression">The definition of the mapping expression</param>
        /// <param name="input">The input value (string)</param>
        /// <returns>Type,The return value casted to the given type</returns>
        public T map<T>(dataMappingDirection currentDirection, // incoming,outgoing
            dataMappingOptions options,
            dataMappingDirection definitionDirection, // incoming,outgoing,both
            string mappingExpression,
            T input) where T : class
        {
             if (currentDirection == dataMappingDirection.none || currentDirection == dataMappingDirection.both)
                 throw new ArgumentException("Current direction cannot be NONE or BOTH");
            if (options.internalData == null || options.forceParsing) // (<) expression needs parse;
            {
                this.parse(options, definitionDirection, mappingExpression);
            }
            return map<T>(currentDirection, options, input);
        }


        /// <summary>
        /// Mapping used parrsed expression
        /// </summary>
        /// <typeparam name="T">The type of the value to be mapped</typeparam>
        /// <param name="currentDirection">The current directions, can be incoming or outgoing</param>
        /// <param name="options">The mapping options</param>
        /// <param name="input">The input value (string)</param>
        /// <returns>Type,The return value casted to the given type</returns>
        public T map<T>(dataMappingDirection currentDirection, // incoming,outgoing
                            dataMappingOptions options,
                            T input) where T : class
        {

            if (options.internalData.returnValue) return input;
            if (options.internalData.returnNull) return null;
            
            // (i) Example of input: <G75.des<>G75.des.cod

            string mappingExpression;
            bool isIncoming = false;
            switch (currentDirection)
            {
                case dataMappingDirection.outgoing:
                    mappingExpression = options.internalData.outExpr;
                    break;
                case dataMappingDirection.incoming:
                    mappingExpression = options.internalData.inExpr;
                    isIncoming = true;
                    break;
                default:
                    throw new Exception("Internal error MAPPING1");
            }

            //
            // (v) run the rules
            //
            if (string.IsNullOrWhiteSpace(mappingExpression)) return input; // nothing more to do, no expression
                
            T returnValue = input;
            var expressionRules = mappingExpression.Split(';');

            Type type = typeHelper.getTypeOrDefault(input);
            foreach (var rule in expressionRules)
            {
                if (rule.StartsWith('='))
                {
                    returnValue = (T)castValue(type, rule.Substring(1));
                    continue; // goto next rule
                }

                // (v) full word rules
                switch (rule.ToLower())
                {
                    case "nop":
                        continue; // goto next rule
                    case "null":
                        returnValue = null;
                        continue;
                    case "empty":
                        if ( (input == null) || (typeHelper.isAnyOf(type,typeof(string),typeof(object) )   ) )
                        { 
                            returnValue = (T)castValue(type, string.Empty);
                            continue;
                        } 
                        else if (typeHelper.isNumeric(type) ) 
                        {
                            returnValue = (T)castValue(type, "0");
                            continue;
                        } 
                        else if (type == typeof(Boolean)) 
                        {
                            returnValue = (T)castValue(type, string.Empty);
                            continue;
                        }
                        else if (typeHelper.isDate(type))
                        {
                            returnValue = (T)castValue(type, DateTime.MinValue.ToString() );
                            continue;
                        }
                        else
                        {
                            returnValue = (T)castValue(type, string.Empty);
                            continue;
                        }
                }

                // (v) if reach at this point, can be only IDENTIFIER with or without comma (,) OR dot (.)
                var p1 = rule.IndexOf(',');
                if(p1 < 0 ) p1 = rule.IndexOf('.');
                object obj;
                if (p1 < 0) // (<) DONT have arguments
                {
                    if (options.addOns != null )
                    {
                        if (options.addOns.ContainsKey(rule))
                        {
                            returnValue = (T)options.addOns[rule](null, returnValue);
                            continue; // with next rule
                        }
                    }

                    // (v) finally invoke the abstract method
                    if (tryMapping(isIncoming, rule, returnValue, out obj))
                    {
                        returnValue = (T)obj;
                        continue; // with next rule
                    }
                }
                else // (<) HAVE arguments
                {
                    var part1 = rule.Substring(0, p1);
                    var part2 = rule.Substring(p1 + 1);

                    if (options.addOns != null)
                    {
                        if (options.addOns.ContainsKey(part1))
                        {
                            returnValue = (T)options.addOns[part1](part2, returnValue);
                            continue; // with next rule
                        }
                    }

                    // (v) finally invoke the abstract method
                    if (tryMapping(isIncoming, part1, part2, returnValue, out obj))
                    {
                        returnValue = (T)obj;
                        continue; // with next rule
                    }
                }

            }

            return returnValue;
        }


        #endregion (+) Main mapping methods    

        #region (+) more mapping methods, wrappers to the map<T>()

        /// <summary>
        /// Mapping for specific direction
        /// </summary>
        /// <typeparam name="T">The type of the value to be mapped</typeparam>
        /// <param name="currentDirection">The current directions, can be incoming or outgoing</param>
        /// <param name="options">The mapping options</param>
        /// <param name="mappingExpression">The definition of the mapping expression</param>
        /// <param name="input">The input value (string)</param>
        /// <returns>Type,The return value casted to the given type</returns>
        public T map<T>(dataMappingDirection currentDirection, // incoming,outgoing
                            dataMappingOptions options,
                            string mappingExpression,
                            T input) where T : class
            => map<T>(currentDirection, options, currentDirection, mappingExpression, input);

        /// <summary>
        /// Map incoming data 
        /// </summary>
        /// <typeparam name="T">The type of the value to be mapped</typeparam>
        /// <param name="options">The mapping options</param>
        /// <param name="mappingExpression">The definition of the mapping expression</param>
        /// <param name="input">The input value (string)</param>
        /// <returns>Type,The return value casted to the given type</returns>
        public T mapIncoming<T>(dataMappingOptions options,
                                string mappingExpression,
                                T input) where T : class
            => map<T>(dataMappingDirection.incoming, options, dataMappingDirection.incoming, mappingExpression, input);

        /// <summary>
        /// Map outgoing data 
        /// </summary>
        /// <typeparam name="T">The type of the value to be mapped</typeparam>
        /// <param name="options">The mapping options</param>
        /// <param name="mappingExpression">The definition of the mapping expression</param>
        /// <param name="input">The input value (string)</param>
        /// <returns>Type,The return value casted to the given type</returns>
        public T mapOutgoing<T>(dataMappingOptions options,
                                string mappingExpression,
                                T input) where T : class
            => map<T>(dataMappingDirection.outgoing, options, dataMappingDirection.outgoing, mappingExpression, input);

        #endregion (+) more mapping methods, wrappers to the map<T>()

        #region (+) more mapping methods, wrappers to the map<T> using the already parsed expression

        /// <summary>
        /// Map incoming data 
        /// </summary>
        /// <typeparam name="T">The type of the value to be mapped</typeparam>
        /// <param name="options">The mapping options</param>
        /// <param name="input">The input value (string)</param>
        /// <returns>Type,The return value casted to the given type</returns>
        public T mapIncoming<T>(dataMappingOptions options,
                                T input) where T : class
            => map<T>(dataMappingDirection.incoming, options, input);

        /// <summary>
        /// Map outgoing data 
        /// </summary>
        /// <typeparam name="T">The type of the value to be mapped</typeparam>
        /// <param name="options">The mapping options</param>
        /// <param name="input">The input value (string)</param>
        /// <returns>Type,The return value casted to the given type</returns>
        public T mapOutgoing<T>(dataMappingOptions options,
                                T input) where T : class
            => map<T>(dataMappingDirection.outgoing, options, input);

        #endregion (+) more mapping methods, wrappers to the map<T> using the already parsed expression


        /// <summary>
        /// Cast a string value to given type,
        /// It is wrapped non-static method of the static dataMappingHelper.castValue()
        /// </summary>
        /// <param name="toType">The type to return</param>
        /// <param name="value">The string value to be casted</param>
        /// <returns>The value casted to the requested type</returns>
        public object castValue(Type toType, string value) => dataMappingHelper.castValue(toType, value);


        #region (+) Abstract methods
        /// <summary>
        /// Abstract method to implement a IDENTIFIER mapping
        /// </summary>
        /// <param name="isIncoming">True for Incoming directions, False for outgoing</param>
        /// <param name="identifier">The mapping identifier</param>
        /// <param name="input">The input value</param>
        /// <param name="value">The output value</param>
        /// <returns>Boolean, true if the mapping succeeds</returns>
        public abstract bool tryMapping(bool isIncoming, string identifier, object input, out object value);


        /// <summary>
        /// Abstract method to implement a IDENTIFIER mapping
        /// </summary>
        /// <param name="isIncoming">True for Incoming directions, False for outgoing</param>
        /// <param name="identifier">The mapping identifier</param>
        /// <param name="args">The arguments to the identifier</param>
        /// <param name="input">The input value</param>
        /// <param name="value">The output value</param>
        /// <returns>Boolean, true if the mapping succeeds</returns>
        public abstract bool tryMapping(bool isIncoming, string identifier, string args, object input, out object value);

        #endregion (+) Abstract methods

        /// <summary>
        /// INTERNAL example of the usage
        /// </summary>
        internal void example()
        {
            string output1, output2, output3; 
            string input = "THIS IS INPUT";
            var opt = new dataMappingOptions();

            // (v) test-1

            opt = new dataMappingOptions() { overrideDirectionFromExpression = true };
            output1 = map(dataMappingDirection.incoming, opt, dataMappingDirection.incoming, "<G75.des<>G75.des.cod", input);
            output2 = map(dataMappingDirection.outgoing, opt, dataMappingDirection.outgoing, "<G75.des<>G75.des.cod", input);
            return;


            // (v) test-2
            output1 = map(dataMappingDirection.outgoing, new(), dataMappingDirection.outgoing, "=Marika", input);

            // (v) test-3
            parse(opt, dataMappingDirection.outgoing, "=Marika;=kiki");
            output2 = map(dataMappingDirection.outgoing, opt, input);
            output3 = map(dataMappingDirection.outgoing, opt, input);


            opt = new dataMappingOptions() {  overrideDirectionFromExpression=true};
            output1 = map(dataMappingDirection.incoming, opt, dataMappingDirection.outgoing, "<=kiki<>=anna", input);
            output1 = map(dataMappingDirection.outgoing, opt, dataMappingDirection.outgoing, "<=kiki<>=anna", input);

            opt = new dataMappingOptions() { overrideDirectionFromExpression = true };

            opt.add("letters", dataMappingHelper.lettersOnly);

            var numbersOnly = (string arg, object input) =>
            {
                var tmps = Strings.stringParsingHelper.keepNumbers(input.ToString(), "");
                return castValue(input.GetType(), tmps);
            };
            opt.add("numbers", numbersOnly);

            output1 = map(dataMappingDirection.incoming, opt, dataMappingDirection.outgoing, "<=kiki<>=anna", input);
            opt.reset();
            output1 = map(dataMappingDirection.outgoing, opt, dataMappingDirection.outgoing, "<=kiki<>=1234ABCD;numbers", input);

        }

    }


}
