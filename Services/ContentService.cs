using HunterCombatMR.Interfaces;
using HunterCombatMR.Models.MoveSet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Services
{
    public sealed class ContentService
    {
        private const int _animationNameMax = 72;
        private IDictionary<Type, ICollection<IContent>> _contentStream;
        private AnimationFileManager _fileManager;

        public ContentService(AnimationFileManager fileManager)
        {
            _contentStream = new Dictionary<Type, ICollection<IContent>>();
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
            //LoadMoveSets();
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

        private void LoadMoveSets()
        {
            var loadedMovesets = new List<MoveSet>();

            loadedMovesets.Add(new SwordAndShieldMoveSet());
            _contentStream.Add(typeof(MoveSet), new List<IContent>(loadedMovesets));
        }
    }
}