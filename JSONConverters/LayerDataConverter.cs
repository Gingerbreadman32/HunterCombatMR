using HunterCombatMR.Models.Animation;
using System;

namespace HunterCombatMR.JSONConverters
{
    public sealed class LayerDataConverter
        : CompactObjectConverter<LayerData, string>
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(LayerData);
        }
    }
}