using Burcat.API.Market.Collectionables;

namespace Burcat.API.Market
{
    /// <summary>Resolves ownership references through their concrete tables.</summary>
    public static class OwnershipQueries
    {
        /// <summary>Returns the currency or card-level ownership matching the identifier, or null when absent.</summary>
        public static IOwnership? TryFind(IBurcatQueryProvider provider, Guid identifier)
            => provider.Get<CurrencyOwnership>().FirstOrDefault(ownership => ownership.Identifier == identifier) as IOwnership
                ?? provider.Get<CardLevelOwnership>().FirstOrDefault(ownership => ownership.Identifier == identifier);
    }
}
