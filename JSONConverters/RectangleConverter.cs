using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;

namespace HunterCombatMR.JSONConverters
{
    public class RectangleConverter
        : JsonConverter
    {
        public override bool CanConvert(Type objectType)
            => typeof(Rectangle) == objectType;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            int X = 0;
            int Y = 0;
            int Width = 0;
            int Height = 0;

            while (reader.Read())
            {
                if (reader.TokenType != JsonToken.PropertyName)
                    break;

                var property = (string)reader.Value;
                if (!reader.Read())
                    continue;

                if (property.Equals("X"))
                    X = serializer.Deserialize<int>(reader);

                if (property.Equals("Y"))
                    Y = serializer.Deserialize<int>(reader);

                if (property.Equals("Width"))
                    Width = serializer.Deserialize<int>(reader);

                if (property.Equals("Height"))
                    Height = serializer.Deserialize<int>(reader);
            }

            return new Rectangle(X, Y, Width, Height);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var rect = (Rectangle)value;

            writer.WriteStartObject();

            writer.WritePropertyName("X");
            serializer.Serialize(writer, rect.X);
            writer.WritePropertyName("Y");
            serializer.Serialize(writer, rect.Y);
            writer.WritePropertyName("Width");
            serializer.Serialize(writer, rect.Width);
            writer.WritePropertyName("Height");
            serializer.Serialize(writer, rect.Height);

            writer.WriteEndObject();
        }
    }
}