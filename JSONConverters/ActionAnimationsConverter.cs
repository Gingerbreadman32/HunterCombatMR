﻿using HunterCombatMR.Interfaces.Animation;
using HunterCombatMR.Models;
using HunterCombatMR.Models.Action;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.JSONConverters
{
    public sealed class ActionAnimationsConverter
        : KeyframeDataCollectionConverter<ICustomAnimationV2, AnimationData>
    {
        public override bool ReferencesAreContent => true;

        public override bool CanConvert(Type objectType)
            => objectType == typeof(ActionAnimations);

        protected override KeyframeDataCollection<ICustomAnimationV2, AnimationData> CreateObjectFromData(KeyframeData<AnimationData>[] frameData, 
            JArray references,
            JsonSerializer serializer)
            => new ActionAnimations(references.ToObject<string[]>(serializer), frameData);

        protected override KeyframeData<AnimationData>[] ReadKeyframeData(JArray token,
            JsonSerializer serializer)
        {
            var value = token.ToObject<KeyframeData<AnimationData>[]>(serializer);

            return value;
        }
    }
}