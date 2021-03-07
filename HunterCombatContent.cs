using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.AnimationEngine.Services;
using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HunterCombatMR
{
    public sealed class HunterCombatContent
    {
        private IDictionary<Type, IEnumerable<HunterCombatContentInstance>> _contentStream;
        private AnimationFileManager _fileManager;
        private AnimationLoader _animationLoader;
        private const int _animationNameMax = 64;

        public bool CheckContentInstanceByName<T>(string name) where T : HunterCombatContentInstance
            => _contentStream[typeof(T)].Any(x => x.InternalName.Equals(name));

        public T GetContentInstance<T>(string name) where T : HunterCombatContentInstance
            => (T)_contentStream[typeof(T)].FirstOrDefault(x => x.InternalName.Equals(name));

        internal void LoadAnimations(IEnumerable<AnimationType> typesToLoad)
        {
            if (_contentStream.ContainsKey(typeof(Animation)))
                _contentStream[typeof(Animation)] = _animationLoader.RegisterAnimations(_fileManager.LoadAnimations(typesToLoad));
            else
                _contentStream.Add(typeof(Animation), _animationLoader.RegisterAnimations(_fileManager.LoadAnimations(typesToLoad)));
        }

        public HunterCombatContent(AnimationFileManager fileManager)
        {
            _contentStream = new Dictionary<Type, IEnumerable<HunterCombatContentInstance>>();
            _animationLoader = new AnimationLoader();
            _fileManager = fileManager;
        }

        public void SetupContent(Type[] assemblyTypes)
        {
            var animTypes = new List<AnimationType>() { AnimationType.Player };
            LoadAnimations(animTypes);
            LoadAttacks(assemblyTypes);
        }

        internal void LoadAttacks(Type[] assemblyTypes)
        {
            var LoadedAttacks = new List<Attack>();
            foreach (Type type in assemblyTypes.Where(x => x.IsSubclassOf(typeof(Attack)) && !x.IsAbstract))
            {
                LoadedAttacks.Add((Attack)type.GetConstructor(new Type[] { typeof(string) }).Invoke(new object[] { type.Name }));
            }

            if (_contentStream.ContainsKey(typeof(Attack)))
                _contentStream[typeof(Attack)] = LoadedAttacks;
            else
                _contentStream.Add(typeof(Attack), LoadedAttacks);
        }

        public bool LoadAnimationFile(AnimationType animationType,
            string fileName,
            bool overrideInternal = false)
        {
            if (_contentStream.ContainsKey(typeof(Animation)))
            {
                var animation = _fileManager.LoadAnimation(animationType, fileName, overrideInternal);
                var stream = new List<HunterCombatContentInstance>(_contentStream[typeof(Animation)]);

                if (animation == null)
                {
                    HunterCombatMR.Instance.StaticLogger.Error($"Animation {fileName} failed to load!");
                    return false;
                }

                if (stream.Any(x => x.InternalName.Equals(fileName)))
                    stream.Remove(stream.First(x => x.InternalName.Equals(fileName)));

                stream.Add(_animationLoader.RegisterAnimation(animation));

                _contentStream[typeof(Animation)] = stream;

                return true;
            }
            else
            {
                throw new Exception("Animation List Not Loaded!");
            }
        }
            internal string DuplicateName<T>(string name,
                int counter) where T : HunterCombatContentInstance
            {
            if (!_contentStream.ContainsKey(typeof(T)))
                return name;

                counter++;

            if (counter >= 200)
                throw new StackOverflowException("Over the max amount of duplicate iterations!");

                // REMINDER: Put a terminator on these recursive functions to prevent stack overflow issues.

                if (_contentStream[typeof(T)].Any(x => x.InternalName.Equals(DuplicateNameFormat(name, counter))))
                {
                    return DuplicateName<T>(name, counter);
                }
                else
                {
                    return DuplicateNameFormat(name, counter);
                }
            }

            private string DuplicateNameFormat(string name,
                int suffix)
            {
                string newName = string.Format(name + "{0}", suffix);

                if (newName.Count() > _animationNameMax)
                {
                    return DuplicateNameFormat(name.Substring(0, name.Count() - 1), suffix);
                }
                else
                {
                    return newName;
                }
            }

            private void LoadInternalAnimations(Type[] types)
            {
                foreach (Type type in types.Where(x => x.IsSubclassOf(typeof(ActionContainer)) && !x.IsAbstract))
                {
                    var container = (ActionContainer)type.GetConstructor(new Type[] { }).Invoke(new object[] { });
                    container.Load();
                    _animationLoader.LoadContainer(container);
                }

                if (_animationLoader.Containers.Any())
                    LoadedAnimations = new List<AnimationEngine.Models.Animation>(_animationLoader.RegisterAnimations());
            }

        internal void DeleteContentInstance<T>(T instance) where T : HunterCombatContentInstance
        {
            if (!_contentStream.ContainsKey(typeof(T)))
                return;

            if (CheckContentInstanceByName<T>(instance.InternalName))
            {
                var stream = new List<HunterCombatContentInstance>(_contentStream[typeof(T)]);
                stream.Remove(GetContentInstance<T>(instance.InternalName));
                _contentStream[typeof(T)] = stream;
            }
        }

        internal string DuplicateContentInstance<T>(T duplicate) where T : HunterCombatContentInstance
        {
            if (!_contentStream.ContainsKey(typeof(T)))
                throw new Exception($"Content stream not initialized for {typeof(T).Name}");

            if (duplicate == null)
                throw new ArgumentNullException("No animation to duplicate!");

            var newInstance = duplicate.Duplicate<T>(DuplicateName<T>(duplicate.InternalName, 0));
            var stream = new List<HunterCombatContentInstance>(_contentStream[typeof(T)]);
            stream.Add(newInstance);
            _contentStream[typeof(T)] = stream;
            return newInstance.InternalName;
        }

    }
}
