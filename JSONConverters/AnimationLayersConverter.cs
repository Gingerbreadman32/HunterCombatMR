﻿using HunterCombatMR.Models;
using HunterCombatMR.Models.Animation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace HunterCombatMR.JSONConverters
{
    public sealed class AnimationLayersConverter
        : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(AnimationLayers);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type != JTokenType.Object)
            {
                throw new JsonReaderException("Animation Layers entry is not an object!");
            }
            return new KeyframeDataCollection<Layer, LayerData>();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var lVal = (AnimationLayers)value;
            var layers = new JArray { lVal.Values.Select(x => JObject.FromObject(x, serializer)) };
            var tempFormat = JsonSerializer.Create(new JsonSerializerSettings() { Formatting = Formatting.None, Converters = serializer.Converters });
            var frames = new JArray
            (
                lVal.FrameData.Select(x =>
                new JObject {
                    { "Length", x.Value.Frames.Value },
                    {
                        "Data",
                        JObject.FromObject(lVal.GetOrderedActiveLayerData(x.Key).ToDictionary(y => y.Layer.ReferenceName, y => JToken.FromObject(y.FrameData, tempFormat)), tempFormat)
                    }
                })
            );
            var jObject = new JObject { { "References", layers }, { "FrameData", frames } };
            jObject.WriteTo(writer, serializer.Converters.ToArray());
        }
    }
}