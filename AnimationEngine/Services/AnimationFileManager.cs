using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.Converters;
using HunterCombatMR.Enumerations;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Terraria;

namespace HunterCombatMR.AnimationEngine.Services
{
    public sealed class AnimationFileManager
    {
        public static string FilePath = Path.Combine(HunterCombatMR.Instance.DataPath, "Animations");
        public static string FileType = ".json";

        public static JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
            Formatting = Formatting.Indented
        };

        public AnimationFileManager()
        {
            serializerSettings.Converters.Add(new KeyFrameProfileConverter());
            serializerSettings.Converters.Add(new RectangleConverter());
        }

        public static string AnimationPath(string name,
            AnimationType type)
                => Path.Combine(FilePath, type.ToString(), name + FileType);

        public bool SetupFolders(IEnumerable<AnimationType> types)
        {
            if (!Directory.GetParent(FilePath).Exists)
            {
                Directory.GetParent(FilePath).Create();
                if (!Directory.Exists(FilePath))
                {
                    Directory.CreateDirectory(FilePath);
                }
            }

            foreach (var animType in types)
            {
                var directory = Path.Combine(FilePath, animType.ToString());
                if (Directory.GetParent(directory).Exists && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }

            return true;
        }

        public FileSaveStatus SaveAnimation(PlayerActionAnimation anim,
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
                HunterCombatMR.Instance.StaticLogger.Error(ex.Message);
                HunterCombatMR.Instance.StaticLogger.Error(ex.StackTrace);
            }

            return status;
        }

        public FileSaveStatus SaveAnimationNewName(PlayerActionAnimation anim,
            string newName,
            bool overwrite = false)
        {
            FileSaveStatus status;
            PlayerActionAnimation renamedAction = new PlayerActionAnimation(newName, anim.LayerData);

            status = SaveAnimation(renamedAction, overwrite);

            if (!status.Equals(FileSaveStatus.Saved))
                return status;

            if (File.Exists(AnimationPath(anim.Name, anim.AnimationType)))
                File.Delete(AnimationPath(anim.Name, anim.AnimationType));

            return status;
        }

        public IEnumerable<PlayerActionAnimation> LoadAnimations(IEnumerable<AnimationType> types)
        {
            var actions = new List<PlayerActionAnimation>();

            foreach (var animType in types)
            {
                var path = Path.Combine(FilePath, animType.ToString());
                if (!Directory.Exists(FilePath) && !Directory.Exists(path))
                {
                    HunterCombatMR.Instance.StaticLogger.Warn($"No directory for animation type: {animType.ToString()}");
                    return actions;
                }

                var files = Directory.GetFiles(path);

                foreach (var file in files.Where(x => x.Contains(FileType)))
                {
                    string json = File.ReadAllText(file);
                    var action = JsonConvert.DeserializeObject<PlayerActionAnimation>(json, serializerSettings);
                    if (action != null)
                        actions.Add(action);
                    else
                        HunterCombatMR.Instance.StaticLogger.Error($"{file} is not a valid animation {FileType} file!");
                }
            }

            return actions;
        }

        public PlayerActionAnimation LoadAnimation(AnimationType type,
            string fileName)
        {
            var path = Path.Combine(FilePath, type.ToString());
            if (!Directory.Exists(FilePath) && !Directory.Exists(path))
            {
                HunterCombatMR.Instance.StaticLogger.Warn($"No directory for animation type: {type.ToString()}");
                return null;
            }

            var file = Path.Combine(path, fileName + FileType);

            string json = File.ReadAllText(file);
            var action = JsonConvert.DeserializeObject<PlayerActionAnimation>(json, serializerSettings);
            if (action != null)
            {
                return action;
            }
            else
            {
                HunterCombatMR.Instance.StaticLogger.Error($"{file} is not a valid animation {FileType} file!");
                return null;
            }
        }
    }
}