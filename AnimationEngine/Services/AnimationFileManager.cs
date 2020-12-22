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

        private string InternalAnimationPath(string name,
            AnimationType type)
                => Path.Combine(_internalFilePath, type.ToString(), name + _fileType);

        private string InternalAnimationManifest(string name,
            AnimationType type)
                => $"{_manifestPath}.{type.ToString()}.{name + _fileType}";

        private FileSaveStatus SaveAnimation(PlayerActionAnimation anim,
            string newName,
            bool overwrite,
            bool internalSave)
        {
            bool rename = newName != null;
            FileSaveStatus status;
            PlayerActionAnimation action = (rename) ? anim : new PlayerActionAnimation(newName, anim.LayerData);
            var animPath = (internalSave) ? InternalAnimationPath(anim.Name, anim.AnimationType) : CustomAnimationPath(anim.Name, anim.AnimationType);

            anim.IsInternal = internalSave;

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

                if (rename && File.Exists(animPath))
                    File.Delete(animPath);
            }
            catch (Exception ex)
            {
                status = FileSaveStatus.Error;
                Main.NewText($"Error: Failed to save animation {anim.Name}! Check log for stacktrace.", Color.Red);
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

        public CustomAnimationFileExistStatus CustomAnimationFileExists(AnimationEngine.Models.Animation animation)
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

        public IEnumerable<PlayerActionAnimation> LoadAnimations(IEnumerable<AnimationType> types)
        {
            var actions = new List<PlayerActionAnimation>();
            var assembly = Assembly.GetExecutingAssembly();
            var manifestStream = assembly.GetManifestResourceNames();

            if (manifestStream == null)
            {
                HunterCombatMR.Instance.StaticLogger.Warn($"No animations found internally!");
            }

            foreach (var animType in types)
            {
                var manifestFolder = _manifestPath + "." + animType.ToString();

                var typeStream = manifestStream.Where(x => x.StartsWith(manifestFolder));

                if (!typeStream.Any())
                {
                    HunterCombatMR.Instance.StaticLogger.Warn($"No animations of type {animType.ToString()} exist internally!");
                    continue;
                }

                foreach (var resource in typeStream)
                {
                    using (Stream stream = assembly.GetManifestResourceStream(resource))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string json = reader.ReadToEnd();
                            var action = JsonConvert.DeserializeObject<PlayerActionAnimation>(json, _serializerSettings);
                            if (action != null)
                                actions.Add(action);
                            else
                                HunterCombatMR.Instance.StaticLogger.Error($"{resource} is not a valid animation {_fileType} file!");
                        }
                    }
                }

                var path = Path.Combine(_customFilePath, animType.ToString());
                if (!Directory.Exists(_customFilePath) || !Directory.Exists(path))
                {
                    HunterCombatMR.Instance.StaticLogger.Warn($"No custom animation directory for animation type: {animType.ToString()}");
                    return actions;
                }

                var files = Directory.GetFiles(path);

                foreach (var file in files.Where(x => x.Contains(_fileType)))
                {
                    string json = File.ReadAllText(file);
                    var action = JsonConvert.DeserializeObject<PlayerActionAnimation>(json, _serializerSettings);
                    if (action != null && !actions.Any(x => x.Name.Equals(action.Name)))
                        actions.Add(action);
                    else
                        HunterCombatMR.Instance.StaticLogger.Error($"{file} is not a valid animation {_fileType} file!");
                }
            }

            return actions;
        }

        /// <summary>
        /// Temporary method to save internal animations, remove/obfuscate on release.
        /// </summary>
        /// <returns>The save status</returns>
        internal FileSaveStatus SaveInternalAnimation(PlayerActionAnimation anim,
            string newName = null,
            bool overwrite = false)
            => SaveAnimation(anim, newName, overwrite, true);

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