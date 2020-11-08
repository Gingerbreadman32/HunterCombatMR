using AnimationEngine.Services;
using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.AnimationEngine.Services;
using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.Enumerations;
using HunterCombatMR.UI;
using log4net;
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

        public static List<Attack> LoadedAttacks { get; private set; }

        public static List<LayeredAnimatedAction> LoadedAnimations { get; private set; }

        public static KeyFrameManager AnimationKeyFrameManager { get; private set; }

        public static AnimationFileManager FileManager { get; private set; }

        public static AnimationEditor EditorInstance { get; private set; }

        public static string ModName = "HunterCombat";

        public static string DataPath = Path.Combine(Program.SavePath, ModName, "Data");

        internal static ILog StaticLogger;

        internal static AnimationLoader AnimLoader;

        internal UserInterface DebugUI;
        internal BufferDebugUIState DebugUIState;
        

        private void LoadInternalAnimations(Type[] types)
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
            StaticLogger = Logger;
            AnimationKeyFrameManager = new KeyFrameManager();
            FileManager = new AnimationFileManager();
            LoadedAttacks = new List<Attack>();
            LoadedAnimations = new List<LayeredAnimatedAction>();
            AnimLoader = new AnimationLoader();

            if (!Main.dedServ)
            {
                var animTypes = new List<AnimationType>() { AnimationType.Player };
                FileManager.SetupFolders(animTypes);
                EditorInstance = new AnimationEditor();

                // Debug UI stuff
                DebugUI = new UserInterface();
                DebugUIState = new BufferDebugUIState();
                DebugUIState.Activate();
                ShowMyUI();
            }
        }

        public static void LoadAnimations(IEnumerable<AnimationType> typesToLoad)
        {
            if (LoadedAnimations != null)
            {
                LoadedAnimations.Clear();
                LoadedAnimations.AddRange(AnimLoader.RegisterAnimations(FileManager.LoadAnimations(typesToLoad)));
            } else
            {
                LoadedAnimations = new List<LayeredAnimatedAction>();
            }
        }

        public static bool LoadAnimation(AnimationType animationType,
            string fileName)
        {
            if (LoadedAnimations != null)
            {
                var animation = FileManager.LoadAnimation(animationType, fileName);

                if (animation == null)
                {
                    StaticLogger.Error($"Animation {fileName} failed to load!");
                    return false;
                }

                if (LoadedAnimations.Any(x => x.Name.Equals(fileName)))
                    LoadedAnimations.Remove(LoadedAnimations.First(x => x.Name.Equals(fileName)));

                LoadedAnimations.Add(AnimLoader.RegisterAnimation(animation));

                return true;
            }
            else
            {
                throw new Exception("Animation List Not Loaded!");
            }
        }

        public override void PostSetupContent()
        {
            /* Libvaxy implementation, will use if I need anything else from it
            foreach (Type type in Reflection.GetNonAbstractSubtypes(typeof(ParentType))
                YourCacheList.Add(Reflection.CreateInstance(type));
            */
            Type[] assemblyTypes = typeof(HunterCombatMR).Assembly.GetTypes();

            if (!Main.dedServ)
            {
                // Load, register, and store all the animations
                //LoadInternalAnimations(assemblyTypes);
                var animTypes = new List<AnimationType>() { AnimationType.Player };
                LoadAnimations(animTypes);
            }

            // Loads all of the attack information
            foreach (Type type in assemblyTypes.Where(x => x.IsSubclassOf(typeof(Attack)) && !x.IsAbstract))
            {
                LoadedAttacks.Add((Attack)type.GetConstructor(new Type[] { }).Invoke(new object[] { }));
            }

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
            DataPath = null;
            FileManager = null;
            StaticLogger = null;
            AnimLoader = null;

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