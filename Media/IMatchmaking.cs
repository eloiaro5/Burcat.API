using BurcatProtocol;
using BurcatProtocol.Annotations;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Burcat.API.Media
{
    [BurcatUnique(nameof(Match), nameof(Advertisement))]
    public interface IMatchmaking : IInterfaceObject
    {
        BurcatIdentifier<IPublisher> Match { get; }
        BurcatIdentifier<AdvertisementPost> Advertisement { get; }
        bool IsInclusive { get; set; }
    }

    public interface IPublisher : IInterfaceObject { }

    [BurcatIdentity("d27073a2-755b-43fc-9dd6-015077d269dc")]
    public class MemberMatchmaking : BurcatObject, IMatchmaking
    {
        public BurcatIdentifier<Member> Member { get; }
        public BurcatIdentifier<AdvertisementPost> Advertisement { get; }
        public bool IsInclusive { get; set; }

        BurcatIdentifier<IPublisher> IMatchmaking.Match => Member.Downcast<IPublisher>();

        public MemberMatchmaking(BurcatIdentifier<Member> member, BurcatIdentifier<AdvertisementPost> advertisement) { Member = member; Advertisement = advertisement; }
        public MemberMatchmaking(BurcatIdentifier<Member> member, BurcatIdentifier<AdvertisementPost> advertisement, bool isInclusive) : this(member, advertisement) { IsInclusive = isInclusive; }

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is not null && InterfaceOptions.UseProvider(provider => (from a in provider.Get<AdvertisementPost>() join p in provider.Get<IPost>() on (Guid)a.Post equals p.Identifier where a == Advertisement && p.Owner == member select true).Any());
        public override object?[] GetBurcatConstructionValues() => [Member, Advertisement];
    }

    [BurcatIdentity("32f310a4-1d98-4905-bb3c-e58905300e3f")]
    public class FactionMatchmaking : BurcatObject, IMatchmaking
    {
        public BurcatIdentifier<Faction> Faction { get; }
        public BurcatIdentifier<AdvertisementPost> Advertisement { get; }
        public bool IsInclusive { get; set; }
        public bool IsRecursive { get; set; }

        BurcatIdentifier<IPublisher> IMatchmaking.Match => Faction.Downcast<IPublisher>();

        public FactionMatchmaking(BurcatIdentifier<Faction> faction, BurcatIdentifier<AdvertisementPost> advertisement) { Faction = faction; Advertisement = advertisement; }
        public FactionMatchmaking(BurcatIdentifier<Faction> faction, BurcatIdentifier<AdvertisementPost> advertisement, bool isInclusive) : this(faction, advertisement) { IsInclusive = isInclusive; }
        public FactionMatchmaking(BurcatIdentifier<Faction> faction, BurcatIdentifier<AdvertisementPost> advertisement, bool isInclusive, bool isRecursive) : this(faction, advertisement, isInclusive) { IsRecursive = isRecursive; }

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => member is not null && InterfaceOptions.UseProvider(provider => (from a in provider.Get<AdvertisementPost>() join p in provider.Get<IPost>() on (Guid)a.Post equals p.Identifier where a == Advertisement && p.Owner == member select true).Any());
        public override object?[] GetBurcatConstructionValues() => [Faction, Advertisement];
    }
}
