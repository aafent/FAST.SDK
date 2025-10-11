using System.Text.Json;

namespace FAST.Core
{
    /// <summary>
    /// Helper class for JSON operations.
    /// </summary>
    public static class jsonBaseHelper
    {
        /// <summary>
        /// Formats a JSON string to be more human-readable with indentation.
        /// </summary>
        /// <param name="unPrettyJson">the input json string</param>
        /// <returns>the pretty output json string</returns>
        public static string prettyJson(string unPrettyJson)
        {
            var options = new JsonSerializerOptions()
            {
                WriteIndented = true
            };
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(unPrettyJson);
            return JsonSerializer.Serialize(jsonElement, options);
        }

        /// <summary>
        /// Converts a JsonElement to array of Dictionaries with field name as key and an object as the value.
        /// </summary>
        /// <param name="jsonElement">The input JsonElement</param>
        /// <returns>The array of Dictionaries</string></returns>
        public static Dictionary<string, object>[] jsonElementToArrayOfDictionary(JsonElement jsonElement)
        {
            var result = new List<Dictionary<string, object>>();
            foreach (var item in jsonElement.EnumerateArray())
            {
                var dict = new Dictionary<string, object>();

                foreach (var property in item.EnumerateObject())
                {
                    dict[property.Name] = property.Value.GetString();
                }

                result.Add(dict);
            }
            return result.ToArray();
        }

    }

    public static class marika
    {

    }


}
