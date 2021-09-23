using HunterCombatMR.Builders.Animation;
using HunterCombatMR.Builders.State;
using HunterCombatMR.Constants;
using HunterCombatMR.Enumerations;
using HunterCombatMR.Extensions;
using HunterCombatMR.Interfaces.Entity;
using HunterCombatMR.Items;
using HunterCombatMR.Managers;
using HunterCombatMR.Messages.AnimationSystem;
using HunterCombatMR.Messages.EntityStateSystem;
using HunterCombatMR.Messages.InputSystem;
using HunterCombatMR.Models.Animation;
using HunterCombatMR.Components;
using HunterCombatMR.Models.State;
using HunterCombatMR.Models.State.Builders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using HunterCombatMR.Models.Behavior;
using HunterCombatMR.Models.Input;

namespace HunterCombatMR
{
    public class HunterCombatPlayer
        : ModPlayer
    {
        private IModEntity _entity;
        private WeaponBase _equippedWeapon;
        public IModEntity EntityReference { get => _entity; }

        public WeaponBase EquippedWeapon
        {
            get => _equippedWeapon;
            set { _equippedWeapon = value; }
        }

        public bool IsMainPlayer { get => player.whoAmI == Main.myPlayer; }

        public override void Initialize()
        {
            _entity = EntityManager.CreateEntity();
        }

        public override void ModifyDrawLayers(List<PlayerLayer> layers)
        {
            _entity.SendMessage(new CreatePlayerLayersMessage(_entity.Id, ref layers));
        }

        public override void OnEnterWorld(Player player)
        {
            _entity.SendMessage(new SetWorldStatusMessage(_entity.Id, EntityWorldStatus.Grounded));
            if (IsMainPlayer && Main.netMode == NetmodeID.SinglePlayer)
            {
                _entity.RegisterComponent(new InputComponent(player.whoAmI));
                StateSet stateSet = CreateTestStateSet();
                var animationSet = CreateTestAnimationSet();

                var behaviorComponent = new BehaviorComponent(new Behavior("Player", stateSet, animationSet, default(CommandList)));
                _entity.RegisterComponent(behaviorComponent);

            }
        }

        public override void OnRespawn(Player player)
        {
            _entity.SendMessage(new SetWorldStatusMessage(_entity.Id, EntityWorldStatus.Grounded));
            _entity.SendMessage(new InputResetMessage(_entity.Id));
        }

        public override void PostSavePlayer()
        {
            if (Main.gameMenu)
            {
                _entity.RemoveAllComponents();
            }

            base.PostSavePlayer();
        }

        public override bool PreItemCheck()
        {
            if (!HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.None) || EquippedWeapon != null)
            {
                if (player.itemTime > 0)
                    player.itemTime = 0;
                if (player.itemAnimation > 0)
                    player.itemAnimation = 0;

                return false;
            }

            return base.PreItemCheck();
        }

        public override void UpdateDead()
        {
            _entity.SendMessage(new SetWorldStatusMessage(_entity.Id, EntityWorldStatus.Dead));
            _entity.SendMessage(new InputResetMessage(_entity.Id));
        }

