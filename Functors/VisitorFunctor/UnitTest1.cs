using Xunit;

namespace VisitorFunctor
{

    public interface IBinaryTree<T>
    {
        TResult Accept<TResult>(IBinaryTreeVisitor<T, TResult> visitor);
    }
    public interface IBinaryTreeVisitor<T, TResult>
    {
    }

    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {

        }
    }
}