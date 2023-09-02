using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TreeFunctor
{
    public sealed class Tree<T> : IReadOnlyCollection<Tree<T>>
    {
        private readonly IReadOnlyCollection<Tree<T>> children;

        public T Item { get; }

        public Tree(T item, IReadOnlyCollection<Tree<T>> children)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), $"{nameof(item)} is null.");
            }

            if ((children == null))
            {
                throw new ArgumentException($"{nameof(children)} is null", nameof(children));
            }

            Item = item;
            this.children = children;
        }

        public Tree<TResult> Select<TResult>(Func<T, TResult> selector)
        {
            var mappedItem = selector(Item);

            var mappedChildren = new List<Tree<TResult>>();
            foreach (var child in children)
                mappedChildren.Add(child.Select(selector));
            return new Tree<TResult>(mappedItem, mappedChildren);
        }

        public int Count => children.Count;

        public IEnumerator<Tree<T>> GetEnumerator() { return children.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator() { return children.GetEnumerator(); }

        public override bool Equals(object obj)
        {
            if (obj is not Tree<T> other)
                return false;

            return Equals(Item, other.Item) && Enumerable.SequenceEqual(this, other);
        }

        public override int GetHashCode() { return Item.GetHashCode() ^ children.GetHashCode(); }
    }


    public static class Tree
    {
        public static Tree<T> Leaf<T>(T item)
        {
            return new Tree<T>(item, new Tree<T>[0]);
        }

        public static Tree<T> Create<T>(T item, params Tree<T>[] children)
        {
            return new Tree<T>(item, children);
        }
    }

}
