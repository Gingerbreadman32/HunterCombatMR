using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.AnimationEngine.Services;
using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR
{
    public sealed class HunterCombatContent
    {
        #region Private Fields

        private const int _animationNameMax = 64;
        private AnimationLoader _animationLoader;
        private IDictionary<Type, IEnumerable<HunterCombatContentInstance>> _contentStream;
        private AnimationFileManager _fileManager;

        #endregion Private Fields

        #region Public Constructors

        public HunterCombatContent(AnimationFileManager fileManager)
        {
            _contentStream = new Dictionary<Type, IEnumerable<HunterCombatContentInstance>>();
            _animationLoader = new AnimationLoader();
            _fileManager = fileManager;
        }

        #endregion Public Constructors

        #region Public Methods

        public bool CheckContentInstanceByName<T>(string name) where T : HunterCombatContentInstance
                    => _contentStream[typeof(T)].Any(x => x.InternalName.Equals(name));

        public T GetContentInstance<T>(string name) where T : HunterCombatContentInstance
            => (T)_contentStream[typeof(T)].FirstOrDefault(x => x.InternalName.Equals(name));

        public T GetContentInstance<T>(T instance) where T : HunterCombatContentInstance
            => (T)_contentStream[typeof(T)].FirstOrDefault(x => x.Equals(instance));

        public IEnumerable<T> GetContentList<T>() where T : HunterCombatContentInstance
            => _contentStream[typeof(T)].Select(x => x as T);

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

        #endregion Public Methods

        #region Internal Methods

        internal void DeleteContentInstance<T>(T instance) where T : HunterCombatContentInstance
        {
            DeleteContentInstance<T>(instance.InternalName);
        }

        internal void DeleteContentInstance<T>(string name) where T : HunterCombatContentInstance
        {
            if (!_contentStream.ContainsKey(typeof(T)))
                return;

            if (CheckContentInstanceByName<T>(name))
            {
                var stream = new List<HunterCombatContentInstance>(_contentStream[typeof(T)]);
                stream.Remove(GetContentInstance<T>(name));
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

        internal string DuplicateName<T>(string name,
                int counter) where T : HunterCombatContentInstance
        {
            if (!_contentStream.ContainsKey(typeof(T)))
                return name;

            counter++;

            if (counter >= 200)
                throw new StackOverflowException("Over the max amount of duplicate iterations!");

            if (_contentStream[typeof(T)].Any(x => x.InternalName.Equals(DuplicateNameFormat(name, counter))))
            {
                return DuplicateName<T>(name, counter);
            }
            else
            {
                return DuplicateNameFormat(name, counter);
            }
        }

        internal void LoadAnimations(IEnumerable<AnimationType> typesToLoad)
        {
            if (_contentStream.ContainsKey(typeof(Animation)))
                _contentStream[typeof(Animation)] = _animationLoader.RegisterAnimations(_fileManager.LoadAnimations(typesToLoad));
            else
                _contentStream.Add(typeof(Animation), _animationLoader.RegisterAnimations(_fileManager.LoadAnimations(typesToLoad)));
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

        internal void SetupContent(Type[] assemblyTypes)
        {
            var animTypes = new List<AnimationType>() { AnimationType.Player };
            LoadAnimations(animTypes);
            LoadAttacks(assemblyTypes);
        }

        #endregion Internal Methods

        #region Private Methods

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

        #endregion Private Methods

        /*
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
        */
    }
}