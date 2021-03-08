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
        public string DataPath = Path.Combine(Program.SavePath, ModName, "Data");

        #endregion Public Fields

        #region Internal Fields

        internal UserInterface EditorUIPanels;
        internal UserInterface EditorUIPopUp;
        internal UIEditorPanelState PanelState;
        internal UIEditorPopUpState PopUpState;
        internal ILog StaticLogger;
        internal IDictionary<string, Texture2D> VariableTextures;

        #endregion Internal Fields

        #region Private Fields

        private const string _variableTexturePath = "Textures/SnS/";
        private GameTime _lastUpdateUiGameTime;

        #endregion Private Fields

        #region Public Constructors

        public HunterCombatMR()
        {
            Instance = this;
        }

        #endregion Public Constructors

        #region Public Properties

        public HunterCombatContent Content { get; private set; }

        public static HunterCombatMR Instance { get; private set; }
        public KeyFrameManager AnimationKeyFrameManager { get; private set; }
        public AnimationEditor EditorInstance { get; private set; }
        public AnimationFileManager FileManager { get; private set; }

        #endregion Public Properties

        #region Public Methods

        public override void Load()
        {
            StaticLogger = Logger;
            AnimationKeyFrameManager = new KeyFrameManager();
            FileManager = new AnimationFileManager();
            Content = new HunterCombatContent(FileManager);

            if (!Main.dedServ)
            {
                VariableTextures = new Dictionary<string, Texture2D>();
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
            Type[] assemblyTypes = typeof(HunterCombatMR).Assembly.GetTypes();
            // Load, register, and store all the animations
            //LoadInternalAnimations(assemblyTypes);
            Content.SetupContent(assemblyTypes);

            foreach (Type type in assemblyTypes.Where(x => x.IsSubclassOf(typeof(AttackProjectile)) && !x.IsAbstract))
            {
                type.GetMethod("Initialize").Invoke(GetProjectile(type.Name), null);
            }

            var modTextures = (IDictionary<string, Texture2D>)typeof(HunterCombatMR).GetField("textures", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Instance);
            VariableTextures = modTextures.Where(x => x.Key.StartsWith(_variableTexturePath)).ToDictionary(x => x.Key, y => y.Value);
            modTextures = null;
            PanelState.PostSetupContent();
        }

        public override void PreSaveAndQuit()
        {
            EditorInstance.CloseEditor();

            base.PreSaveAndQuit();
        }

        public override void Unload()
        {
            EditorInstance.Dispose();
            EditorInstance = null;

            Instance.EditorInstance.Dispose();
            Instance = null;
        }

        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUiGameTime = gameTime;

            if (EditorInstance.CurrentEditMode.Equals(EditorMode.None))
                HideMyUI(EditorUIPanels);
            else
                ShowMyUI(EditorUIPanels, PanelState);

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

        internal void DeleteAnimation(AnimationEngine.Models.Animation animation)
        {
            Content.DeleteContentInstance(animation);

            FileManager.DeleteCustomAnimation(animation.AnimationType, animation.Name);
        }

        internal void HideMyUI()
        {
            EditorUIPanels?.SetState(null);
            EditorUIPopUp?.SetState(null);
        }

        internal void HideMyUI(UserInterface ui)
        {
            if (ui?.CurrentState != null)
                ui?.SetState(null);
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

        internal void ShowMyUI(UserInterface ui,
            UIState state)
        {
            if (ui?.CurrentState != state)
                ui?.SetState(state);
        }

        #endregion Internal Methods

    }
}