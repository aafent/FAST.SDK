using System.Data;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace FAST.Tools
{
    /// <summary>
    /// Markdown helping methods
    /// </summary>
    public static class markdownHelper
    {
        /// <summary>
        /// Convert datatable into markdown table
        /// </summary>
        /// <param name="builder">The string builder instance</param>
        /// <param name="source">The datatable</param>
        public static void asMarkdownTable(StringBuilder builder, DataTable source )
        {
            foreach (DataColumn column in source.Columns)
                builder.Append("|").Append(column.ColumnName.Trim());

            builder.AppendLine("|");

            for (int i = 0; i < source.Columns.Count; i++)
                builder.Append("|----");

            builder.AppendLine("|");

            foreach (DataRow row in source.Rows)
            {
                foreach (object val in row.ItemArray)
                    builder.Append("|").Append(Convert.ToString(val).Trim());

                builder.AppendLine("|");
            }

            return;
        }

        private static string[] createRow(string input)
        {
            int start = input[0] is '|' ? 1 : 0;
            int end = input[^1] is '|' ? 1 : 0;
            return input[start..^end].Split('|');
        }
        private static void insertRow(string[,] table, string[] row, int rowindex)
        {
            int numColumns = table.GetLength(1);
            for (int i = 0; i < numColumns; i++)
            {
                table[rowindex, i] = row[i];
            }
        }
        private static (string[,] table, string[] header) createTable(IList<string> input)
        {
            string[] headerRow = createRow(input[0]);
            int numColumns = headerRow.Length;
            int numRows = input.Count - 1; // without the horizontal line row.
            string[,] table = new string[numRows, numColumns];

            return (table, headerRow);
        }

        /// <summary>
        /// Convert a markdown table to two dimensions array
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string[,] toTwoDimArray(List<string> input)
        {
            //
            // kudos: https://codereview.stackexchange.com/questions/257315/markdown-table-strings-to-two-dimensional-array-converter-implementation-in-c
            //
            var (table, header) = createTable(input);
            insertRow(table, header, 0);

            for (int rowIndex = 2; rowIndex < input.Count; rowIndex++)
            {
                string[] dataRow = createRow(input[rowIndex]);
                insertRow(table, dataRow, rowIndex - 1);
            }

            return table;
        }

        /// <summary>
        /// Convert a collection to markdown table
        /// </summary>
        /// <typeparam name="T">The type of the collection items</typeparam>
        /// <param name="source">The collection</param>
        /// <returns></returns>
        public static string toMarkdownTable<T>(IEnumerable<T> source)
        {
            var properties = typeof(T).GetRuntimeProperties();
            var fields = typeof(T)
                .GetRuntimeFields()
                .Where(f => f.IsPublic);

            var gettables = Enumerable.Union(
                properties.Select(p => new { p.Name, GetValue = (Func<object, object>)p.GetValue, Type = p.PropertyType }),
                fields.Select(p => new { p.Name, GetValue = (Func<object, object>)p.GetValue, Type = p.FieldType }));

            var maxColumnValues = source
                .Select(x => gettables.Select(p => p.GetValue(x)?.ToString()?.Length ?? 0))
                .Union(new[] { gettables.Select(p => p.Name.Length) }) // Include header in column sizes
                .Aggregate(
                    new int[gettables.Count()].AsEnumerable(),
                    (accumulate, x) => accumulate.Zip(x, Math.Max))
                .ToArray();

            var columnNames = gettables.Select(p => p.Name);

            var headerLine = "| " + string.Join(" | ", columnNames.Select((n, i) => n.PadRight(maxColumnValues[i]))) + " |";

            var isNumeric = new Func<Type, bool>(type =>
                type == typeof(Byte) ||
                type == typeof(SByte) ||
                type == typeof(UInt16) ||
                type == typeof(UInt32) ||
                type == typeof(UInt64) ||
                type == typeof(Int16) ||
                type == typeof(Int32) ||
                type == typeof(Int64) ||
                type == typeof(Decimal) ||
                type == typeof(Double) ||
                type == typeof(Single));

            var rightAlign = new Func<Type, char>(type => isNumeric(type) ? ':' : ' ');

            var headerDataDividerLine =
                "| " +
                    string.Join(
                        "| ",
                        gettables.Select((g, i) => new string('-', maxColumnValues[i]) + rightAlign(g.Type))) +
                "|";

            var lines = new[]
                {
                headerLine,
                headerDataDividerLine,
            }.Union(
                    source
                    .Select(s =>
                        "| " + string.Join(" | ", gettables.Select((n, i) => (n.GetValue(s)?.ToString() ?? "").PadRight(maxColumnValues[i]))) + " |"));

            return lines
                .Aggregate((p, c) => p + Environment.NewLine + c);
        }

        /// <summary>
        /// Extract tables from an markdown input
        /// </summary>
        /// <param name="input">The markdown</param>
        /// <returns>A list of strings. Each item is a markdown table</returns>
        public static List<string> extractTables(string input)
        {
            //
            //Kudos: https://regex101.com/r/8pNnaG/1
            //
            string expr =@"((\r?\n){2}|^)([^\r\n]*\|[^\r\n]*(\r?\n)?)+(?=(\r?\n){2}|$)";  //gm

            MatchCollection matches = Regex.Matches(input, expr,RegexOptions.IgnoreCase); //Matches stands for option g:global

            var result = matches.Cast<Match>()
                             .Select(m => m.Value)
                             .ToList();
            return result;
        }


        /// <summary>
        /// Strip markdown tags from a content 
        /// based on: https://github.com/stiang/remove-markdown/blob/master/index.js
        /// </summary>
        /// <param name="content">The markdown</param>
        /// <returns>String, the content without the markdown tags</returns>
        public static string stripMarkdownTags(string content)
        {
            // Headers
            content = Regex.Replace(content, "/\n={2,}/g", "\n");
            // Strikethrough
            content = Regex.Replace(content, "/~~/g", "");
            // Codeblocks
            content = Regex.Replace(content, "/`{3}.*\n/g", "");
            // HTML Tags
            content = Regex.Replace(content, "/<[^>]*>/g", "");
            // Remove setext-style headers
            content = Regex.Replace(content, "/^[=\\-]{2,}\\s*$/g", "");
            // Footnotes
            content = Regex.Replace(content, "/\\[\\^.+?\\](\\: .*?$)?/g", "");
            content = Regex.Replace(content, "/\\s{0,2}\\[.*?\\]: .*?$/g", "");
            // Images
            content = Regex.Replace(content, "/\\!\\[.*?\\][\\[\\(].*?[\\]\\)]/g", "");
            // Links
            content = Regex.Replace(content, "/\\[(.*?)\\][\\[\\(].*?[\\]\\)]/g", "$1");
            return content;
        }

    }
}
