using System;
using Xunit;

namespace MaybeFunctor
{
    public class MaybeFunctorTests
    {
        [Fact]
        public void Usage()
        {
            var source = new Maybe<int>(42);

            Maybe<string> dest = source.Select(x => x.ToString());

            var dest2 = from x in source select x.ToString();

            Assert.True(true);
        }

        [Theory]
        [InlineData("")]
        [InlineData("foo")]
        [InlineData("bar")]
        [InlineData("corge")]
        [InlineData("antidisestablishmentarianism")]
        public void PopulatedMaybeObeysFirstFunctorLaw(string value)
        {
            Func<string, string> id = x => x;
            var m = new Maybe<string>(value);

            Assert.Equal(m, m.Select(id));
        }

        [Fact]
        public void EmptyMaybeObeysFirstFunctorLaw()
        {
            Func<string, string> id = x => x;
            var m = new Maybe<string>();

            Assert.Equal(m, m.Select(id));
        }

        [Theory]
        [InlineData("", true)]
        [InlineData("foo", false)]
        [InlineData("bar", false)]
        [InlineData("corge", false)]
        [InlineData("antidisestablishmentarianism", true)]
        public void PopulatedMaybeObeysSecondFunctorLaw(string value, bool expected)
        {
            Func<string, int> g = s => s.Length;
            Func<int, bool> f = i => i % 2 == 0;
            var m = new Maybe<string>(value);

            Assert.Equal(m.Select(g).Select(f), m.Select(s => f(g(s))));
        }

        [Fact]
        public void EmptyMaybeObeysSecondFunctorLaw()
        {
            Func<string, int> g = s => s.Length;
            Func<int, bool> f = i => i % 2 == 0;
            var m = new Maybe<string>();

            Assert.Equal(m.Select(g).Select(f), m.Select(s => f(g(s))));
        }
    }
}