﻿using HunterCombatMR.Extensions;
using HunterCombatMR.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HunterCombatMR.JSONConverters
{
    public class KeyFrameProfileConverter
        : JsonConverter
    {
        public override bool CanConvert(Type objectType)
            => objectType == typeof(KeyFrameProfile);

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

            return new KeyFrameProfile(KeyFrameAmount, DefaultKeyFrameSpeed, SpecificKeyFrameSpeeds.ConvertToLengthList() ?? new SortedList<int, FrameLength>());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var profile = (KeyFrameProfile)value;

            writer.WriteStartObject();

            writer.WritePropertyName("KeyFrameAmount");
            serializer.Serialize(writer, (int)profile.KeyFrameAmount);
            writer.WritePropertyName("DefaultKeyFrameSpeed");
            serializer.Serialize(writer, (int)profile.DefaultKeyFrameLength);

            if (profile.KeyFrameLengths.Count > 0)
            {
                writer.WritePropertyName("SpecificKeyFrameSpeeds");
                serializer.Serialize(writer, profile.KeyFrameLengths);
            }

            writer.WriteEndObject();
        }
    }
}