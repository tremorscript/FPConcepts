namespace Either.Tests;

using static F;

public class UnitTest1
{
    [Fact]
    public void TestIfLeftIsCreated()
    {
        var left = new Either.Left<string>("test");
        Assert.Equal("test", left.Value);
    }

    [Fact]
    public void TestIfRightIsCreated()
    {
        var right = new Either.Right<string>("test");
        Assert.Equal("test", right.Value);
    }

    [Fact]
    public void CheckIfTheConstructorCreatesTheAppropriateLeftValue()
    {
        Either<int, string> leftValue = new Either<int, string>(1);
        Assert.Equal(1, leftValue.Left);
        Assert.Null(leftValue.Right);
    }

    [Fact]
    public void CheckIfTheConstructorCreatesTheAppropriateRightValue()
    {
        Either<int, string> rightValue = new Either<int, string>("test");
        Assert.Equal(0, rightValue.Left);
        Assert.Equal("test", rightValue.Right);
    }

    [Fact]
    public void CheckImplicitOperatorsForLeftvalues()
    {
        Either<int, string> leftValue1 = 1;
        Either<int, string> leftValue2 = new Either.Left<int>(1);

        Assert.Equal(1, leftValue1.Left);
        Assert.Null(leftValue1.Right);

        Assert.Equal(1, leftValue2.Left);
        Assert.Null(leftValue2.Right);
    }

    [Fact]
    public void CheckImplicitOperatorsForRightvalues()
    {
        Either<int, string> rightValue1 = "test";
        Either<int, string> rightValue2 = new Either.Right<string>("test");

        Assert.Equal("test", rightValue1.Right);
        Assert.Equal(0, rightValue1.Left);

        Assert.Equal("test", rightValue2.Right);
        Assert.Equal(0, rightValue2.Left);

    }

    [Fact]
    public void TestStaticFClass()
    {
        var left = Left<int>(0);
        var right = Right<string>("test");

        Assert.Equal(0, left.Value);
        Assert.Equal("test", right.Value);
    }

    [Fact]
    public void TestTheMethodWhenLeftIsPassed()
    {
        Either<int, string> leftValue = Left<int>(1);

        var returnValue = leftValue.Match(
            left: l => $"Value is {l}",
            right: r => $"Value is {r}"
        );

        Assert.Equal("Value is 1", returnValue);
    }

    [Fact]
    public void TestTheMethodWhenRightIsPassed()
    {
        Either<int, string> rightValue = Right<string>("test");

        var returnValue = rightValue.Match(
            left: l => $"Value is {l}",
            right: r => $"Value is {r}"
        );

        Assert.Equal("Value is test", returnValue);
    }

    [Fact]
    public void CheckIfMapWorks()
    {
        Either<int, string> testMap = Right<string>("test");

        var count = testMap
                        .Map(s => s.Length)
                        .Match(
                            left: l => l,
                            right: r => r
                        );

        Assert.Equal(4, count);

    }

    [Fact]
    public void CheckIfSelectWorks()
    {
        Either<int, string> testMap = Right<string>("test");

        var count = testMap
                        .Select(s => s.Length)
                        .Match<int>(
                            left: l => l,
                            right: r => r
                        );

        Assert.Equal(4, count);

    }

    [Fact]
    public void CheckIfBindWorks()
    {
        Either<string, string> testMap = Right<string>("test");

        var count = testMap
                        .Bind(s => new Either<string, int>(s.Length))
                        .Match(
                            left: l => 0,
                            right: r => r
                        );

        Assert.Equal(4, count);

    }

    [Fact]
    public void CheckIfSelectManyWorks()
    {
        Either<string, string> testMap = Right<string>("test");

        var count = testMap
                        .SelectMany(s => new Either<string, int>(s.Length))
                        .Match(
                            left: l => 0,
                            right: r => r
                        );

        Assert.Equal(4, count);

    }
}

public static partial class F
{
    public static Either.Left<L> Left<L>(L l) => new(l);
    public static Either.Right<R> Right<R>(R r) => new(r);
}

public static class Either
{
    public struct Left<L>
    {
        internal L Value { get; }

        internal Left(L value) { Value = value; }

        public override string ToString() => $"Left({Value})";
    }

    public struct Right<R>
    {
        internal R Value { get; }

        internal Right(R value) { Value = value; }

        public override string ToString() => $"Right({Value})";
    }
}

public struct Either<L, R>
{
    internal L Left { get; }

    internal R Right { get; }

    private bool IsRight { get; }

    private bool IsLeft => !IsRight;

    internal Either(L left)
    {
        IsRight = false;
        Left = left;
        Right = default(R);
    }

    internal Either(R right)
    {
        IsRight = true;
        Right = right;
        Left = default(L);
    }

    public static implicit operator Either<L, R>(L left) => new(left);
    public static implicit operator Either<L, R>(R right) => new(right);

    public static implicit operator Either<L, R>(Either.Left<L> left) => new(left.Value);
    public static implicit operator Either<L, R>(Either.Right<R> right) => new(right.Value);

    public TR Match<TR>(Func<L, TR> left, Func<R, TR> right)
       => IsLeft ? left(this.Left) : right(this.Right);

    public override string ToString() => Match(l => $"Left({l})", r => $"Right({r})");
}

public static class EitherExtensions
{
    public static Either<L, RR> Map<L, R, RR>(this Either<L, R> @this, Func<R, RR> f)
    => @this.Match<Either<L, RR>>(
            l => F.Left(l),
            r => F.Right(f(r))
        );

    public static Either<LL, RR> Map<L, LL, R, RR>(this Either<L, R> @this, Func<L, LL> left, Func<R, RR> right)
    => @this.Match<Either<LL, RR>>(
            l => F.Left(left(l)),
            r => F.Right(right(r))
        );

    public static Either<L, R> Select<L, T, R>(this Either<L, T> @this, Func<T, R> map)
        => @this.Map(map);

    public static Either<L, RR> Bind<L, R, RR>(this Either<L, R> @this, Func<R, Either<L, RR>> f)
        => @this.Match(
            l => F.Left(l),
            r => f(r)
        );

    public static Either<L, RR> SelectMany<L, R, RR>(this Either<L, R> @this, Func<R, Either<L, RR>> bind)
        => @this.Bind(bind);
}

