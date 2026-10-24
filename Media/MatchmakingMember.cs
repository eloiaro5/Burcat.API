using BurcatProtocol;
using BurcatProtocol.Annotations;

namespace Burcat.API.Media
{
    [BurcatIdentity("d27073a2-755b-43fc-9dd6-015077d269dc")]
    public class MatchmakingMember : BurcatObject, IMatchmaking
    {
        public BurcatIdentifier<Member> Member { get; }
        public BurcatIdentifier<MessageAdvertisement> Advertisement { get; }
        public bool IsInclusive { get; set; }

        BurcatIdentifier<IPublisher> IMatchmaking.Match => Member.Downcast<IPublisher>();

        public MatchmakingMember(BurcatIdentifier<Member> member, BurcatIdentifier<MessageAdvertisement> advertisement) { Member = member; Advertisement = advertisement; }
        public MatchmakingMember(BurcatIdentifier<Member> member, BurcatIdentifier<MessageAdvertisement> advertisement, bool isInclusive) : this(member, advertisement) { IsInclusive = isInclusive; }

        [NotBurcatInvokable]
        public bool ShouldDelete(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is not null && CanManageAdvertisement(Advertisement, member.Value);

        internal static bool CanManageAdvertisement(BurcatIdentifier<MessageAdvertisement> advertisement, BurcatIdentifier<Member> member)
        {
            MessageAdvertisement? value = InterfaceOptions.TryFind(advertisement);
            return value is not null && Queries.Where(InterfaceOptions.Provider, post => post.Identifier == value.Post.Value && (Guid)post.Owner == member.Value).Any();
        }
        public override object?[] GetBurcatConstructionValues() => [Member, Advertisement];
    }
}
