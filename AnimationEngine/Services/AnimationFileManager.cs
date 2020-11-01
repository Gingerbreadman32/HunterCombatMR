using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.Converters;
using HunterCombatMR.Enumerations;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;

namespace HunterCombatMR.AnimationEngine.Services
{
    public sealed class AnimationFileManager
    {
        public static string FilePath = Path.Combine(HunterCombatMR.DataPath, "Animations");
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

        public FileSaveStatus SaveAnimation(LayeredAnimatedAction anim,
            bool overwrite = false)
        {
            FileSaveStatus status;
            var animPath = Path.Combine(FilePath, anim.LayerData.ParentType.ToString(), anim.Name + FileType);

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
                Main.NewText("Error: Failed to save animation! Check log for stacktrace.", Color.Red);
                HunterCombatMR.StaticLogger.Error(ex.Message);
                HunterCombatMR.StaticLogger.Error(ex.StackTrace);
            }

            return status;
        }

        public IEnumerable<LayeredAnimatedAction> LoadAnimations(IEnumerable<AnimationType> types)
        {
            var actions = new List<LayeredAnimatedAction>();

            foreach (var animType in types)
            {
                var path = Path.Combine(FilePath, animType.ToString());
                if (!Directory.Exists(FilePath) && !Directory.Exists(path))
                {
                    HunterCombatMR.StaticLogger.Info($"No directory for animation type: {animType.ToString()}");
                    return actions;
                }

                var files = Directory.GetFiles(path);

                foreach (var file in files.Where(x => x.Contains(FileType)))
                {
                    string json = File.ReadAllText(file);
                    var action = JsonConvert.DeserializeObject<LayeredAnimatedAction>(json, serializerSettings);
                    if (action != null)
                        actions.Add(action);
                    else
                        HunterCombatMR.StaticLogger.Warn($"{file} is not a valid animation {FileType} file!");
                }
            }

            return actions;
        }
    }
}