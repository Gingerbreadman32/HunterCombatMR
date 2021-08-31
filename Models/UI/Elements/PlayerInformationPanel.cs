using HunterCombatMR.Extensions;
using HunterCombatMR.Managers;
using HunterCombatMR.Models.Components;
using Microsoft.Xna.Framework;
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
        private readonly bool _stateVersion = true;

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

            // Debug
            if (_stateVersion)
            {
                if (!Player.EntityReference.HasComponent<EntityStateComponent>())
                    return;

                var component = Player.EntityReference.GetComponent<EntityStateComponent>();
                var currentState = component.GetCurrentState();

                _parameters = new string[]
                {
                $"State No.: {component.CurrentStateNumber}",
                $"W. Status: {currentState.Definition.WorldStatus}",
                $"A. Status: {currentState.Definition.ActionStatus}",
                $"State Time: {component.CurrentStateInfo.Time}",
                $"State Set: {component.CurrentStateInfo.StateSet}"
                };
            } else
            {
                _parameters = new string[5]
                {
                $"WStatus: {Player.StateController.State.ToString()} ({(int)Player.StateController.State})",
                $"AStatus: {Player.StateController.ActionState.ToString()} ({(int)Player.StateController.ActionState})",
                $"Vel: {Math.Round(Player.player.velocity.X, 2)}, {Math.Round(Player.player.velocity.Y, 2)}",
                $"State No.: {ActionText()}",
                $"Equip: {EquipText()}"
                };
            }

            for (var i = 0; i < _parameters.Length; i++)
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
            => Player.EntityReference.HasComponent<EntityStateComponent>()
                ? $"{Player.EntityReference.GetComponent<EntityStateComponent>().CurrentStateNumber}"
                : _noneText;

        private string EquipText()
            => Player.EquippedWeapon != null
                ? $"{Player.EquippedWeapon.Name}"
                : _noneText;
    }
}