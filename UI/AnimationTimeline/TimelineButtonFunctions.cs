using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.Constants;
using HunterCombatMR.Enumerations;
using System;
using System.Linq;
using Terraria.UI;

namespace HunterCombatMR.UI.AnimationTimeline
{
    public partial class Timeline
    {
        #region Public Methods

        public void AddButton(string name,
            TimelineButtonIcon iconType,
            Action buttonEvent,
            Func<Animation, bool> conditionEvent = null)
        {
            var currentButtons = Elements.Where(x => x.GetType().IsAssignableFrom(typeof(TimelineButton))).Count();
            var newButton = new TimelineButton(name, iconType, Scale, activeCondition: conditionEvent)
            {
                Top = new StyleDimension(20f * Scale, 0f),
                Left = new StyleDimension((28f * Scale) + (30f * currentButtons * Scale), 0f)
            };
            newButton.ClickActionEvent += buttonEvent;

            Append(newButton);
        }

        #endregion Public Methods

        #region Private Methods

        private void InitializeButtons()
        {
            AddButton(TimelineButtonNames.AddKeyButton, TimelineButtonIcon.Plus, AddButtonLogic, AddButtonCondition);
            AddButton(TimelineButtonNames.CopyKeyButton, TimelineButtonIcon.Duplicate, CopyButtonLogic, AddButtonCondition);
            AddButton(TimelineButtonNames.RemoveKeyButton, TimelineButtonIcon.Minus, DeleteButtonLogic, DeleteButtonCondition);
            AddButton(TimelineButtonNames.MoveLeftKeyButton, TimelineButtonIcon.LeftArrow, MoveButtonLogic(false), MoveLeftButtonCondition);
            AddButton(TimelineButtonNames.MoveRightKeyButton, TimelineButtonIcon.RightArrow, MoveButtonLogic(true), MoveRightButtonCondition);
            Buttons = Elements.Where(x => x.GetType().IsAssignableFrom(typeof(TimelineButton))).Select(x => (x as TimelineButton));
        }

        #endregion Private Methods

        #region Button Events

        private void AddButtonLogic()
        {
            Animation.AddKeyFrame();
            HunterCombatMR.Instance.EditorInstance.AnimationEdited = true;
        }

        private void CopyButtonLogic()
        {
            var copyKeyframe = Animation.AnimationData.GetCurrentKeyFrame();
            Animation.AddKeyFrame(copyKeyframe, Animation.LayerData.GetFrameInfoForLayers(copyKeyframe.KeyFrameOrder));
            HunterCombatMR.Instance.EditorInstance.AnimationEdited = true;
        }

        private void DeleteButtonLogic()
        {
            Animation.RemoveKeyFrame(Animation.AnimationData.GetCurrentKeyFrameIndex());
            HunterCombatMR.Instance.EditorInstance.AnimationEdited = true;
        }

        private Action MoveButtonLogic(bool forward)
        {
            return () =>
            {
                int currentKeyFrame = Animation.AnimationData.GetCurrentKeyFrameIndex();
                Animation.MoveKeyFrame(currentKeyFrame, (forward) ? currentKeyFrame + 1 : currentKeyFrame - 1);
                HunterCombatMR.Instance.EditorInstance.AnimationEdited = true;
            };
        }

        #endregion Button Events

        #region Button Conditions

        private bool AddButtonCondition(Animation animation)
            => (TimelineButton.DefaultCondition(animation)) && !(!ShowAllFrames && animation.AnimationData.GetTotalKeyFrames() == _maxFrames)
                || (ShowAllFrames && animation.AnimationData.TotalFrames == _maxFrames);

        private bool DeleteButtonCondition(Animation animation)
            => (TimelineButton.DefaultCondition(animation)) && (animation.AnimationData.GetTotalKeyFrames() > 1);

        private bool MoveLeftButtonCondition(Animation animation)
            => (TimelineButton.DefaultCondition(animation)) && (animation.AnimationData.GetCurrentKeyFrameIndex() > 0);

        private bool MoveRightButtonCondition(Animation animation)
                    => (TimelineButton.DefaultCondition(animation)) && (animation.AnimationData.GetCurrentKeyFrameIndex() < animation.AnimationData.GetTotalKeyFrames() - 1);

        #endregion Button Conditions
    }
}