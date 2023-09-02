using System.Collections.Generic;
using Xunit;

namespace TreeFunctor
{
    public class TreeFunctorTests
    {
        public static IEnumerable<object[]> Trees
        {
            get
            {
                yield return new[] { Tree.Leaf(42) };
                yield return new[] { Tree.Create(-32, Tree.Leaf(0)) };
                yield return new[] { Tree.Create(99, Tree.Leaf(90), Tree.Leaf(2)) };
                yield return new[] { Tree.Create(99, Tree.Leaf(90), Tree.Create(2, Tree.Leaf(-3))) };
                yield return new[]
                {
                    Tree.Create(
                    42,
                    Tree.Create(1337, Tree.Leaf(-3)),
                    Tree.Create(7, Tree.Leaf(-99), Tree.Leaf(100), Tree.Leaf(0)))
                };
            }
        }

        [Fact]
        public void Usage()
        {
            var source =
                Tree.Create(
                42,
                Tree.Create(1337, Tree.Leaf(-3)),
                Tree.Create(7, Tree.Leaf(-99), Tree.Leaf(100), Tree.Leaf(0)));

            var dest = source.Select(i => i.ToString());

            var dest1 = from i in source select i.ToString();
            Assert.True(true);
        }

        [Theory, MemberData(nameof(Trees))]
        public void FirstFunctorLaw(Tree<int> tree) { Assert.Equal(tree, tree.Select(x => x)); }

        [Theory, MemberData(nameof(Trees))]
        public void SecondFunctorLaw(Tree<int> tree)
        {
            string g(int i) => i.ToString();
            bool f(string s) => s.Length % 2 == 0;

            Assert.Equal(tree.Select(g).Select(f), tree.Select(i => f(g(i))));
        }
    }
}