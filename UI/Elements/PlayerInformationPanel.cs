﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace HunterCombatMR.UI.Elements
{
    internal class PlayerInformationPanel
        : UIPanel
    {
        private const string _noneText = "N/A";
        private string[] _parameters = new string[6];

        public PlayerInformationPanel()
        {
            ParameterList = new UIList();
        }

        internal UIList ParameterList { get; }

        protected HunterCombatPlayer Player { get; set; }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Player == null)
                return;

            base.Draw(spriteBatch);
        }

        public override void OnInitialize()
        {
            base.OnInitialize();

            MinWidth.Set(PaddingLeft + PaddingRight, 0f);
            MinHeight.Set(PaddingTop + PaddingBottom, 0f);

            ParameterList.Width.Set(0f, 1f);
            ParameterList.Height.Set(0f, 1f);

            for (int p = 0; p < _parameters.Count(); p++)
            {
                UIText parameter = new UIText("");
                ParameterList.Add(parameter);
            }

            Append(ParameterList);
        }

        public override void Recalculate()
        {
            if (Player == null)
                return;

            if (ParameterList._items.Any())
            {
                MinWidth.Set(PaddingLeft + PaddingRight + ParameterList._items.OrderBy(x => x.GetDimensions().Width).LastOrDefault().GetDimensions().Width, 0f);
                MinHeight.Set(PaddingTop + PaddingBottom + ParameterList._items.Count() * 32f, 0f);
            }

            Left.Set(Player.player.BottomLeft.X - MinWidth.Pixels / 2 - Main.screenPosition.X, 0f);
            Top.Set(Player.player.BottomLeft.Y + 25f - Main.screenPosition.Y, 0f);

            base.Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            if (Player == null)
                return;

            _parameters = new string[6]
            {
                $"PState: {Player.StateController.State.ToString()} ({(int)Player.StateController.State})",
                $"Anim: {AnimationText()}",
                $"Vel: {Math.Round(Player.player.velocity.X, 2)}, {Math.Round(Player.player.velocity.Y, 2)}",
                $"Action: {ActionText()}",
                $"AState: {Player.StateController.ActionState.ToString()} ({(int)Player.StateController.ActionState})",
                $"Equip: {EquipText()}"
            };

            for (var i = 0; i < ParameterList._items.Count(); i++)
            {
                (ParameterList._items[i] as UIText).SetText(_parameters[i]);
            }

            Recalculate();

            base.Update(gameTime);
        }

        internal void SetPlayer(HunterCombatPlayer player)
        {
            Player = player;
        }

        private string ActionText()
            => Player.StateController.CurrentAction != null
                ? $"{Player.StateController.CurrentAction.DisplayName} - {Player.StateController.CurrentActionKeyFrame}"
                : _noneText;

        private string AnimationText()
            => Player.AnimationController.CurrentAnimation != null
                ? $"{Player.AnimationController.CurrentAnimation.InternalName} - {Player.AnimationController.Animator.CurrentFrame}"
                : _noneText;

        private string EquipText()
            => Player.EquippedWeapon != null
                ? $"{Player.EquippedWeapon.Name}"
                : _noneText;
    }
}