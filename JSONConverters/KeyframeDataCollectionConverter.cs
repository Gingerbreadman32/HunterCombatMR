using HunterCombatMR.Extensions;
using HunterCombatMR.Interfaces;
using HunterCombatMR.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.JSONConverters
{
    public abstract class KeyframeDataCollectionConverter<TReference, TData>
        : JsonConverter
        where TData : class
        where TReference : IKeyframeDataReference
    {
        public abstract bool ReferencesAreContent { get; }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            CreateKeyframeConverter(serializer);
            JToken token = JToken.Load(reader);
            if (token.Type != JTokenType.Object || !token.Children().Any(x => x.Type == JTokenType.Property))
            {
                throw new JsonReaderException("Keyframe collection should be an object and its children should be properties!");
            }

            var referencesProp = token.Children<JProperty>().SingleOrDefault(x => x.Name.EqualsIgnoreCase("References"));
            var frameDataProp = token.Children<JProperty>().SingleOrDefault(x => x.Name.EqualsIgnoreCase("FrameData"));

            if (referencesProp == null || frameDataProp == null)
                throw new JsonReaderException("References and FrameData properties not found!");

            return CreateObjectFromData(ReadKeyframeData((JArray)frameDataProp.Value, serializer), (JArray)referencesProp.Value, serializer);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            CreateKeyframeConverter(serializer);
            var lVal = (KeyframeDataCollection<TReference, TData>)value;
            var references = CreateReferences(lVal, serializer);
            var frames = new JArray
            (
                lVal.FrameData.Select(x =>
                new JObject {
                    { "Length", x.Frames.Value },
                    {
                        "Data",
                        JObject.FromObject(x.ToDictionary(y => y.Key, y => JToken.FromObject(y.Value, serializer)), serializer)
                    }
                })
            );
            var jObject = new JObject { { "References", references }, { "FrameData", frames } };
            jObject.WriteTo(writer, serializer.Converters.ToArray());
        }

        protected abstract KeyframeDataCollection<TReference, TData> CreateObjectFromData(KeyframeData<TData>[] frameData, 
            JArray references,
            JsonSerializer serializer);

        protected abstract KeyframeData<TData>[] ReadKeyframeData(JArray token,
            JsonSerializer serializer);

        private JArray CreateReferences(KeyframeDataCollection<TReference, TData> value, JsonSerializer serializer)
        {
            if (ReferencesAreContent)
            {
                return new JArray { value.Keys };
            }

            return new JArray { value.Values.Select(x => JObject.FromObject(x, serializer)) };
        }

        private void CreateKeyframeConverter(JsonSerializer serializer)
        {
            if (!serializer.Converters.Any(x => x.GetType() == typeof(KeyframeDataConverter<TData>)))
            {
                serializer.Converters.Add(new KeyframeDataConverter<TData>());
            }
        }
    }
}