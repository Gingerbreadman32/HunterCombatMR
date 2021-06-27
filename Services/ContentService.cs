using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.AttackEngine.MoveSets;
using HunterCombatMR.Enumerations;
using HunterCombatMR.Events;
using HunterCombatMR.Interfaces;
using HunterCombatMR.Interfaces.Action;
using HunterCombatMR.Interfaces.Animation;
using HunterCombatMR.Models;
using HunterCombatMR.Seeds.Attacks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Services
{
    public sealed class ContentService
    {
        private const int _animationNameMax = 72;

        private readonly Type[] _animationMap = new Type[] { typeof(PlayerAnimation),
            typeof(ProjectileAnimation) };

        private AnimationLoader _animationLoader;
        private IDictionary<Type, ICollection<IContent>> _contentStream;
        private AnimationFileManager _fileManager;

        public ContentService(AnimationFileManager fileManager)
        {
            _contentStream = new Dictionary<Type, ICollection<IContent>>();
            _animationLoader = new AnimationLoader();
            _fileManager = fileManager;
        }

        public bool CheckContentInstanceByName<T>(string name) where T : IContent
                    => _contentStream[typeof(T)].Any(x => x.InternalName.Equals(name));

        public T GetContentInstance<T>(string name) where T : IContent
            => (T)_contentStream[typeof(T)].FirstOrDefault(x => x.InternalName.Equals(name));

        public IContent GetContentInstance(string name, Type type)
            => _contentStream[type].FirstOrDefault(x => x.InternalName.Equals(name));

        public T GetContentInstance<T>(T instance) where T : IContent
            => (T)_contentStream[typeof(T)].FirstOrDefault(x => x.Equals(instance));

        public IEnumerable<T> GetContentList<T>() where T : IContent
            => _contentStream[typeof(T)].Select(x => (T)x);

        public bool LoadAnimationFile(AnimationType animationType,
            string fileName,
            bool overrideInternal = false)
        {
            if (_contentStream.ContainsKey(_animationMap[(int)animationType]))
            {
                var animation = _fileManager.LoadAnimation(animationType, fileName, overrideInternal);
                var stream = new List<IContent>(_contentStream[_animationMap[(int)animationType]]);

                if (animation == null)
                {
                    HunterCombatMR.Instance.StaticLogger.Error($"Animation {fileName} failed to load!");
                    return false;
                }

                if (stream.Any(x => x.InternalName.Equals(fileName)))
                    stream.Remove(stream.First(x => x.InternalName.Equals(fileName)));

                stream.Add(_animationLoader.RegisterAnimation(animation));

                _contentStream[_animationMap[(int)animationType]] = stream;

                return true;
            }
            else
            {
                throw new Exception("Animation List Not Loaded!");
            }
        }

        internal void DeleteContentInstance<T>(T instance) where T : IContent
        {
            DeleteContentInstance<T>(instance.InternalName);
        }

        internal void DeleteContentInstance<T>(string name) where T : IContent
        {
            if (!_contentStream.ContainsKey(typeof(T)))
                return;

            if (CheckContentInstanceByName<T>(name))
            {
                var stream = new List<IContent>(_contentStream[typeof(T)]);
                stream.Remove(GetContentInstance<T>(name));
                _contentStream[typeof(T)] = stream;
            }
        }

        internal IContent DuplicateContentInstance<T>(T duplicate) where T : IContent
        {
            if (!_contentStream.ContainsKey(typeof(T)))
                throw new Exception($"Content stream not initialized for {typeof(T).Name}");

            if (duplicate == null)
                throw new ArgumentNullException("No instance to duplicate!");

            var newInstance = GetContentInstance(duplicate).CreateNew(DuplicateName<T>(duplicate.InternalName, 0));
            _contentStream[typeof(T)].Add(newInstance);
            return newInstance;
        }

        internal string DuplicateName<T>(string name,
                int counter) where T : IContent
        {
            if (!_contentStream.ContainsKey(typeof(T)))
                return name;

            counter++;

            if (counter >= 200)
                throw new StackOverflowException("Over the max amount of duplicate name iterations!");

            if (_contentStream[typeof(T)].Any(x => x.InternalName.Equals(DuplicateNameFormat(name, counter))))
            {
                return DuplicateName<T>(name, counter);
            }
            else
            {
                return DuplicateNameFormat(name, counter);
            }
        }

        internal void SetupContent()
        {
            var animTypes = new List<AnimationType>() { AnimationType.Player, AnimationType.Projectile };
            LoadAnimations(animTypes);
            LoadAttacks();
            LoadMoveSets();
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

        // @@warn Can probably genericize most of these loads as well as split them between internal
        // loads and file loads, will need to figure out a way to keep The reload file versions
        // seperate from the internal ones as well.
        private void LoadAnimations(IEnumerable<AnimationType> typesToLoad)
        {
            //var actions = _fileManager.LoadAnimations(AnimationType.Player, typeof(PlayerAnimation));
            //_contentStream.Add(typeof(PlayerAnimation), new List<IHunterCombatContentInstance>(_animationLoader.RegisterAnimations(actions)));
            _contentStream.Add(typeof(ICustomAnimationV2), new List<IContent>(_fileManager.LoadAnimations()));
        }

        private void LoadAttacks()
        {
            var loadedAttacks = new List<ICustomAction<HunterCombatPlayer>>();

            // Seeding, add a dedicated method for this later
            loadedAttacks.Add(PlayerAttackSeed
                .CreateDefault("DoubleSlash", "Double Slash", 2, 6));
            loadedAttacks.Add(PlayerAttackSeed
                .CreateDefault("RunningSlash", "Running Slash", 3, 5)
                .WithEvent(new SetPlayerVelocityDirect(3, 0, true, 1), 0)
                .WithEvent(new SetPlayerVelocityDirect(0, 0, true, 1), 5));

            _contentStream.Add(typeof(ICustomAction<HunterCombatPlayer>), new List<IContent>(loadedAttacks));
        }

        private void LoadMoveSets()
        {
            var loadedMovesets = new List<MoveSet>();

            loadedMovesets.Add(new SwordAndShieldMoveSet());
            _contentStream.Add(typeof(MoveSet), new List<IContent>(loadedMovesets));
        }
    }
}