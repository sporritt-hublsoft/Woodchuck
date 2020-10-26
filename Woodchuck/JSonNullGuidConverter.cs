using Newtonsoft.Json;
using System;

namespace Woodchuck
{
    /// <summary>
    /// We're using this converter to check for nulls. Logit.io will report null Guids as a string, "<none>"
    /// This can cause some mighty funky behaviour in our application if left unchecked.
    /// </summary>
    class JSonNullGuidConverter : JsonConverter<Guid?>
    {
        public override void WriteJson(JsonWriter writer, Guid? guidValue, JsonSerializer serializer)
        {
            writer.WriteValue(guidValue.HasValue ? guidValue.ToString() : "<none>");
        }

        public override Guid? ReadJson(JsonReader reader, Type objectType, Guid? guidValue, bool hasExistingValue, JsonSerializer serializer)
        {
            try { return Guid.Parse((string)reader.Value); }
            catch { return null; }
        }           
    }
}
