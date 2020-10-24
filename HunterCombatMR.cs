using HunterCombatMR.AnimationEngine.Services;
using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.UI;
using HunterCombatMR.Enumerations;
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
using HunterCombatMR.AnimationEngine.Models;

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

        public static List<string> HighlightedLayers { get; set; }

        public static Point? SelectedLayerNudgeAmount { get; set; }

        public static int? NudgeCooldown { get; set; }

        public static string SelectedLayer { get; set; }

        public static string ModName = "HunterCombat";

        public static EditorMode EditMode { get; set; }

        internal UserInterface DebugUI;
        internal BufferDebugUIState DebugUIState;
        internal AnimationLoader AnimLoader = new AnimationLoader();

        private void LoadAnimations(Type[] types)
        {
            foreach (Type type in types.Where(x => x.IsSubclassOf(typeof(ActionContainer)) && !x.IsAbstract))
            {
                AnimLoader.LoadContainer((ActionContainer)type.GetConstructor(new Type[] { }).Invoke(new object[] { }));
            }

            if (AnimLoader.Containers.Any())
                AnimLoader.RegisterAnimations();
        }

        public override void Load()
        {
            /* Libvaxy implementation, will use if I need anything else from it
            foreach (Type type in Reflection.GetNonAbstractSubtypes(typeof(ParentType))
                YourCacheList.Add(Reflection.CreateInstance(type));
            */

            Type[] assemblyTypes = typeof(HunterCombatMR).Assembly.GetTypes();

            LoadAnimations(assemblyTypes);

            if (!Main.dedServ)
            {
                // Loads all of the attacks into a static list for use later.
                LoadedAttacks = new List<Attack>();

                foreach (Type type in assemblyTypes.Where(x => x.IsSubclassOf(typeof(Attack)) && !x.IsAbstract))
                {
                    LoadedAttacks.Add((Attack)type.GetConstructor(new Type[] { }).Invoke(new object[] { }));
                }

                AnimationKeyFrameManager = new KeyFrameManager();

                HighlightedLayers = new List<string>();
                SelectedLayer = "";
                SelectedLayerNudgeAmount = new Point(0, 0);
                NudgeCooldown = 2;
                EditMode = EditorMode.None;

                // Debug UI stuff
                DebugUI = new UserInterface();
                DebugUIState = new BufferDebugUIState();
                DebugUIState.Activate();
                ShowMyUI();
            }
        }

        public override void PostSetupContent()
        {
            if (!Main.dedServ)
            {
                Type[] assemblyTypes = typeof(HunterCombatMR).Assembly.GetTypes();

                foreach (Type type in assemblyTypes.Where(x => x.IsSubclassOf(typeof(AttackProjectile)) && !x.IsAbstract))
                {
                    type.GetMethod("Initialize").Invoke(GetProjectile(type.Name), null);
                }
            }
        }

        public override void Unload()
        {
            LoadedAttacks = null;
            AnimationKeyFrameManager = null;
            DebugUIState = null;
            ModName = null;
            HighlightedLayers = null;
            SelectedLayer = null;
            SelectedLayerNudgeAmount = null;
            NudgeCooldown = null;
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