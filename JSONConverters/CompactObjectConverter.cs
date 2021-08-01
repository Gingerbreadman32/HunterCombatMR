using HunterCombatMR.Interfaces;
using HunterCombatMR.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace HunterCombatMR.JSONConverters
{
    public abstract class CompactObjectConverter<T, Ta>
        : JsonConverter
        where T : ICompact<Ta>
    {
        public override bool CanWrite
        {
            get { return true; }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type != JTokenType.Array)
            {
                throw new JsonReaderException("Compact object value not an array of values!");
            }
            return LoadingUtils.LoadCompact<T, Ta>(token.ToObject<Ta[]>());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var array = ((ICompact<Ta>)value).Save().Where(x => x != null).ToArray();
            if (!array.Any())
                return;

            writer.WriteRawValue(JsonConvert.SerializeObject(array, Formatting.None));
        }
    }
}