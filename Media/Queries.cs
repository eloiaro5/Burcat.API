using BurcatProtocol;
using System.Linq.Expressions;

namespace Burcat.API.Media
{
    /// <summary>Queries concrete post tables and combines their results after SQL filtering.</summary>
    public static class Queries
    {
        /// <summary>Returns matching posts, preserving their concrete types. Further enumeration operators run in memory.</summary>
        public static IEnumerable<IPost> Where(IBurcatQueryProvider provider, Expression<Func<IPost, bool>> predicate)
        {
            ArgumentNullException.ThrowIfNull(provider);
            ArgumentNullException.ThrowIfNull(predicate);

            return Where<PostMessage>(provider, predicate).AsEnumerable()
                .Concat<IPost>(Where<PostRepost>(provider, predicate))
                .Concat(Where<PostImage>(provider, predicate));
        }

        /// <summary>Finds a post across its concrete tables, or returns null when absent.</summary>
        public static IPost? TryFind(IBurcatQueryProvider provider, Guid identifier)
            => provider.Get<PostMessage>().FirstOrDefault(post => post.Identifier == identifier) as IPost
                ?? provider.Get<PostRepost>().FirstOrDefault(post => post.Identifier == identifier) as IPost
                ?? provider.Get<PostImage>().FirstOrDefault(post => post.Identifier == identifier);

        private static IQueryable<TPost> Where<TPost>(IBurcatQueryProvider provider, Expression<Func<IPost, bool>> predicate) where TPost : BurcatObject, IPost
        {
            ParameterExpression parameter = Expression.Parameter(typeof(TPost), predicate.Parameters[0].Name);
            Expression body = new PostParameterVisitor(predicate.Parameters[0], parameter).Visit(predicate.Body);
            return provider.Get<TPost>().Where(Expression.Lambda<Func<TPost, bool>>(body, parameter));
        }

        private sealed class PostParameterVisitor(ParameterExpression source, ParameterExpression target) : ExpressionVisitor
        {
            protected override Expression VisitParameter(ParameterExpression node) => node == source ? target : node;

            protected override Expression VisitMember(MemberExpression node)
            {
                if (node.Expression == source) return Expression.Property(target, node.Member.Name);
                return base.VisitMember(node);
            }
        }
    }
}
