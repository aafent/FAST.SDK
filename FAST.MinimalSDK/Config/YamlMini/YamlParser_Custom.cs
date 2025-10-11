using FAST.Config.YamlMini.Grammar;

namespace FAST.Config.YamlMini
{
    public partial class YamlParser
    {
        public static YamlStream Load(string file)
        {
            string text = File.ReadAllText(file);
            TextInput input = new TextInput(text);
            YamlParser parser = new YamlParser();
            bool success;
            YamlStream stream = parser.ParseYamlStream(input, out success);
            if (success)
            {
                return stream;
            }
            else
            {
                string message = parser.GetErrorMessages();
                throw new Exception(message);
            }
        }

        private YamlDocument currentDocument;

        private void SetDataItemProperty(DataItem dataItem, NodeProperty property)
        {
            if (property.Anchor != null)
            {
                currentDocument.AnchoredItems[property.Anchor] = dataItem;
            }
            dataItem.Property = property;
        }

        private DataItem GetAnchoredDataItem(string name)
        {
            if (currentDocument.AnchoredItems.ContainsKey(name))
            {
                return currentDocument.AnchoredItems[name];
            }
            else
            {
                Error(name + " is not anchored.");
                return null;
            }
        }

        private int currentIndent = -1;
        private bool detectIndent = false;

        private Stack<int> Indents = new Stack<int>();

        private bool ParseIndent()
        {
            bool success;
            for (int i = 0; i < currentIndent; i++)
            {
                MatchTerminal(' ', out success);
                if (!success)
                {
                    position -= i;
                    return false;
                }
            }
            if (detectIndent)
            {
                int additionalIndent = 0;
                while (true)
                {
                    MatchTerminal(' ', out success);
                    if (success)
                    {
                        additionalIndent++;
                    }
                    else { break; }
                }
                currentIndent += additionalIndent;
                detectIndent = false;
            }
            return true;
        }

        /// <summary>
        /// Mandatory Indentation for "non-indented" Scalar
        /// </summary>
        private void IncreaseIndentIfZero()
        {
            Indents.Push(currentIndent);
            if (currentIndent == 0)
            {
                currentIndent++;
            }
            detectIndent = true;
        }

        /// <summary>
        /// Increase Indent for Nested Block Collection
        /// </summary>
        private void IncreaseIndent()
        {
            Indents.Push(currentIndent);
            currentIndent++;
            detectIndent = true;
        }

        /// <summary>
        /// Decrease Indent for Nested Block Collection
        /// </summary>
        private void DecreaseIndent()
        {
            currentIndent = Indents.Pop();
        }

        private void RememberIndent()
        {
            Indents.Push(currentIndent);
        }

        private void RestoreIndent()
        {
            currentIndent = Indents.Pop();
        }

        private ChompingMethod CurrentChompingMethod;

        private void AddIndent(BlockScalarModifier modifier, bool success)
        {
            if (success)
            {
                Indents.Push(currentIndent);
                currentIndent += modifier.GetIndent();
                detectIndent = true;
            }
            else
            {
                IncreaseIndentIfZero();
            }

            CurrentChompingMethod = modifier.GetChompingMethod();
        }

        private string Chomp(string linebreaks)
        {
            switch (CurrentChompingMethod)
            {
                case ChompingMethod.Strip:
                    return String.Empty;
                case ChompingMethod.Keep:
                    return linebreaks;
                case ChompingMethod.Clip:
                default:
                    if (linebreaks.StartsWith("\r\n"))
                    {
                        return "\r\n";
                    }
                    else if (linebreaks.Length == 0)
                    {
                        return Environment.NewLine;
                    }
                    else
                    {
                        return linebreaks.Substring(0, 1);
                    }
            }
        }
    }
}
