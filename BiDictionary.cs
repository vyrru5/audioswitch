using System.Collections.Generic;

namespace AudioSwitch
{
    internal class BiDictionary<TFirst, TSecond>
    {
        private readonly IDictionary<TFirst, TSecond> firstToSecond = new Dictionary<TFirst, TSecond>();
        private readonly IDictionary<TSecond, TFirst> secondToFirst = new Dictionary<TSecond, TFirst>();

        internal void Add(TFirst first, TSecond second)
        {
            firstToSecond.Add(first, second);
            secondToFirst.Add(second, first);
        }

        internal TSecond GetByFirst(TFirst first)
        {
            return firstToSecond[first];
        }

        internal TFirst GetBySecond(TSecond second)
        {
            return secondToFirst[second];
        }

        internal void Clear()
        {
            firstToSecond.Clear();
            secondToFirst.Clear();
        }
    }
}