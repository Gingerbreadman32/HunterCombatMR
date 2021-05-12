using HunterCombatMR.AttackEngine.Models;
using Newtonsoft.Json;
using System;

namespace HunterCombatMR.JSONConverters
{
    public class EventTagConverter
        : JsonConverter
    {
        public override bool CanConvert(Type objectType)
            => typeof(EventTag) == objectType;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            int start = 0;
            int end = 0;
            int tag = 0;

            while (reader.Read())
            {
                if (reader.TokenType != JsonToken.PropertyName)
                    break;

                var property = (string)reader.Value;
                if (!reader.Read())
                    continue;

                if (property.Equals("StartKeyFrame"))
                    start = serializer.Deserialize<int>(reader);

                if (property.Equals("EndKeyFrame"))
                    end = serializer.Deserialize<int>(reader);

                if (property.Equals("Id"))
                    tag = serializer.Deserialize<int>(reader);
            }

            return new EventTag(tag, start, end);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var tag = (EventTag)value;

            writer.WriteStartObject();

            writer.WritePropertyName("StartKeyFrame");
            serializer.Serialize(writer, (int)tag.StartKeyFrame);
            writer.WritePropertyName("EndKeyFrame");
            serializer.Serialize(writer, (int)tag.EndKeyFrame);
            writer.WritePropertyName("Id");
            serializer.Serialize(writer, tag.Id);

            writer.WriteEndObject();
        }
    }
}