using HunterCombatMR.Extensions;
using HunterCombatMR.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.JSONConverters
{
    public class KeyframeDataConverter<TData>
        : JsonConverter
    {
        public override bool CanConvert(Type objectType)
            => objectType == (typeof(KeyframeData<TData>));

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type != JTokenType.Object || !token.Children().Any(x => x.Type == JTokenType.Property))
            {
                throw new JsonReaderException("Keyframe data should be an object and its children should be properties!");
            }

            var referencesProp = token.Children<JProperty>().SingleOrDefault(x => x.Name.EqualsIgnoreCase("Length"));
            var frameDataProp = token.Children<JProperty>().SingleOrDefault(x => x.Name.EqualsIgnoreCase("Data"));

            if (referencesProp == null || frameDataProp == null)
                throw new JsonReaderException("References and FrameData properties not found!");

            return new KeyframeData<TData>(new FrameLength(referencesProp.ToObject<int>()), frameDataProp.Value.ToObject<Dictionary<string, TData>>(serializer));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var data = (KeyframeData<TData>)value;
            var dictionary = JObject.FromObject(data.ToDictionary(x => x.Key, x => x.Value));
            var length = new JValue(data.Frames.Value);

            var json = new JObject { { "Length", length }, { "Data", dictionary } };
            json.WriteTo(writer, serializer.Converters.ToArray());
        }
    }
}