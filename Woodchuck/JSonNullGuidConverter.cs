using Newtonsoft.Json;
using System;
using System.Globalization;


namespace Woodchuck
{
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
