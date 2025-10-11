using FAST.Config.YamlMini;
using FAST.Config.YamlMini.Grammar;
using FAST.Core;
using FAST.Core.Models;
using FAST.Logging;
using FAST.Types;
using System.Linq.Expressions;
using System.Text;

namespace FAST.Config
{
    public class yamlFlattener : IvariablesContainer, ImultiErrorCarrier
    {
        private TextInput input=null;
        private YamlStream yamlStream=null;

        private Dictionary< string, List<Tuple<string,string>> > topLevelSectionsVariables = new(); // Section.Variable.Value
        private List<string> collectionVariables = new();

        public yamlFlattener()
        {
        }
        public yamlFlattener(string filename):this()
        {
            setInputFromYAMLFile(filename);
        }

        public bool dumpInternal { get; set; } = false;

        public void setInputFromYAMLFile(string filename)
        {
            string contents = File.ReadAllText(filename);
            this.input = new TextInput(contents);
        }
        public void setInput(string yamlContent)
        {
            this.input = new TextInput(yamlContent);
        }
        public void setInput(StringBuilder builder)
        {
            this.setInput(builder.ToString());
        }


        public bool tryParse()
        {
            if (input == null ) throw new ArgumentNullException("There is no input. Bad time for this method call. Specify the input first.");

            clearAllVariables();

            bool success;
            var parser = new YamlParser();
            yamlStream = parser.ParseYamlStream(input, out success);
            if ( !success ) 
            {
                this.errors = parser.errors;
                this.errorText = parser.errorText;
                this.extendedErrorText = parser.extendedErrorText;
                return false;
            }
            processYAMLstructure();
            return true;
        }

        /*
                    treeViewData.Nodes.Clear();
                    foreach (YamlDocument doc in yamlStream.Documents)
                    {
                        treeViewData.Nodes.Add(YamlEmittor.CreateNode(doc.Root));
                    }
                    treeViewData.ExpandAll();
                    tabControl1.SelectedTab = tabPageDataTree;
                    }
                    else
                    {
                       MessageBox.Show(parser.GetErrorMessages());
         */

        private void clearAllVariables()
        {
            topLevelSectionsVariables.Clear();
        }


        public List<string> errors { get; set; } = null;
        public string errorText { get; set; } = null;
        public string extendedErrorText { get; set; } = null;
        public bool hasError
        {
            get
            {
                return !string.IsNullOrEmpty(errorText);
            }
            set
            {
                // do nothing
            }
        }





        public void setAny(string name, object value)
        {
            throw new Exception("Setting/Changing values to YAMLMini it is not supported");
        }

        public object getAsObject(Type type, string section, string variable, bool nullable)
        {
            if (string.IsNullOrEmpty(section)) section = "-";
            if (topLevelSectionsVariables.ContainsKey(section))
            {
                var value = topLevelSectionsVariables[section].First(i => i.Item1 == variable);
                return Convert.ChangeType(value.Item2, type);
            }
            return null;
        }
        public object getAsObject(Type type, string name, bool nullable)
        {
            string section, variable;
            if (!trySplitNameToParts(name, out section, out variable)) return null;
            return getAsObject(type,section,variable, nullable);
        }
        public bool isVariable(string name)
        {
            string section, variable;
            if ( ! trySplitNameToParts(name, out section, out variable)) return false;
            return isVariable(section,variable);
        }
        public bool isVariable(string section,string variable)
        {
            if ( string.IsNullOrEmpty(section) ) section="-";
            if (topLevelSectionsVariables.ContainsKey(section))
            {
                if (topLevelSectionsVariables[section].Any(i=>i.Item1==variable)) return true;
            }
            return false;
        }

