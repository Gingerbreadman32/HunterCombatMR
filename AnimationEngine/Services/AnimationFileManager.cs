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
        #region Public Fields

        public static string CustomFilePath = Path.Combine(HunterCombatMR.Instance.DataPath, "Animations");
        public static string ManifestPath = "HunterCombatMR.Animations";
        public static string FileType = ".json";

        public static JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
            Formatting = Formatting.Indented
        };

        #endregion Public Fields

        #region Public Constructors

        public AnimationFileManager()
        {
            serializerSettings.Converters.Add(new KeyFrameProfileConverter());
            serializerSettings.Converters.Add(new RectangleConverter());
        }

        #endregion Public Constructors

        #region Public Methods

        public static string AnimationPath(string name,
            AnimationType type)
                => Path.Combine(CustomFilePath, type.ToString(), name + FileType);

        public CustomAnimationFileExistStatus CustomAnimationFileExists(string name,
            AnimationType type)
        {
            if (!Directory.Exists(CustomFilePath))
            {
                return CustomAnimationFileExistStatus.BaseDirectoryMissing;
            }

            if (!Directory.Exists(Path.Combine(CustomFilePath, type.ToString())))
            {
                HunterCombatMR.Instance.StaticLogger.Warn($"No custom animation directory for animation type: {type.ToString()}");
                return CustomAnimationFileExistStatus.TypeDirectoryMissing;
            }

            if (!File.Exists(AnimationPath(name, type)))
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
                var manifestFolder = Path.Combine(ManifestPath, type.ToString());
                var manifestFile = Path.Combine(manifestFolder, fileName + FileType);

                var manifestStream = assembly.GetManifestResourceStream(manifestFile);

                if (manifestStream == null)
                {
                    HunterCombatMR.Instance.StaticLogger.Warn($"{manifestFile} does not exist!");
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
                    json = File.ReadAllText(AnimationPath(fileName, type));
                else
                    return null;
            }

            PlayerActionAnimation action = null;

            if (!string.IsNullOrEmpty(json))
                action = JsonConvert.DeserializeObject<PlayerActionAnimation>(json, serializerSettings);

            if (action == null)
            {
                HunterCombatMR.Instance.StaticLogger.Error($"{fileName} is not a valid animation {FileType} file!");
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
                var manifestFolder = ManifestPath + "." + animType.ToString();

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
                            var action = JsonConvert.DeserializeObject<PlayerActionAnimation>(json, serializerSettings);
                            if (action != null)
                                actions.Add(action);
                            else
                                HunterCombatMR.Instance.StaticLogger.Error($"{resource} is not a valid animation {FileType} file!");
                        }
                    }
                }

                var path = Path.Combine(CustomFilePath, animType.ToString());
                if (!Directory.Exists(CustomFilePath) || !Directory.Exists(path))
                {
                    HunterCombatMR.Instance.StaticLogger.Warn($"No custom animation directory for animation type: {animType.ToString()}");
                    return actions;
                }

                var files = Directory.GetFiles(path);

                foreach (var file in files.Where(x => x.Contains(FileType)))
                {
                    string json = File.ReadAllText(file);
                    var action = JsonConvert.DeserializeObject<PlayerActionAnimation>(json, serializerSettings);
                    if (action != null && !actions.Any(x => x.Name.Equals(action.Name)))
                        actions.Add(action);
                    else
                        HunterCombatMR.Instance.StaticLogger.Error($"{file} is not a valid animation {FileType} file!");
                }
            }

            return actions;
        }

        public FileSaveStatus SaveCustomAnimation(PlayerActionAnimation anim,
            bool overwrite = false)
        {
            FileSaveStatus status;
            var animPath = AnimationPath(anim.Name, anim.AnimationType);

            if (!overwrite & File.Exists(animPath))
            {
                return FileSaveStatus.FileExists;
            }
            try
            {
                var serialized = JsonConvert.SerializeObject(anim, serializerSettings);
                File.WriteAllText(animPath, serialized);
                File.SetAttributes(animPath, FileAttributes.Normal);
                status = FileSaveStatus.Saved;
            }
            catch (Exception ex)
            {
                status = FileSaveStatus.Error;
                Main.NewText($"Error: Failed to save animation {anim.Name}! Check log for stacktrace.", Color.Red);
                HunterCombatMR.Instance.StaticLogger.Error(ex.Message, ex);
            }

            return status;
        }

        public FileSaveStatus SaveCustomAnimationNewName(PlayerActionAnimation anim,
            string newName,
            bool overwrite = false)
        {
            FileSaveStatus status;
            PlayerActionAnimation renamedAction = new PlayerActionAnimation(newName, anim.LayerData);

            status = SaveCustomAnimation(renamedAction, overwrite);

            if (!status.Equals(FileSaveStatus.Saved))
                return status;

            if (File.Exists(AnimationPath(anim.Name, anim.AnimationType)))
                File.Delete(AnimationPath(anim.Name, anim.AnimationType));

            return status;
        }

        public bool SetupFolders(IEnumerable<AnimationType> types)
        {
            if (!Directory.GetParent(CustomFilePath).Exists)
            {
                Directory.GetParent(CustomFilePath).Create();
                if (!Directory.Exists(CustomFilePath))
                {
                    Directory.CreateDirectory(CustomFilePath);
                }
            }

            foreach (var animType in types)
            {
                var directory = Path.Combine(CustomFilePath, animType.ToString());
                if (Directory.GetParent(directory).Exists && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }

            return true;
        }

        #endregion Public Methods

        #region Internal Methods

        internal void DeleteAnimation(AnimationType type,
            string fileName)
        {
            if (CustomAnimationFileExists(fileName, type).Equals(CustomAnimationFileExistStatus.FileExists))
                File.Delete(AnimationPath(fileName, type));
        }

        #endregion Internal Methods
    }
}