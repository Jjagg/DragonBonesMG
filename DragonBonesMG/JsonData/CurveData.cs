using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DragonBonesMG.JsonData {
    public class CurveData : IList<int> {
        public List<int> Data;

        public IEnumerator<int> GetEnumerator() {
            return Data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable) Data).GetEnumerator();
        }

        public void Add(int item) {
            Data.Add(item);
        }

        public void Clear() {
            Data.Clear();
        }

        public bool Contains(int item) {
            return Data.Contains(item);
        }

        public void CopyTo(int[] array, int arrayIndex) {
            Data.CopyTo(array, arrayIndex);
        }

        public bool Remove(int item) {
            return Data.Remove(item);
        }

        public int Count => Data.Count;

        public bool IsReadOnly => true;

        public int IndexOf(int item) {
            return Data.IndexOf(item);
        }

        public void Insert(int index, int item) {
            Data.Insert(index, item);
        }

        public void RemoveAt(int index) {
            Data.RemoveAt(index);
        }

        public int this[int index]
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }
    }
}