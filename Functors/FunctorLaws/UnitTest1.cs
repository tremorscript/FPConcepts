using System;
using Xunit;

namespace FunctorLaws
{

    //https://blog.ploeh.dk/2018/03/22/functors/

    class Functor<T>
    {
        private readonly T _value;

        public Functor(T value) { _value = value; }

        public Functor<TResult> Select<TResult>(Func<T, TResult> selector)
        { return new Functor<TResult>(selector(_value)); }

        public override bool Equals(object? obj)
        {
            var other = obj as Functor<T>;
            if (other == null)
                return false;

            return object.Equals(this._value, other._value);
        }
    }

    static class StringExtension
    {
        public static string Reverse(this string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }


    }

    public class UnitTest1
    {
        /*
        The first law is that mapping the identity function returns the functor unchanged. 
        The identity function is a function that returns all input unchanged. 
        (It's called the identity function because it's the identity for the endomorphism monoid.) 
        In F# and Haskell, this is simply a built-in function called id. 
         */


        [Theory]
        [InlineData(-101)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(42)]
        [InlineData(1337)]
        public void FunctorObeysFirstFunctorLaw(int value)
        {
            Func<int, int> id = x => x;
            var sut = new Functor<int>(value);
            var sut1 = sut.Select(id);
            Assert.Equal(sut, sut1);
        }

        /*
         The second law states that if you have two functions, f and g, 
        then mapping over one after the other should be the same 
        as mapping over the composition of f and g
         */
        [Theory]
        [InlineData(-101)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(42)]
        [InlineData(1337)]
        public void FunctorObeysSecondFunctorLaw(int value)
        {
            Func<int, string> g = i => i.ToString();
            Func<string, string> f = s => new string(s.Reverse());
            var sut = new Functor<int>(value);

            Assert.Equal(sut.Select(g).Select(f), sut.Select(i => f(g(i))));
        }
        /*
         g is a function that translates an int to a string, and f reverses a string. 
         Since g returns string, you can compose it with f, which takes string as input. 
         */
    }
}