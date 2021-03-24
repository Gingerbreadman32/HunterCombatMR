using HunterCombatMR.AnimationEngine.Interfaces;
using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.Enumerations;
using System;
using System.Linq;
using Terraria.UI;

namespace HunterCombatMR.UI.AnimationTimeline
{
    public partial class Timeline
    {
        #region Internal Classes

        internal static class TimelineButtonNames
        {
            #region Internal Fields

            internal const string AddKeyButton = "AddKeyFrame";
            internal const string CopyKeyButton = "CopyKeyFrame";
            internal const string MoveLeftKeyButton = "MoveKeyFrameLeft";
            internal const string MoveRightKeyButton = "MoveKeyFrameRight";
            internal const string PlayPauseButton = "PlayPauseButton";
            internal const string RemoveKeyButton = "RemoveKeyFrame";
            internal const string StopButton = "StopButton";
            internal const string LoopButton = "LoopButton";

            #endregion Internal Fields
        }

        #endregion Internal Classes

        #region Public Methods

        public void AddButton(string name,
            TimelineButtonIcon iconType,
            Action buttonEvent,
            Func<IAnimation, TimelineButton, bool> conditionEvent = null)
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
            AddButton(TimelineButtonNames.PlayPauseButton, TimelineButtonIcon.Play, PlayPauseButtonLogic, PlayPauseButtonCondition);
            AddButton(TimelineButtonNames.StopButton, TimelineButtonIcon.Stop, StopButtonLogic, StopButtonCondition);
            AddButton(TimelineButtonNames.LoopButton, TimelineButtonIcon.Loop, LoopButtonLogic, LoopButtonCondition);
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
            Animation.AddKeyFrame((FrameLength)Animation.KeyFrameProfile.DefaultKeyFrameSpeed);
            HunterCombatMR.Instance.EditorInstance.AnimationEdited = true;
        }

        private void CopyButtonLogic()
        {
            var copyKeyframe = Animation.AnimationData.GetCurrentKeyFrame();
            Animation.AddKeyFrame(copyKeyframe);
            HunterCombatMR.Instance.EditorInstance.AnimationEdited = true;
        }

        private void DeleteButtonLogic()
        {
            Animation.RemoveKeyFrame(Animation.AnimationData.CurrentKeyFrameIndex);
            HunterCombatMR.Instance.EditorInstance.AnimationEdited = true;
        }

        private Action MoveButtonLogic(bool forward)
        {
            return () =>
            {
                int currentKeyFrame = Animation.AnimationData.CurrentKeyFrameIndex;
                Animation.MoveKeyFrame(currentKeyFrame, (forward) ? currentKeyFrame + 1 : currentKeyFrame - 1);
                HunterCombatMR.Instance.EditorInstance.AnimationEdited = true;
            };
        }

        private void PlayPauseButtonLogic()
        {
            if (!Animation.AnimationData.IsPlaying)
                Animation.Play();
            else
                Animation.Pause();
        }

        private void StopButtonLogic()
        {
            Animation.Stop();
        }

        private void LoopButtonLogic()
        {
            if (!Animation.AnimationData.CurrentLoopStyle.Equals(LoopStyle.PlayPause))
                Animation.UpdateLoopType(Animation.AnimationData.CurrentLoopStyle + 1);
            else
                Animation.UpdateLoopType(0);

            HunterCombatMR.Instance.EditorInstance.AnimationEdited = true;
        }

        #endregion Button Events

        #region Button Conditions

        private bool AddButtonCondition(IAnimation animation,
            TimelineButton button)
            => (TimelineButton.DefaultCondition(animation, button)) && !(!ShowAllFrames && animation.AnimationData.KeyFrames.Count == _maxFrames)
                || (ShowAllFrames && animation.AnimationData.TotalFrames == _maxFrames);

        private bool DeleteButtonCondition(IAnimation animation,
            TimelineButton button)
            => (TimelineButton.DefaultCondition(animation, button)) && (animation.AnimationData.KeyFrames.Count > 1);

        private bool MoveLeftButtonCondition(IAnimation animation,
            TimelineButton button)
            => (TimelineButton.DefaultCondition(animation, button)) && (animation.AnimationData.CurrentKeyFrameIndex > 0);

        private bool MoveRightButtonCondition(IAnimation animation,
            TimelineButton button)
                    => (TimelineButton.DefaultCondition(animation, button)) 
                        && (animation.AnimationData.CurrentKeyFrameIndex < animation.AnimationData.KeyFrames.Count - 1);

        private bool PlayPauseButtonCondition(IAnimation animation,
            TimelineButton button)
        {
            if (TimelineButton.DefaultCondition(animation, button))
            {
                if (Animation.AnimationData.IsPlaying)
                    button.Icon = TimelineButtonIcon.Pause;
                else
                    button.Icon = TimelineButtonIcon.Play;

                return true;
            } else
            {
                return false;
            }
        }

        private bool LoopButtonCondition(IAnimation animation,
            TimelineButton button)
        {
            if (TimelineButton.DefaultCondition(animation, button))
            {
                switch (Animation.AnimationData.CurrentLoopStyle)
                {
                    case LoopStyle.Loop:
                        button.Icon = TimelineButtonIcon.Loop;
                        break;
                    case LoopStyle.Once:
                        button.Icon = TimelineButtonIcon.LoopOnce;
                        break;
                    case LoopStyle.PlayPause:
                        button.Icon = TimelineButtonIcon.PlayPause;
                        break;
                    case LoopStyle.PingPong:
                        button.Icon = TimelineButtonIcon.PingPong;
                        break;
                    default:
                        button.Icon = TimelineButtonIcon.NotFound;
                        break;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        private bool StopButtonCondition(IAnimation animation,
            TimelineButton button)
            => TimelineButton.DefaultCondition(animation, button) && animation.AnimationData.InProgress;

        #endregion Button Conditions
    }
}