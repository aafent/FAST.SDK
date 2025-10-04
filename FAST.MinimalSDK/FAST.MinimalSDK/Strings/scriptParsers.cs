using FAST.Core;
using System.Xml.Serialization;

namespace FAST.Strings
{

    /// <summary>
    /// parser for parsing expressions containing variables with assignments and attributes
    /// </summary>
    public class variablesAndAttributes : IcopyTo<variablesAndAttributes>
    {

        /// <summary>
        /// The list of attributes
        /// </summary>
        [XmlArray("attributes"), XmlArrayItem("at")]
        public List<string> attributes = new List<string>();

        /// <summary>
        /// The list of assignments
        /// </summary>
        [XmlArray("assigments"), XmlArrayItem("as")]
        public List<stringsPair> assignments = new List<stringsPair>();

        /// <summary>
        /// The list of variables are needing quotes
        /// </summary>
        [XmlArray("quotes"), XmlArrayItem("v")]
        public List<string> variablesNeedsQuotes = new List<string>();


        /// <summary>
        /// Empty constructor 
        /// </summary>
        public variablesAndAttributes()
        {
        }

        /// <summary>
        /// Contructor with initial values taken from other variablesAndAttributes object
        /// </summary>
        /// <param name="cloneFromValues"></param>
        public variablesAndAttributes(variablesAndAttributes cloneFromValues)
        {
            this.attributes.AddRange(cloneFromValues.attributes.ToArray());
            //this.assignments.AddRange(defaultValues.assignments.ToArray());
            this.variablesNeedsQuotes.AddRange(cloneFromValues.variablesNeedsQuotes.ToArray());
            foreach (var assignment in cloneFromValues.assignments)
            {
                this.assignments.Add(new stringsPair(assignment));
            }

        }

        /// <summary>
        /// Indexer to access an assignment value by name
        /// </summary>
        /// <param name="assignmentName">The assignment name</param>
        /// <returns></returns>
        public string this[string assignmentName]
        {
            get
            {
                return assignments.First(f => f.Item1 == assignmentName).Item2;
            }
            set
            {
                if ( isAssignment(assignmentName) )
                { 
                    setAssignmentValue(assignmentName,value);
                } else
                {
                    //todo change the falst to needquotes values
                    addAssignment(assignmentName,value,false);
                }
            }
        }


        /// <summary>
        /// Remove an existing assignment
        /// </summary>
        /// <param name="variableName">Variable name to remove</param>
        public void removeAssignment(string variableName)
        {
            if (this.assignments.Any(t => t.Item1 == variableName))
            {
                this.assignments.RemoveAll(t => t.Item1 == variableName);
            }
        }

        /// <summary>
        /// Check if a variable name is an assignment
        /// </summary>
        /// <param name="variableName"></param>
        /// <returns></returns>
        public bool isAssignment(string variableName)
        {
            return this.assignments.Any(t => t.Item1 == variableName);
        }

        /// <summary>
        /// Add an assignment to the existing asssignments
        /// </summary>
        /// <param name="variableName">Name of the variable</param>
        /// <param name="value">Value of the variable</param>
        /// <param name="needQuotes">True if the value need quotes</param>
        public void addAssignment(string variableName, string value, bool needQuotes)
        {
            if ( isAssignment(variableName) ) throw new Exception($"Assignment for variable {variableName} already exists");
            assignments.Add(new stringsPair(variableName,value));
            if ( needQuotes ) variablesNeedsQuotes.Add(variableName);
        }

        /// <summary>
        /// Set the value for an assignment
        /// </summary>
        /// <param name="variableName">The variable name</param>
        /// <param name="value">The value to set</param>
        public void setAssignmentValue(string variableName, string value)
        {
            if (!isAssignment(variableName)) throw new Exception($"Assignment for variable {variableName} does not exists");
            
            removeAssignment(variableName);
            assignments.Add(new stringsPair(variableName, value));
        }

        /// <summary>
        /// Reset the object, clear all the asiignments and attributes
        /// </summary>
        public void clear()
        {
            this.variablesNeedsQuotes.Clear();
            this.assignments.Clear();
            this.attributes.Clear();
        }

