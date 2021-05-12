using HunterCombatMR.Enumerations;
using HunterCombatMR.Interfaces;
using HunterCombatMR.Models;
using System;
using System.Linq;
using Terraria.UI;

namespace HunterCombatMR.UI.AnimationTimeline
{
    public partial class Timeline
    {
        public void AddButton(string name,
            TimelineButtonIcon iconType,
            Action buttonEvent,
            Func<ICustomAnimation, TimelineButton, bool> conditionEvent = null)
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

        private bool AddButtonCondition(ICustomAnimation animation,
            TimelineButton button)
            => (TimelineButton.DefaultCondition(animation, button)) && !(!ShowAllFrames && animation.AnimationData.KeyFrames.Count == _maxFrames)
                || (ShowAllFrames && animation.AnimationData.TotalFrames == _maxFrames);

        private void AddButtonLogic()
        {
            Animation.AddKeyFrame((FrameLength)Animation.KeyFrameProfile.DefaultKeyFrameLength);
            HunterCombatMR.Instance.EditorInstance.AnimationEdited = true;
        }

        private void CopyButtonLogic()
        {
            var copyKeyframe = Animation.AnimationData.CurrentKeyFrame;
            Animation.AddKeyFrame(copyKeyframe);
            HunterCombatMR.Instance.EditorInstance.AnimationEdited = true;
        }

        private bool DeleteButtonCondition(ICustomAnimation animation,
            TimelineButton button)
            => (TimelineButton.DefaultCondition(animation, button)) && (animation.AnimationData.KeyFrames.Count > 1);

        private void DeleteButtonLogic()
        {
            Animation.RemoveKeyFrame(Animation.AnimationData.CurrentKeyFrameIndex);
            HunterCombatMR.Instance.EditorInstance.AnimationEdited = true;
        }

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

        private bool LoopButtonCondition(ICustomAnimation animation,
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

        private void LoopButtonLogic()
        {
            if (!Animation.AnimationData.CurrentLoopStyle.Equals(LoopStyle.PlayPause))
                Animation.UpdateLoopType(Animation.AnimationData.CurrentLoopStyle + 1);
            else
                Animation.UpdateLoopType(0);

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

        private bool MoveLeftButtonCondition(ICustomAnimation animation,
            TimelineButton button)
            => (TimelineButton.DefaultCondition(animation, button)) && (animation.AnimationData.CurrentKeyFrameIndex > 0);

        private bool MoveRightButtonCondition(ICustomAnimation animation,
            TimelineButton button)
                    => (TimelineButton.DefaultCondition(animation, button))
                        && (animation.AnimationData.CurrentKeyFrameIndex < animation.AnimationData.KeyFrames.Count - 1);

        private bool PlayPauseButtonCondition(ICustomAnimation animation,
            TimelineButton button)
        {
            if (TimelineButton.DefaultCondition(animation, button))
            {
                button.Icon = (Animation.AnimationData.Flags.HasFlag(AnimatorFlags.Locked))
                    ? TimelineButtonIcon.Play
                    : TimelineButtonIcon.Pause;

                return true;
            }
            else
            {
                return false;
            }
        }

        private void PlayPauseButtonLogic()
        {
            Animation.AnimationData.PlayPause();
        }

        private bool StopButtonCondition(ICustomAnimation animation,
            TimelineButton button)
            => TimelineButton.DefaultCondition(animation, button) && animation.AnimationData.Flags.HasFlag(AnimatorFlags.Started);

        private void StopButtonLogic()
        {
            Animation.AnimationData.Stop(false);
        }

        internal static class TimelineButtonNames
        {
            internal const string AddKeyButton = "AddKeyFrame";
            internal const string CopyKeyButton = "CopyKeyFrame";
            internal const string LoopButton = "LoopButton";
            internal const string MoveLeftKeyButton = "MoveKeyFrameLeft";
            internal const string MoveRightKeyButton = "MoveKeyFrameRight";
            internal const string PlayPauseButton = "PlayPauseButton";
            internal const string RemoveKeyButton = "RemoveKeyFrame";
            internal const string StopButton = "StopButton";
        }
    }
}