using BurcatProtocol;
using BurcatProtocol.Annotations;

namespace Burcat.API.Media
{
    [BurcatIdentity("32f310a4-1d98-4905-bb3c-e58905300e3f")]
    public class MatchmakingFaction : BurcatObject, IMatchmaking
    {
        public BurcatIdentifier<Faction> Faction { get; }
        public BurcatIdentifier<MessageAdvertisement> Advertisement { get; }
        public bool IsInclusive { get; set; }
        public bool IsRecursive { get; set; }

        BurcatIdentifier<IPublisher> IMatchmaking.Match => Faction.Downcast<IPublisher>();

        public MatchmakingFaction(BurcatIdentifier<Faction> faction, BurcatIdentifier<MessageAdvertisement> advertisement) { Faction = faction; Advertisement = advertisement; }
        public MatchmakingFaction(BurcatIdentifier<Faction> faction, BurcatIdentifier<MessageAdvertisement> advertisement, bool isInclusive) : this(faction, advertisement) { IsInclusive = isInclusive; }
        public MatchmakingFaction(BurcatIdentifier<Faction> faction, BurcatIdentifier<MessageAdvertisement> advertisement, bool isInclusive, bool isRecursive) : this(faction, advertisement, isInclusive) { IsRecursive = isRecursive; }

        [NotBurcatInvokable]
        public bool ShouldDelete(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is not null && MatchmakingMember.CanManageAdvertisement(Advertisement, member.Value);
        public override object?[] GetBurcatConstructionValues() => [Faction, Advertisement];
    }
}