        public IEnumerable<string> sections()
        {
            List<string> sections = new List<string>();

            foreach ( var item in topLevelSectionsVariables.Keys )
            {
                sections.Add(item);
            }

            return sections;
        }
        public IEnumerable<string> variables(string section)
        {
            List<string> variables = new();
            foreach( var item in topLevelSectionsVariables[section] )
            {
                variables.Add(item.Item1);
            }
            return variables;
        }
        public IEnumerable<string> names()
        {
            List<string> names = new();
            foreach (var section in topLevelSectionsVariables.Keys)
            {
                foreach (var vName in topLevelSectionsVariables[section])
                {
                    if ( section == "-") 
                        names.Add(vName.Item1);
                    else
                        names.Add(section + "." + vName.Item1);
                }
            }
            return names;
        }

        public bool trySplitNameToParts(string name, out string section, out string variable)
        {
            section=null;
            variable=null;
            if ( string.IsNullOrEmpty(name) ) return false;

            var parts= name.Split(".");

            if ( parts.Length == 1)
            {
                section="";
                variable=name;
                return true;
            }
            if (parts.Length == 2)
            {
                section = parts[0];
                variable = parts[1];
                return true;
            }

            section=string.Join(".",parts,0, parts.Length - 1);
            variable = parts[parts.Length];
            return true;
        }

        public T getAs<T>(string section, string variable)
        {
            var value=getAsObject(typeof(T), section, variable, false);
            return (T)Convert.ChangeType(value,typeof(T));
        }
        public T getAs<T>(string name)
        {
            string section, variable;
            if (!trySplitNameToParts(name, out section, out variable)) return default(T);
            return getAs<T>(section, variable);
        }


        /// <summary>
        /// Get entry as string. 
        /// Entry must exists 
        /// </summary>
        /// <param name="section">section name</param>
        /// <param name="variable">variable name</param>
        /// <returns>string value</returns>
        public string getAsString(string section, string variable)
        {
            return (string)getAsObject(typeof(string),section,variable,false);
        }

        /// <summary>
        /// Get entry as string. 
        /// Entry must exists 
        /// </summary>
        /// <param name="name">name</param>
        /// <returns>string value</returns>
        public string getAsString(string name)
        {
            string section, variable;
            if (!trySplitNameToParts(name, out section, out variable)) return null;
            return getAsString(section,variable);
        }

        /// <summary>
        /// Get entry as string. 
        /// If entry does not exists, returns empty string 
        /// </summary>
        /// <param name="section">section name</param>
        /// <param name="variable">variable name</param>
        /// <returns>value or empry string</returns>
        public string asString(string section, string variable)
        {
            if ( isVariable(section,variable) ) return (string)getAsObject(typeof(string), section, variable, false);
            return string.Empty;
        }

        /// <summary>
        /// Get entry as string. 
        /// If entry does not exists, returns empty string 
        /// </summary>
        /// <param name="name">name</param>
        /// <returns>value or empty string</returns>
        public string asString(string name)
        {
            string section, variable;
            if (!trySplitNameToParts(name, out section, out variable)) return null;
            return asString(section, variable);
        }

        public IEnumerable<IEnumerable<FastKeyValuePair<string,string>>> getAsKeyValuePairCollection(string section, string variable)
        {
            if (string.IsNullOrEmpty(section)) section = "-";
            string nameSingle = $"{section}.{variable}";
            string namedItem = $"{section}.{variable}_1";

            List<IEnumerable<FastKeyValuePair<string, string>>> results=new();
            List<FastKeyValuePair<string, string>> valuePairs;

            if (topLevelSectionsVariables.ContainsKey(variable))
            {
                var query = from t in topLevelSectionsVariables[variable]
                            select t;
                results.Add( query.ToList().Select(t => new FastKeyValuePair<string, string>(t.Item1, t.Item2)
                                                  )
                                            .ToList()
                           );
                return results;
            }
            else
            if (topLevelSectionsVariables.ContainsKey(namedItem))
            {
                
                int cnt=1;
                while (true)
                {
                    var allItems = topLevelSectionsVariables[namedItem];
                    valuePairs = new();
                    foreach (var item in allItems)
                    {
                        valuePairs.Add(new FastKeyValuePair<string, string>(item.Item1, item.Item2));
                    }
                    results.Add(valuePairs);

                    // (v) go to next (2nd...3rd...)
                    cnt++;
                    namedItem = $"{section}.{variable}_{cnt}";
                    if (!topLevelSectionsVariables.ContainsKey(namedItem)) break;
                }
                return results;
            }
            else
            if (topLevelSectionsVariables.ContainsKey(nameSingle))
            {
                var allItems = topLevelSectionsVariables[nameSingle];
                valuePairs = new();
                foreach (var item in allItems) 
                {
                    valuePairs.Add( new FastKeyValuePair<string, string>(item.Item1, item.Item2));
                }
                results.Add(valuePairs);
                return results;
            }
            else
            {
                throw new Exception("use getAsCollection() to retrieve this collection or other get*() methods");
            }
        }
        public IEnumerable<IEnumerable<FastKeyValuePair<string, string>>> getAsKeyValuePairCollection(string name)
        {
            string section, variable;
            if (!trySplitNameToParts(name, out section, out variable)) return null;
            return getAsKeyValuePairCollection(section, variable);
        }

