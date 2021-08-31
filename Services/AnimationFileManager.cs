using HunterCombatMR.Enumerations;
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

        public FileStatus CustomAnimationFileExists(string name,
            AnimationType type)
        {
            if (!Directory.Exists(_customFilePath))
            {
                return FileStatus.BaseDirectoryMissing;
            }

            if (!Directory.Exists(Path.Combine(_customFilePath, type.ToString())))
            {
                HunterCombatMR.Instance.Logger.Warn($"No custom animation directory for animation type: {type.ToString()}");
                return FileStatus.TypeDirectoryMissing;
            }

            if (!File.Exists(CustomAnimationPath(name, type)))
            {
                HunterCombatMR.Instance.Logger.Warn($"Custom animation {name} does not exist!");
                return FileStatus.FileMissing;
            }

            return FileStatus.FileExists;
        }

        public FileStatus CustomAnimationFileExists(CustomAnimation animation)
            => CustomAnimationFileExists(animation.Name, animation.Category);

        public CustomAnimation? LoadAnimation(AnimationType type,
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
                if (CustomAnimationFileExists(fileName, type).Equals(FileStatus.FileExists))
                    json = File.ReadAllText(CustomAnimationPath(fileName, type));
                else
                    return null;
            }

            if (string.IsNullOrEmpty(json))
                throw new Exception("File is invalid or empty!");

            return JsonConvert.DeserializeObject<CustomAnimation>(json, _serializerSettings); ;
        }

        public IEnumerable<CustomAnimation> LoadAnimations()
        {
            var actions = new List<CustomAnimation>();

            var path = Path.Combine(_customFilePath);
            if (!Directory.Exists(_customFilePath) || !Directory.Exists(path))
            {
                HunterCombatMR.Instance.Logger.Warn($"No custom animation directory for animation type: Player");
                return actions;
            }

            actions = LoadAnimationsExternal(path);

            return actions;
        }

        public FileSaveStatus SaveAnimation(CustomAnimation anim,
            bool overwrite)
        {
            var animPath = Path.Combine(_customFilePath, anim.Name + _fileType);

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
                Main.NewText($"Error: Failed to save animation {anim.Name}! Check log for stacktrace.", Color.Red);
                HunterCombatMR.Instance.Logger.Error(ex.Message, ex);
                return FileSaveStatus.Error;
            }
        }

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
            if (CustomAnimationFileExists(fileName, type).Equals(FileStatus.FileExists))
                File.Delete(CustomAnimationPath(fileName, type));
        }

        private string CustomAnimationPath(string name,
            AnimationType type)
                => Path.Combine(_customFilePath, type.ToString(), name + _fileType);

        private string InternalAnimationManifest(string name,
            AnimationType type)
                => $"{_manifestPath}.{type.ToString()}.{name + _fileType}";

        private string InternalAnimationPath(string name,
            AnimationType type)
                => Path.Combine(_internalFilePath, type.ToString(), name + _fileType);

        private List<CustomAnimation> LoadAnimationsExternal(string path)
        {
            var files = Directory.GetFiles(path);
            var actions = new List<CustomAnimation>();

            foreach (var file in files.Where(x => x.Contains(_fileType)))
            {
                string json = File.ReadAllText(file);
                var action = (CustomAnimation)JsonConvert.DeserializeObject(json, typeof(CustomAnimation), _serializerSettings);
                if (!actions.Any(x => x.Name.Equals(action.Name)))
                {
                    actions.Add(action);
                    continue;
                }
                HunterCombatMR.Instance.Logger.Error($"{file} is not a valid animation {_fileType} file or an animation already exists with that name!");
            }

            return actions;
        }
    }
}