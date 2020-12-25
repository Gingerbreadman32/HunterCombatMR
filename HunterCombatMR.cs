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
        #region Public Fields

        public const string ModName = "HunterCombat";
        public const int AnimationNameMax = 64;
        public string DataPath = Path.Combine(Program.SavePath, ModName, "Data");

        #endregion Public Fields

        #region Internal Fields

        internal AnimationLoader AnimLoader;
        internal UserInterface EditorUIPanels;
        internal UIEditorPanelState PanelState;
        internal UserInterface EditorUIPopUp;
        internal UIEditorPopUpState PopUpState;
        internal ILog StaticLogger;

        #endregion Internal Fields

        #region Private Fields

        private GameTime _lastUpdateUiGameTime;

        #endregion Private Fields

        #region Public Constructors

        public HunterCombatMR()
        {
            Instance = this;
        }

        #endregion Public Constructors

        #region Public Properties

        public static HunterCombatMR Instance { get; set; }
        public KeyFrameManager AnimationKeyFrameManager { get; private set; }
        public AnimationEditor EditorInstance { get; private set; }
        public AnimationFileManager FileManager { get; private set; }
        public List<AnimationEngine.Models.Animation> LoadedAnimations { get; private set; }
        public List<Attack> LoadedAttacks { get; private set; }

        #endregion Public Properties

        #region Public Methods

        public override void Load()
        {
            StaticLogger = Logger;
            AnimationKeyFrameManager = new KeyFrameManager();
            FileManager = new AnimationFileManager();
            LoadedAttacks = new List<Attack>();
            LoadedAnimations = new List<AnimationEngine.Models.Animation>();
            AnimLoader = new AnimationLoader();

            if (!Main.dedServ)
            {
                var animTypes = new List<AnimationType>() { AnimationType.Player };
                FileManager.SetupCustomFolders(animTypes);
                EditorInstance = new AnimationEditor();

                EditorUIPanels = new UserInterface();
                PanelState = new UIEditorPanelState();
                PanelState.Activate();
                EditorUIPopUp = new UserInterface();
                PopUpState = new UIEditorPopUpState();
                PopUpState.Activate();
                ShowMyUI();
            }
        }

        public AnimationEngine.Models.Animation GetLoadedAnimation(string name)
            => LoadedAnimations.FirstOrDefault(x => x.Name.Equals(name));

        public bool DoesLoadedAnimationExist(string name)
            => LoadedAnimations.Any(x => x.Name.Equals(name));


        public bool LoadAnimationFile(AnimationType animationType,
            string fileName,
            bool overrideInternal = false)
        {
            if (LoadedAnimations != null)
            {
                var animation = FileManager.LoadAnimation(animationType, fileName, overrideInternal);

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

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "Hunter Combat Editor: Pop-Ups",
                    delegate
                    {
                        if (_lastUpdateUiGameTime != null && EditorUIPopUp?.CurrentState != null)
                        {
                            EditorUIPopUp.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        return true;
                    },
                       InterfaceScaleType.UI));

                mouseTextIndex--;

                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "Hunter Combat Editor: Panels",
                    delegate
                    {
                        if (_lastUpdateUiGameTime != null && EditorUIPanels?.CurrentState != null)
                        {
                            EditorUIPanels.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        return true;
                    },
                       InterfaceScaleType.UI));
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

        public override void PreSaveAndQuit()
        {
            EditorInstance.CloseEditor();

            base.PreSaveAndQuit();
        }

        public override void Unload()
        {
            Instance = null;

            EditorInstance.Dispose();
            EditorInstance = null;
        }

        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUiGameTime = gameTime;
            if (EditorUIPanels?.CurrentState != null)
            {
                EditorUIPanels.Update(gameTime);
            }
            if (EditorUIPopUp?.CurrentState != null)
            {
                PopUpState.UpdateActiveLayers(PanelState?.CurrentAnimationLayers ?? new List<LayerText>());
                EditorUIPopUp.Update(gameTime);
            }
        }

        #endregion Public Methods

        #region Internal Methods

        internal static Texture2D ReadTexture(string file)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(file + ".png");
            if (stream == null)
            {
                throw new ArgumentException("Failed creating IO stream for grabbing new textures.");
            }
            return Texture2D.FromStream(Main.instance.GraphicsDevice, stream);
        }

        internal void HideMyUI()
        {
            EditorUIPanels?.SetState(null);
            EditorUIPopUp?.SetState(null);
        }

        internal void LoadAnimations(IEnumerable<AnimationType> typesToLoad)
        {
            if (LoadedAnimations != null)
            {
                LoadedAnimations.Clear();
                LoadedAnimations.AddRange(AnimLoader.RegisterAnimations(FileManager.LoadAnimations(typesToLoad)));
            }
            else
            {
                LoadedAnimations = new List<AnimationEngine.Models.Animation>();
            }
        }

        internal string DuplicateAnimation(AnimationEngine.Models.Animation duplicate)
        {
            var newAnim = duplicate.Duplicate(DuplicateName(duplicate.Name, 0));
            LoadedAnimations.Add(newAnim);

            if (duplicate == null)
            {
                throw new ArgumentNullException("No animation to duplicate!");
            }

            return newAnim.Name;
        }

        internal void DeleteAnimation(AnimationEngine.Models.Animation animation)
        {
            if (DoesLoadedAnimationExist(animation.Name))
            {
                LoadedAnimations.Remove(GetLoadedAnimation(animation.Name));
            }

            FileManager.DeleteCustomAnimation(animation.AnimationType, animation.Name);
        }

        internal void SetUIPlayer(HunterCombatPlayer player)
        {
            if (PanelState != null)
                PanelState.Player = player;
        }

        internal void ShowMyUI()
        {
            EditorUIPanels?.SetState(PanelState);
            EditorUIPopUp?.SetState(PopUpState);
        }

        #endregion Internal Methods

        #region Private Methods

        private void LoadInternalAnimations(Type[] types)
        {
            foreach (Type type in types.Where(x => x.IsSubclassOf(typeof(ActionContainer)) && !x.IsAbstract))
            {
                var container = (ActionContainer)type.GetConstructor(new Type[] { }).Invoke(new object[] { });
                container.Load();
                AnimLoader.LoadContainer(container);
            }

            if (AnimLoader.Containers.Any())
                LoadedAnimations = new List<AnimationEngine.Models.Animation>(AnimLoader.RegisterAnimations());
        }

        private string DuplicateName(string name,
            int counter)
        {
            counter++;

            // REMINDER: Put a terminator on these recursive functions to prevent stack overflow issues.

            if (LoadedAnimations.Any(x => x.Name.Equals(DuplicateNameFormat(name, counter))))
            {
                return DuplicateName(name, counter);
            }
            else
            {
                return DuplicateNameFormat(name, counter);
            }
        }

        private string DuplicateNameFormat(string name,
            int suffix)
        {
            string newName = string.Format(name + "{0}", suffix);

            if (newName.Count() > AnimationNameMax)
            {
                return DuplicateNameFormat(name.Substring(0, name.Count() - 1), suffix);
            } else
            {
                return newName;
            }
        }

        #endregion Private Methods
    }
}