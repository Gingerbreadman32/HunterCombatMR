using HunterCombatMR.AnimationEngine.Interfaces;
using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.Converters;
using HunterCombatMR.Enumerations;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Terraria;

namespace HunterCombatMR.AnimationEngine.Services
{
    public sealed class AnimationFileManager
    {
        #region Private Fields

        private const string _fileType = ".json";

        private readonly string _customFilePath = Path.Combine(HunterCombatMR.Instance.DataPath, "Animations");
        private readonly string _internalFilePath = Path.Combine(Program.SavePath, "Mod Sources", "HunterCombatMR", "Animations");
        private readonly string _manifestPath = "HunterCombatMR.Animations";

        private JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
            Formatting = Formatting.Indented
        };

        #endregion Private Fields

        #region Public Constructors

        public AnimationFileManager()
        {
            _serializerSettings.Converters.Add(new KeyFrameProfileConverter());
            _serializerSettings.Converters.Add(new RectangleConverter());
        }

        #endregion Public Constructors

        #region Private Methods

        private string CustomAnimationPath(string name,
            AnimationType type)
                => Path.Combine(_customFilePath, type.ToString(), name + _fileType);

        private string InternalAnimationManifest(string name,
            AnimationType type)
                => $"{_manifestPath}.{type.ToString()}.{name + _fileType}";

        private string InternalAnimationPath(string name,
                    AnimationType type)
                => Path.Combine(_internalFilePath, type.ToString(), name + _fileType);

        private void LoadAnimationsExternal(List<IAnimation> actions, string path, Type type)
        {
            var files = Directory.GetFiles(path);

            foreach (var file in files.Where(x => x.Contains(_fileType)))
            {
                string json = File.ReadAllText(file);
                var action = JsonConvert.DeserializeObject(json, type, _serializerSettings) as IAnimation;
                if (action != null && !actions.Any(x => x.Name.Equals(action.Name)))
                    actions.Add(action);
                else
                    HunterCombatMR.Instance.StaticLogger.Error($"{file} is not a valid animation {_fileType} file!");
            }
        }