        /// <summary>
        /// Copy current assignments and attributes to another variablesAndAttributes object 
        /// </summary>
        /// <param name="dest">The variablesAndAttributes object to receive the current assignments and attributes</param>
        /// <param name="doMerge">True if a merge will performed or False if the destination will cleared before the copy</param>
        public void copyTo(variablesAndAttributes dest,bool doMerge = true)
        {
            if (!doMerge)
            {
                dest.attributes.Clear();
                dest.assignments.Clear();
                dest.variablesNeedsQuotes.Clear();
            }
            dest.attributes = this.attributes.Union(dest.attributes).ToList();
            foreach (var assign in assignments)
            {
                dest.assignments.Add(new stringsPair(assign));
            }
            dest.variablesNeedsQuotes = this.variablesNeedsQuotes.Union(dest.variablesNeedsQuotes).ToList();
        }

        /// <summary>
        /// Copy current assignments and attributes to a variables container 
        /// </summary>
        /// <param name="dest"></param>
        public void copyTo(IvariablesContainer dest)
        {
            foreach (var assign in assignments)
            {
                dest.setAny(assign.left, assign.right);
            }
        }

        /// <summary>
        /// Get (copy) assignments from a collection of FastKeyValuePair
        /// </summary>
        /// <param name="keyValues"></param>
        public void copyAssignmentsFrom(IEnumerable<FastKeyValuePair<string,string>> keyValues)
        {
            foreach( var item in keyValues )
            {
                if (isAssignment(item.Key))
                {
                    var toChange=assignments.First(f=>f.Item1==item.Key);
                    if ( toChange!=null ) toChange.right=item.Value;
                } else
                { 
                    assignments.Add(new stringsPair(item) );
                }
            }
        }

        /// <summary>
        /// Get (copy) assignments from a collection of Tuples with 2 strings the key and the value
        /// </summary>
        /// <param name="keyValues">The tuple with the key and the value</param>
        public void copyAssignmentsFrom(IEnumerable<Tuple<string, string>> keyValues)
        {
            foreach (var item in keyValues)
            {
                if (isAssignment(item.Item1))
                {
                    var toChange = assignments.First(f => f.Item1 == item.Item1);
                    if (toChange != null) toChange.right = item.Item2;
                }
                else
                {
                    assignments.Add(new stringsPair(item));
                }
            }
        }

        /// <summary>
        /// Clone the object to another instance
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var obj = new variablesAndAttributes();
            this.copyTo(obj);
            return obj;
        }