        public IEnumerable<T> getAsCollection<T>(string section, string variable)
        {
            List<T> values = new();
            if ( string.IsNullOrEmpty(section) ) section="-";
            string name=$"{section}.{variable}";

            if (topLevelSectionsVariables.ContainsKey(name) )
            {
                throw new Exception("use getAsKeyValuePair() to retrieve this collection");
            } 
            else // (v) style of item_1, item_2 .... item_N
            {
                int inx=0;
                do
                {
                    inx++;
                    string itemName = variable + "_" + inx.ToString();
                    if ( topLevelSectionsVariables[section].Any(i => i.Item1 == itemName) )
                    {
                        var value=topLevelSectionsVariables[section].First(i => i.Item1 == itemName).Item2;
                        values.Add( (T)Convert.ChangeType(value, typeof(T) ) );
                    } 
                    else
                    {
                        break;
                    }
                } while(true);
            }


            return values;
        }
        public IEnumerable<T> getAsCollection<T>(string name)
        {
            string section, variable;
            if (!trySplitNameToParts(name, out section, out variable)) return null;
            return getAsCollection<T>(section, variable);
        }
        public IEnumerable<Tuple<T1,T2>> getAsTupleCollection<T1,T2>(string section, string variable)
        {
            List<Tuple<T1,T2>> values = new();
            if (string.IsNullOrEmpty(section)) section = "-";

            int inx = 0;
            do
            {
                inx++;
                string itemName = variable + "_" + inx.ToString();
                if (topLevelSectionsVariables[section].Any(i => i.Item1 == itemName))
                {
                    string stringValue = topLevelSectionsVariables[section].First(i => i.Item1 == itemName).Item2;
                    if ( string.IsNullOrEmpty(stringValue) )
                    {
                        values.Add( new Tuple<T1,T2>(default(T1),default(T2)) );
                    }
                    else
                    {
                        int p1=stringValue.IndexOf(":");
                        if ( p1 < 0 )
                        {
                            values.Add( new Tuple<T1,T2>( (T1)Convert.ChangeType(stringValue, typeof(T1) ),default(T2) ) );
                        } else
                        {
                            var part1 = stringValue.Substring(0, p1);
                            var part2 = stringValue.Substring(p1+1);

                            values.Add(new Tuple<T1, T2>(typeConverter.convertObject<T1>(part1), typeConverter.convertObject<T2>(part2)));
                        }
                    }
                }
                else
                {
                    break;
                }
            } while (true);


            return values;
        }
        public IEnumerable<Tuple<T1, T2>> getAsTupleCollection<T1, T2>(string name)
        {
            string section, variable;
            if (!trySplitNameToParts(name, out section, out variable)) return null;
            return getAsTupleCollection<T1,T2>(section,variable);
        }

        //public void copyTo(IvariablesContainer variables)
        //{
        //    foreach (var item in variables.assignments)
        //    {
        //        variables.setAny(item.Item1, item.Item2);
        //    }
        //}