        private void LoadAnimationsInternal(List<IAnimation> actions, Assembly assembly, string resource, Type animType)
        {
            using (Stream stream = assembly.GetManifestResourceStream(resource))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string json = reader.ReadToEnd();
                    IAnimation action = JsonConvert.DeserializeObject(json, animType, _serializerSettings) as IAnimation;
                    if (action != null)
                        actions.Add(action);
                    else
                        HunterCombatMR.Instance.StaticLogger.Error($"{resource} is not a valid animation {_fileType} file!");
                }
            }
        }

        private FileSaveStatus SaveAnimation(PlayerActionAnimation anim,
                            string newName,
            bool overwrite,
            bool internalSave)
        {
            bool rename = !string.IsNullOrEmpty(newName);
            FileSaveStatus status;
            PlayerActionAnimation action = (rename) ? new PlayerActionAnimation(newName, anim.LayerData, anim.IsInternal) : anim;
            var animPath = (internalSave) ? InternalAnimationPath(action.Name, action.AnimationType) : CustomAnimationPath(action.Name, action.AnimationType);
            var oldPath = (internalSave) ? InternalAnimationPath(anim.Name, anim.AnimationType) : CustomAnimationPath(anim.Name, anim.AnimationType);

            if (!overwrite & File.Exists(animPath))
            {
                return FileSaveStatus.FileExists;
            }
            try
            {
                var serialized = JsonConvert.SerializeObject(action, _serializerSettings);
                File.WriteAllText(animPath, serialized);
                File.SetAttributes(animPath, FileAttributes.Normal);
                status = FileSaveStatus.Saved;

                if (rename && File.Exists(oldPath))
                    File.Delete(oldPath);
            }
            catch (Exception ex)
            {
                status = FileSaveStatus.Error;
                Main.NewText($"Error: Failed to save animation {action.Name}! Check log for stacktrace.", Color.Red);
                HunterCombatMR.Instance.StaticLogger.Error(ex.Message, ex);
            }

            return status;
        }

        #endregion Private Methods

        #region Public Methods

        public CustomAnimationFileExistStatus CustomAnimationFileExists(string name,
            AnimationType type)
        {
            if (!Directory.Exists(_customFilePath))
            {
                return CustomAnimationFileExistStatus.BaseDirectoryMissing;
            }

            if (!Directory.Exists(Path.Combine(_customFilePath, type.ToString())))
            {
                HunterCombatMR.Instance.StaticLogger.Warn($"No custom animation directory for animation type: {type.ToString()}");
                return CustomAnimationFileExistStatus.TypeDirectoryMissing;
            }

            if (!File.Exists(CustomAnimationPath(name, type)))
            {
                HunterCombatMR.Instance.StaticLogger.Warn($"Custom animation {name} does not exist!");
                return CustomAnimationFileExistStatus.FileMissing;
            }
            else
            {
                return CustomAnimationFileExistStatus.FileExists;
            }
        }

        public CustomAnimationFileExistStatus CustomAnimationFileExists<TEntity, TAnimationType>(Animation<TEntity, TAnimationType> animation)
            where TEntity : IAnimatedEntity<TAnimationType>
            where TAnimationType : IAnimated
            => CustomAnimationFileExists(animation.Name, animation.AnimationType);

        public PlayerActionAnimation LoadAnimation(AnimationType type,
            string fileName,
            bool overrideInternal = false)
        {
            bool fileFound = false;
            string json = "";

            if (!overrideInternal)
            {
                var assembly = Assembly.GetExecutingAssembly();

                var manifestStream = assembly.GetManifestResourceStream(InternalAnimationManifest(fileName, type));

                if (manifestStream == null)
                {
                    HunterCombatMR.Instance.StaticLogger.Warn($"{InternalAnimationManifest(fileName, type)} does not exist!");
                    return null;
                }

                fileFound = true;

                using (Stream stream = manifestStream)
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        json = reader.ReadToEnd();
                    }
                }
            }

            if (!fileFound)
            {
                if (CustomAnimationFileExists(fileName, type).Equals(CustomAnimationFileExistStatus.FileExists))
                    json = File.ReadAllText(CustomAnimationPath(fileName, type));
                else
                    return null;
            }

            PlayerActionAnimation action = null;

            if (!string.IsNullOrEmpty(json))
                action = JsonConvert.DeserializeObject<PlayerActionAnimation>(json, _serializerSettings);

            if (action == null)
            {
                HunterCombatMR.Instance.StaticLogger.Error($"{fileName} is not a valid animation {_fileType} file!");
            }

            return action;
        }

        public IEnumerable<IAnimation> LoadAnimations(AnimationType animType, Type type)
        {
            var actions = new List<IAnimation>();
            var assembly = Assembly.GetExecutingAssembly();
            var manifestStream = assembly.GetManifestResourceNames();

            if (manifestStream == null)
            {
                HunterCombatMR.Instance.StaticLogger.Warn($"No animations found internally!");
            }

            var manifestFolder = _manifestPath + "." + animType.ToString();

            var typeStream = manifestStream.Where(x => x.StartsWith(manifestFolder));

            if (!typeStream.Any())
            {
                HunterCombatMR.Instance.StaticLogger.Warn($"No animations of type {animType.ToString()} exist internally!");
            }

            foreach (var resource in typeStream)
            {
                LoadAnimationsInternal(actions, assembly, resource, type);
            }

            var path = Path.Combine(_customFilePath, animType.ToString());
            if (!Directory.Exists(_customFilePath) || !Directory.Exists(path))
            {
                HunterCombatMR.Instance.StaticLogger.Warn($"No custom animation directory for animation type: {animType.ToString()}");
                return actions;
            }

            LoadAnimationsExternal(actions, path, type);

            return actions;
        }

        public FileSaveStatus SaveCustomAnimation(PlayerActionAnimation anim,
                    string newName = null,
                    bool overwrite = false)
                    => SaveAnimation(anim, newName, overwrite, false);

        public bool SetupCustomFolders(IEnumerable<AnimationType> types)
        {
            if (!Directory.GetParent(_customFilePath).Exists)
            {
                Directory.GetParent(_customFilePath).Create();
                if (!Directory.Exists(_customFilePath))
                {
                    Directory.CreateDirectory(_customFilePath);
                }
            }

            foreach (var animType in types)
            {
                var directory = Path.Combine(_customFilePath, animType.ToString());
                if (Directory.GetParent(directory).Exists && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }

            return true;
        }

        /// <summary>
        /// Temporary method to save internal animations, remove/obfuscate on release.
        /// </summary>
        /// <returns>The save status</returns>
        internal FileSaveStatus SaveInternalAnimation(PlayerActionAnimation anim,
            string newName = null,
            bool overwrite = false)
            => SaveAnimation(anim, newName, overwrite, true);

        #endregion Public Methods

        #region Internal Methods

        internal void DeleteCustomAnimation(AnimationType type,
            string fileName)
        {
            if (CustomAnimationFileExists(fileName, type).Equals(CustomAnimationFileExistStatus.FileExists))
                File.Delete(CustomAnimationPath(fileName, type));
        }

        #endregion Internal Methods
    }
}