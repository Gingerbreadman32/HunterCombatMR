using HunterCombatMR.AttackEngine.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.JSONConverters
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
            var keyevents = new List<Tuple<EventTag, bool, string>>();
            var lifeevents = new Dictionary<string, bool>();

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
                    keyevents = serializer.Deserialize<List<Tuple<EventTag, bool, string>>>(reader);

                if (property.Equals("LifetimeEvents"))
                    lifeevents = serializer.Deserialize<Dictionary<string, bool>>(reader);
            }

            return new PlayerAction(internalname, name, animations, keyevents, lifeevents);
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

            if (eventRefs.Any())
            {
                writer.WritePropertyName("KeyFrameEvents");
                serializer.Serialize(writer, eventRefs);
            }

            var lifeevents = action.LifetimeEvents.Select(x => new KeyValuePair<string, bool>(x.Key.Name, x.Value));

            if (lifeevents.Any())
            {
                writer.WritePropertyName("LifetimeEvents");
                serializer.Serialize(writer, eventRefs);
            }

            writer.WriteEndObject();
        }
    }
}