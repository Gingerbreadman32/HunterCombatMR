using AnimationEngine.Services;
using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.AnimationEngine.Services;
using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.Enumerations;
using HunterCombatMR.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace HunterCombatMR
{
    public class HunterCombatMR
        : Mod
    {
        /* Singleton code I should probably use at some point
        public static HunterCombatMR Instance { get; set; }

        public HunterCombatMR() { Instance = this; }
        */

        private GameTime _lastUpdateUiGameTime;

        public static List<Attack> LoadedAttacks { get; set; }

        public static List<LayeredAnimatedAction> LoadedAnimations { get; set; }

        public static KeyFrameManager AnimationKeyFrameManager { get; set; }

        public static AnimationEditor EditorInstance { get; set; }

        public static string ModName = "HunterCombat";

        internal UserInterface DebugUI;
        internal BufferDebugUIState DebugUIState;
        internal AnimationLoader AnimLoader = new AnimationLoader();

        private void LoadAnimations(Type[] types)
        {
            foreach (Type type in types.Where(x => x.IsSubclassOf(typeof(ActionContainer)) && !x.IsAbstract))
            {
                var container = (ActionContainer)type.GetConstructor(new Type[] { }).Invoke(new object[] { });
                container.Load();
                AnimLoader.LoadContainer(container);
            }

            if (AnimLoader.Containers.Any())
                LoadedAnimations = new List<LayeredAnimatedAction>(AnimLoader.RegisterAnimations());
        }

        public override void Load()
        {
            /* Libvaxy implementation, will use if I need anything else from it
            foreach (Type type in Reflection.GetNonAbstractSubtypes(typeof(ParentType))
                YourCacheList.Add(Reflection.CreateInstance(type));
            */

            AnimationKeyFrameManager = new KeyFrameManager();

            // Load all reflected types in the mod
            Type[] assemblyTypes = typeof(HunterCombatMR).Assembly.GetTypes();

            // Load, register, and store all the animations
            LoadAnimations(assemblyTypes);

            // Loads all of the attack information
            LoadedAttacks = new List<Attack>();

            foreach (Type type in assemblyTypes.Where(x => x.IsSubclassOf(typeof(Attack)) && !x.IsAbstract))
            {
                LoadedAttacks.Add((Attack)type.GetConstructor(new Type[] { }).Invoke(new object[] { }));
            }

            if (!Main.dedServ)
            {
                EditorInstance = new AnimationEditor();

                // Debug UI stuff
                DebugUI = new UserInterface();
                DebugUIState = new BufferDebugUIState();
                DebugUIState.Activate();
                ShowMyUI();
            }
        }

        public override void PostSetupContent()
        {
            Type[] assemblyTypes = typeof(HunterCombatMR).Assembly.GetTypes();

            foreach (Type type in assemblyTypes.Where(x => x.IsSubclassOf(typeof(AttackProjectile)) && !x.IsAbstract))
            {
                type.GetMethod("Initialize").Invoke(GetProjectile(type.Name), null);
            }
        }

        public override void Unload()
        {
            LoadedAttacks = null;
            AnimationKeyFrameManager = null;
            DebugUIState = null;
            ModName = null;
            LoadedAnimations = null;

            EditorInstance.Dispose();
            EditorInstance = null;
        }

        public override void PreSaveAndQuit()
        {
            HideMyUI();
        }

        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUiGameTime = gameTime;
            if (DebugUI?.CurrentState != null)
            {
                DebugUI.Update(gameTime);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "HunterCombat: Buffer Debug",
                    delegate
                    {
                        if (_lastUpdateUiGameTime != null && DebugUI?.CurrentState != null)
                        {
                            DebugUI.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        return true;
                    },
                       InterfaceScaleType.UI));
            }
        }

        internal void ShowMyUI()
        {
            DebugUI?.SetState(DebugUIState);
        }

        internal void HideMyUI()
        {
            DebugUI?.SetState(null);
        }

        internal static Texture2D ReadTexture(string file)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(file + ".png");
            if (stream == null)
            {
                throw new ArgumentException("Failed creating IO stream for grabbing new textures.");
            }
            return Texture2D.FromStream(Main.instance.GraphicsDevice, stream);
        }
    }
}