using BurcatProtocol;
using BurcatProtocol.Annotations;

namespace Burcat.API
{
    [BurcatIdentity("83b81c67-a184-4c6d-9656-0f22c306af9e")]
    [BurcatUnique(nameof(Faction), nameof(Member))]
    public class FactionBan : BurcatObject, IInterfaceObject
    {
        public BurcatIdentifier<Faction> Faction { get; }
        public BurcatIdentifier<Member> Member { get; }

        public FactionBan(BurcatIdentifier<Faction> faction, BurcatIdentifier<Member> member) { Faction = faction; Member = member; }

        public bool ShouldCreate(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldSelect(BurcatIdentifier<Member>? member) => ShouldManage(member);
        [NotBurcatInvokable]
        public bool ShouldDelete(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldManage(BurcatIdentifier<Member>? member) => member is not null && (InterfaceOptions.Find(Faction).Owner == member || (from m in InterfaceOptions.Provider.Get<FactionMembership>() join r in InterfaceOptions.Provider.Get<FactionRole>() on (Guid?)m.Role equals r.Identifier where m.Member == member && r.Faction == Faction && r.CanManageUsers == true select true).Any());

        public override object?[] GetBurcatConstructionValues() => [Faction, Member];
    }
}
