using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DragonBonesMG.Util {
    public class KeyedCollectionImpl<TKey, TValue> : KeyedCollection<TKey, TValue> {
        private readonly Func<TValue, TKey> _selector;

        public KeyedCollectionImpl(Func<TValue, TKey> selector) {
            _selector = selector;
        }

        public void AddRange(IEnumerable<TValue> range) {
            foreach (var val in range)
                Add(val);
        }

        protected override TKey GetKeyForItem(TValue item) {
            return _selector(item);
        }

        public bool TryGet(TKey key, out TValue value) {
            if (Contains(key)) {
                value = this[key];
                return true;
            }
            value = default(TValue);
            return false;
        }

        public ICollection<TKey> Keys() {
            return Dictionary.Keys;
        }

        public ICollection<TValue> Values() {
            return Dictionary.Values;
        }
    }
}