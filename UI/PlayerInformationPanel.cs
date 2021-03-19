﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace HunterCombatMR.UI
{
    internal class PlayerInformationPanel
        : UIPanel
    {
        #region Private Fields

        private readonly int _parameterCount = 5;

        #endregion Private Fields

        #region Public Constructors

        public PlayerInformationPanel()
        {
            ParameterList = new UIList();
        }

        #endregion Public Constructors

        #region Internal Properties

        internal UIList ParameterList { get; }

        #endregion Internal Properties

        #region Protected Properties

        protected HunterCombatPlayer Player { get; set; }

        #endregion Protected Properties

        #region Public Methods

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

            for (int p = 0; p < _parameterCount; p++)
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

            Left.Set(Player.player.BottomLeft.X - (MinWidth.Pixels/2) - Main.screenPosition.X, 0f);
            Top.Set(Player.player.BottomLeft.Y + 25f - Main.screenPosition.Y, 0f);

            base.Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            if (Player == null)
                return;

            (ParameterList._items[0] as UIText).SetText($"PState: {Player.StateController.State.ToString()} ({((int)Player.StateController.State)})");
            (ParameterList._items[1] as UIText).SetText($"Anim: {AnimationText()}");
            (ParameterList._items[2] as UIText).SetText($"Vel: {Math.Round(Player.player.velocity.X, 2)}, {Math.Round(Player.player.velocity.Y, 2)}");
            (ParameterList._items[3] as UIText).SetText($"Action: {ActionText()}");
            (ParameterList._items[4] as UIText).SetText($"AState: {Player.StateController.ActionState.ToString()} ({((int)Player.StateController.ActionState)})");

            Recalculate();

            base.Update(gameTime);
        }

        #endregion Public Methods

        #region Internal Methods

        internal void SetPlayer(HunterCombatPlayer player)
        {
            Player = player;
        }

        #endregion Internal Methods

        private string AnimationText()
            => (Player.CurrentAnimation != null) 
                ? $"{Player.CurrentAnimation.Name} - {Player.CurrentAnimation.AnimationData.CurrentFrame}" 
                : "N/A";

        private string ActionText()
            => (Player.StateController.CurrentAction != null) 
                ? $"{Player.StateController.CurrentAction.Name} - {Player.StateController.GetCurrentActionFrame()}" 
                : "N/A";

    }
}