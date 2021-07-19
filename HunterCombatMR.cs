using HunterCombatMR.Constants;
using HunterCombatMR.Enumerations;
using HunterCombatMR.Interfaces.System;
using HunterCombatMR.Models.Systems;
using HunterCombatMR.Services;
using HunterCombatMR.UI;
using HunterCombatMR.UI.Elements;
using HunterCombatMR.Utilities;
using log4net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
            SystemManager.Initialize(LoadSystems());
            FileManager = new AnimationFileManager();
            Content = new ContentService(FileManager);
            GameCommandCache.Initialize();

            if (!Main.dedServ)
            {
                VariableTextures = new Dictionary<string, Texture2D>();
                FileManager.SetupCustomFolders(new List<AnimationType>() { AnimationType.Player });
                EditorInstance = new AnimationEditor();

                _editorUIPanels = new UserInterface();
                _panelState = new EditorUI();
                _editorUIPopUp = new UserInterface();
                _popUpState = new DebugUI();
            }

            Content.SetupContent();
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

        public override void PostUpdateInput()
        {
            SystemManager.PostInputUpdate();
        }

        public override void PreSaveAndQuit()
        {
            EditorInstance.CloseEditor();

            base.PreSaveAndQuit();
        }

        public override void Unload()
        {
            GameCommandCache.Uninitialize();
            EditorInstance?.Dispose();
            EditorInstance = null;

            SystemManager.Dispose();

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

        private static IEnumerable<IModSystem> LoadSystems()
        {
            var systemsList = new List<IModSystem>();

            systemsList.Add(new InputSystem());
            systemsList.Add(new EntityStateSystem());

            return systemsList;
        }
    }
}