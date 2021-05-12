using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.AnimationEngine.Services;
using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.AttackEngine.MoveSets;
using HunterCombatMR.Enumerations;
using HunterCombatMR.Events;
using HunterCombatMR.Interfaces;
using HunterCombatMR.Seeds.Attacks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR
{
    public sealed class HunterCombatContent
    {
        private const int _animationNameMax = 64;

        private readonly Type[] _animationMap = new Type[] { typeof(PlayerAnimation),
            typeof(ProjectileAnimation) };

        private AnimationLoader _animationLoader;
        private IDictionary<Type, IEnumerable<IHunterCombatContentInstance>> _contentStream;
        private AnimationFileManager _fileManager;

        public HunterCombatContent(AnimationFileManager fileManager)
        {
            _contentStream = new Dictionary<Type, IEnumerable<IHunterCombatContentInstance>>();
            _animationLoader = new AnimationLoader();
            _fileManager = fileManager;
        }

        public bool CheckContentInstanceByName<T>(string name) where T : IHunterCombatContentInstance
                    => _contentStream[typeof(T)].Any(x => x.InternalName.Equals(name));

        public T GetContentInstance<T>(string name) where T : IHunterCombatContentInstance
            => (T)_contentStream[typeof(T)].FirstOrDefault(x => x.InternalName.Equals(name));

        public IHunterCombatContentInstance GetContentInstance(string name, Type type)
            => _contentStream[type].FirstOrDefault(x => x.InternalName.Equals(name));

        public T GetContentInstance<T>(T instance) where T : IHunterCombatContentInstance
            => (T)_contentStream[typeof(T)].FirstOrDefault(x => x.Equals(instance));

        public IEnumerable<T> GetContentList<T>() where T : HunterCombatContentInstance
            => _contentStream[typeof(T)].Select(x => x as T);

        public bool LoadAnimationFile(AnimationType animationType,
            string fileName,
            bool overrideInternal = false)
        {
            if (_contentStream.ContainsKey(_animationMap[(int)animationType]))
            {
                var animation = _fileManager.LoadAnimation(animationType, fileName, overrideInternal);
                var stream = new List<IHunterCombatContentInstance>(_contentStream[_animationMap[(int)animationType]]);

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

        internal void DeleteContentInstance<T>(T instance) where T : IHunterCombatContentInstance
        {
            DeleteContentInstance<T>(instance.InternalName);
        }

        internal void DeleteContentInstance<T>(string name) where T : IHunterCombatContentInstance
        {
            if (!_contentStream.ContainsKey(typeof(T)))
                return;

            if (CheckContentInstanceByName<T>(name))
            {
                var stream = new List<IHunterCombatContentInstance>(_contentStream[typeof(T)]);
                stream.Remove(GetContentInstance<T>(name));
                _contentStream[typeof(T)] = stream;
            }
        }

        internal string DuplicateContentInstance<T>(T duplicate) where T : IHunterCombatContentInstance
        {
            if (!_contentStream.ContainsKey(typeof(T)))
                throw new Exception($"Content stream not initialized for {typeof(T).Name}");

            if (duplicate == null)
                throw new ArgumentNullException("No animation to duplicate!");

            var newInstance = GetContentInstance(duplicate).CloneFrom(DuplicateName<T>(duplicate.InternalName, 0));
            var stream = new List<IHunterCombatContentInstance>(_contentStream[typeof(T)]);
            stream.Add(newInstance);
            _contentStream[typeof(T)] = stream;
            return newInstance.InternalName;
        }

        internal string DuplicateName<T>(string name,
                int counter) where T : IHunterCombatContentInstance
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

        // @@warn Can probably genericize most of these loads as well as split them between internal loads and file loads, will need to figure out a way to keep
        // The reload file versions seperate from the internal ones as well.
        private void LoadAnimations(IEnumerable<AnimationType> typesToLoad)
        {
            foreach (var type in typesToLoad)
            {
                Type typeDef = _animationMap[(int)type];
                if (_contentStream.ContainsKey(typeDef))
                    _contentStream[typeDef] = _animationLoader.RegisterAnimations(_fileManager.LoadAnimations(type, typeDef));
                else
                    _contentStream.Add(typeDef, _animationLoader.RegisterAnimations(_fileManager.LoadAnimations(type, typeDef)));
            }
        }

        private void LoadAttacks()
        {
            var loadedAttacks = new List<PlayerAction>();

            // Seeding, add a dedicated method for this later
            loadedAttacks.Add(PlayerAttackSeed
                .CreateDefault("DoubleSlash", "Double Slash", 2, 6));
            loadedAttacks.Add(PlayerAttackSeed
                .CreateDefault("RunningSlash", "Running Slash", 3, 5)
                .WithEvent(new SetPlayerVelocityDirect(3, 0, true, 1), 0)
                .WithEvent(new SetPlayerVelocityDirect(0, 0, true, 1), 5));

            foreach (var attack in loadedAttacks)
            {
                attack.Initialize<PlayerAnimation>();
            }

            _contentStream.Add(typeof(PlayerAction), loadedAttacks);
        }

        private void LoadMoveSets()
        {
            var loadedMovesets = new List<MoveSet>();

            loadedMovesets.Add(new SwordAndShieldMoveSet());
            _contentStream.Add(typeof(MoveSet), loadedMovesets);
        }
    }
}