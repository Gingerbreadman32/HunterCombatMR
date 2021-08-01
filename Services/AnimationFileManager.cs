using HunterCombatMR.Enumerations;
using HunterCombatMR.Interfaces;
using HunterCombatMR.Interfaces.Animation;
using HunterCombatMR.Models;
using HunterCombatMR.Models.Animation;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Terraria;

namespace HunterCombatMR.Services
{
    public sealed class AnimationFileManager
    {
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

        public AnimationFileManager()
        {
            var converters = typeof(AnimationFileManager).Assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(JsonConverter)) && !x.IsAbstract);

            foreach (var converter in converters.Where(x => !x.IsGenericType))
            {
                _serializerSettings.Converters.Add((JsonConverter)Activator.CreateInstance(converter));
            }
        }

        public CustomAnimationFileExistStatus CustomAnimationFileExists(string name,
            AnimationType type)
        {
            if (!Directory.Exists(_customFilePath))
            {
                return CustomAnimationFileExistStatus.BaseDirectoryMissing;
            }

            if (!Directory.Exists(Path.Combine(_customFilePath, type.ToString())))
            {
                HunterCombatMR.Instance.Logger.Warn($"No custom animation directory for animation type: {type.ToString()}");
                return CustomAnimationFileExistStatus.TypeDirectoryMissing;
            }

            if (!File.Exists(CustomAnimationPath(name, type)))
            {
                HunterCombatMR.Instance.Logger.Warn($"Custom animation {name} does not exist!");
                return CustomAnimationFileExistStatus.FileMissing;
            }

            return CustomAnimationFileExistStatus.FileExists;
        }

        public CustomAnimationFileExistStatus CustomAnimationFileExists(ICustomAnimation animation)
            => CustomAnimationFileExists(animation.DisplayName, animation.AnimationType);

        public PlayerAnimation LoadAnimation(AnimationType type,
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
                    HunterCombatMR.Instance.Logger.Warn($"{InternalAnimationManifest(fileName, type)} does not exist!");
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

            PlayerAnimation action = null;

            if (!string.IsNullOrEmpty(json))
                action = JsonConvert.DeserializeObject<PlayerAnimation>(json, _serializerSettings);

            if (action == null)
            {
                HunterCombatMR.Instance.Logger.Error($"{fileName} is not a valid animation {_fileType} file!");
            }

            return action;
        }

        public IEnumerable<ICustomAnimationV2> LoadAnimations()
        {
            var actions = new List<ICustomAnimationV2>();

            var path = Path.Combine(_customFilePath, "Player", "V2");
            if (!Directory.Exists(_customFilePath) || !Directory.Exists(path))
            {
                HunterCombatMR.Instance.Logger.Warn($"No custom animation directory for animation type: Player");
                return actions;
            }

            actions = LoadAnimationsExternal(path);

            return actions;
        }

        public IEnumerable<ICustomAnimation> LoadLegacyAnimations(AnimationType animType, Type type)
        {
            var actions = new List<ICustomAnimation>();
            var assembly = Assembly.GetExecutingAssembly();
            var manifestStream = assembly.GetManifestResourceNames();

            if (manifestStream == null)
            {
                HunterCombatMR.Instance.Logger.Warn($"No animations found internally!");
            }

            var manifestFolder = _manifestPath + "." + animType.ToString();

            var typeStream = manifestStream.Where(x => x.StartsWith(manifestFolder));

            if (!typeStream.Any())
            {
                HunterCombatMR.Instance.Logger.Warn($"No animations of type {animType.ToString()} exist internally!");
            }

            foreach (var resource in typeStream)
            {
                LoadAnimationsInternal(actions, assembly, resource, type);
            }

            /*
            var path = Path.Combine(_customFilePath, animType.ToString());
            if (!Directory.Exists(_customFilePath) || !Directory.Exists(path))
            {
                HunterCombatMR.Instance.Logger.Warn($"No custom animation directory for animation type: {animType.ToString()}");
                return actions;
            }

            LoadAnimationsExternal(actions, path, type);
            */

            return actions;
        }

        public FileSaveStatus SaveAnimation(ICustomAnimationV2 anim,
            bool overwrite)
        {
            var animPath = Path.Combine(_customFilePath, anim.AnimationType.ToString(), "V2", anim.InternalName + _fileType);

            if (!overwrite & File.Exists(animPath))
            {
                return FileSaveStatus.FileExists;
            }

            try
            {
                var serialized = JsonConvert.SerializeObject(anim, _serializerSettings);
                File.WriteAllText(animPath, serialized);
                File.SetAttributes(animPath, FileAttributes.Normal);
                return FileSaveStatus.Saved;
            }
            catch (Exception ex)
            {
                Main.NewText($"Error: Failed to save animation {anim.InternalName}! Check log for stacktrace.", Color.Red);
                HunterCombatMR.Instance.Logger.Error(ex.Message, ex);
                return FileSaveStatus.Error;
            }
        }

        public FileSaveStatus SaveCustomAnimation(PlayerAnimation anim,
                    string newName = null,
                    bool overwrite = false)
                    => SavePlayerAnimation(anim, newName, overwrite, false);

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

        internal void DeleteCustomAnimation(AnimationType type,
            string fileName)
        {
            if (CustomAnimationFileExists(fileName, type).Equals(CustomAnimationFileExistStatus.FileExists))
                File.Delete(CustomAnimationPath(fileName, type));
        }

        /// <summary>
        /// Temporary method to save internal animations, remove/obfuscate on release.
        /// </summary>
        /// <returns>The save status</returns>
        internal FileSaveStatus SaveInternalAnimation(PlayerAnimation anim,
            string newName = null,
            bool overwrite = false)
            => SavePlayerAnimation(anim, newName, overwrite, true);

        private string CustomAnimationPath(string name, 
            AnimationType type)
                => Path.Combine(_customFilePath, type.ToString(), name + _fileType);

        private string InternalAnimationManifest(string name,
            AnimationType type)
                => $"{_manifestPath}.{type.ToString()}.{name + _fileType}";

        private string InternalAnimationPath(string name,
            AnimationType type)
                => Path.Combine(_internalFilePath, type.ToString(), name + _fileType);

        private List<ICustomAnimationV2> LoadAnimationsExternal(string path)
        {
            var files = Directory.GetFiles(path);
            var actions = new List<ICustomAnimationV2>();

            foreach (var file in files.Where(x => x.Contains(_fileType)))
            {
                string json = File.ReadAllText(file);
                var action = JsonConvert.DeserializeObject(json, typeof(CustomAnimationV2), _serializerSettings) as ICustomAnimationV2;
                if (action != null && !actions.Any(x => x.InternalName.Equals(action.InternalName)))
                {
                    actions.Add(action);
                    continue;
                }
                HunterCombatMR.Instance.Logger.Error($"{file} is not a valid animation {_fileType} file or an animation already exists with that name!");
            }

            return actions;
        }

        private void LoadAnimationsInternal(List<ICustomAnimation> actions, Assembly assembly, string resource, Type animType)
        {
            using (Stream stream = assembly.GetManifestResourceStream(resource))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string json = reader.ReadToEnd();
                    ICustomAnimation action = JsonConvert.DeserializeObject(json, animType, _serializerSettings) as ICustomAnimation;
                    if (action != null)
                        actions.Add(action);
                    else
                        HunterCombatMR.Instance.Logger.Error($"{resource} is not a valid animation {_fileType} file!");
                }
            }
        }

        private void LoadLegacyAnimationsExternal(List<ICustomAnimation> actions, string path, Type type)
        {
            var files = Directory.GetFiles(path);

            foreach (var file in files.Where(x => x.Contains(_fileType)))
            {
                string json = File.ReadAllText(file);
                var action = JsonConvert.DeserializeObject(json, type, _serializerSettings) as ICustomAnimation;
                if (action != null && !actions.Any(x => x.DisplayName.Equals(action.DisplayName)))
                    actions.Add(action);
                else
                    HunterCombatMR.Instance.Logger.Error($"{file} is not a valid animation {_fileType} file!");
            }
        }

        private FileSaveStatus SavePlayerAnimation(PlayerAnimation anim,
                            string newName,
            bool overwrite,
            bool internalSave)
        {
            bool rename = !string.IsNullOrEmpty(newName);
            FileSaveStatus status;
            PlayerAnimation action = rename ? new PlayerAnimation(newName, anim.LayerData, anim.IsStoredInternally) : anim;
            var animPath = internalSave ? InternalAnimationPath(action.DisplayName, action.AnimationType) : CustomAnimationPath(action.DisplayName, action.AnimationType);
            var oldPath = internalSave ? InternalAnimationPath(anim.DisplayName, anim.AnimationType) : CustomAnimationPath(anim.DisplayName, anim.AnimationType);

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
                Main.NewText($"Error: Failed to save animation {action.DisplayName}! Check log for stacktrace.", Color.Red);
                HunterCombatMR.Instance.Logger.Error(ex.Message, ex);
            }

            return status;
        }
    }
}