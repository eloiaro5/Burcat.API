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
        IQueryable<T> Get<T>() where T : class, IInterfaceObject;
    }

    [BurcatIdentity("d6c84fbc-10b8-49ec-9eb4-e639223b8613")]
    public static class InterfaceOptions
    {
        public static T? TryFind<T>(BurcatIdentifier<T> identifier) where T : class, IInterfaceObject => UseProvider(provider => (from o in provider.Get<T>() where o.Identifier == (Guid)identifier select o).FirstOrDefault());
        public static T Find<T>(BurcatIdentifier<T> identifier) where T : class, IInterfaceObject => UseProvider(provider => (from o in provider.Get<T>() where o.Identifier == (Guid)identifier select o).First());

        public static IEnumerable<FE> FindMany<PE, FE>(BurcatIdentifier<PE> primaryIdentifier, Expression<Func<FE, BurcatIdentifier<PE>>> primaryIdentifierSelector) where PE : class, IInterfaceObject where FE : class, IInterfaceObject
        {
            var body = Expression.Convert(primaryIdentifierSelector.Body, typeof(Guid));
            var lambda = Expression.Lambda<Func<FE, Guid>>(body, primaryIdentifierSelector.Parameters);
            return UseProvider(provider => (IEnumerable<FE>)[.. provider.Get<PE>().Where(o1 => o1.Identifier == (Guid)primaryIdentifier).Join(provider.Get<FE>(), o1 => o1.Identifier, lambda, (o1, o2) => o2)]);
        }
        public static BurcatList<FE> FindMany<PE, FE>(BurcatIdentifier<PE> primaryIdentifier, string primaryIdentifierSelector) where PE : class, IInterfaceObject where FE : class, IInterfaceObject
        {
            var target = Expression.Parameter(typeof(FE), "target");
            var property = Expression.Property(target, primaryIdentifierSelector);
            var lambda = Expression.Lambda<Func<FE, BurcatIdentifier<PE>>>(property, target);
            return [.. FindMany(primaryIdentifier, lambda)];
        }

        public static Func<IBurcatQueryProvider> Provider { get; set; } = () => new EmptyBurcatProvider();

        public static T UseProvider<T>(Func<IBurcatQueryProvider, T> use)
        {
            using IBurcatQueryProvider provider = Provider();
            return use(provider);
        }
    }

    public sealed class EmptyBurcatProvider : IBurcatQueryProvider
    {
        public IQueryable<T> Get<T>() where T : class, IInterfaceObject => Enumerable.Empty<T>().AsQueryable();

        public void Dispose() { }
    }
}