        /// <summary>
        /// dump internal flatten values to console or to the fastLogger
        /// </summary>
        /// <param name="dumpToLogger"></param>
        public void dumpNames(bool dumpToLogger=false)
        {
            if ( !dumpToLogger )
            { 
                Console.WriteLine("NAMES DUMP:");
                foreach (var name in this.names()) Console.WriteLine(name);
            } else
            {
                fastLogger.debug("NAMES DUMP:");
                foreach (var name in this.names()) fastLogger.debug(name);
            }
        }

        private void processYAMLstructure()
        {
            clearAllVariables();
            foreach ( var doc in yamlStream.Documents)
            {
                if (doc.Root is Mapping) //YamlMini.Grammar.Mapping
                {
                    process_Mapping((Mapping)doc.Root,"-");

                } 
            }
        }

        private void process_singleVariable(string section, string vname, string value )
        {
            if (!topLevelSectionsVariables.ContainsKey(section)) topLevelSectionsVariables.Add(section, new List<Tuple<string, string>>());

            if (!topLevelSectionsVariables[section].Any(i => i.Item1 == vname))
            {
                topLevelSectionsVariables[section].Add(new Tuple<string, string>(vname, value));
            }
            else
            {
                var tupleToRemove = topLevelSectionsVariables[section].First(i => i.Item1 == vname);
                topLevelSectionsVariables[section].Remove(tupleToRemove);
                topLevelSectionsVariables[section].Add(new Tuple<string, string>(vname, value));
            }
        }

        private void process_Mapping( Mapping map, string section, int depth=0 )
        {
            string vname=null;
            string value=null;
            foreach (var ent in map.Enties)
            {
                if ( dumpInternal ) Console.WriteLine($"{section} :::> {ent}");

                if (ent.Key is Scalar & ent.Value is Mapping) process_Mapping((Mapping)ent.Value, ((Scalar)ent.Key).Text, depth + 1);
                if (ent.Key is Scalar & ent.Value is Scalar)
                {
                    vname= ((Scalar)ent.Key).Text;
                    value= ((Scalar)ent.Value).Text;
                    process_singleVariable(section, vname, value);
                    continue;
                }
                if (ent.Key is Scalar & ent.Value is Sequence)
                {
                    vname = ((Scalar)ent.Key).Text;
                    if (! collectionVariables.Contains(vname) ) collectionVariables.Add(vname);

                    int itemNumber=0;
                    foreach( var item in ((Sequence)ent.Value).Enties )
                    {
                        if (item is Scalar )
                        { 
                            value=((Scalar)item).Text;
                        } 
                        else
                        if ( item is Mapping)
                        {
                            if ( ((Mapping)item).Enties != null )
                            { 
                                if (((Mapping)item).Enties.Count==1 )
                                {
                                    var scalarKey=((Scalar)((Mapping)item).Enties[0].Key).Text;
                                    var scalarValue = ((Scalar)((Mapping)item).Enties[0].Value).Text;
                                    value = scalarKey + ":" + scalarValue;
                                }
                                else
                                {
                                    itemNumber++;
                                    process_Mapping((Mapping)item, $"{section}.{vname}_{itemNumber}", ++depth);
                                    continue;
                                }
                            }
                        } 
                        else
                        { 
                            throw new Exception($"Item of type {item.GetType()} is not yet supported.");
                            //value = ((Scalar)ent.Value).Text;
                        }
                        itemNumber++;
                        process_singleVariable(section, $"{vname}_{itemNumber}", value);
                    }
                    continue;
                }

            }
        }

        private static TResult ConvertFromObject<TResult>(object a)
        {
            if (a == null) return default(TResult);
            var p = Expression.Parameter(typeof(object));
            var c1 = Expression.Convert(p, a.GetType());
            var c2 = Expression.Convert(c1, typeof(TResult));
            var e = (Func<object, TResult>)Expression.Lambda(c2, p).Compile();
            return e(a);
        }

        public static yamlFlattener getFrom(IvariablesContainer configuration)
        {
            if (configuration is yamlFlattener) return (yamlFlattener)configuration;
            throw new Exception("configuration it is not compatible with yamlFlattener::class");
        }

    }
}
