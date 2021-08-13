using HunterCombatMR.Models;
using HunterCombatMR.Models.Animation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.JSONConverters
{
    public sealed class AnimationLayersConverter
        : KeyframeDataCollectionConverter<Layer, LayerData>
    {
        public override bool ReferencesAreContent => false;

        public override bool CanConvert(Type objectType)
            => objectType == typeof(AnimationLayers);

        protected override KeyframeDataCollection<Layer, LayerData> CreateObjectFromData(KeyframeData<LayerData>[] frameData, 
            JArray references,
            JsonSerializer serializer)
            => new AnimationLayers(references.ToObject<Layer[]>(serializer), frameData);

        protected override KeyframeData<LayerData>[] ReadKeyframeData(JArray token,
            JsonSerializer serializer)
        {
            var value = token.ToObject<KeyframeData<LayerData>[]>(serializer);
            return value;
        }
    }
}