using HunterCombatMR.AnimationEngine.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace HunterCombatMR.Converters
{
    public class KeyFrameProfileConverter
        : JsonConverter
    {
        public override bool CanConvert(Type objectType)
            => objectType == typeof(KeyFrameProfile);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var profile = (KeyFrameProfile)value;

            writer.WriteStartObject();

            writer.WritePropertyName("KeyFrameAmount");
            serializer.Serialize(writer, profile.KeyFrameAmount);
            writer.WritePropertyName("DefaultKeyFrameSpeed");
            serializer.Serialize(writer, profile.DefaultKeyFrameSpeed);
            writer.WritePropertyName("SpecificKeyFrameSpeeds");
            serializer.Serialize(writer, profile.SpecificKeyFrameSpeeds);

            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var KeyFrameAmount = 0;
            var DefaultKeyFrameSpeed = 0;
            var SpecificKeyFrameSpeeds = new Dictionary<int, int>();

            while (reader.Read())
            {
                if (reader.TokenType != JsonToken.PropertyName)
                    break;

                var property = (string)reader.Value;
                if (!reader.Read())
                    continue;

                if (property.Equals("KeyFrameAmount"))
                    KeyFrameAmount = serializer.Deserialize<int>(reader);

                if (property.Equals("DefaultKeyFrameSpeed"))
                    DefaultKeyFrameSpeed = serializer.Deserialize<int>(reader);

                if (property.Equals("SpecificKeyFrameSpeeds"))
                    SpecificKeyFrameSpeeds = serializer.Deserialize<Dictionary<int, int>>(reader);
            }

            if (!(KeyFrameAmount > 0 && DefaultKeyFrameSpeed > 0))
            {
                throw new InvalidDataException("Animation file must contain a keyframe profile with more than 0 total keyframes and keyframe speed.");
            }

            return new KeyFrameProfile(KeyFrameAmount, DefaultKeyFrameSpeed, SpecificKeyFrameSpeeds);
        }
    }
}