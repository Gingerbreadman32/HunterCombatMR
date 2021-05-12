using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Converters
{
    public class PlayerActionConverter
        : JsonConverter
    {
        public override bool CanConvert(Type objectType)
            => objectType == typeof(PlayerAction);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var internalname = "";
            var name = "";
            var animations = new Dictionary<int, string>();
            var events = new List<Tuple<EventTag, bool, string>>();

            while (reader.Read())
            {
                if (reader.TokenType != JsonToken.PropertyName)
                    break;

                var property = (string)reader.Value;
                if (!reader.Read())
                    continue;

                if (property.Equals("InternalName"))
                    internalname = serializer.Deserialize<string>(reader);

                if (property.Equals("Name"))
                    name = serializer.Deserialize<string>(reader);

                if (property.Equals("Animations"))
                    animations = serializer.Deserialize<Dictionary<int, string>>(reader);

                if (property.Equals("KeyFrameEvents"))
                    events = serializer.Deserialize<List<Tuple<EventTag, bool, string>>>(reader);
            }

            return new PlayerAction(internalname, name, animations, events);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var action = (PlayerAction)value;
            

            writer.WriteStartObject();

            writer.WritePropertyName("InternalName");
            serializer.Serialize(writer, action.InternalName);
            writer.WritePropertyName("Name");
            serializer.Serialize(writer, action.Name);

            action.Animations.SaveAnimations();
            writer.WritePropertyName("Animations");
            serializer.Serialize(writer, action.Animations.AnimationReferences);

            var eventRefs = action.KeyFrameEvents.Select(x => Tuple.Create(x.Tag, x.IsEnabled, x.Event.Name));

            writer.WritePropertyName("KeyFrameEvents");
            serializer.Serialize(writer, eventRefs);

            writer.WriteEndObject();
        }
    }
}