        /// <summary>
        /// return the assignments as a collection of FastKeyValuePair
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FastKeyValuePair<string, string>> getAssignmentsAsKeyValuePairCollection()
        {
            return  assignments.Select(t => new FastKeyValuePair<string, string>(t.Item1, t.Item2))
                                .ToArray();
        }

    }

    
    /// <summary>
    /// Static parsers 
    /// </summary>
    public static class scriptParsers 
    {
        public static void testing_parsers(string input)
        {
            //string input = "aaa='10 1', dddd='marika thanasis' kiki='anna' attr1 attr2 var1=12, var2='aaa=xxx=222' xxx#comments";
            if (string.IsNullOrEmpty(input))
            {
                input = "var1=12 var2='marika kiki' attr1 attr2";
            }
            

            Console.WriteLine(input);
            Console.WriteLine("=========================================================================");
            variablesAndAttributes results = new variablesAndAttributes();
            parseForVariablesAndAttributes(input, null, results);
            foreach (var item in results.assignments)
            {
                Console.Write($"{item.Item1}={item.Item2}");
                if (results.variablesNeedsQuotes.Contains(item.Item1))
                {
                    Console.WriteLine("  (in quotes)");
                }
                else
                {
                    Console.WriteLine("");
                }
            }
            foreach (var item in results.attributes)
            {
                Console.WriteLine($"Attribute: {item}");
            }

        }

        /// <summary>
        /// Parse an expression and populate a variablesBuffer with the result assignments
        /// </summary>
        /// <param name="input">The input expression</param>
        /// <param name="quotes">The quotes pair (open,close)</param>
        /// <param name="results">The variables buffer result</param>
        public static void parseForVariablesAndAttributes(string input, stringsPair quotes, variablesContainer results)
        {
            variablesAndAttributes varAttr = new variablesAndAttributes();
            parseForVariablesAndAttributes(input, quotes, varAttr);
            var vars = new variablesContainer();
            toVariables(varAttr, vars);
            vars.copyVariablesTo(results);
            return;
        }

        /// <summary>
        /// Parse an expression and populate a variablesBuffer with the result assignments
        /// </summary>
        /// <param name="input">The input expression</param>
        /// <param name="results">The variables buffer result</param>
        public static void parseForVariablesAndAttributes(string input, variablesContainer results)
        {
            parseForVariablesAndAttributes(input,null,results);
        }

        /// <summary>
        /// Parse an expression and populate a variablesAndAttributes with the result assignments
        /// </summary>
        /// <param name="input">The input expression</param>
        /// <param name="quotes">The quotes pair (open,close)</param>
        /// <param name="results">The variablesAndAttributes result</param>
        public static void parseForVariablesAndAttributes(string input, stringsPair quotes, variablesAndAttributes results )
        {
            string word = "";
            int nextIndex = 0;
            int wordEnd = 0;
            int specialBackOffset = 0;
            stringsPair assignment = new stringsPair();
            input = input.Trim(); // remove not necessary leading and trailing white spaces

            if (quotes == null)
            {
                quotes = new stringsPair(stringValue.doubleQuote, stringValue.doubleQuote);
            }
            
            bool foundInQuotes = false;
            bool nextWordIsVariableValue= false;
            bool lastWordIsVariableValue = false; 


            // (v) remove the inline comments
            nextIndex = 0;
            while ( nextIndex >=0 )
            {
                if (nextIndex > input.Length) break;
                wordEnd = stringParsingHelper.nextWord(input, "=#", nextIndex, true, quotes, ref word, ref foundInQuotes);
                if (wordEnd == (-1) )
                {
                    nextIndex++;
                    continue;
                }

                // (v) check for inline comments
                if (!foundInQuotes)
                {
                    // (v) check and remove comments
                    int p1 = word.IndexOf("#");
                    if (p1 > 0)
                    {
                        word = word.Substring(0, p1);

                        wordEnd = input.Length;
                        //stopLooping = true;
                    }
                }

                if (nextWordIsVariableValue)
                {
                    // 
                    assignment.Item2 = word;
                    // 

                    nextWordIsVariableValue = false;
                    lastWordIsVariableValue = true;
                }

                specialBackOffset = 0;
                if (!foundInQuotes)
                {
                    // (v) check for variable name:  var1=....
                    if (word.Substring(word.Length - 1, 1) == "=")
                    {
                        nextWordIsVariableValue = true;
                        word = word.Substring(0, word.Length - 1);
                    }
                    else // (v) check for variable name:  var1=value....
                    {
                        int p1 = word.IndexOf("=");
                        if (p1 > 0) // at least at the second position
                        {
                            nextWordIsVariableValue = false;
                            assignment = new stringsPair(word.Substring(0, p1 ) , string.Empty);
                            word=word.Substring(p1+1);
                            lastWordIsVariableValue = true;
                        }
                    }

                }

                if (nextWordIsVariableValue)
                {
                    assignment = new stringsPair(word, string.Empty);
                }
                else if (lastWordIsVariableValue)
                {
                    assignment.right = word;

                    results.removeAssignment(assignment.Item1);
                    results.assignments.Add(assignment);
                    if (foundInQuotes)
                    {
                        if (!results.variablesNeedsQuotes.Contains(assignment.left)) results.variablesNeedsQuotes.Add(assignment.left);
                    }

                    lastWordIsVariableValue = false;
                }
                else // it is attribute
                {
                    results.attributes.Add(word);
                }
                    
                nextIndex = (wordEnd - specialBackOffset) + 1;
            }

            return;

        }

        /// <summary>
        /// Parse an expression and populate a variablesAndAttributes with the result assignments
        /// </summary>
        /// <param name="input">The input expression</param>
        /// <param name="results">The variablesAndAttributes result</param>
        public static void parseForVariablesAndAttributes(string input, variablesAndAttributes results)
        {
            parseForVariablesAndAttributes(input,null,results);
        }


        /// <summary>
        /// Convert a variablesAndAttributes object to IvariablesContainer object
        /// The interface IvariablesContainer does not contain the information regarding the quotes
        /// To include the quotes information, use a stringReplacer type of variables container
        /// </summary>
        /// <param name="varAttr">The input objectr</param>
        /// <param name="variables">The output object</param>
        public static void toVariables(variablesAndAttributes varAttr, IvariablesContainer variables)
        {
            foreach (var item in varAttr.assignments)
            {
                variables.setAny(item.Item1, item.Item2);
            }
        }


        /// <summary>
        /// Convert a variablesAndAttributes object to stringReplacer object
        /// Will  include the assignments and the quotes information
        /// </summary>
        /// <param name="varAttr">The input objectr</param>
        /// <param name="variables">The output object</param>
        public static void toVariables(variablesAndAttributes varAttr, stringReplacer variables)
        {
            toVariables(varAttr, (IvariablesContainer)variables);
            variables.variablesNeedQuotes = varAttr.variablesNeedsQuotes;
        }

    }
}