        private static Dictionary<int, CustomAnimation> CreateTestAnimationSet()
        {
            var animations = new Dictionary<int, CustomAnimation>();

            var builder = new AnimationBuilder("Test", AnimationType.Player, 12)
            {
                LoopType = LoopStyle.Loop
            };

            // Temporarily using sheet index for texture height.
            builder.AddLayer(new LayerBuilder("HC_Head")
                .WithKeyframe(depth: 2, sheetNumber: 18, position: new Point(3, 13)) //1 ["255","3","13","0","0","None","1"]
                .BuildKeyframe(CopyKeyframe.Last).AtPosition(5, 15).FinishKeyframe() //2 ["255","5","15","0","0","None","1"]
                .BuildKeyframe(CopyKeyframe.Last).AtPosition(5, 13).WithDepth(4).NextSprite(1).FinishKeyframe() //3 ["255","5","13","0","1","None","1","4"]
                .BuildKeyframe(CopyKeyframe.First).AtPosition(5, 11).NextSprite().FinishKeyframe() //4 ["255","5","11","0","1","None","1"]
                .BuildKeyframe(CopyKeyframe.Last).AtPosition(5, 13).NextSprite().FinishKeyframe() //5 ["255","5","13","0","2","None","1"]
                .BuildKeyframe(CopyKeyframe.Set, 3).FinishKeyframe() //6 ["255","5","13","0","1","None","1","4"]
                .BuildKeyframe(CopyKeyframe.First).AtPosition(1, 13).NextSprite(3).FinishKeyframe() //7 ["255","1","13","0","3","None","1"]
                .BuildKeyframe(CopyKeyframe.First).AtPosition(3, 13).FinishKeyframe()); //8 ["255","3","13","0","0","None","1"]
            builder.AddLayer(new LayerBuilder("HC_FrontArm")
                .WithKeyframe(depth: 1, sheetNumber: 14, position: new Point(-13, 19)) //1 ["255","-13","19","0","0","None","1"]
                .BuildKeyframe(CopyKeyframe.Last).AtPosition(-5, 27).NextSprite().FinishKeyframe() //2 ["255","-5","27","0","1","None","1"]
                .BuildKeyframe(CopyKeyframe.Last).AtPosition(-15, 21).NextSprite(0).Flipped(SpriteEffects.FlipHorizontally).FinishKeyframe() //3 ["255","-15","21","0","0","FlipHorizontally","1"]
                .BuildKeyframe(CopyKeyframe.Last).AtPosition(17, 17).NextSprite(2).WithDepth(4).Flipped(SpriteEffects.FlipHorizontally).FinishKeyframe() //4 ["255","17","17","0","2","None","1","4"]
                .BuildKeyframe(CopyKeyframe.Last).AtPosition(17, 19).FinishKeyframe() //5 ["255","17","19","0","2","None","1","4"]
                .BuildKeyframe(CopyKeyframe.Set, 3).FinishKeyframe() //6 ["255","-15","21","0","0","FlipHorizontally","1"]
                .BuildKeyframe(CopyKeyframe.First).AtPosition(-17, 25).NextSprite(3).FinishKeyframe() //7 ["255","-17","25","0","3","None","1"]
                .BuildKeyframe(CopyKeyframe.Last).AtPosition(-13, 21).NextSprite(0).Flipped(SpriteEffects.FlipVertically).FinishKeyframe()); //8 ["255","-13","21","0","0","FlipVertically","1"]
            builder.AddLayer(new LayerBuilder("HC_BackArm")
                .WithKeyframe(depth: 5, sheetNumber: 12, position: new Point(16, 30)) //1 ["255","16","30","0","0","None","1"]
                .BuildKeyframe(CopyKeyframe.Last).AtPosition(16, 26).NextSprite().FinishKeyframe() //2 ["255","16","26","0","1","None","1"]
                .BuildKeyframe(CopyKeyframe.Last).AtPosition(14, 26).FinishKeyframe() //3 ["255","14","26","0","1","None","1"]
                .BuildKeyframe(CopyKeyframe.Last).AtPosition(-2, 14).NextSprite().FinishKeyframe() //4 ["255","-2","14","0","2","None","1"]
                .BuildKeyframe(CopyKeyframe.Last).AtPosition(0, 16).FinishKeyframe() //5 ["255","0","16","0","2","None","1"]
                .BuildKeyframe(CopyKeyframe.Set, 3).FinishKeyframe() //6 ["255","14","26","0","1","None","1"]
                .BuildKeyframe(CopyKeyframe.First).AtPosition(14, 26).NextSprite(3).FinishKeyframe() //7 ["255","14","26","0","3","None","1"]
                .BuildKeyframe(CopyKeyframe.Last).AtPosition(16, 24).NextSprite().FinishKeyframe()); //8 ["255","16","24","0","4","None","1"]
            builder.AddLayer(new LayerBuilder("HC_Body")
                .WithKeyframe(depth: 4, sheetNumber: 16, position: new Point(5, 24)) //1 ["255","5","24","0","0","None","1"]
                .BuildKeyframe(CopyKeyframe.Last).AtPosition(7, 28).NextSprite().FinishKeyframe() //2 ["255","7","28","0","1","None","1"]
                .BuildKeyframe(CopyKeyframe.Last).AtPosition(7, 24).NextSprite().WithDepth(2).FinishKeyframe() //3 ["255","7","24","0","2","None","1","2"]
                .BuildKeyframe(CopyKeyframe.Last).AtPosition(7, 22).NextSprite().WithDepth(1).FinishKeyframe() //4 ["255","7","22","0","3","None","1","1"]
                .BuildKeyframe(CopyKeyframe.Last).AtPosition(7, 24).NextSprite().FinishKeyframe() //5 ["255","7","24","0","4","None","1","1"]
                .BuildKeyframe(CopyKeyframe.Set, 3).FinishKeyframe() //6 ["255","7","24","0","2","None","1","2"]
                .BuildKeyframe(CopyKeyframe.First).AtPosition(3, 26).NextSprite(5).FinishKeyframe() //7 ["255","3","26","0","5","None","1"]
                .BuildKeyframe(CopyKeyframe.Last).AtPosition(5, 24).NextSprite().FinishKeyframe()); //8 ["255","5","24","0","6","None","1"]
            builder.AddLayer(new LayerBuilder("HC_FrontLeg")
                .WithKeyframe(depth: 3, sheetNumber: 14, position: new Point(-2, 39)) //1 ["255","-2","39","0","0","None","1"]
                .BuildKeyframe(CopyKeyframe.Last).AtPosition(0, 39).NextSprite().FinishKeyframe() //2 ["255","0","39","0","1","None","1"]
                .BuildKeyframe(CopyKeyframe.Last).NextSprite().FinishKeyframe() //3 ["255","0","39","0","2","None","1"]
                .BuildKeyframe(CopyKeyframe.Last).AtPosition(0, 37).NextSprite().FinishKeyframe() //4 ["255","0","37","0","3","None","1"]
                .BuildKeyframe(CopyKeyframe.Last).AtPosition(2, 37).FinishKeyframe() //5 ["255","2","37","0","3","None","1"]
                .BuildKeyframe(CopyKeyframe.Set, 3).FinishKeyframe() //6 ["255","0","39","0","2","None","1"]
                .BuildKeyframe(CopyKeyframe.First).AtPosition(-6, 39).NextSprite(4).FinishKeyframe() //7 ["255","-6","39","0","4","None","1"]
                .BuildKeyframe(CopyKeyframe.Last).AtPosition(-4, 39).NextSprite().FinishKeyframe()); //8 ["255","-4","39","0","5","None","1"]
            builder.AddLayer(new LayerBuilder("HC_BackLeg")
                .WithKeyframe(depth: 6, sheetNumber: 16, position: new Point(11, 40)) //1 ["255","11","40","0","0","None","1"]
                .BuildKeyframe(CopyKeyframe.Last).FinishKeyframe() //2 ["255","11","40","0","0","None","1"]
                .BuildKeyframe(CopyKeyframe.Last).NextSprite().FinishKeyframe() //3 ["255","11","40","0","1","None","1"]
                .BuildKeyframe(CopyKeyframe.Last).AtPosition(11, 36).NextSprite().FinishKeyframe() //4 ["255","11","36","0","2","None","1"]
                .BuildKeyframe(CopyKeyframe.Last).AtPosition(13, 40).NextSprite(1).FinishKeyframe() //5 ["255","13","40","0","1","None","1"]
                .BuildKeyframe(CopyKeyframe.Set, 3).FinishKeyframe() //6 ["255","11","40","0","1","None","1"]
                .BuildKeyframe(CopyKeyframe.First).AtPosition(9, 40).NextSprite(3).FinishKeyframe() //7 ["255","9","40","0","3","None","1"]
                .BuildKeyframe(CopyKeyframe.Last).AtPosition(11, 40).NextSprite().FinishKeyframe()); //8 ["255","11","40","0","4","None","1"]

            builder.AddKeyframes(CopyKeyframe.None, 12, 4, 8, 16, 4, 12, 52);

            animations.Add(0, builder.Build());

            return animations;
        }

        private static StateSet CreateTestStateSet()
        {
            var states = new EntityState[2];
            states[0] = new StateBuilder()
                .WithNewController(StateControllerTypes.ChangeState, 1, new StateTrigger("time = 100"))
                .WithEntityStatuses(EntityWorldStatus.Grounded, EntityActionStatus.Idle)
                .WithParameters(animation: -1)
                .Build();
            states[1] = new StateBuilder()
                .WithNewController(StateControllerTypes.ChangeState, 0, new StateTrigger("time = 250"))
                .WithEntityStatuses(EntityWorldStatus.Grounded, EntityActionStatus.ActionStartup)
                .WithParameters(animation: 0)
                .Build();

            var stateSet = new StateSetBuilder()
                .WithState(StateNumberConstants.Default, states[0])
                .WithState(1, states[1]);

            stateSet.AddGlobalController("TestController", 
                ControllerPriorities.Local, 
                new StateControllerBuilder(StateControllerTypes.ChangeState)
                    .WithTrigger("frame = 107", 1)
                    .WithTrigger("state = 1", 1)
                    .WithParameter(0)
                    .Build());

            return stateSet.Build();
        }
    }
}