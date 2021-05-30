using HunterCombatMR.Models.Animation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return new AnimationLayers();
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
                    { "FrameLength", x.Value.Frames.Value },
                    { 
                        "Layers", 
                        JObject.FromObject(lVal.GetOrderedActiveLayerData(x.Key).ToDictionary(y => y.Layer.Name, y => JArray.FromObject(y.FrameData, tempFormat)), serializer) 
                    }
                })
            );
            var jObject = new JObject { { "LayerReferences", layers }, { "FrameData", frames } };
            jObject.WriteTo(writer, serializer.Converters.ToArray());
        }
    }
}
