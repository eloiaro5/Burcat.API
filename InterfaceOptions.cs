using BurcatProtocol;
using BurcatProtocol.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;

namespace Burcat.API
{
    public interface IBurcatQueryProvider : IDisposable
    {
        IQueryable<T> Get<T>() where T : IInterfaceObject;
    }

    [BurcatIdentity("d6c84fbc-10b8-49ec-9eb4-e639223b8613")]
    public static class InterfaceOptions
    {
        public static T? TryFind<T>(BurcatIdentifier<T> identifier) where T : IInterfaceObject => (from o in GetSingleUse<T>() where o.Identifier == (Guid)identifier select o).FirstOrDefault();
        public static T Find<T>(BurcatIdentifier<T> identifier) where T : IInterfaceObject => (from o in GetSingleUse<T>() where o.Identifier == (Guid)identifier select o).First();

        public static IEnumerable<FE> FindMany<PE, FE>(BurcatIdentifier<PE> primaryIdentifier, Expression<Func<FE, BurcatIdentifier<PE>>> primaryIdentifierSelector) where PE : IInterfaceObject where FE : IInterfaceObject
        {
            var body = Expression.Convert(primaryIdentifierSelector.Body, typeof(Guid));
            var lambda = Expression.Lambda<Func<FE, Guid>>(body, primaryIdentifierSelector.Parameters);
            return GetSingleUse<PE>().Where(o1 => o1.Identifier == (Guid)primaryIdentifier).Join(GetSingleUse<FE>(), o1 => o1.Identifier, lambda, (o1, o2) => o2);
        }
        public static BurcatList<FE> FindMany<PE, FE>(BurcatIdentifier<PE> primaryIdentifier, string primaryIdentifierSelector) where PE : IInterfaceObject where FE : IInterfaceObject
        {
            var target = Expression.Parameter(typeof(FE), "target");
            var property = Expression.Property(target, primaryIdentifierSelector);
            var lambda = Expression.Lambda<Func<FE, BurcatIdentifier<PE>>>(property, target);
            return [.. FindMany(primaryIdentifier, lambda)];
        }

        public static Func<IBurcatQueryProvider> Provider { get; set; } = () => new EmptyBurcatProvider();

        public static IQueryable<T> GetSingleUse<T>() where T : IInterfaceObject
        {
            IBurcatQueryProvider provider = Provider(); // no using here
            return new SingleUseQueryable<T>(new(provider), provider.Get<T>());
        }

        private sealed class QueryLifetime : IDisposable
        {
            private IDisposable? disposable;
            private int disposed;

            public QueryLifetime(IDisposable disposable) { this.disposable = disposable; }

            public void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(disposed != 0, this);

            public void Dispose()
            {
                if (Interlocked.Exchange(ref disposed, 1) == 0)
                {
                    disposable?.Dispose();
                    disposable = null;
                }
            }
        }

        private sealed class SingleUseQueryable<T> : IQueryable<T>, IOrderedQueryable<T>, IDisposable
        {
            private readonly QueryLifetime _lifetime;
            private readonly IQueryable<T> _inner;

            public SingleUseQueryable(QueryLifetime lifetime, IQueryable<T> inner)
            {
                _lifetime = lifetime;
                _inner = inner;
            }

            public Type ElementType => _inner.ElementType;

            public Expression Expression => _inner.Expression;

            public IQueryProvider Provider =>
                new SingleUseQueryProvider(_lifetime, _inner.Provider);

            public IEnumerator<T> GetEnumerator()
            {
                _lifetime.ThrowIfDisposed();

                IEnumerator<T>? enumerator = null;

                try
                {
                    enumerator = _inner.GetEnumerator();
                    return new DisposingEnumerator<T>(enumerator, _lifetime);
                }
                catch (Exception e)
                {
                    enumerator?.Dispose();
                    _lifetime.Dispose();
                    throw;
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public void Dispose()
            {
                _lifetime.Dispose();
            }

            public static IQueryable Wrap(QueryLifetime lifetime, IQueryable queryable)
            {
                var wrapperType = typeof(SingleUseQueryable<>)
                    .MakeGenericType(queryable.ElementType);

                return (IQueryable)Activator.CreateInstance(
                    wrapperType,
                    lifetime,
                    queryable)!;
            }
        }

        private sealed class SingleUseQueryProvider : IQueryProvider
        {
            private readonly QueryLifetime _lifetime;
            private readonly IQueryProvider _inner;

            public SingleUseQueryProvider(
                QueryLifetime lifetime,
                IQueryProvider inner)
            {
                _lifetime = lifetime;
                _inner = inner;
            }

            public IQueryable CreateQuery(Expression expression)
            {
                _lifetime.ThrowIfDisposed();

                var query = _inner.CreateQuery(expression);

                // Do NOT dispose here.
                // CreateQuery only builds a new IQueryable.
                return SingleUseQueryable<object>.Wrap(_lifetime, query);
            }

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            {
                _lifetime.ThrowIfDisposed();

                var query = _inner.CreateQuery<TElement>(expression);

                // Do NOT dispose here.
                return new SingleUseQueryable<TElement>(_lifetime, query);
            }

            public object? Execute(Expression expression)
            {
                _lifetime.ThrowIfDisposed();

                try
                {
                    return _inner.Execute(expression);
                }
                finally
                {
                    _lifetime.Dispose();
                }
            }

            public TResult Execute<TResult>(Expression expression)
            {
                _lifetime.ThrowIfDisposed();

                try
                {
                    return _inner.Execute<TResult>(expression);
                }
                finally
                {
                    _lifetime.Dispose();
                }
            }
        }

        private sealed class DisposingEnumerator<T> : IEnumerator<T>
        {
            private readonly IEnumerator<T> _inner;
            private readonly IDisposable _owner;

            public DisposingEnumerator(IEnumerator<T> inner, IDisposable owner)
            {
                _inner = inner;
                _owner = owner;
            }

            public T Current => _inner.Current;

            object IEnumerator.Current => Current!;

            public bool MoveNext() => _inner.MoveNext();

            public void Reset() => _inner.Reset();

            public void Dispose()
            {
                try
                {
                    _inner.Dispose();
                }
                finally
                {
                    _owner.Dispose();
                }
            }
        }
    }

    public sealed class EmptyBurcatProvider : IBurcatQueryProvider
    {
        public EmptyBurcatProvider() { }

        public IQueryable<T> Get<T>() where T : IInterfaceObject => Enumerable.Empty<T>().AsQueryable();
        public void RequestUpdate(IInterfaceObject objectAPI) { }

        public void Dispose() { }
    }
}
