using HunterCombatMR.Builders.Animation;
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
            Func<AnimationBuilder, TimelineButton, bool> conditionEvent = null)
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

        private bool AddButtonCondition(AnimationBuilder animation,
            TimelineButton button)
            => (TimelineButton.DefaultCondition(animation, button)) 
                && !(!ShowAllFrames && animation.Keyframes.Sum(x => x.Value) == _maxFrames)
                    || (ShowAllFrames && Animator.GetTotalFrames() == _maxFrames);

        private void AddButtonLogic()
        {
            //Animation.AddKeyFrame(Animation.LayerData.KeyFrameProfile.DefaultKeyFrameLength);
            HunterCombatMR.Instance.EditorInstance.AnimationEdited = true;
        }

        private void CopyButtonLogic()
        {
            //var copyKeyframe = Animation.AnimationData.CurrentKeyFrame;
            //Animation.AddKeyFrame(copyKeyframe);
            HunterCombatMR.Instance.EditorInstance.AnimationEdited = true;
        }

        private bool DeleteButtonCondition(AnimationBuilder animation,
            TimelineButton button)
            => (TimelineButton.DefaultCondition(animation, button)) && (Animator.Keyframes.Count() > 1);

        private void DeleteButtonLogic()
        {
            //Animation.RemoveKeyFrame(Animation.AnimationData.CurrentKeyFrameIndex);
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

        private bool LoopButtonCondition(AnimationBuilder animation,
            TimelineButton button)
        {
            if (TimelineButton.DefaultCondition(animation, button))
            {
                switch (Animator.LoopStyle)
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
            if (!Animator.LoopStyle.Equals(LoopStyle.PlayPause))
                Animator.LoopStyle = (Animator.LoopStyle + 1);
            else
                Animator.LoopStyle = 0;

            HunterCombatMR.Instance.EditorInstance.AnimationEdited = true;
        }

        private Action MoveButtonLogic(bool forward)
        {
            return () =>
            {
                //int currentKeyFrame = Animation.AnimationData.CurrentKeyFrameIndex;
                //Animation.MoveKeyFrame(currentKeyFrame, (forward) ? currentKeyFrame + 1 : currentKeyFrame - 1);
                HunterCombatMR.Instance.EditorInstance.AnimationEdited = true;
            };
        }

        private bool MoveLeftButtonCondition(AnimationBuilder animation,
            TimelineButton button)
            => (TimelineButton.DefaultCondition(animation, button)) && (Animator.GetCurrentKeyframe() > 0);

        private bool MoveRightButtonCondition(AnimationBuilder animation,
            TimelineButton button)
                    => (TimelineButton.DefaultCondition(animation, button))
                        && (Animator.GetCurrentKeyframe() < Animator.Keyframes.Count() - 1);

        private bool PlayPauseButtonCondition(AnimationBuilder animation,
            TimelineButton button)
        {
            if (TimelineButton.DefaultCondition(animation, button))
            {
                button.Icon = (Animator.Flags.HasFlag(AnimationFlags.Paused))
                    ? TimelineButtonIcon.Play
                    : TimelineButtonIcon.Pause;

                return true;
            }

                return false;
            
        }

        private void PlayPauseButtonLogic()
        {
            if (!Animator.Flags.HasFlag(AnimationFlags.Paused))
            {
                Animator.Pause();
                return;
            }

            Animator.Play();
        }

        private bool StopButtonCondition(AnimationBuilder animation,
            TimelineButton button)
            => TimelineButton.DefaultCondition(animation, button) && Animator.Frame > 0;

        private void StopButtonLogic()
        {
            Animator.Stop(false);
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