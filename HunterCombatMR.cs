using HunterCombatMR.Attributes;
using HunterCombatMR.Constants;
using HunterCombatMR.Enumerations;
using HunterCombatMR.Managers;
using HunterCombatMR.Models.UI;
using HunterCombatMR.Models.UI.Elements;
using HunterCombatMR.Services;
using HunterCombatMR.Utilities;
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
        public string DataPath = Path.Combine(Program.SavePath, ModConstants.ModName, "Data");

        private UserInterface _editorUIPanels;
        private UserInterface _editorUIPopUp;
        private GameTime _lastUpdateUiGameTime;
        private IEnumerable<ManagerBase> _managers;
        private bool _managersInitialized;
        private bool _managersLoaded;
        private EditorUI _panelState;
        private DebugUI _popUpState;

        public HunterCombatMR()
        {
            Instance = this;
        }

        public static HunterCombatMR Instance { get; private set; }
        public ContentService Content { get; private set; }
        public AnimationEditor EditorInstance { get; private set; }
        public AnimationFileManager FileManager { get; private set; }
        public IDictionary<string, Texture2D> VariableTextures { get; set; }

        public override void Load()
        {
            ManagerSetup();
            FileManager = new AnimationFileManager();
            Content = new ContentService(FileManager);

            if (Main.dedServ)
                return;

            VariableTextures = new Dictionary<string, Texture2D>();
            FileManager.SetupCustomFolders(new List<AnimationType>() { AnimationType.Player });
            EditorInstance = new AnimationEditor();

            _editorUIPanels = new UserInterface();
            _panelState = new EditorUI();
            _editorUIPopUp = new UserInterface();
            _popUpState = new DebugUI();
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
                        if (_lastUpdateUiGameTime != null && _editorUIPopUp?.CurrentState != null)
                        {
                            _editorUIPopUp.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        return true;
                    },
                       InterfaceScaleType.UI));

                mouseTextIndex--;

                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "Hunter Combat Editor: Panels",
                    delegate
                    {
                        if (_lastUpdateUiGameTime != null && _editorUIPanels?.CurrentState != null)
                        {
                            _editorUIPanels.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        return true;
                    },
                       InterfaceScaleType.UI));
            }
        }

        public override void PostSetupContent()
        {
            var modTextures = (IDictionary<string, Texture2D>)typeof(HunterCombatMR).GetField("textures", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Instance);
            VariableTextures = modTextures.Where(x => x.Key.StartsWith(ModConstants.VariableTexturePath)).ToDictionary(x => x.Key, y => y.Value);
            modTextures = null;

            _panelState.Activate();
            _popUpState.Activate();
            _editorUIPanels.SetState(_panelState);
            _editorUIPopUp.SetState(_popUpState);
            _panelState.PostSetupContent();
        }

        public override void PostUpdateEverything()
        {
            if (_managersInitialized)
                SystemManager.PostUpdateEverything();
        }

        public override void PostUpdateInput()
        {
            if (_managersInitialized)
                SystemManager.PostInputUpdate();
        }

        public override void PreSaveAndQuit()
        {
            EditorInstance.CloseEditor();

            base.PreSaveAndQuit();
        }

        public override void PreUpdateEntities()
        {
            if (_managersInitialized)
                SystemManager.PreUpdateEverything();
        }

        public override void Unload()
        {
            DisposeManagers();
            EditorInstance?.Dispose();
            EditorInstance = null;
            Instance.EditorInstance?.Dispose();
            Instance = null;
        }

        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUiGameTime = gameTime;

            if (_editorUIPopUp?.CurrentState != null)
            {
                _popUpState.UpdateActiveLayers(_panelState?.CurrentAnimationLayers ?? new List<LayerText>());
                _editorUIPopUp.Update(gameTime);
            }

            if (EditorInstance.CurrentEditMode.Equals(EditorMode.None))
            {
                _editorUIPanels?.SetState(null);
                return;
            }

            if (_editorUIPanels?.CurrentState == null)
            {
                _editorUIPanels?.SetState(_panelState);
            }

            _editorUIPanels.Update(gameTime);
        }

        private void DisposeManagers()
        {
            if (!_managersLoaded || _managers == null)
                throw new Exception("Managers not loaded or loaded improperly.");

            foreach (var manager in _managers)
            {
                manager.Dispose();
            }

            _managers = null;
        }

        private void InitializeManagers(IEnumerable<ManagerBase> managers)
        {
            var dependents = new List<ManagerBase>();
            int initialized = 0;
            foreach (var manager in managers)
            {
                if (ManagerDependencyCheck(manager))
                {
                    dependents.Add(manager);
                    continue;
                }

                manager.Initialize();
                initialized++;
            }

            if (initialized == 0)
                throw new StackOverflowException("No managers initialized in cycle, one or more manager dependencies are circular!");

            if (dependents.Any())
                InitializeManagers(dependents);
        }

        private bool ManagerDependencyCheck(ManagerBase manager)
        {
            var dependency = manager.GetType().GetCustomAttribute<ManagerDependency>();

            if (dependency is null)
                return false;

            var managerReference = _managers.SingleOrDefault(x => x.GetType() == dependency.Manager);

            if (managerReference is null)
                throw new KeyNotFoundException("No managers loaded that correspond to given type!");

            if (managerReference.Initialized)
                return false;

            return true;
        }

        private void ManagerSetup()
        {
            _managers = ReflectionUtils.InstatiateTypesFromInterface<ManagerBase>();
            _managersLoaded = _managers.Any();
            if (!_managersLoaded)
                throw new Exception("Managers not loaded or loaded improperly.");

            InitializeManagers(_managers);
            _managersInitialized = _managers.All(x => x.Initialized);
        }
    }
}