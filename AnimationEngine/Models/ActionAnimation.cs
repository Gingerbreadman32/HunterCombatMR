using HunterCombatMR.AnimationEngine.Interfaces;
using HunterCombatMR.Enumerations;
using Newtonsoft.Json;
using System;

namespace HunterCombatMR.AnimationEngine.Models
{
    public class ActionAnimation
        : Animation,
        IEquatable<ActionAnimation>
    {
        [JsonConstructor]
        public ActionAnimation(string name,
            LayerData layerData)
        {
            Name = name;
            AnimationData = new AnimatedData();
            LayerData = layerData;
        }

        public ActionAnimation(ActionAnimation copy)
        {
            Name = copy.Name;
            AnimationData = new AnimatedData(copy.AnimationData);
            HunterCombatMR.Instance.AnimationKeyFrameManager.SyncFrames(AnimationData);
            LayerData = new LayerData(copy.LayerData);
        }

        public bool Equals(ActionAnimation other)
        {
            if (other?.Name == null || other?.LayerData == null)
                return false;

            return Name.Equals(other.Name) && LayerData.Equals(other.LayerData);
        }
        
    }
}