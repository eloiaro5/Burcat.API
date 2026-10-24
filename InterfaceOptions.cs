using BurcatProtocol;
using BurcatProtocol.Annotations;
using Burcat.API.Media;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;

namespace Burcat.API
{
    public interface IBurcatQueryProvider
    {
        /// <summary>Returns a query for a concrete or abstract entity class.</summary>
        /// <typeparam name="T">An entity class implementing <see cref="IInterfaceObject"/>. Interface type arguments are not supported.</typeparam>
        /// <exception cref="ArgumentException"><typeparamref name="T"/> is an interface.</exception>
        /// <remarks>Query each implementing class separately when working with a polymorphic interface.</remarks>
        IQueryable<T> Get<T>() where T : class, IInterfaceObject;
    }

    [BurcatIdentity("d6c84fbc-10b8-49ec-9eb4-e639223b8613")]
    public static class InterfaceOptions
    {
        /// <summary>Finds a message in its concrete comment or post table, or returns null when absent.</summary>
        public static IMessage? TryFindMessage(Guid identifier)
            => Provider.Get<CommentMessage>().FirstOrDefault(comment => comment.Identifier == identifier) as IMessage
                ?? Queries.TryFind(Provider, identifier);

        /// <summary>Finds a message in its concrete table; throws when the reference does not exist.</summary>
        public static IMessage FindMessage(BurcatIdentifier<IMessage> identifier)
            => TryFindMessage(identifier.Value) ?? throw new InvalidOperationException("The referenced message does not exist.");

        /// <summary>Finds a reportable message, faction, member, or application in its concrete entity table.</summary>
        public static Burcat.API.System.IReport.IReportable? TryFindReportable(Guid identifier)
            => TryFindMessage(identifier) as Burcat.API.System.IReport.IReportable
                ?? Provider.Get<Faction>().FirstOrDefault(faction => faction.Identifier == identifier) as Burcat.API.System.IReport.IReportable
                ?? Provider.Get<Member>().FirstOrDefault(member => member.Identifier == identifier) as Burcat.API.System.IReport.IReportable
                ?? Provider.Get<Burcat.API.Development.Application>().FirstOrDefault(application => application.Identifier == identifier);

        /// <summary>Finds a report target, throwing when it no longer exists.</summary>
        public static Burcat.API.System.IReport.IReportable FindReportable(BurcatIdentifier<Burcat.API.System.IReport.IReportable> identifier)
            => TryFindReportable(identifier.Value) ?? throw new InvalidOperationException("The report target does not exist.");

        public static T? TryFind<T>(BurcatIdentifier<T> identifier) where T : class, IInterfaceObject => (from o in Provider.Get<T>() where o.Identifier == (Guid)identifier select o).FirstOrDefault();
        public static T Find<T>(BurcatIdentifier<T> identifier) where T : class, IInterfaceObject => (from o in Provider.Get<T>() where o.Identifier == (Guid)identifier select o).First();

        public static IEnumerable<FE> FindMany<PE, FE>(BurcatIdentifier<PE> primaryIdentifier, Expression<Func<FE, BurcatIdentifier<PE>>> primaryIdentifierSelector) where PE : class, IInterfaceObject where FE : class, IInterfaceObject
        {
            var body = Expression.Convert(primaryIdentifierSelector.Body, typeof(Guid));
            var lambda = Expression.Lambda<Func<FE, Guid>>(body, primaryIdentifierSelector.Parameters);
            IBurcatQueryProvider provider = Provider;
            return [.. provider.Get<PE>().Where(o1 => o1.Identifier == (Guid)primaryIdentifier).Join(provider.Get<FE>(), o1 => o1.Identifier, lambda, (o1, o2) => o2)];
        }
        public static BurcatList<FE> FindMany<PE, FE>(BurcatIdentifier<PE> primaryIdentifier, string primaryIdentifierSelector) where PE : class, IInterfaceObject where FE : class, IInterfaceObject
        {
            var target = Expression.Parameter(typeof(FE), "target");
            var property = Expression.Property(target, primaryIdentifierSelector);
            var lambda = Expression.Lambda<Func<FE, BurcatIdentifier<PE>>>(property, target);
            return [.. FindMany(primaryIdentifier, lambda)];
        }

        private static IBurcatQueryProvider _provider = new EmptyBurcatProvider();
        public static AsyncLocal<IBurcatQueryProvider> LocalProvider { get; } = new();
        public static IBurcatQueryProvider Provider
        {
            get => LocalProvider.Value ?? _provider;
            set => _provider = value;
        }
    }

    internal sealed class EmptyBurcatProvider : IBurcatQueryProvider
    {
        IQueryable<T> IBurcatQueryProvider.Get<T>() where T : class
        {
            if (typeof(T).IsInterface) throw new ArgumentException($"{typeof(T).Name} must be a concrete or abstract entity class.");
            return Enumerable.Empty<T>().AsQueryable();
        }
    }
}
